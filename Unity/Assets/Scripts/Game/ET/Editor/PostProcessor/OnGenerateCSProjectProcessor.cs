using UnityEditor;
using System.Xml;
using System.IO;
using Game.Editor;

namespace ET.Editor
{
    public class OnGenerateCSProjectProcessor: AssetPostprocessor
    {
        /// <summary>
        /// 对生成的C#项目文件(.csproj)进行处理
        /// 文档:https://learn.microsoft.com/zh-cn/visualstudio/gamedev/unity/extensibility/customize-project-files-created-by-vstu#%E6%A6%82%E8%A7%88
        /// </summary>
        private static string OnGeneratedCSProject(string path, string content)
        {
            if (path.EndsWith("Game.ET.Loader.csproj"))
            {
                content = GenerateCustomProject(content);
            }
            
            if (path.EndsWith("ET.csproj"))
            {
                content = GenerateCustomProject(content);
            }

            if (path.EndsWith("Game.ET.Code.Hotfix.csproj") ||
                path.EndsWith("Game.ET.Code.Model.csproj") ||
                path.EndsWith("Game.ET.Code.HotfixView.csproj") ||
                path.EndsWith("Game.ET.Code.ModelView.csproj"))
            {
                if (!EditorUserBuildSettings.development)
                {
                    content = content.Replace("<Optimize>false</Optimize>", "<Optimize>true</Optimize>");
                    content = content.Replace(";DEBUG;", ";");
                }
                content = GenerateCustomProject(content);
                content = AddCopyAfterBuild(content);
            }
            return content;
        }

        /// <summary>
        /// 对生成的解决方案文件(.sln)进行处理, 此处主要为了隐藏一些没有作用的C#项目
        /// </summary>
        private static string OnGeneratedSlnSolution(string _, string content)
        {
            // Client
            content = HideCSProject(content, "Ignore.Generate.Client.csproj");
            content = HideCSProject(content, "Ignore.Model.Client.csproj");
            content = HideCSProject(content, "Ignore.Hotfix.Client.csproj");
            content = HideCSProject(content, "Ignore.ModelView.Client.csproj");
            content = HideCSProject(content, "Ignore.HotfixView.Client.csproj");

            // Server
            content = HideCSProject(content, "Ignore.Generate.Server.csproj");
            content = HideCSProject(content, "Ignore.Model.Server.csproj");
            content = HideCSProject(content, "Ignore.Hotfix.Server.csproj");

            // ClientServer
            content = HideCSProject(content, "Ignore.Generate.ClientServer.csproj");

            return content;
        }

        /// <summary>
        /// 自定义C#项目配置
        /// 参考链接:
        /// https://zhuanlan.zhihu.com/p/509046784
        /// https://learn.microsoft.com/zh-cn/visualstudio/ide/reference/build-events-page-project-designer-csharp?view=vs-2022
        /// https://learn.microsoft.com/zh-cn/visualstudio/ide/how-to-specify-build-events-csharp?view=vs-2022
        /// </summary>
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
                compile.AppendChild(link);
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

        /// <summary>
        /// 编译dll文件后额外复制的目录配置
        /// </summary>
        private static string AddCopyAfterBuild(string content)
        {
            return content = content.Replace("<Target Name=\"AfterBuild\" />",
                "<Target Name=\"PostBuild\" AfterTargets=\"PostBuildEvent\">\n" +
                $"    <Copy SourceFiles=\"$(TargetDir)/$(TargetName).dll\" DestinationFiles=\"$(ProjectDir)/{BuildAssemblyTool.CodeDir}/$(TargetName).dll.bytes\" ContinueOnError=\"false\" />\n" +
                $"    <Copy SourceFiles=\"$(TargetDir)/$(TargetName).pdb\" DestinationFiles=\"$(ProjectDir)/{BuildAssemblyTool.CodeDir}/$(TargetName).pdb.bytes\" ContinueOnError=\"false\" />\n" +
                $"    <Copy SourceFiles=\"$(TargetDir)/$(TargetName).dll\" DestinationFiles=\"$(ProjectDir)/{BuildAssemblyHelper.BuildOutputDir}/$(TargetName).dll\" ContinueOnError=\"false\" />\n" +
                $"    <Copy SourceFiles=\"$(TargetDir)/$(TargetName).pdb\" DestinationFiles=\"$(ProjectDir)/{BuildAssemblyHelper.BuildOutputDir}/$(TargetName).pdb\" ContinueOnError=\"false\" />\n" +
                "  </Target>\n");
        }

        /// <summary>
        /// 隐藏指定项目
        /// </summary>
        private static string HideCSProject(string content, string projectName)
        {
            return Regex.Replace(content, $"Project.*{projectName}.*\nEndProject", string.Empty);
        }
    }
}