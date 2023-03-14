using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace QFSW.QC.Parsers
{
    public class CollectionParser : MassGenericQcParser
    {
        protected override HashSet<Type> GenericTypes { get; } = new HashSet<Type>
        {
            typeof(List<>),
            typeof(Stack<>),
            typeof(Queue<>),
            typeof(HashSet<>),
            typeof(LinkedList<>),
            typeof(ConcurrentStack<>),
            typeof(ConcurrentQueue<>),
            typeof(ConcurrentBag<>)
        };

        public override object Parse(string value, Type type)
        {
            Type arrayType = type.GetGenericArguments()[0].MakeArrayType();
            object array = ParseRecursive(value, arrayType);
            return Activator.CreateInstance(type, array);
        }
    }
}
