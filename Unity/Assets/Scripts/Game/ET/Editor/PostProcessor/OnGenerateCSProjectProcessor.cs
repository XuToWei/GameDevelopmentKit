using UnityEditor;
using System.Xml;
using System.IO;

namespace ET.Editor
{
    public class OnGenerateCSProjectProcessor: AssetPostprocessor
    {
        /// <summary>
        /// 文档:https://learn.microsoft.com/zh-cn/visualstudio/gamedev/unity/extensibility/customize-project-files-created-by-vstu#%E6%A6%82%E8%A7%88
        /// </summary>
        private static string OnGeneratedCSProject(string path, string content)
        {
            if (!EditorUserBuildSettings.development)
            {
                content = content.Replace("<Optimize>false</Optimize>", "<Optimize>true</Optimize>");
                content = content.Replace(";DEBUG;", ";");
            }
            
            if (path.EndsWith("Game.ET.Loader.csproj"))
            {
                content = GenerateCustomProject(content);
            }
            
            if (path.EndsWith("ET.csproj"))
            {
                content = GenerateCustomProject(content);
            }

            if (path.EndsWith("Game.ET.Code.Hotfix.csproj"))
            {
                content = GenerateCustomProject(content);
                content = AddCopyAfterBuild(content);
            }

            if (path.EndsWith("Game.ET.Code.Model.csproj"))
            {
                content = GenerateCustomProject(content);
                content = AddCopyAfterBuild(content);
            }

            if (path.EndsWith("Game.ET.Code.HotfixView.csproj"))
            {
                content = GenerateCustomProject(content);
                content = AddCopyAfterBuild(content);
            }

            if (path.EndsWith("Game.ET.Code.ModelView.csproj"))
            {
                content = GenerateCustomProject(content);
                content = AddCopyAfterBuild(content);
            }
            return content;
        }

        /// <summary>
        /// 编译dll文件后额外复制的目录配置
        /// </summary>
        private static string AddCopyAfterBuild(string content)
        {
            content = content.Replace("<Target Name=\"AfterBuild\" />",
                "<Target Name=\"PostBuild\" AfterTargets=\"PostBuildEvent\">\n" +
                $"    <Copy SourceFiles=\"$(TargetDir)/$(TargetName).dll\" DestinationFiles=\"$(ProjectDir)/{BuildAssemblyTool.CodeDir}/$(TargetName).dll.bytes\" ContinueOnError=\"false\" />\n" +
                $"    <Copy SourceFiles=\"$(TargetDir)/$(TargetName).pdb\" DestinationFiles=\"$(ProjectDir)/{BuildAssemblyTool.CodeDir}/$(TargetName).pdb.bytes\" ContinueOnError=\"false\" />\n" +
                "  </Target>\n");
            return content;
        }

        private static string GenerateCustomProject(string content, params string[] links)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var newDoc = doc.Clone() as XmlDocument;

            var rootNode = newDoc.GetElementsByTagName("Project")[0];

            var target = newDoc.CreateElement("Target", newDoc.DocumentElement.NamespaceURI);
            target.SetAttribute("Name", "AfterBuild");
            rootNode.AppendChild(target);

            XmlElement itemGroup = newDoc.CreateElement("ItemGroup", newDoc.DocumentElement.NamespaceURI);
            foreach (var s in links)
            {
                string[] ss = s.Split(' ');
                string p = ss[0];
                string linkStr = ss[1];
                XmlElement compile = newDoc.CreateElement("Compile", newDoc.DocumentElement.NamespaceURI);
                XmlElement link = newDoc.CreateElement("Link", newDoc.DocumentElement.NamespaceURI);
                link.InnerText = linkStr;
                //compile.AppendChild(link);
                compile.SetAttribute("Include", p);
                itemGroup.AppendChild(compile);
            }

            var projectReference = newDoc.CreateElement("ProjectReference", newDoc.DocumentElement.NamespaceURI);
            projectReference.SetAttribute("Include", @"..\Share\Analyzer\Share.Analyzer.csproj");
            projectReference.SetAttribute("OutputItemType", @"Analyzer");
            projectReference.SetAttribute("ReferenceOutputAssembly", @"false");

            var project = newDoc.CreateElement("Project", newDoc.DocumentElement.NamespaceURI);
            project.InnerText = @"{d1f2986b-b296-4a2d-8f12-be9f470014c3}";
            projectReference.AppendChild(project);

            var name = newDoc.CreateElement("Name", newDoc.DocumentElement.NamespaceURI);
            name.InnerText = "Analyzer";
            projectReference.AppendChild(project);

            itemGroup.AppendChild(projectReference);

            rootNode.AppendChild(itemGroup);

            using StringWriter sw = new();
            using XmlTextWriter tx = new(sw);
            tx.Formatting = Formatting.Indented;
            newDoc.WriteTo(tx);
            tx.Flush();
            return sw.GetStringBuilder().ToString();
        }
    }
}