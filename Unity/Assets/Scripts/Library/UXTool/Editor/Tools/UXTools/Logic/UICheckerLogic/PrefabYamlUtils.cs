#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

using UnityEngine;
using UnityEditor.SceneManagement;
using ThunderFireUnityEx;

namespace ThunderFireUITool
{
    public class PrefabYamlUtils
    {
        private static Dictionary<string, string> GuidToUGUIType = new Dictionary<string, string>()
        {
            { "fe87c0e1cc204ed48ad3b37840f39efc", "Image" },
            { "1344c3c82d62a2a41a3576d8abb8e3ea", "RawImage" },
            { "9085046f02f69544eb97fd06b6048fe2", "Toggle" },
            { "67db9e8f0e2ae9c40bc1e2b64352a6b4", "Slider" },
            { "2a4db7a114972834c8e4117be1d82ba3", "Scrollbar" },
            { "4e29b1a8efbd4b44bb3f3716e73f07ff", "Button" },
            { "0d0b652f32a2cc243917e4028fa0f046", "DropDown" },
            { "7b743370ac3e4ec2a1668f5455a8ef8a", "DropDown" },
            { "d199490a83bb2b844b9695cbf13b01ef", "InputField" },
            { "2da0c512f12947e489f739169773d7ca", "InputField" },
            { "5f7201a12d95ffc409449d95f23cf332", "Text" },
            { "f4688fdb7df04437aeb418b961361dc5", "Text" },
        };

        private static string GetUGUIType(string guid)
        {
            if (GuidToUGUIType.ContainsKey(guid))
            {
                return GuidToUGUIType[guid];
            }
            return null;
        }

        /// <summary>
        /// 获取Prefab文件中单个组件引用的其他组件的InstanceID
        /// </summary>
        /// <param name="content">这里使用的是Component运行后通过ToYAMLString方法转的string, 里面有instanceID, 从文本读取出来的是没有的</param>
        /// <param name="assignedInstanceIdList">该Component引用的其他component对象的InstanceID</param>
        public static void LoadDependInstanceIdFromComponentString(string content, ref List<string> assignedInstanceIdList)
        {
            List<string> ignoreKeys = new List<string>()
            {
                "m_Script",
                "m_GameObject",
                "m_PrefabAsset",
                "m_Father",
                "m_Children",
                "m_CorrespondingSourceObject",
                "m_PrefabInstance",
                "m_Font",
                "m_Sprite",
                "m_Material"
            };
            string currentKey = "";
            int currentDepth = 0;

            string[] lines = content.Split('\n');

            int[] depths = new int[lines.Length];
            for (int i = 0; i < lines.Length; ++i)
            {
                string temp = lines[i];
                byte spaceCount = 0;
                for (int j = 0; j < temp.Length; ++j)
                {
                    if (temp[j] == ' ')
                    {
                        spaceCount++;
                    }
                    else break;
                }
                depths[i] = spaceCount;
                lines[i] = lines[i].Substring(spaceCount, lines[i].Length - spaceCount);
            }

            for (int i = 0; i < lines.Length; ++i)
            {
                string temp = lines[i];
                currentDepth = depths[i];

                if (temp.Length == 0) continue;

                string key = temp;
                string val = null;
                int idx = key.IndexOf(':');
                if (idx != -1)
                {
                    val = key.Substring(idx + 1);
                    key = key.Substring(0, idx);
                    if (!key.StartsWith("-"))
                    {
                        // - 开头的是列表中的元素 没有自己的Key
                        currentKey = key;
                    }
                }

                if (ignoreKeys.Contains(currentKey)) continue;

                if (val != null && (val.Contains("instanceID") || key.Contains("instanceID")))
                {
                    string insId = Regex.Replace(val, @"[^0-9]+", "");
                    if (insId != "" && insId != "0")
                    {
                        assignedInstanceIdList.Add(insId);
                    }
                }
            }
        }

        /// <summary>
        /// 查找一个Prefab中哪些节点引用了某个资产
        /// </summary>
        /// Prefab Yaml文件中 GameObject和Component之间的挂载关系是通过fileID标记的
        /// <param name="prefabPath">待查找Prefab的路径</param>
        /// <param name="guid">待查找资产的Guid</param>
        public static void FindGuidReference(string prefabPath, string guid, ref List<ReferenceGoInfo> referenceInfos)
        {

            string content = File.ReadAllText(prefabPath);
            string[] components = Regex.Split(content, "--- !u!", RegexOptions.IgnoreCase);
            //删除第一个Yaml语言标记
            components = components.RemoveAt(0);

            string nameRegStr = "m_Name: \\s*(\\w+)";
            Regex nameReg = new Regex(nameRegStr);

            string guidRegStr = "{fileID:\\s?[a-fA-F0-9]+, guid:\\s?([a-fA-F0-9]+), type: 3}";
            Regex guidReg = new Regex(guidRegStr);

            string goRegStr = "m_GameObject: {fileID:\\s*(\\w+)";
            Regex goReg = new Regex(goRegStr);

            string scriptRegStr = "m_Script: {fileID:\\s?[a-fA-F0-9]+, guid:\\s?([a-fA-F0-9]+), type: 3}";
            Regex scriptReg = new Regex(scriptRegStr);


            //Prefab中所有GameObject的FileId 和 节点名称
            //用来查引用资源的Component挂在哪个节点上
            Dictionary<string, string> GoDic = new Dictionary<string, string>();

            //引用资源的Component所挂的节点的FileId 和 自己的Script的Guid
            Dictionary<string, string> referenceComponentDic = new Dictionary<string, string>();

            foreach (var component in components)
            {
                string[] componentLines = component.Split('\n');
                bool isGo = componentLines[1] == "GameObject:";
                if (isGo)
                {
                    string goFileID = componentLines[0].Split('&')[1];
                    string goName = nameReg.Match(component).Groups[1].Value;
                    GoDic.Add(goFileID, goName);
                }

                bool hasReference = false;
                MatchCollection matches = guidReg.Matches(component);
                foreach (Match m in matches)
                {
                    //正则表达式内括号内匹配的guid是否是查找的guid
                    if (m.Groups[1].Value == guid)
                    {
                        hasReference = true;
                    }
                }
                if (hasReference)
                {
                    string goId = goReg.Match(component).Groups[1].Value;
                    string scriptID = scriptReg.Match(component).Groups[1].Value;
                    referenceComponentDic.Add(goId, scriptID);
                }
            }

            foreach (var r in referenceComponentDic)
            {
                string goId = r.Key;
                string scriptId = r.Value;

                string name = "";
                string script = "";
                if (GoDic.ContainsKey(goId))
                {
                    name = GoDic[goId];
                }
                script = GetUGUIType(scriptId);

                ReferenceGoInfo info = new ReferenceGoInfo()
                {
                    goName = name,
                    scriptName = script,
                    goFileId = goId
                };
                referenceInfos.Add(info);
            }
        }

        /// <summary>
        /// prefab文件中存在嵌套Prefab时,去掉嵌套Prefab带来的guid引用
        /// 1.PrefabInstance下的target:   每行都会带一个prefab guid 去掉
        /// 2.部分挂在嵌套 Prefab上的component: 每个都会带一个m_CorrespondingSourceObject guid 去掉
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <returns></returns>
        public static string FilterNestPrefabInstanceContent(string prefabPath)
        {
            List<string> filterLines = new List<string>();
            bool inPrefabInstance = false;


            string[] lines = File.ReadAllLines(prefabPath, Encoding.UTF8);
            foreach (string line in lines)
            {
                bool bfilter = false;
                if (line.StartsWith("PrefabInstance"))
                {
                    inPrefabInstance = true;
                }
                if (line.StartsWith("m_SourcePrefab"))
                {
                    inPrefabInstance = false;
                }

                if (inPrefabInstance && line.Contains("- target"))
                {
                    //去掉1的情形
                    bfilter = true;
                }

                if (line.Contains("m_CorrespondingSourceObject"))
                {
                    //去掉2的情形
                    bfilter = true;
                }

                if (!bfilter)
                {
                    filterLines.Add(line);
                }
            }

            return string.Join("", filterLines);
        }

    }
}
#endif