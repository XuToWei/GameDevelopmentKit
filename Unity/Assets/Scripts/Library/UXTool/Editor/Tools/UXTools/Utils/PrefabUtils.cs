#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace ThunderFireUITool
{
    public static class PrefabUtils
    {
        public static List<FileInfo> GetWidgetList()
        {
            // string componentPath = AssetDatabase.LoadAssetAtPath<DefaultPrefabPathSetting>(ThunderFireUIToolConfig.DefaultPrefabPathSettingFullPath).PathList[(int)PrefabPath.Component];
            // List<FileInfo> prefabList = new List<FileInfo>();
            // if (Directory.Exists(componentPath))
            // {
            //     DirectoryInfo direction = new DirectoryInfo(componentPath);
            //     FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            //
            //     for (int i = 0; i < files.Length; i++)
            //     {
            //         if (files[i].Name.EndsWith(".prefab"))
            //         {
            //             prefabList.Add(files[i]);
            //         }
            //     }
            // }
            // return prefabList;

            WidgetListSetting list = JsonAssetManager.GetAssets<WidgetListSetting>();
            List<string> List = list.List;
            List<FileInfo> prefabList = new List<FileInfo>();
            for (int i = List.Count - 1; i >= 0; i--)
            {
                //AssetDatabase.Refresh();
                string path = AssetDatabase.GUIDToAssetPath(List[i]);
                if (!File.Exists(path) || path == "")
                {
                    list.Remove(List[i]);
                }
                else
                {
                    prefabList.Add(new FileInfo(path));
                }

            }
            return prefabList;
        }

        public static void ReplaceLabel(string oldLabel, string newLabel)
        {
            //先修改 所有prefab中的label
            List<FileInfo> prefabInfoList = PrefabUtils.GetWidgetList();
            for (int i = 0; i < prefabInfoList.Count; i++)
            {
                string tmp = prefabInfoList[i].DirectoryName.Replace("\\", "/");
                string path = FileUtil.GetProjectRelativePath(tmp) + "/" + prefabInfoList[i].Name;
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                string[] labels = AssetDatabase.GetLabels(prefab);
                if (labels.Length > 0)
                {
                    List<string> LabelList = labels.ToList();
                    int index = LabelList.FindIndex(s => s == oldLabel);
                    if (index != -1)
                    {
                        LabelList[index] = newLabel;
                    }

                    string[] newlabels = LabelList.ToArray();
                    AssetDatabase.ClearLabels(prefab);
                    AssetDatabase.SetLabels(prefab, newlabels);
                }
            }

            //再修改setting中的label数据
            var labelSetting = JsonAssetManager.GetAssets<WidgetLabelsSettings>();
            var labelList = labelSetting.labelList;

            labelSetting.RemoveLabel(oldLabel);

            if (!labelList.Contains(newLabel))
            {
                labelSetting.AddNewLabel(newLabel);
            }
        }

        public static void DeleteLabel(string label)
        {
            //先删除 所有prefab中的label
            List<FileInfo> prefabInfoList = PrefabUtils.GetWidgetList();
            for (int i = 0; i < prefabInfoList.Count; i++)
            {
                string tmp = prefabInfoList[i].DirectoryName.Replace("\\", "/");
                string path = FileUtil.GetProjectRelativePath(tmp) + "/" + prefabInfoList[i].Name;
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                string[] labels = AssetDatabase.GetLabels(prefab);
                if (labels.Length > 0)
                {
                    List<string> LabelList = labels.ToList();
                    int index = LabelList.FindIndex(s => s == label);
                    if (index != -1)
                    {
                        LabelList.RemoveAt(index);
                    }

                    string[] newlabels = LabelList.ToArray();
                    AssetDatabase.ClearLabels(prefab);
                    AssetDatabase.SetLabels(prefab, newlabels);
                }
            }

            //删除setting中的label数据
            var labelSetting = JsonAssetManager.GetAssets<WidgetLabelsSettings>();
            var labelList = labelSetting.labelList;

            labelSetting.RemoveLabel(label);
        }
    }
}
#endif
