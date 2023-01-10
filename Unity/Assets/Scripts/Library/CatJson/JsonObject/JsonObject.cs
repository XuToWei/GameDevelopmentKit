using System.Collections;
using System.Collections.Generic;

namespace CatJson
{
    /// <summary>
    /// json对象
    /// </summary>
    public class JsonObject
    {
        internal Dictionary<string, JsonValue> ValueDict;

        public JsonValue this[string key]
        {
            get
            {
                if (ValueDict == null)
                {
                    return default;
                }

                return ValueDict[key];
            }

            set
            {
                if (ValueDict == null)
                {
                    ValueDict = new Dictionary<string, JsonValue>();
                }
                ValueDict[key] = value;
            }
        }
        
        public override string ToString()
        {
            string json = JsonParser.Default.ToJson(this);
            return json;
        }
    }
}

