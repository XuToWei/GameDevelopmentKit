using QFSW.QC.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace QFSW.QC
{
    [CreateAssetMenu(fileName = "Untitled Theme", menuName = "Quantum Console/Theme")]
    public class QuantumTheme : ScriptableObject
    {
        [SerializeField] public TMP_FontAsset Font = null;
        [SerializeField] public Material PanelMaterial = null;
        [SerializeField] public Color PanelColor = Color.white;

        [SerializeField] public Color CommandLogColor = new Color(0, 1, 1);
        [SerializeField] public Color SelectedSuggestionColor = new Color(1, 1, 0.55f);
        [SerializeField] public Color SuggestionColor = Color.gray;
        [SerializeField] public Color ErrorColor = Color.red;
        [SerializeField] public Color WarningColor = new Color(1, 0.5f, 0);
        [SerializeField] public Color SuccessColor = Color.green;

        [SerializeField] public string TimestampFormat = "[{0}:{1}:{2}]";
        [SerializeField] public string CommandLogFormat = "> {0}";

        [SerializeField] public Color DefaultReturnValueColor = Color.white;
        [SerializeField] public List<TypeColorFormatter> TypeFormatters = new List<TypeColorFormatter>(0);
        [SerializeField] public List<CollectionFormatter> CollectionFormatters = new List<CollectionFormatter>(0);

        private T FindTypeFormatter<T>(List<T> formatters, Type type) where T : TypeFormatter
        {
            foreach (T formatter in formatters)
            {
                if (type == formatter.Type || type.IsGenericTypeOf(formatter.Type))
                {
                    return formatter;
                }
            }

            foreach (T formatter in formatters)
            {
                if (formatter.Type.IsAssignableFrom(type))
                {
                    return formatter;
                }
            }

            return null;
        }

        public string ColorizeReturn(string data, Type type)
        {
            TypeColorFormatter formatter = FindTypeFormatter(TypeFormatters, type);
            if (formatter == null) { return data.ColorText(DefaultReturnValueColor); }
            else { return data.ColorText(formatter.Color); }
        }

        public void GetCollectionFormatting(Type type, out string leftScoper, out string seperator, out string rightScoper)
        {
            CollectionFormatter formatter = FindTypeFormatter(CollectionFormatters, type);
            if (formatter == null)
            {
                leftScoper = "[";
                seperator = ",";
                rightScoper = "]";
            }
            else
            {
                leftScoper = formatter.LeftScoper.Replace("\\n", "\n");
                seperator = formatter.SeperatorString.Replace("\\n", "\n");
                rightScoper = formatter.RightScoper.Replace("\\n", "\n");
            }
        }
    }
}
