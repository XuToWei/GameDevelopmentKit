using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace QFSW.QC.Parsers
{
    public class ColorParser : BasicCachedQcParser<Color>
    {
        private readonly Dictionary<string, Color> _colorLookup;

        public ColorParser()
        {
            _colorLookup = new Dictionary<string, Color>();

            PropertyInfo[] colorProperties = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (PropertyInfo prop in colorProperties)
            {
                if (prop.CanRead && !prop.CanWrite)
                {
                    MethodInfo propReader = prop.GetMethod;
                    if (propReader.ReturnType == typeof(Color))
                    {
                        _colorLookup.Add(prop.Name, (Color)propReader.Invoke(null, Array.Empty<object>()));
                    }
                }
            }
        }

        public override Color Parse(string value)
        {
            if (_colorLookup.ContainsKey(value.ToLower()))
            {
                return _colorLookup[value.ToLower()];
            }

            try
            {
                if (value.StartsWith("0x"))
                {
                    return ParseHexColor(value);
                }
                else
                {
                    return ParseRGBAColor(value);
                }
            }
            catch (FormatException e)
            {
                throw new ParserInputException($"{e.Message}\nThe format must be either of:" +
                                                          $"\n   - R,G,B" +
                                                          $"\n   - R,G,B,A" +
                                                          $"\n   - 0xRRGGBB" +
                                                          $"\n   - 0xRRGGBBAA" +
                                                          $"\n   - A preset color such as 'red'", e);
            }
        }

        private Color ParseRGBAColor(string value)
        {
            string[] colorParts = value.Split(',');
            Color parsedColor = Color.white;
            int i = 0;

            if (colorParts.Length < 3 || colorParts.Length > 4) { throw new FormatException($"Cannot parse '{value}' as a Color."); }

            float ParsePart(string part)
            {
                float val = float.Parse(part);
                if (val < 0 || val > 1) { throw new FormatException($"{val} falls outside of the valid [0,1] range for a component of a Color."); }
                return val;
            }

            try
            {
                for (; i < colorParts.Length; i++)
                {
                    parsedColor[i] = ParsePart(colorParts[i]);
                }

                return parsedColor;
            }
            catch (FormatException)
            {
                throw new FormatException($"Cannot parse '{colorParts[i]}' as part of a Color, it must be numerical and in the valid range [0,1].");
            }
        }

        private Color ParseHexColor(string value)
        {
            int digitCount = value.Length - 2;
            if (digitCount != 6 && digitCount != 8)
            {
                throw new FormatException("Hex colors must contain either 6 or 8 hex digits.");
            }

            Color parsedColor = Color.white;
            int byteCount = digitCount / 2;
            int i = 0;

            try
            {
                for (; i < byteCount; i++)
                {
                    parsedColor[i] = int.Parse(value.Substring(2 * (1 + i), 2), System.Globalization.NumberStyles.HexNumber) / 255f;
                }

                return parsedColor;
            }
            catch (FormatException)
            {
                throw new FormatException($"Cannot parse '{value.Substring(2 * (1 + i), 2)}' as part of a Color as it was invalid hex.");
            }
        }
    }
}
