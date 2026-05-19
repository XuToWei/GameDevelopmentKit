using GameFramework;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Extension.Editor
{
    internal sealed partial class ResourceVersionAnalyzerEditor
    {
        private void DrawSummary()
        {
            DrawVersionFileSelector();

            if (m_VersionData == null)
            {
                EditorGUILayout.HelpBox("Please select and load a version list file (GameFrameworkVersion.dat).", MessageType.Info);
                return;
            }

            m_SummaryScroll = EditorGUILayout.BeginScrollView(m_SummaryScroll);
            {
                DrawVersionInfo(m_VersionData);

                GUILayout.Space(Spacing);
                DrawDistributionTable("Distribution by Extension", m_ExtensionDistribution, m_VersionData.TotalLength);

                GUILayout.Space(Spacing);
                DrawDistributionTable("Distribution by File System", m_FileSystemDistribution, m_VersionData.TotalLength);

                GUILayout.Space(Spacing);
                DrawDistributionTable("Distribution by Resource Group", m_ResourceGroupDistribution, m_VersionData.TotalLength);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawVersionInfo(ResourceVersionAnalyzerController.VersionData data)
        {
            EditorGUILayout.LabelField("Version Information", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Version List Type", data.VersionListType.ToString());
                EditorGUILayout.LabelField("Applicable Game Version", data.ApplicableGameVersion);
                EditorGUILayout.LabelField("Internal Resource Version", data.InternalResourceVersion.ToString());
                EditorGUILayout.LabelField("Asset Count", data.AssetCount.ToString());
                EditorGUILayout.LabelField("Resource Count", data.ResourceCount.ToString());
                EditorGUILayout.LabelField("File System Count", data.FileSystemCount.ToString());
                EditorGUILayout.LabelField("Resource Group Count", data.ResourceGroupCount.ToString());
                EditorGUILayout.LabelField("Total Size", GetSizeString(data.TotalLength));
                if (data.VersionListType == ResourceVersionAnalyzerController.VersionListType.Updatable)
                {
                    EditorGUILayout.LabelField("Total Compressed Size", GetSizeString(data.TotalCompressedLength));
                    if (data.TotalLength > 0)
                    {
                        float ratio = (float)data.TotalCompressedLength / data.TotalLength * 100f;
                        EditorGUILayout.LabelField("Compression Ratio", Utility.Text.Format("{0:F1}%", ratio));
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawDistributionTable(string title, ResourceVersionAnalyzerController.DistributionInfo[] distribution, long totalLength)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            if (distribution == null || distribution.Length == 0)
            {
                EditorGUILayout.LabelField("  No data.");
                return;
            }

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.Width(200f));
                    EditorGUILayout.LabelField("Count", EditorStyles.boldLabel, GUILayout.Width(60f));
                    EditorGUILayout.LabelField("Size", EditorStyles.boldLabel, GUILayout.Width(100f));
                    EditorGUILayout.LabelField("Percentage", EditorStyles.boldLabel, GUILayout.Width(80f));
                }
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < distribution.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(distribution[i].Name, GUILayout.Width(200f));
                        EditorGUILayout.LabelField(distribution[i].Count.ToString(), GUILayout.Width(60f));
                        EditorGUILayout.LabelField(GetSizeString(distribution[i].TotalLength), GUILayout.Width(100f));
                        if (totalLength > 0)
                        {
                            float percentage = (float)distribution[i].TotalLength / totalLength * 100f;
                            EditorGUILayout.LabelField(Utility.Text.Format("{0:F1}%", percentage), GUILayout.Width(80f));

                            Rect progressRect = GUILayoutUtility.GetRect(0f, 16f, GUILayout.ExpandWidth(true));
                            EditorGUI.ProgressBar(progressRect, percentage / 100f, string.Empty);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
