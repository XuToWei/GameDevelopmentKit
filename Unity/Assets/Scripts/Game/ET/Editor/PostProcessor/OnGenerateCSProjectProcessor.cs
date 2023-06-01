using UnityEditor;
using System.Xml;
using System.IO;

namespace ET.Editor
{
    public class OnGenerateCSProjectProcessor: AssetPostprocessor
    {
        private static string OnGeneratedCSProject(string path, string content)
        {
            if (path.EndsWith("Game.csproj"))
            {
                return GenerateCustomProject(content);
            }
            
            if (path.EndsWith("Game.Hot.csproj"))
            {
                return GenerateCustomProject(content);
            }
            
            if (path.EndsWith("ET.Runtime.csproj"))
            {
                return GenerateCustomProject(content);
            }

            if (path.EndsWith("Game.ET.Code.Hotfix.csproj"))
            {
                return GenerateCustomProject(content);
            }

            if (path.EndsWith("Game.ET.Code.Model.csproj"))
            {
                return GenerateCustomProject(content);
            }

            if (path.EndsWith("Game.ET.Code.HotfixView.csproj"))
            {
                return GenerateCustomProject(content);
            }

            if (path.EndsWith("Game.ET.Code.ModelView.csproj"))
            {
                return GenerateCustomProject(content);
            }
            return content;
        }

        private static string GenerateCustomProject(string content, params string[] links)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var newDoc = doc.Clone() as XmlDocument;

            var rootNode = newDoc.GetElementsByTagName("Project")[0];

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

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter tx = new XmlTextWriter(sw))
                {
                    tx.Formatting = Formatting.Indented;
                    newDoc.WriteTo(tx);
                    tx.Flush();
                    return sw.GetStringBuilder().ToString();
                }
            }
        }
    }
}