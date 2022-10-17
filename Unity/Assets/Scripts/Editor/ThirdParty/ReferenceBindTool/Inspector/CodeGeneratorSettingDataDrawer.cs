using System;
using System.IO;
using System.Text;
using ReferenceBindTool.Runtime;
using UnityEditor;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    /// <summary>
    /// 引用绑定代码生成配置数据Drawer
    /// </summary>
    [CustomPropertyDrawer(typeof(CodeGeneratorSettingData))]
    public class CodeGeneratorSettingDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // var nameProperty = property.FindPropertyRelative("m_Name");
            // Rect nameRect = new Rect(position.x, position.y, position.width, 18);
            // nameProperty.stringValue = EditorGUI.TextField(nameRect, nameProperty.name, nameProperty.stringValue);

            var codeFolderPathProperty = property.FindPropertyRelative("m_CodeFolderPath");
            Rect codeFolderPathRect = new Rect(position.x, position.y, position.width - 80, 18);
            EditorGUI.LabelField(codeFolderPathRect, "CodeFolderPath", codeFolderPathProperty.stringValue);

            Rect codePathFolderButtonRect = new Rect(position.x + position.width - 70, codeFolderPathRect.y, 70, 18);
            if (GUI.Button(codePathFolderButtonRect, "选择路径"))
            {
                var folderPath = EditorUtility.OpenFolderPanel("选择代码保存文件夹", "", "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    folderPath = GetRelativePath(folderPath, Directory.GetParent(Application.dataPath).FullName);
                    codeFolderPathProperty.stringValue = folderPath;
                    codeFolderPathProperty.serializedObject.ApplyModifiedProperties();
                    // 如果不退出 会报错 stack empty  Unity的吊轨bug
                    GUIUtility.ExitGUI();
                }
            }

            var namespaceProperty = property.FindPropertyRelative("m_Namespace");
            Rect namespaceRect = new Rect(position.x, codeFolderPathRect.y + 18, position.width, 18);
            namespaceProperty.stringValue = EditorGUI.TextField(namespaceRect, namespaceProperty.name,
                namespaceProperty.stringValue);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18*2;
        }

        private static string GetRelativePath(string path, string parentPath)
        {
            if (IsNullOrWhitespace(parentPath))
                return path;
            if (IsNullOrWhitespace(path))
                return (string)null;
            parentPath = Path.GetFullPath(parentPath);
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(parentPath, path);
                path = path.Replace('\\', '/');
            }

            return MakeRelative(parentPath, path);
        }

        private static bool IsNullOrWhitespace(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                for (int index = 0; index < str.Length; ++index)
                {
                    if (!char.IsWhiteSpace(str[index]))
                        return false;
                }
            }

            return true;
        }

        private static string MakeRelative(string absoluteParentPath, string absolutePath)
        {
            absoluteParentPath = absoluteParentPath.TrimEnd('\\', '/');
            absolutePath = absolutePath.TrimEnd('\\', '/');
            string[] strArray1 = absoluteParentPath.Split('/', '\\');
            string[] strArray2 = absolutePath.Split('/', '\\');
            int num = -1;
            for (int index = 0;
                 index < strArray1.Length && index < strArray2.Length && strArray1[index]
                     .Equals(strArray2[index], StringComparison.CurrentCultureIgnoreCase);
                 ++index)
                num = index;
            if (num == -1)
                throw new InvalidOperationException("No common directory found.");
            StringBuilder stringBuilder = new StringBuilder();
            if (num + 1 < strArray1.Length)
            {
                for (int index = num + 1; index < strArray1.Length; ++index)
                {
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append('/');
                    stringBuilder.Append("..");
                }
            }

            for (int index = num + 1; index < strArray2.Length; ++index)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append('/');
                stringBuilder.Append(strArray2[index]);
            }

            return stringBuilder.ToString();
        }
    }
}