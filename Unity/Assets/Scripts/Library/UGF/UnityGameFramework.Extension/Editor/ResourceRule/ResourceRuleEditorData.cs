using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace UnityGameFramework.Extension.Editor
{
    [CreateAssetMenu(fileName = "ResourceRuleEditorData", menuName = "UGF/ResourceRuleEditorData")]
    public sealed class ResourceRuleEditorData : ScriptableObject
    {
        [SerializeField, ReadOnly]
        private bool m_IsActivate = false;

        [SerializeField]
        private List<ResourceRule> m_Rules = new List<ResourceRule>();

        public bool IsActivate
        {
            get
            {
                return m_IsActivate;
            }
            set
            {
                m_IsActivate = value;
            }
        }

        public List<ResourceRule> Rules
        {
            get
            {
                return m_Rules;
            }
        }
    }

    [Serializable]
    public sealed class ResourceRule
    {
        [SerializeField]
        private bool m_Valid = true;

        [SerializeField]
        private string m_Name = string.Empty;

        [SerializeField]
        private string m_Variant = null;

        [SerializeField]
        private string m_FileSystem = string.Empty;

        [SerializeField]
        private string m_Groups = string.Empty;

        [SerializeField]
        private string m_AssetsDirectoryPath = string.Empty;

        [SerializeField]
        private LoadType m_LoadType = LoadType.LoadFromFile;

        [SerializeField]
        private bool m_Packed = false;

        [SerializeField]
        private ResourceFilterType m_FilterType = ResourceFilterType.Root;

        [SerializeField]
        private string m_SearchPatterns = "*.*";

        public bool Valid
        {
            get
            {
                return m_Valid;
            }
            set
            {
                m_Valid = value;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        public string Variant
        {
            get
            {
                return m_Variant;
            }
            set
            {
                m_Variant = value;
            }
        }

        public string FileSystem
        {
            get
            {
                return m_FileSystem;
            }
            set
            {
                m_FileSystem = value;
            }
        }

        public string Groups
        {
            get
            {
                return m_Groups;
            }
            set
            {
                m_Groups = value;
            }
        }

        public string AssetsDirectoryPath
        {
            get
            {
                return m_AssetsDirectoryPath;
            }
            set
            {
                m_AssetsDirectoryPath = value;
            }
        }

        public LoadType LoadType
        {
            get
            {
                return m_LoadType;
            }
            set
            {
                m_LoadType = value;
            }
        }

        public bool Packed
        {
            get
            {
                return m_Packed;
            }
            set
            {
                m_Packed = value;
            }
        }

        public ResourceFilterType FilterType
        {
            get
            {
                return m_FilterType;
            }
            set
            {
                m_FilterType = value;
            }
        }

        public string SearchPatterns
        {
            get
            {
                return m_SearchPatterns;
            }
            set
            {
                m_SearchPatterns = value;
            }
        }
    }

    public enum ResourceFilterType
    {
        [Tooltip("指定文件夹打成一个Resource")]
        Root,
        [Tooltip("指定文件夹下的文件分别打成一个Resource")]
        Children,
        [Tooltip("指定文件夹下的子文件夹分别打成一个Resource")]
        ChildrenFoldersOnly,
        [Tooltip("指定文件夹下的子文件夹的文件分别打成一个Resource")]
        ChildrenFilesOnly,
    }
}
