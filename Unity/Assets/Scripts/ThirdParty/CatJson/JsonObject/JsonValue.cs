using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CatJson
{
    /// <summary>
    /// Json值
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public class JsonValue
    {
        [FieldOffset(0)]
        public ValueType Type;
        
        [FieldOffset(1)]
        private bool boolean;
        
        [FieldOffset(1)]
        private double number;
        
        [FieldOffset(8)]
        private string str;
        
        [FieldOffset(8)]
        private List<JsonValue> array;
        
        [FieldOffset(8)]
        private JsonObject obj;

        #region 构造方法

        public JsonValue()
        {
            Type = ValueType.Null;
        }
        
        public JsonValue(bool b)
        {
            Type = ValueType.Boolean;
            boolean = b;
        }
        public JsonValue(double d)
        {
            Type = ValueType.Number;
            number = d;
        }
        public JsonValue(string s)
        {
            Type = ValueType.String;
            str = s;
        }
        public JsonValue(List<JsonValue> arr)
        {
            Type = ValueType.Array;
            array = arr;
        }
        public JsonValue(JsonObject jo)
        {
            Type = ValueType.Object;
            obj = jo;
        }

        #endregion
        
        
        public JsonValue this[int index]
        {
            get
            {
                if (Type != ValueType.Array)
                {
                    return default;
                }

                return array[index];
            }
            set
            {
                if (Type != ValueType.Array)
                {
                    return;
                }

                array[index] = value;
            }
        }

        public JsonValue this[string key]
        {
            get
            {
                if (Type != ValueType.Object)
                {
                    return default;
                }

                return obj[key];
            }
            set
            {
                if (Type != ValueType.Object)
                {
                    return;
                }

                obj[key] = value;
            }
        }

        #region 隐式类型转换

        public static implicit operator JsonValue(bool b)
        {
            JsonValue value = new JsonValue(b);
            return value;
        }
        
        public static implicit operator bool(JsonValue value)
        {
            if (value.Type != ValueType.Boolean)
            {
                throw new Exception("JsonValue转换bool失败");
            }
            return value.boolean;
        }

        public static implicit operator JsonValue(double d)
        {
            JsonValue value = new JsonValue(d);
            return value;
        }
        
        public static implicit operator double(JsonValue value)
        {
            if (value.Type != ValueType.Number)
            {
                throw new Exception("JsonValue转换double失败");
            }
            
            return value.number;
        }
        
        public static implicit operator JsonValue(string s)
        {
            JsonValue value = new JsonValue(s);
            return value;
        }
        
        public static implicit operator string(JsonValue value)
        {
            if (value.Type != ValueType.String)
            {
                throw new Exception("JsonValue转换string失败");
            }
            return value.str;
        }
        public static implicit operator JsonValue(JsonObject obj)
        {
            JsonValue value = new JsonValue(obj);
            return value;
        }
        
        public static implicit operator List<JsonValue>(JsonValue value)
        {
            if (value.Type != ValueType.Array)
            {
                throw new Exception("JsonValue转换List<JsonValue>失败");
            }

            return value.array;
        }
        public static implicit operator JsonValue(List<JsonValue> arr)
        {
            JsonValue value = new JsonValue(arr);
            return value;
        }
        
        public static implicit operator JsonObject(JsonValue value)
        {
            if (value.Type != ValueType.Object)
            {
                throw new Exception("JsonValue转换JsonObject失败");
            }

            return value.obj;
        }

        #endregion
        
        
        public override string ToString()
        {
            string json = JsonParser.Default.ToJson(this);
            return json;
        }
    }

}
