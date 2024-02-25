#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    [Serializable]
    public class LocationLineData
    {
        public int Id;
        public bool Horizontal;
        public float Pos;
    }

    [Serializable]
    public class LocationLinesData : ScriptableObject
    {
        public List<LocationLineData> List = new List<LocationLineData>();

        public int LastLineId
        {
            get { return List.Count > 0 ? List.Max(data => data.Id) : 0; }
        }

        public void Add(LocationLineData line)
        {
            List.Add(line);
            //Save();
            OnValueChanged();
        }

        public void Remove(int id)
        {
            var index = List.FindIndex(l => id == l.Id);
            if (index >= 0)
            {
                List.RemoveAt(index);
            }
            //Save();
            OnValueChanged();
        }

        public void Modify(LocationLineData line)
        {
            var index = List.FindIndex(l => line.Id == l.Id);
            if (index >= 0)
            {
                List[index].Horizontal = line.Horizontal;
                List[index].Pos = line.Pos;
            }
            //Save();
        }

        public void Clear()
        {
            List.Clear();
            //Save();
        }

        public void Save()
        {
            JsonAssetManager.SaveAssets(this);
        }

        public static LocationLinesData Create()
        {
            var assetPath = ThunderFireUIToolConfig.LocationLinesDataPath;
            var data = JsonAssetManager.CreateAssets<LocationLinesData>(assetPath);
            if (data == null)
                Debug.LogError("Create LocationLinesData Failed!");
            return data;
        }

        private void OnValueChanged()
        {
        }
    }

    public class CreateLocationLinesData : Editor
    {
        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.Menu_ReferenceLine + "/ReferenceLinesData", false, -46)]
        public static void Create()
        {
            var assetPath = ThunderFireUIToolConfig.LocationLinesDataPath;
            JsonAssetManager.CreateAssets<LocationLinesData>(assetPath);
        }
    }
}
#endif