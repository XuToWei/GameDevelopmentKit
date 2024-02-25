#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    [Serializable]
    public class ManagementChannel
    {
        public int ID;
        public string Name;
        public List<int> LevelIDList = new List<int>();
    }

    [Serializable]
    public class ManagementLevel
    {
        public int ID;
        public int Index;
        public int ChannelID;
        public List<int> PrefabDetailIDList = new List<int>();

    }

    [Serializable]
    public class PrefabDetail
    {
        public int ID;
        public string Name;
        public string Guid;
        public int ChannelID;
        public int LevelID;
        public List<TagDetail> Tags = new List<TagDetail>();

        public static PrefabDetail DeepCopyByXml(PrefabDetail obj)
        {
            object retval;
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (PrefabDetail)retval;
        }
    }

    [Serializable]
    public class TagDetail
    {
        public string Name;
        public int Num;

        public override bool Equals(object obj)
        {
            var atom = obj as TagDetail;
            return atom != null && Name == atom.Name;
        }

        public override int GetHashCode()
        {
            return Num;
        }

        public static TagDetail DeepCopyByXml(TagDetail obj)
        {
            object retval;
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (TagDetail)retval;
        }
    }

    [Serializable]
    public class TagColor
    {
        public string Name;
        public Color Color;
    }

    [Serializable]
    public class HierarchyManagementEditorData : ScriptableObject
    {
        public List<PrefabDetail> dataList = new List<PrefabDetail>();
        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.HierarchyManage + "/HierarchyManagementEditorData", false, -49)]
        public static void Create()
        {
            JsonAssetManager.CreateAssets<HierarchyManagementEditorData>(ThunderFireUIToolConfig.HierarchyManagemenEditorDataPath);
        }

        public void Save()
        {
            if (HierarchyManagementEvent.isDemo)
            {
                JsonAssetManager.SaveAssetsAtPath(this, ThunderFireUIToolConfig.HierarchyManagementEditorDataPath_Sample);
            }
            else
            {
                JsonAssetManager.SaveAssets(this);
            }
        }
    }

    public class HierarchyManagementSetting : ScriptableObject
    {
        //Serialize Field
        public List<ManagementChannel> managementChannelList = new List<ManagementChannel>();
        public List<ManagementLevel> managementLevelList = new List<ManagementLevel>();
        public List<TagColor> tagColors = new List<TagColor>();
        public int range = 10;

        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.HierarchyManage + "/HierarchyManagementSetting", false, -49)]
        public static void Create()
        {
            HierarchyManagementSetting setting = JsonAssetManager.CreateAssets<HierarchyManagementSetting>(ThunderFireUIToolConfig.HierarchyManagementSettingPath);
            InitDefault(setting);
            JsonAssetManager.SaveAssets(setting);
        }
        //新建文件时初始化配置
        public static void InitDefault(HierarchyManagementSetting setting)
        {
            List<string> channelNameList = new List<string>() { "HUD", "界面", "弹窗" };
            List<int> levelList = new List<int>() { 0, 10, 20 };

            for (int i = 0; i <= channelNameList.Count - 1; i++)
            {
                string name = channelNameList[i];
                ManagementChannel channel = new ManagementChannel()
                {
                    ID = i,
                    Name = name,
                    LevelIDList = new List<int>() { i }
                };
                setting.managementChannelList.Add(channel);
            };

            for (int i = 0; i <= levelList.Count - 1; i++)
            {
                int index = levelList[i];
                ManagementLevel level = new ManagementLevel()
                {
                    ID = i,
                    Index = index,
                    ChannelID = i,
                    PrefabDetailIDList = new List<int>()
                };
                setting.managementLevelList.Add(level);
            }
        }

        public void Save()
        {
            if (HierarchyManagementEvent.isDemo)
            {
                JsonAssetManager.SaveAssetsAtPath(this, ThunderFireUIToolConfig.HierarchyManagementSettingPath_Sample);
            }
            else
            {
                JsonAssetManager.SaveAssets(this);
            }
        }
    }
}
#endif