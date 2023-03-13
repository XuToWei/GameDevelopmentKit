using System;
using System.Reflection;

namespace QFSW.QC.Grammar
{
    internal class BinaryOperatorData : IBinaryOperator
    {
        public Type LArg { get; }
        public Type RArg { get; }
        public Type Ret { get; }

        private readonly MethodInfo _method;

        public BinaryOperatorData(MethodInfo OperatorMethod)
        {
            _method = OperatorMethod;
            Ret = OperatorMethod.ReturnType;

            ParameterInfo[] paramData = _method.GetParameters();
            if (paramData.Length != 2)
            {
                throw new ArgumentException($"Cannot create a binary operator from a method with {paramData.Length} parameters");
            }

            LArg = paramData[0].ParameterType;
            RArg = paramData[1].ParameterType;
        }

        public object Invoke(object left, object right)
        {
            return _method.Invoke(null, new[] { left, right });
        }
    }
}