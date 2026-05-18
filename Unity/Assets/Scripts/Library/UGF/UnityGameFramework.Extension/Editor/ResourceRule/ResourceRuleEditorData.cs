using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace UnityGameFramework.Extension.Editor
{
    [CreateAssetMenu(fileName = "ResourceRuleEditorData", menuName = "UGF/ResourceRuleEditorData")]
    public class ResourceRuleEditorData : ScriptableObject
    {
        [ReadOnly]
        public bool isActivate;
        public List<ResourceRule> rules = new List<ResourceRule>();
    }

    [System.Serializable]
    public class ResourceRule
    {
        public bool valid = true;
        public string name = string.Empty;
        public string variant = null;
        public string fileSystem = string.Empty;
        public string groups = string.Empty;
        public string assetsDirectoryPath = string.Empty;
        public LoadType loadType = LoadType.LoadFromFile;
        public bool packed = false;
        public ResourceFilterType filterType = ResourceFilterType.Root;
        public string searchPatterns = "*.*";
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