using System;
using System.Collections.Generic;

namespace QFSW.QC.Parsers
{
    public class TupleParser : MassGenericQcParser
    {
        private const int MaxFlatTupleSize = 8;

        protected override HashSet<Type> GenericTypes { get; } = new HashSet<Type>
        {
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>),
            typeof(Tuple<>),
            typeof(Tuple<,>),
            typeof(Tuple<,,>),
            typeof(Tuple<,,,>),
            typeof(Tuple<,,,,>),
            typeof(Tuple<,,,,,>),
            typeof(Tuple<,,,,,,>),
            typeof(Tuple<,,,,,,,>)
        };

        public override object Parse(string value, Type type)
        {
            string[] inputParts = value.ReduceScope('(', ')').SplitScoped(',', MaxFlatTupleSize);
            Type[] elementTypes = type.GetGenericArguments();

            if (elementTypes.Length != inputParts.Length)
            {
                throw new ParserInputException($"Desired tuple type {type} has {elementTypes.Length} elements but input contained {inputParts.Length}.");
            }

            object[] tupleParts = new object[inputParts.Length];
            for (int i = 0; i < tupleParts.Length; i++)
            {
                tupleParts[i] = ParseRecursive(inputParts[i], elementTypes[i]);
            }

            return Activator.CreateInstance(type, tupleParts);
        }
    }
}
