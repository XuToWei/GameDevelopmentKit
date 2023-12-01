using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace UnityGameFramework.Extension.Editor
{
    public sealed class ResourceListGenerator
    {
        private readonly string ExceptName = "Auto/";
        private readonly string XmlDataFilePath = "Assets/Res/Editor/Config/ResourceList.xml";
        
        //有需要改名，修改这里
        public static string GetNewName(string fullPath)
        {
            string parentDir = fullPath.Split("/")[2];
            string fileName = Path.GetFileName(fullPath);
            return Utility.Text.Format("{0}_{1}", parentDir, fileName);
        }

        private Asset[] m_Assets;
        private readonly SortedDictionary<string, string> m_ResourceListInfoDict = new();

        public void GenerateList(ResourceCollection resourceCollection = null)
        {
            Prepare(resourceCollection);
            GenerateData();
            SaveBytesData();
            SaveXmlData();
        }

        private void Prepare(ResourceCollection resourceCollection = null)
        {
            if (resourceCollection == null)
            {
                resourceCollection = new ResourceCollection();
                resourceCollection.Load();
            }
            m_Assets = resourceCollection.GetAssets();
            m_ResourceListInfoDict.Clear();
        }

        private void GenerateData()
        {
            foreach (var asset in m_Assets)
            {
                //程序自动生成的，不加入
                if(asset.Resource.Name.StartsWith(ExceptName))
                    continue;
                string fullPath = Utility.Path.GetRegularPath(asset.Name);
                string name = GetNewName(fullPath);
                if (m_ResourceListInfoDict.TryGetValue(name, out string path))
                {
                    throw new GameFrameworkException(Utility.Text.Format("Resource name '{0}' is duplicate, {1} : {2} !", name, path, fullPath));
                }
                m_ResourceListInfoDict.Add(GetNewName(fullPath), fullPath);
            }
            Debug.Log(Utility.Text.Format("生成资源列表，{0}个资源！", m_ResourceListInfoDict.Count));
        }

        private void SaveBytesData()
        {
            using (FileStream fileStream = new FileStream(ResourceListComponent.BytesDataFilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.UTF8))
                {
                    binaryWriter.Write7BitEncodedInt32(m_ResourceListInfoDict.Count);
                    foreach (var kv in m_ResourceListInfoDict)
                    {
                        binaryWriter.Write(kv.Key);
                        binaryWriter.Write(kv.Value);
                    }
                }
            }
            AssetDatabase.Refresh();
            Debug.Log(Utility.Text.Format("生成资源列表文件：{0}！", ResourceListComponent.BytesDataFilePath));
        }

        private void SaveXmlData()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement xmlRoot = xmlDocument.CreateElement("ResourceList");
            xmlDocument.AppendChild(xmlRoot);
            foreach (var kv in m_ResourceListInfoDict)
            {
                XmlElement xmlElementInner = xmlDocument.CreateElement("Resource");
                XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Name");
                xmlAttribute.Value = kv.Key;
                xmlElementInner.Attributes.SetNamedItem(xmlAttribute);
                xmlRoot.AppendChild(xmlElementInner);
                xmlAttribute = xmlDocument.CreateAttribute("Path");
                xmlAttribute.Value = kv.Value;
                xmlElementInner.Attributes.SetNamedItem(xmlAttribute);
                xmlRoot.AppendChild(xmlElementInner);
            }
            xmlDocument.Save(XmlDataFilePath);
            AssetDatabase.Refresh();
            Debug.Log(Utility.Text.Format("生成资源列表文件：{0}！", XmlDataFilePath));
        }
    }
}
