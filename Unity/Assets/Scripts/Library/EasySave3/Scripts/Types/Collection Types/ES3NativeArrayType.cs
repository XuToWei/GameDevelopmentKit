using System;
using System.Collections;
using System.Collections.Generic;
using ES3Internal;
using System.Linq;
using Unity.Collections;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3NativeArrayType : ES3CollectionType
	{
		public ES3NativeArrayType(Type type) : base(type){}
		public ES3NativeArrayType(Type type, ES3Type elementType) : base(type, elementType){}

		public override void Write(object obj, ES3Writer writer, ES3.ReferenceMode memberReferenceMode)
		{
            if (elementType == null)
                throw new ArgumentNullException("ES3Type argument cannot be null.");

            var enumerable = (IEnumerable)obj;

            int i = 0;
            foreach(var item in enumerable)
            {
                writer.StartWriteCollectionItem(i);
                writer.Write(item, elementType, memberReferenceMode);
                writer.EndWriteCollectionItem(i);
                i++;
            }
		}

        public override object Read(ES3Reader reader)
        {
            var array = ReadAsArray(reader);

            return ES3Reflection.CreateInstance(type, new object[] { array, Allocator.Persistent });
        }

        public override object Read<T>(ES3Reader reader)
		{
            return Read(reader);
		}

		public override void ReadInto<T>(ES3Reader reader, object obj)
		{
            ReadInto(reader, obj);
		}

		public override void ReadInto(ES3Reader reader, object obj)
		{
            var array = ReadAsArray(reader);
            var copyFromMethod = ES3Reflection.GetMethods(type, "CopyFrom").First(m => ES3Reflection.TypeIsArray(m.GetParameters()[0].GetType()));
            copyFromMethod.Invoke(obj, new object[] { array });
        }

        private System.Array ReadAsArray(ES3Reader reader)
        {
            var list = new List<object>();
            if (!ReadICollection(reader, list, elementType))
                return null;

            var array = ES3Reflection.ArrayCreateInstance(elementType.type, list.Count);
            int i = 0;
            foreach (var item in list)
            {
                array.SetValue(item, i);
                i++;
            }

            return array;
        }
	}
}