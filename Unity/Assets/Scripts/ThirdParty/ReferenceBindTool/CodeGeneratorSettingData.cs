#if UNITY_EDITOR
using System;
using UnityEngine;

namespace ReferenceBindTool.Runtime
{
    /// <summary>
    /// 引用绑定工具代码生成配置数据类
    /// </summary>
    [Serializable]
    public class CodeGeneratorSettingData
    {
        /// <summary>
        /// 配置名
        /// </summary>
        [SerializeField] private string m_Name;
        /// <summary>
        /// 代码生成目录
        /// </summary>
        [SerializeField] private string m_CodeFolderPath;
        /// <summary>
        /// 命名空间
        /// </summary>
        [SerializeField] private string m_Namespace;

        public CodeGeneratorSettingData(string name)
        {
            m_Name = name;
        }

        public CodeGeneratorSettingData(string name, string codeFolderPath, string nameSpace)
        {
            m_Name = name;
            m_CodeFolderPath = codeFolderPath;
            m_Namespace = nameSpace;
        }

        public string Name => m_Name;

        public string CodePath
        {
            get => m_CodeFolderPath;

            set => m_CodeFolderPath = value;
        }

        public string Namespace
        {
            get => m_Namespace;

            set => m_Namespace = value;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Name) &&
                   string.IsNullOrEmpty(m_CodeFolderPath) &&
                   string.IsNullOrEmpty(m_Namespace);
        }

        public bool Equals(CodeGeneratorSettingData data)
        {
            return Name == data.m_Name && m_CodeFolderPath == data.m_CodeFolderPath && m_Namespace == data.m_Namespace;
        }
    }
}

#endif