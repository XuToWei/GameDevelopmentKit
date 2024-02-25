using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    public class HierarchyManagementEvent
    {
        public static HierarchyManagementSetting hierarchyManagementSetting = null;
        public static List<ManagementChannel> _managementChannels
        {
            get { return hierarchyManagementSetting.managementChannelList; }
            set { hierarchyManagementSetting.managementChannelList = value; }
        }
        public static List<ManagementLevel> _managementLevels
        {
            get { return hierarchyManagementSetting.managementLevelList; }
            set { hierarchyManagementSetting.managementLevelList = value; }
        }
        public static List<TagColor> _mTagColors
        {
            get { return hierarchyManagementSetting.tagColors; }
            set { hierarchyManagementSetting.tagColors = value; }
        }
        public static int _range
        {
            get { return hierarchyManagementSetting.range; }
            set { hierarchyManagementSetting.range = value; }
        }

        public static HierarchyManagementEditorData hierarchyManagementEditorData = null;
        public static List<PrefabDetail> _prefabDetails
        {
            get { return hierarchyManagementEditorData.dataList; }
            set { hierarchyManagementEditorData.dataList = value; }

        }


        public static int maxPrefabNum;

        public static bool isDemo;

        public delegate void SaveDelegate();
        public static SaveDelegate OnSave;

        public static void Save()
        {
            SaveSetting();
            SavePrefabData();

            //额外的保存回调, 给项目组添加自定义的导出数据接口, 参考Samples
            OnSave?.Invoke();
        }

        public static void SaveSetting()
        {
            if (hierarchyManagementSetting != null)
            {
                hierarchyManagementSetting.Save();
            }
        }
        public static void SavePrefabData()
        {
            if (hierarchyManagementEditorData == null || hierarchyManagementEditorData.dataList == null) return;

            var hierarchyManagementData = JsonAssetManager.GetAssets<HierarchyManagementData>();
            if (isDemo)
            {
                hierarchyManagementData = JsonAssetManager.LoadAssetAtPath<HierarchyManagementData>(
                    ThunderFireUIToolConfig.HierarchyManagementDataPath_Sample);
            }
            hierarchyManagementData.dataList.Clear();

            foreach (var item in _prefabDetails)
            {
                var tags = new List<TagDetail>();
                foreach (var tag in item.Tags)
                {
                    var tagDetail = TagDetail.DeepCopyByXml(tag);
                    tags.Add(tagDetail);
                }
                var path = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(item.Guid));
                var prefabName = string.IsNullOrEmpty(path) ? item.Guid : path;

                var prefabHierarchyData = new PrefabHierarchyData()
                {
                    Name = prefabName,
                    Guid = item.Guid,
                    Channel = _managementChannels.Where(t => t.ID == item.ChannelID).FirstOrDefault().Name,
                    Level = _managementLevels.Where(t => t.ID == item.LevelID).FirstOrDefault().Index,
                };

                hierarchyManagementData.dataList.Add(prefabHierarchyData);
            }

            hierarchyManagementEditorData.dataList.Sort((x, y) =>
            {
                return x.LevelID == y.LevelID
                            ? hierarchyManagementEditorData.dataList.IndexOf(x).CompareTo(hierarchyManagementEditorData.dataList.IndexOf(y))
                            : _managementLevels.Where(t => t.ID == x.LevelID).FirstOrDefault().Index.CompareTo(_managementLevels.Where(t => t.ID == y.LevelID).FirstOrDefault().Index);
            });

            hierarchyManagementData.dataList.Sort((x, y) =>
            {
                return x.Level == y.Level
                    ? hierarchyManagementData.dataList.IndexOf(x).CompareTo(hierarchyManagementData.dataList.IndexOf(y))
                    : x.Level.CompareTo(y.Level);
            });

            hierarchyManagementEditorData.Save();
            hierarchyManagementData.Save();
        }

        public static void Init()
        {
            hierarchyManagementSetting = JsonAssetManager.GetAssets<HierarchyManagementSetting>();
            if (isDemo)
            {
                hierarchyManagementSetting = JsonAssetManager.LoadAssetAtPath<HierarchyManagementSetting>(
                ThunderFireUIToolConfig.HierarchyManagementSettingPath_Sample);
            }

            hierarchyManagementEditorData = JsonAssetManager.GetAssets<HierarchyManagementEditorData>();
            if (isDemo)
            {
                hierarchyManagementEditorData = JsonAssetManager.LoadAssetAtPath<HierarchyManagementEditorData>(
                ThunderFireUIToolConfig.HierarchyManagementEditorDataPath_Sample);
            }

            maxPrefabNum = 0;
            foreach (var item in _managementLevels)
            {
                if (item.PrefabDetailIDList.Count > maxPrefabNum)
                {
                    maxPrefabNum = item.PrefabDetailIDList.Count;
                }
            }
        }

        public static bool CheckNeedSave()
        {
            HierarchyManagementSetting savedHierarchyManagementSetting = JsonAssetManager.GetAssets<HierarchyManagementSetting>();
            if (isDemo)
            {
                savedHierarchyManagementSetting = JsonAssetManager.LoadAssetAtPath<HierarchyManagementSetting>(
                ThunderFireUIToolConfig.HierarchyManagementSettingPath_Sample);
            }

            HierarchyManagementEditorData savedHierarchyManagementEditorData = JsonAssetManager.GetAssets<HierarchyManagementEditorData>();
            if (isDemo)
            {
                savedHierarchyManagementEditorData = JsonAssetManager.LoadAssetAtPath<HierarchyManagementEditorData>(
                ThunderFireUIToolConfig.HierarchyManagementEditorDataPath_Sample);
            }


            if (!JsonUtility.ToJson(savedHierarchyManagementSetting).Equals(JsonUtility.ToJson(hierarchyManagementSetting)))
            {
                return true;
            }

            if (!JsonUtility.ToJson(savedHierarchyManagementEditorData).Equals(JsonUtility.ToJson(hierarchyManagementEditorData)))
            {
                return true;
            }
            return false;
        }

        #region Level
        public static void AddLevel(ManagementLevel level, int definedIndex, bool isAfter = false)
        {
            var newLevel = new ManagementLevel()
            {
                ID = !isAfter
                    ? level.ID
                    : level.ID + 1,
                Index = definedIndex,
                ChannelID = definedIndex / _range,
            };

            ManagementChannel channel = _managementChannels.Where(t => t.ID == newLevel.ChannelID).FirstOrDefault();
            if (channel.LevelIDList.Count == _range)
            {
                EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_已经达到该层级最大容纳量Tip),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消));
                return;
            }
            foreach (var item in _managementLevels)
            {
                if (item.ID >= newLevel.ID) item.ID += 1;
            }

            foreach (var item in _managementChannels)
            {
                var list = item.LevelIDList;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] >= newLevel.ID) list[i] += 1;
                }
                // item.LevelIDList = list;
            }
            foreach (var item in _prefabDetails)
            {
                if (item.LevelID >= newLevel.ID) item.LevelID += 1;
            }
            if (channel.LevelIDList.Count - channel.LevelIDList.IndexOf(level.ID) ==
                (channel.ID + 1) * _range - level.Index)
            {
                if (isAfter && definedIndex != level.Index || !isAfter && definedIndex == level.Index) newLevel.Index -= 1;
                var tmp = newLevel.Index;
                for (var i = _managementLevels.Count - 1; i >= 0; i--)
                {
                    var item = _managementLevels[i];
                    if (item.Index != tmp) continue;
                    item.Index -= 1;
                    tmp -= 1;
                }
            }
            else
            {
                var tmp = newLevel.Index;
                foreach (var item in _managementLevels)
                {
                    if (item.Index != tmp) continue;
                    item.Index += 1;
                    tmp += 1;
                }
            }

            _managementLevels.Insert(newLevel.ID, newLevel);
            var ind = !isAfter
                ? channel.LevelIDList.IndexOf(level.ID)
                : channel.LevelIDList.IndexOf(level.ID) + 1;
            channel.LevelIDList.Insert(ind, newLevel.ID);
            HierarchyManagementWindow.GetInstance()?.RefreshPaint();
        }

        public static void DeleteLevel(ManagementLevel level)
        {
            ManagementChannel channel = _managementChannels.Where(t => t.ID == level.ChannelID).FirstOrDefault();
            if (level.PrefabDetailIDList.Count != 0)
            {
                EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_只能删除不含组件信息的层级),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定));
                return;
            }

            if (channel.LevelIDList.Count <= 1)
            {
                EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_至少保留一个层级),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定));
                return;
            }
            _managementLevels.Remove(level);
            channel.LevelIDList.Remove(level.ID);
            foreach (var item in _managementChannels)
            {
                var list = item.LevelIDList;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] > level.ID) list[i] -= 1;
                }
            }
            foreach (var item in _managementLevels)
            {
                if (item.ID > level.ID)
                {
                    item.ID -= 1;
                }
            }
            foreach (var item in _prefabDetails)
            {
                if (item.LevelID > level.ID) item.LevelID -= 1;
            }
            HierarchyManagementWindow.GetInstance()?.RefreshPaint();
        }

        #endregion

        #region Prefab
        public static void AddNewPrefab(ManagementLevel level = null)
        {
            if (level != null)
                PrefabDetailWindow.OpenWindow(level);
        }

        public static void OpenPrefabDetail(PrefabDetail prefabDetail)
        {
            PrefabDetailWindow.OpenWindow(prefabDetail);
        }

        public static void DeletePrefab(PrefabDetail prefabDetail)
        {
            ManagementChannel channel = _managementChannels.Where(t => t.ID == prefabDetail.ChannelID).FirstOrDefault();
            ManagementLevel level = _managementLevels.Where(t => t.ID == prefabDetail.LevelID).FirstOrDefault();
            var path = Path.GetFileNameWithoutExtension(
                AssetDatabase.GUIDToAssetPath(prefabDetail.Guid));
            var tooltipText = string.IsNullOrEmpty(path)
                ? prefabDetail.Guid
                : Path.GetFileName(AssetDatabase.GUIDToAssetPath(prefabDetail.Guid));
            if (EditorUtility.DisplayDialog("messageBox", EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确认删除该节点) + "：" + tooltipText,
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确定),
                    EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_取消)))
            {
                level.PrefabDetailIDList.Remove(prefabDetail.ID);
                foreach (var item in _managementLevels)
                {
                    var list = item.PrefabDetailIDList;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] > prefabDetail.ID) list[i] -= 1;
                    }
                }

                var prefab = _prefabDetails.Find(s => s.Guid == prefabDetail.Guid);
                _prefabDetails.Remove(prefab);
                foreach (var item in _prefabDetails)
                {
                    if (item.ID > prefabDetail.ID) item.ID -= 1;
                }

                DeleteTags(prefabDetail);

                foreach (var item in _managementLevels)
                {
                    if (item.PrefabDetailIDList.Count > maxPrefabNum)
                    {
                        maxPrefabNum = item.PrefabDetailIDList.Count;
                    }
                }

                //重绘窗口
                if (HierarchyManagementWindow.GetInstance() != null)
                {
                    HierarchyManagementWindow.GetInstance().chosenPrefab = new PrefabDetail() { ID = -1 };
                    HierarchyManagementWindow.GetInstance().DrawAllPrefabs();
                }
            }
            else
            {
                if (HierarchyManagementWindow.GetInstance() != null)
                {
                    HierarchyManagementWindow.GetInstance()._firstFlag = true;
                    HierarchyManagementWindow.GetInstance().chosenPrefab = prefabDetail;
                    HierarchyManagementWindow.GetInstance().DrawAllPrefabs();
                }
            }
        }

        public static void DragSubmit(PrefabDetail prefabDetail, ManagementLevel newLevel)
        {
            _managementLevels.Where(t => t.ID == prefabDetail.LevelID).FirstOrDefault().PrefabDetailIDList.Remove(prefabDetail.ID);
            newLevel.PrefabDetailIDList.Insert(0, prefabDetail.ID);
            prefabDetail.LevelID = newLevel.ID;
            prefabDetail.ChannelID = newLevel.ChannelID;

            foreach (var item in _managementLevels)
            {
                if (item.PrefabDetailIDList.Count > maxPrefabNum)
                    maxPrefabNum = item.PrefabDetailIDList.Count;
            }

            if (HierarchyManagementWindow.GetInstance() != null)
            {
                //重绘窗口
                HierarchyManagementWindow.GetInstance().chosenLevel = new ManagementLevel() { ID = -1 };
                HierarchyManagementWindow.GetInstance().DrawAllPrefabs();
            }
        }

        #endregion

        #region Tag
        private static void DeleteTags(PrefabDetail prefabDetail)
        {
            var prefabDetails = new List<PrefabDetail>();
            foreach (var item in _prefabDetails)
            {
                prefabDetails.Add(PrefabDetail.DeepCopyByXml(item));
            }

            foreach (var tag in prefabDetail.Tags)
            {
                //判断是否要进行tag的减
                var flag = prefabDetails.FindAll(s => s.Tags.Contains(tag) && s.ID != prefabDetail.ID)
                    .Any(item => item.Tags.Find(s => s.Equals(tag)).Num == tag.Num);
                if (!flag)
                {
                    foreach (var item in prefabDetails.FindAll(s =>
                                 s.Tags.Contains(tag) && s.ID != prefabDetail.ID))
                    {
                        var t = item.Tags.Find(s => s.Equals(tag));
                        if (t.Num > tag.Num)
                            t.Num -= 1;
                    }
                }
            }
            //SetPrefabDetails(prefabDetails);
            _prefabDetails = prefabDetails;
        }

        #endregion

    }
}