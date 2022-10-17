using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UGF.Editor
{
    /// <summary>
    /// Lua脚本生成器。
    /// </summary>
    public static class ScriptCreator
    {

        public static void CreatNewScript(string templatePath, string scriptPath)
        {
            if (File.Exists(scriptPath))
            {
                throw new Exception($"{scriptPath}已存在！");
            }

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
                scriptPath, null,
                templatePath);
        }

        private class MyDoCreateScriptAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
                ProjectWindowUtil.ShowCreatedAsset(o);
            }

            private static Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
            {
                string fullPath = Path.GetFullPath(pathName);
                StreamReader streamReader = new StreamReader(resourceFile);
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
                text = Regex.Replace(text, "#NAME#", fileNameWithoutExtension);
                bool encoderShouldEmitUTF8Identifier = true;
                bool throwOnInvalidBytes = false;
                UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
                bool append = false;
                StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
                streamWriter.Write(text);
                streamWriter.Close();
                AssetDatabase.ImportAsset(pathName);
                return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            }
 
        }
    }
}
