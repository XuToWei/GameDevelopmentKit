using System;
using System.Collections;
using System.Collections.Generic;

namespace CatJson
{
    /// <summary>
    /// List类型的Json格式化器
    /// </summary>
    public class ListFormatter : BaseJsonFormatter<IList>
    {
        /// <inheritdoc />
        public override void ToJson(JsonParser parser, IList value, Type type, Type realType, int depth)
        {
            parser.AppendLine("[");
            Type listType = type;
            if (!listType.IsGenericType)
            {
                //此处的处理原因类似ArrayFormatter
                listType = realType;
            }
            Type elementType = TypeUtil.GetArrayOrListElementType(listType);
            for (int i = 0; i < value.Count; i++)
            {
                object element = value[i];
                parser.AppendTab(depth);
                if (element == null)
                {
                    parser.Append("null");
                }
                else
                {
                    parser.InternalToJson(element,elementType,null,depth + 1);
                }
                if (i < value.Count - 1)
                {
                    parser.AppendLine(",");
                }
                 
            }
            parser.AppendLine(string.Empty);
            parser.Append("]", depth - 1);
        }

        /// <inheritdoc />
        public override IList ParseJson(JsonParser parser, Type type, Type realType)
        {
            IList list = (IList)TypeUtil.CreateInstance(realType,parser.IsUseParamCtor);
            
            Type listType = type;
            if (!listType.IsGenericType)
            {
                listType = realType;
            }
            Type elementType = TypeUtil.GetArrayOrListElementType(listType);
            
            ParserHelper.ParseJsonArrayProcedure(parser,list, elementType, (userdata1, userdata2) =>
            {
                IList localList = (IList) userdata1;
                Type localElementType = (Type) userdata2;
                
                object value = parser.InternalParseJson(localElementType);
                localList.Add(value);
            });

            return list;
        }
    }
}