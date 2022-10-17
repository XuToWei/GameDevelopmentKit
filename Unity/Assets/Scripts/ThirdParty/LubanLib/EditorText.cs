using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bright.Config
{
    public class EditorText
    {
        public string Key { get; }

        public string Text { get; }

        public EditorText(string key, string text)
        {
            Key = key;
            Text = text;
        }

        public static EditorText LoadJson(SimpleJSON.JSONNode json)
        {
            string key = json["key"];
            if (key == null)
            {
                throw new Exception("key missing");
            }
            string text = json["text"];
            if (text == null)
            {
                throw new Exception("text missing");
            }
            return new EditorText(key, text);
        }

        public static SimpleJSON.JSONObject SaveJson(EditorText text)
        {
            var json = new SimpleJSON.JSONObject();
            if (string.IsNullOrEmpty(text.Key))
            {
                throw new Exception("key can't be null");
            }
            if (string.IsNullOrEmpty(text.Text))
            {
                throw new Exception("text can't be null");
            }
            json["key"] = text.Key;
            json["text"] = text.Text;
            return json;
        }
    }
}
