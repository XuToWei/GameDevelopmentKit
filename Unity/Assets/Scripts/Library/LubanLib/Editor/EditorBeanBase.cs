using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban
{
    public abstract class EditorBeanBase
    {
        public abstract void LoadJson(SimpleJSON.JSONObject json);

        public abstract void SaveJson(SimpleJSON.JSONObject json);

        public void LoadJsonFile(string file)
        {
            string jsonText = System.IO.File.ReadAllText(file, Encoding.UTF8);
            LoadJson((SimpleJSON.JSONObject)SimpleJSON.JSON.Parse(jsonText));
        }

        public void SaveJsonFile(string file)
        {
            var json = new SimpleJSON.JSONObject();
            SaveJson(json);
            System.IO.File.WriteAllText(file, json.ToString(), System.Text.Encoding.UTF8);
        }
    }
}
