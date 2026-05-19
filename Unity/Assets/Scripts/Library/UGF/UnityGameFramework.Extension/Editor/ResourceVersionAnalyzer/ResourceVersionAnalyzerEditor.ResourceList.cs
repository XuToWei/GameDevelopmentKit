using System;
using System.Collections.Generic;
using GameFramework;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Extension.Editor
{
    internal sealed partial class ResourceVersionAnalyzerEditor
    {
        private void DrawResourceList()
        {
            DrawVersionFileSelector();

            if (m_VersionData == null)
            {
                EditorGUILayout.HelpBox("Please select and load a version list file (GameFrameworkVersion.dat).", MessageType.Info);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Search:", GUILayout.Width(50f));
                string newSearchText = EditorGUILayout.TextField(m_SearchText);
                if (newSearchText != m_SearchText)
                {
                    m_SearchText = newSearchText;
                    RefreshFilteredResources();
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2f);

            DrawResourceListHeader();

            float detailHeight = m_SelectedResource != null ? position.height * 0.3f : 0f;
            m_ResourceListScroll = EditorGUILayout.BeginScrollView(m_ResourceListScroll, GUILayout.Height(position.height - detailHeight - 170f));
            {
                if (m_FilteredResources != null)
                {
                    for (int i = 0; i < m_FilteredResources.Count; i++)
                    {
                        DrawResourceRow(m_FilteredResources[i], m_VersionData.VersionListType == ResourceVersionAnalyzerController.VersionListType.Updatable);
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.LabelField(Utility.Text.Format("{0} resources displayed.", m_FilteredResources != null ? m_FilteredResources.Count : 0));

            if (m_SelectedResource != null)
            {
                GUILayout.Space(Spacing);
                DrawResourceDetail();
            }
        }

        private void DrawResourceListHeader()
        {
            EditorGUILayout.BeginHorizontal("box");
            {
                if (GUILayout.Button(GetSortColumnTitle("Name", m_SortColumn, 0, m_SortAscending), EditorStyles.boldLabel, GUILayout.MinWidth(200f)))
                {
                    SetSortColumn(0);
                }

                if (GUILayout.Button(GetSortColumnTitle("Extension", m_SortColumn, 1, m_SortAscending), EditorStyles.boldLabel, GUILayout.Width(80f)))
                {
                    SetSortColumn(1);
                }

                if (GUILayout.Button(GetSortColumnTitle("Size", m_SortColumn, 2, m_SortAscending), EditorStyles.boldLabel, GUILayout.Width(100f)))
                {
                    SetSortColumn(2);
                }

                if (m_VersionData != null && m_VersionData.VersionListType == ResourceVersionAnalyzerController.VersionListType.Updatable)
                {
                    if (GUILayout.Button(GetSortColumnTitle("Compressed", m_SortColumn, 3, m_SortAscending), EditorStyles.boldLabel, GUILayout.Width(100f)))
                    {
                        SetSortColumn(3);
                    }
                }

                if (GUILayout.Button(GetSortColumnTitle("Assets", m_SortColumn, 4, m_SortAscending), EditorStyles.boldLabel, GUILayout.Width(80f)))
                {
                    SetSortColumn(4);
                }

                if (GUILayout.Button(GetSortColumnTitle("FileSystem", m_SortColumn, 5, m_SortAscending), EditorStyles.boldLabel, GUILayout.Width(120f)))
                {
                    SetSortColumn(5);
                }

                if (GUILayout.Button(GetSortColumnTitle("Group", m_SortColumn, 6, m_SortAscending), EditorStyles.boldLabel, GUILayout.Width(120f)))
                {
                    SetSortColumn(6);
                }

                EditorGUILayout.LabelField("HashCode", EditorStyles.boldLabel, GUILayout.Width(90f));
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawResourceRow(ResourceVersionAnalyzerController.ResourceInfo resource, bool showCompressed)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(resource.FullName, GUILayout.MinWidth(200f));
                EditorGUILayout.LabelField(resource.Extension, GUILayout.Width(80f));
                EditorGUILayout.LabelField(GetSizeString(resource.Length), GUILayout.Width(100f));
                if (showCompressed)
                {
                    EditorGUILayout.LabelField(GetSizeString(resource.CompressedLength), GUILayout.Width(100f));
                }

                EditorGUILayout.LabelField(resource.AssetCount.ToString(), GUILayout.Width(80f));
                EditorGUILayout.LabelField(resource.FileSystem ?? string.Empty, GUILayout.Width(120f));
                EditorGUILayout.LabelField(GetResourceGroupsString(resource.ResourceGroups), GUILayout.Width(120f));
                EditorGUILayout.LabelField(Utility.Text.Format("{0:X8}", resource.HashCode), GUILayout.Width(90f));
            }
            EditorGUILayout.EndHorizontal();

            Rect rowRect = GUILayoutUtility.GetLastRect();
            if (m_SelectedResource == resource && Event.current.type == EventType.Repaint)
            {
                EditorGUI.DrawRect(rowRect, SelectionColor);
            }

            if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
            {
                if (m_SelectedResource == resource)
                {
                    m_SelectedResource = null;
                    m_SelectedResourceAssets = null;
                }
                else
                {
                    m_SelectedResource = resource;
                    RefreshSelectedResourceAssets();
                }

                Event.current.Use();
            }
        }

        private void DrawResourceDetail()
        {
            EditorGUILayout.LabelField("Resource Detail", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Name", m_SelectedResource.FullName);
                EditorGUILayout.LabelField("Extension", m_SelectedResource.Extension);
                EditorGUILayout.LabelField("Size", GetSizeString(m_SelectedResource.Length));
                if (m_SelectedResource.CompressedLength > 0)
                {
                    EditorGUILayout.LabelField("Compressed Size", GetSizeString(m_SelectedResource.CompressedLength));
                }

                EditorGUILayout.LabelField("FileSystem", m_SelectedResource.FileSystem ?? string.Empty);
                EditorGUILayout.LabelField("HashCode", Utility.Text.Format("{0:X8}", m_SelectedResource.HashCode));

                if (m_SelectedResource.ResourceGroups != null && m_SelectedResource.ResourceGroups.Length > 0)
                {
                    EditorGUILayout.LabelField("Resource Groups", string.Join(", ", m_SelectedResource.ResourceGroups));
                }

                GUILayout.Space(Spacing);

                EditorGUILayout.LabelField(Utility.Text.Format("Assets ({0})", m_SelectedResourceAssets != null ? m_SelectedResourceAssets.Count : 0), EditorStyles.boldLabel);

                m_ResourceDetailScroll = EditorGUILayout.BeginScrollView(m_ResourceDetailScroll);
                {
                    if (m_SelectedResourceAssets != null && m_SelectedResourceAssets.Count > 0)
                    {
                        EditorGUILayout.BeginHorizontal("box");
                        {
                            EditorGUILayout.LabelField("Asset Name", EditorStyles.boldLabel, GUILayout.MinWidth(300f));
                            EditorGUILayout.LabelField("Deps", EditorStyles.boldLabel, GUILayout.Width(50f));
                            EditorGUILayout.LabelField("Refs", EditorStyles.boldLabel, GUILayout.Width(50f));
                        }
                        EditorGUILayout.EndHorizontal();

                        for (int i = 0; i < m_SelectedResourceAssets.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                DrawAssetNameLink(m_SelectedResourceAssets[i].Name, GUILayout.MinWidth(300f));
                                EditorGUILayout.LabelField(m_SelectedResourceAssets[i].DependencyCount.ToString(), GUILayout.Width(50f));
                                EditorGUILayout.LabelField(m_SelectedResourceAssets[i].DependentCount.ToString(), GUILayout.Width(50f));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void RefreshFilteredResources()
        {
            if (m_VersionData == null)
            {
                m_FilteredResources = null;
                return;
            }

            ResourceVersionAnalyzerController.ResourceInfo[] resources = m_VersionData.Resources;
            m_FilteredResources = new List<ResourceVersionAnalyzerController.ResourceInfo>();

            for (int i = 0; i < resources.Length; i++)
            {
                if (!string.IsNullOrEmpty(m_SearchText) &&
                    resources[i].FullName.IndexOf(m_SearchText, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                m_FilteredResources.Add(resources[i]);
            }

            SortFilteredResources();
        }

        private void RefreshSelectedResourceAssets()
        {
            m_SelectedResourceAssets = new List<ResourceVersionAnalyzerController.AssetInfo>();
            if (m_VersionData == null || m_SelectedResource == null)
            {
                return;
            }

            ResourceVersionAnalyzerController.AssetInfo[] assets = m_VersionData.AssetInfos;
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i].ResourceFullName == m_SelectedResource.FullName)
                {
                    m_SelectedResourceAssets.Add(assets[i]);
                }
            }
        }

        private void SetSortColumn(int column)
        {
            if (m_SortColumn == column)
            {
                m_SortAscending = !m_SortAscending;
            }
            else
            {
                m_SortColumn = column;
                m_SortAscending = true;
            }

            SortFilteredResources();
        }

        private void SortFilteredResources()
        {
            if (m_FilteredResources == null)
            {
                return;
            }

            Comparison<ResourceVersionAnalyzerController.ResourceInfo> comparison;
            switch (m_SortColumn)
            {
                case 0:
                    comparison = (a, b) => string.Compare(a.FullName, b.FullName, StringComparison.Ordinal);
                    break;
                case 1:
                    comparison = (a, b) => string.Compare(a.Extension, b.Extension, StringComparison.Ordinal);
                    break;
                case 2:
                    comparison = (a, b) => a.Length.CompareTo(b.Length);
                    break;
                case 3:
                    comparison = (a, b) => a.CompressedLength.CompareTo(b.CompressedLength);
                    break;
                case 4:
                    comparison = (a, b) => a.AssetCount.CompareTo(b.AssetCount);
                    break;
                case 5:
                    comparison = (a, b) => string.Compare(a.FileSystem ?? string.Empty, b.FileSystem ?? string.Empty, StringComparison.Ordinal);
                    break;
                case 6:
                    comparison = (a, b) => string.Compare(GetResourceGroupsString(a.ResourceGroups), GetResourceGroupsString(b.ResourceGroups), StringComparison.Ordinal);
                    break;
                default:
                    comparison = (a, b) => string.Compare(a.FullName, b.FullName, StringComparison.Ordinal);
                    break;
            }

            if (m_SortAscending)
            {
                m_FilteredResources.Sort(comparison);
            }
            else
            {
                m_FilteredResources.Sort((a, b) => comparison(b, a));
            }
        }
    }
}
