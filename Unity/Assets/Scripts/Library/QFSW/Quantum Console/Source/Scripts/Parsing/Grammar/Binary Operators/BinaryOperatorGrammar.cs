using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using QFSW.QC.Utilities;

namespace QFSW.QC.Grammar
{
    public abstract class BinaryOperatorGrammar : IQcGrammarConstruct
    {
        public abstract int Precedence { get; }
        protected abstract char OperatorToken { get; }
        protected abstract string OperatorMethodName { get; }
        protected abstract Func<Expression, Expression, BinaryExpression> PrimitiveExpressionGenerator { get; }

        private Regex _operatorRegex;

        private readonly HashSet<Type> _missingOperatorTable = new HashSet<Type>();
        private readonly Dictionary<Type, IBinaryOperator> _foundOperatorTable = new Dictionary<Type, IBinaryOperator>();

        public bool Match(string value, Type type)
        {
            if (_missingOperatorTable.Contains(type))
            {
                return false;
            }

            if (!IsSyntaxMatch(value))
            {
                return false;
            }

            if (_foundOperatorTable.ContainsKey(type))
            {
                return true;
            }

            IBinaryOperator operatorData = GetOperatorData(type);

            if (operatorData != null)
            {
                _foundOperatorTable.Add(type, operatorData);
                return true;
            }

            _missingOperatorTable.Add(type);
            return false;
        }

        private bool IsSyntaxMatch(string value)
        {
            if (_operatorRegex == null)
            {
                _operatorRegex = new Regex($@"^.+\{OperatorToken}.+$");
            }

            if (!_operatorRegex.IsMatch(value))
            {
                return false;
            }

            int operatorPos = GetOperatorPosition(value);
            return operatorPos > 0 && operatorPos < value.Length;
        }

        private IBinaryOperator GetOperatorData(Type type)
        {
            if (type.IsPrimitive)
            {
#if !UNITY_EDITOR && ENABLE_IL2CPP && !UNITY_2022_2_OR_NEWER
                string typeName = QFSW.QC.Utilities.ReflectionExtensions.GetDisplayName(type);
                UnityEngine.Debug.LogWarning($"{typeName} {OperatorToken} {typeName} is not supported as IL2CPP does not support dynamic value typed generics before Unity 2022.2");
#else
                return GeneratePrimitiveOperator(type);
#endif

            }

            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            BinaryOperatorData[] candidates = methods.Where(x => x.Name == OperatorMethodName)
                                                     .Where(x => x.ReturnType == type)
                                                     .Where(x => x.GetParameters().Length == 2)
                                                     .Select(x => new BinaryOperatorData(x))
                                                     .ToArray();

            BinaryOperatorData idealCandidate = candidates.FirstOrDefault(x => x.LArg == type && x.RArg == type)
                                             ?? candidates.FirstOrDefault(x => x.LArg == type)
                                             ?? candidates.FirstOrDefault(x => x.RArg == type)
                                             ?? candidates.FirstOrDefault();

            return idealCandidate;
        }

        private IBinaryOperator GeneratePrimitiveOperator(Type type)
        {
            ParameterExpression leftParam = Expression.Parameter(type, "left");
            ParameterExpression rightParam = Expression.Parameter(type, "right");
            BinaryExpression body;

            try
            {
                body = PrimitiveExpressionGenerator(leftParam, rightParam);
            }
            catch (InvalidOperationException)
            {
                return null;
            }

            Delegate expr = Expression.Lambda(body, true, leftParam, rightParam).Compile();

            return new DynamicBinaryOperator(expr, type, type, type);
        }

        /// <summary>
        /// Get the position of the right-most valid operator token.
        /// </summary>
        /// <param name="value">The string to find the operator in.</param>
        /// <returns>The position of the operator. -1 if none can be found</returns>
        protected virtual int GetOperatorPosition(string value)
        {
            return TextProcessing.GetScopedSplitPoints(value, OperatorToken, TextProcessing.DefaultLeftScopers, TextProcessing.DefaultRightScopers).LastOr(-1);
        }

        public object Parse(string value, Type type, Func<string, Type, object> recursiveParser)
        {
            IBinaryOperator operatorData = _foundOperatorTable[type];

            int splitIndex = GetOperatorPosition(value);
            string left = value.Substring(0, splitIndex);
            string right = value.Substring(splitIndex + 1);

            object leftVal = recursiveParser(left, operatorData.LArg);
            object rightVal = recursiveParser(right, operatorData.RArg);

            try
            {
                return operatorData.Invoke(leftVal, rightVal);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
        }
    }
}
