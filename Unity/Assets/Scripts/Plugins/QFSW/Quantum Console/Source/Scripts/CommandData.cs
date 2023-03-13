using QFSW.QC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QFSW.QC
{
    /// <summary>
    /// Contains the full data about a command and provides an execution point for invoking the command.
    /// </summary>
    public class CommandData
    {
        public readonly string CommandName;
        public readonly string CommandDescription;
        public readonly string CommandSignature;
        public readonly string ParameterSignature;
        public readonly string GenericSignature;

        public readonly ParameterInfo[] MethodParamData;
        public readonly Type[] ParamTypes;
        public readonly Type[] GenericParamTypes;
        public readonly MethodInfo MethodData;
        public readonly MonoTargetType MonoTarget;

        private readonly object[] _defaultParameters;

        public bool IsGeneric => GenericParamTypes.Length > 0;
        public bool IsStatic => MethodData.IsStatic;
        public bool HasDescription => !string.IsNullOrWhiteSpace(CommandDescription);
        public int ParamCount => ParamTypes.Length - _defaultParameters.Length;

        public Type[] MakeGenericArguments(params Type[] genericTypeArguments)
        {
            if (genericTypeArguments.Length != GenericParamTypes.Length)
            {
                throw new ArgumentException("Incorrect number of generic substitution types were supplied.");
            }

            Dictionary<string, Type> substitutionTable = new Dictionary<string, Type>();
            for (int i = 0; i < genericTypeArguments.Length; i++)
            {
                substitutionTable.Add(GenericParamTypes[i].Name, genericTypeArguments[i]);
            }

            Type[] types = new Type[ParamTypes.Length];
            for (int i = 0; i < types.Length; i++)
            {
                if (ParamTypes[i].ContainsGenericParameters)
                {
                    Type substitution = ConstructGenericType(ParamTypes[i], substitutionTable);
                    types[i] = substitution;
                }
                else
                {
                    types[i] = ParamTypes[i];
                }
            }

            return types;
        }

        private Type ConstructGenericType(Type genericType, Dictionary<string, Type> substitutionTable)
        {
            if (!genericType.ContainsGenericParameters) { return genericType; }
            if (substitutionTable.ContainsKey(genericType.Name)) { return substitutionTable[genericType.Name]; }
            if (genericType.IsArray) { return ConstructGenericType(genericType.GetElementType(), substitutionTable).MakeArrayType(); }
            if (genericType.IsGenericType)
            {
                Type baseType = genericType.GetGenericTypeDefinition();
                Type[] typeArguments = genericType.GetGenericArguments();
                for (int i = 0; i < typeArguments.Length; i++)
                {
                    typeArguments[i] = ConstructGenericType(typeArguments[i], substitutionTable);
                }

                return baseType.MakeGenericType(typeArguments);
            }

            throw new ArgumentException($"Could not construct the generic type {genericType}");
        }

        public object Invoke(object[] paramData, Type[] genericTypeArguments)
        {
            object[] data = new object[paramData.Length + _defaultParameters.Length];
            Array.Copy(paramData, 0, data, 0, paramData.Length);
            Array.Copy(_defaultParameters, 0, data, paramData.Length, _defaultParameters.Length);

            MethodInfo invokingMethod = GetInvokingMethod(genericTypeArguments);

            if (IsStatic)
            {
                return invokingMethod.Invoke(null, data);
            }

            IEnumerable<object> targets = GetInvocationTargets(invokingMethod);
            return InvocationTargetFactory.InvokeOnTargets(invokingMethod, targets, data);
        }

        protected virtual IEnumerable<object> GetInvocationTargets(MethodInfo invokingMethod)
        {
            return InvocationTargetFactory.FindTargets(invokingMethod.DeclaringType, MonoTarget);
        }

        private MethodInfo GetInvokingMethod(Type[] genericTypeArguments)
        {
            if (!IsGeneric)
            {
                return MethodData;
            }

            T WrapConstruction<T>(Func<T> f)
            {
                try
                {
                    return f();
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException($"Supplied generic parameters did not satisfy the generic constraints imposed by '{CommandName}'");
                }
            }

            Type declaringType = MethodData.DeclaringType;
            MethodInfo method = MethodData;

            if (declaringType.IsGenericTypeDefinition)
            {
                int typeCount = declaringType.GetGenericArguments().Length;

                Type[] genericTypes = genericTypeArguments
                    .Take(typeCount)
                    .ToArray();

                genericTypeArguments = genericTypeArguments
                    .Skip(typeCount)
                    .ToArray();

                declaringType = WrapConstruction(() => declaringType.MakeGenericType(genericTypes));
                method = method.RebaseMethod(declaringType);
            }

            return genericTypeArguments.Length == 0
                ? method
                : WrapConstruction(() => method.MakeGenericMethod(genericTypeArguments));
        }

        private string BuildPrefix(Type declaringType)
        {
            List<string> prefixes = new List<string>();
            Assembly assembly = declaringType.Assembly;

            void AddPrefixes(IEnumerable<CommandPrefixAttribute> prefixAttributes, string defaultName)
            {
                foreach (CommandPrefixAttribute prefixAttribute in prefixAttributes.Reverse())
                {
                    if (prefixAttribute.Valid)
                    {
                        string prefix = prefixAttribute.Prefix;
                        if (string.IsNullOrWhiteSpace(prefix)) { prefix = defaultName; }

                        prefixes.Add(prefix);
                    }
                }
            }

            while (declaringType != null)
            {
                IEnumerable<CommandPrefixAttribute> typePrefixes = declaringType.GetCustomAttributes<CommandPrefixAttribute>();
                AddPrefixes(typePrefixes, declaringType.Name);

                declaringType = declaringType.DeclaringType;
            }

            IEnumerable<CommandPrefixAttribute> assemblyPrefixes = assembly.GetCustomAttributes<CommandPrefixAttribute>();
            AddPrefixes(assemblyPrefixes, assembly.GetName().Name);

            return string.Join("", prefixes.Reversed());
        }

        private string BuildGenericSignature(Type[] genericParamTypes)
        {
            if (genericParamTypes.Length == 0)
            {
                return string.Empty;
            }

            IEnumerable<string> names = genericParamTypes.Select(x => x.Name);
            return $"<{string.Join(", ", names)}>";
        }

        private string BuildParameterSignature(ParameterInfo[] methodParams, int defaultParameterCount)
        {
            string signature = string.Empty;
            for (int i = 0; i < methodParams.Length - defaultParameterCount; i++)
            {
                signature += $"{(i == 0 ? string.Empty : " ")}{methodParams[i].Name}";
            }

            return signature;
        }

        private Type[] BuildGenericParamTypes(MethodInfo method, Type declaringType)
        {
            List<Type> types = new List<Type>();

            if (declaringType.IsGenericTypeDefinition)
            {
                types.AddRange(declaringType.GetGenericArguments());
            }

            if (method.IsGenericMethodDefinition)
            {
                types.AddRange(method.GetGenericArguments());
            }

            return types.ToArray();
        }

        public CommandData(MethodInfo methodData, int defaultParameterCount = 0) : this(methodData, methodData.Name, defaultParameterCount) { }
        public CommandData(MethodInfo methodData, string commandName, int defaultParameterCount = 0)
        {
            CommandName = commandName;
            MethodData = methodData;

            if (string.IsNullOrWhiteSpace(commandName))
            {
                CommandName = methodData.Name;
            }

            Type declaringType = methodData.DeclaringType;

            string prefix = BuildPrefix(declaringType);
            CommandName = $"{prefix}{CommandName}";

            MethodParamData = methodData.GetParameters();
            ParamTypes = MethodParamData
                .Select(x => x.ParameterType)
                .ToArray();

            _defaultParameters = new object[defaultParameterCount];
            for (int i = 0; i < defaultParameterCount; i++)
            {
                int j = MethodParamData.Length - defaultParameterCount + i;
                _defaultParameters[i] = MethodParamData[j].DefaultValue;
            }

            GenericParamTypes = BuildGenericParamTypes(methodData, declaringType);

            ParameterSignature = BuildParameterSignature(MethodParamData, defaultParameterCount);
            GenericSignature = BuildGenericSignature(GenericParamTypes);
            CommandSignature = ParamCount > 0
                ? $"{CommandName}{GenericSignature} {ParameterSignature}"
                : $"{CommandName}{GenericSignature}";
        }

        public CommandData(MethodInfo methodData, CommandAttribute commandAttribute, int defaultParameterCount = 0) : this(methodData, commandAttribute.Alias, defaultParameterCount)
        {
            CommandDescription = commandAttribute.Description;
            MonoTarget = commandAttribute.MonoTarget;
        }

        public CommandData(MethodInfo methodData, CommandAttribute commandAttribute, CommandDescriptionAttribute descriptionAttribute, int defaultParameterCount = 0)
            : this(methodData, commandAttribute, defaultParameterCount)
        {
            if ((descriptionAttribute?.Valid ?? false) && string.IsNullOrWhiteSpace(commandAttribute.Description))
            {
                CommandDescription = descriptionAttribute.Description;
            }
        }
    }
}
