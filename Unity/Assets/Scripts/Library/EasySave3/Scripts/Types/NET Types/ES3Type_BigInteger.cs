using System.Numerics;
using UnityEngine;

namespace ES3Types
{
    [UnityEngine.Scripting.Preserve]
    [ES3Properties("bytes")]
    public class ES3Type_BigInteger : ES3Type
    {
        public static ES3Type Instance = null;

        public ES3Type_BigInteger() : base(typeof(BigInteger))
        {
            Instance = this;
        }

        public override void Write(object obj, ES3Writer writer)
        {
            BigInteger casted = (BigInteger)obj;
            writer.WriteProperty("bytes", casted.ToByteArray(), ES3Type_byteArray.Instance);
        }

        public override object Read<T>(ES3Reader reader)
        {
            return new BigInteger(reader.ReadProperty<byte[]>(ES3Type_byteArray.Instance));
        }
    }

    public class ES3Type_BigIntegerArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3Type_BigIntegerArray() : base(typeof(BigInteger[]), ES3Type_BigInteger.Instance)
        {
            Instance = this;
        }
    }
}