using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ES3Internal;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3TupleType : ES3Type
	{
        public ES3Type[] es3Types;
        public Type[] types;

		protected ES3Reflection.ES3ReflectedMethod readMethod = null;
		protected ES3Reflection.ES3ReflectedMethod readIntoMethod = null;

		public ES3TupleType(Type type) : base(type)
		{
			types = ES3Reflection.GetElementTypes(type);
            es3Types = new ES3Type[types.Length];

            for(int i=0; i<types.Length; i++)
            {
                es3Types[i] = ES3TypeMgr.GetOrCreateES3Type(types[i], false);
                if (es3Types[i] == null)
                    isUnsupported = true;
            }

			isTuple = true;
		}

        public override void Write(object obj, ES3Writer writer)
		{
			Write(obj, writer, writer.settings.memberReferenceMode);
		}

		public void Write(object obj, ES3Writer writer, ES3.ReferenceMode memberReferenceMode)
		{
            if (obj == null) { writer.WriteNull(); return; };

            writer.StartWriteCollection();

            for (int i=0; i<es3Types.Length; i++)
            {
                var itemProperty = ES3Reflection.GetProperty(type, "Item"+(i+1));
                var item = itemProperty.GetValue(obj);
                writer.StartWriteCollectionItem(i);
                writer.Write(item, es3Types[i], memberReferenceMode);
                writer.EndWriteCollectionItem(i);
            }

            writer.EndWriteCollection();
		}

        public override object Read<T>(ES3Reader reader)
        {
            var objects = new object[types.Length];
            
            if (reader.StartReadCollection())
                return null;

            for(int i=0; i<types.Length; i++)
            {
                reader.StartReadCollectionItem();
                objects[i] = reader.Read<object>(es3Types[i]);
                reader.EndReadCollectionItem();
            }

            reader.EndReadCollection();

            var constructor = type.GetConstructor(types);
            var instance = constructor.Invoke(objects);

            return instance;
        }
    }
}