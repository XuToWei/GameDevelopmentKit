using System;
using System.Collections.Generic;

namespace CatJson
{
    /// <summary>
    /// JsonObject类型的Json格式化器
    /// </summary>
    public class JsonObjectFormatter : BaseJsonFormatter<JsonObject>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, JsonObject value, Type type, Type realType, int depth)
        {
            parser.AppendLine("{");

            if (value.ValueDict != null)
            {
                int index = 0;
                foreach (KeyValuePair<string, JsonValue> item in value.ValueDict)
                {
                    
                    parser.Append("\"", depth);
                    parser.Append(item.Key);
                    parser.Append("\"");

                    parser.Append(":");

                    parser.ToJson(item.Value,depth + 1);

                    if (index < value.ValueDict.Count-1)
                    {
                        parser.AppendLine(",");
                    }
                    index++;
                }
            }

            parser.AppendLine(string.Empty);
            parser.Append("}", depth - 1);
        }

        /// <inheritdoc />
        public override JsonObject ParseJson(JsonParser parser, Type type, Type realType)
        {
            JsonObject obj = new JsonObject();

            ParserHelper.ParseJsonObjectProcedure(parser, obj, default, default, (localParser, userdata1, userdata2, userdata3, key) =>
            {
                JsonObject localObj = (JsonObject) userdata1;
                JsonValue value = localParser.ParseJson<JsonValue>();
                localObj[key.ToString()] = value;
            });

            return obj;
        }


    }
}