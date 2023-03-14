using QFSW.QC.Pooling;
using System;
using System.Collections;
using System.Text;

namespace QFSW.QC.Serializers
{
    public class IEnumerableSerializer : IEnumerableSerializer<IEnumerable>
    {
        public override int Priority => base.Priority - 1000;

        protected override IEnumerable GetObjectStream(IEnumerable value)
        {
            return value;
        }
    }

    public abstract class IEnumerableSerializer<T> : PolymorphicQcSerializer<T> where T : class, IEnumerable
    {
        private readonly StringBuilderPool _builderPool = new StringBuilderPool();

        public override string SerializeFormatted(T value, QuantumTheme theme)
        {
            Type type = value.GetType();
            StringBuilder builder = _builderPool.GetStringBuilder();

            string left = "[";
            string seperator = ",";
            string right = "]";
            if (theme)
            {
                theme.GetCollectionFormatting(type, out left, out seperator, out right);
            }

            builder.Append(left);

            bool firstIteration = true;
            foreach (object item in GetObjectStream(value))
            {
                if (firstIteration)
                {
                    firstIteration = false;
                }
                else
                {
                    builder.Append(seperator);
                }

                builder.Append(SerializeRecursive(item, theme));
            }

            builder.Append(right);

            return _builderPool.ReleaseAndToString(builder);
        }

        protected abstract IEnumerable GetObjectStream(T value);
    }
}
