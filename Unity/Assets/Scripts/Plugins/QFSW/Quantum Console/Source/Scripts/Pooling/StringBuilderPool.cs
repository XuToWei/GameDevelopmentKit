using System.Text;

namespace QFSW.QC.Pooling
{
    public class ConcurrentStringBuilderPool : StringBuilderPool<ConcurrentPool<StringBuilder>>
    {

    }

    public class StringBuilderPool : StringBuilderPool<Pool<StringBuilder>>
    {

    }

    public class StringBuilderPool<TPool> where TPool : IPool<StringBuilder>, new()
    {
        private readonly TPool _pool = new TPool();

        public StringBuilder GetStringBuilder(int minCapacity = 0)
        {
            StringBuilder stringBuilder = _pool.GetObject();
            stringBuilder.Clear();
            stringBuilder.EnsureCapacity(minCapacity);

            return stringBuilder;
        }

        public string ReleaseAndToString(StringBuilder stringBuilder)
        {
            string result = stringBuilder.ToString();
            _pool.Release(stringBuilder);

            return result;
        }
    }
}