using System;
using UnityEngine.Profiling;


namespace CatJson
{
    /// <summary>
    /// 范围字符串
    /// 表示在Source字符串中，从StartIndex到EndIndex范围的字符构成的字符串
    /// </summary>
    public struct RangeString : IEquatable<RangeString>
    {
        /// <summary>
        /// 源字符串
        /// </summary>
        private string source;

        /// <summary>
        /// 开始索引
        /// </summary>
        private int startIndex;

        /// <summary>
        /// 结束索引
        /// </summary>
        private int endIndex;

        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; }
        
        /// <summary>
        /// 哈希码
        /// </summary>
        private int hashCode;

        public char this[int index] => source[startIndex + index];

        public RangeString(string source) : this(source,0,source.Length - 1)
        {
        }

        public RangeString(string source, int startIndex, int endIndex)
        {
            this.source = source;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            Length = (endIndex - startIndex) + 1;
            hashCode = 0;
        }

        public bool Equals(RangeString other)
        {
            bool isSourceNullOrEmpty = string.IsNullOrEmpty(source);
            bool isOtherNullOrEmpty = string.IsNullOrEmpty(other.source);

            if (isSourceNullOrEmpty && isOtherNullOrEmpty)
            {
                return true;
            }

            if (isSourceNullOrEmpty || isOtherNullOrEmpty)
            {
                return false;
            }

            if (Length != other.Length)
            {
                return false;
            }

            for (int i = startIndex, j = other.startIndex; i <= endIndex; i++, j++)
            {
                if (source[i] != other.source[j])
                {
                    return false;
                }
            }
            
            return true;
            
      
        }

        public bool Equals(string str)
        {
            return Equals(new RangeString(str));
        }
        
        public override int GetHashCode()
        {
            if (hashCode == 0 && !string.IsNullOrEmpty(source))
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    hashCode = 31 * hashCode + source[i];
                }
            }

            return hashCode;
        }

        public override string ToString()
        {
            string str = ToString(JsonParser.Default);
            return str;
        }

        public string ToString(JsonParser parser)
        {
            if (endIndex - startIndex + 1 == 0)
            {
                //长度为0 处理空字符串的情况
                return string.Empty;
            }

            if (startIndex == 0 && endIndex == source.Length - 1)
            {
                //处理表示整个source范围的情况
                return source;
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                char c = source[i];

                if (c != '\\')
                {
                    //普通字符
                    parser.CachedSB.Append(source[i]);
                    continue;
                }

                //处理转义字符

                if (i == endIndex)
                {
                    throw new Exception("处理转义字符失败，\\后没有剩余字符");
                }

                //检测\后的下一个字符
                i++;
                c = source[i];
                switch (c)
                {
                    case '"':
                        parser.CachedSB.Append('\"');
                        break;
                    case '\\':
                        parser.CachedSB.Append('\\');
                        break;
                    case '/':
                        parser.CachedSB.Append('/');
                        break;
                    case 'b':
                        parser.CachedSB.Append('\b');
                        break;
                    case 'f':
                        parser.CachedSB.Append('\f');
                        break;
                    case 'n':
                        parser.CachedSB.Append('\n');
                        break;
                    case 'r':
                        parser.CachedSB.Append('\r');
                        break;
                    case 't':
                        parser.CachedSB.Append('\t');
                        break;
                    case 'u':
                        //unicode字符
                        char codePoint = TextUtil.GetUnicodeCodePoint(source[i + 1], source[i + 2], source[i + 3], source[i + 4]);
                        parser.CachedSB.Append(codePoint);
                        i += 4;
                        break;
                    default:
                        throw new Exception("处理转义字符失败，\\后的字符不在可转义范围内");
                }



            }

            string str = parser.CachedSB.ToString();
            parser.CachedSB.Clear();

            return str;
        }
        
#if UNITY_2021_2_OR_NEWER
        public ReadOnlySpan<char> AsSpan()
        {
            ReadOnlySpan<char> span = source.AsSpan(startIndex, Length);
            return span;
        }

        public byte AsByte()
        {
            return byte.Parse(AsSpan());
        }
        
        public sbyte AsSByte()
        {
            return sbyte.Parse(AsSpan());
        }

        public short AsShort()
        {
            return short.Parse(AsSpan());
        }
        
        public ushort AsUShort()
        {
            return ushort.Parse(AsSpan());
        }
        
        public int AsInt()
        {
            return int.Parse(AsSpan());
        }
        
        public uint AsUInt()
        {
            return uint.Parse(AsSpan());
        }

        public long AsLong()
        {
            return long.Parse(AsSpan());
        }
        
        public ulong AsULong()
        {
            return ulong.Parse(AsSpan());
        }
        
        public float AsFloat()
        {
            return float.Parse(AsSpan());
        }

        public double AsDouble()
        {
            return double.Parse(AsSpan());
        }

        public decimal AsDecimal()
        {
            return decimal.Parse(AsSpan());
        }

        public DateTime AsDateTime()
        {
            return DateTime.Parse(AsSpan());
        }
#else
        public byte AsByte()
        {
            return byte.Parse(ToString());
        }
        
        public sbyte AsSByte()
        {
            return sbyte.Parse(ToString());
        }

        public short AsShort()
        {
            return short.Parse(ToString());
        }
        
        public ushort AsUShort()
        {
            return ushort.Parse(ToString());
        }
        
        public int AsInt()
        {
            return int.Parse(ToString());
        }
        
        public uint AsUInt()
        {
            return uint.Parse(ToString());
        }

        public long AsLong()
        {
            return long.Parse(ToString());
        }
        
        public ulong AsULong()
        {
            return ulong.Parse(ToString());
        }
        
        public float AsFloat()
        {
            return float.Parse(ToString());
        }

        public double AsDouble()
        {
            return double.Parse(ToString());
        }

        public decimal AsDecimal()
        {
            return decimal.Parse(ToString());
        }

        public DateTime AsDateTime()
        {
            return DateTime.Parse(ToString());
        }
#endif
        
        
    }
}
