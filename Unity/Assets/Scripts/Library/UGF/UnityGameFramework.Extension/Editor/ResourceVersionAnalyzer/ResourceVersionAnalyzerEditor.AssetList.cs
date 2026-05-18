using System;
using System.Collections.Generic;
using GameFramework;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Extension.Editor
{
    internal sealed partial class ResourceVersionAnalyzerEditor
    {
        private void DrawAssetList()
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
                string newSearchText = EditorGUILayout.TextField(m_AssetSearchText);
                if (newSearchText != m_AssetSearchText)
                {
                    m_AssetSearchText = newSearchText;
                    RefreshFilteredAssets();
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2f);
            DrawAssetListHeader();

            float detailHeight = m_SelectedAsset != null ? position.height * 0.3f : 0f;
            m_AssetListScroll = EditorGUILayout.BeginScrollView(m_AssetListScroll, GUILayout.Height(position.height - detailHeight - 170f));
            {
                if (m_FilteredAssets != null)
                {
                    for (int i = 0; i < m_FilteredAssets.Count; i++)
                    {
                        DrawAssetRow(m_FilteredAssets[i]);
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.LabelField(Utility.Text.Format("{0} assets displayed.", m_FilteredAssets != null ? m_FilteredAssets.Count : 0));

            if (m_SelectedAsset != null)
            {
                GUILayout.Space(Spacing);
                DrawAssetDetail();
            }
        }

        private void DrawAssetListHeader()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(GetSortColumnTitle("Asset Name", m_AssetSortColumn, 0, m_AssetSortAscending), EditorStyles.boldLabel, GUILayout.MinWidth(250f)))
                {
                    SetAssetSortColumn(0);
                }

                if (GUILayout.Button(GetSortColumnTitle("Resource", m_AssetSortColumn, 1, m_AssetSortAscending), EditorStyles.boldLabel, GUILayout.Width(200f)))
                {
                    SetAssetSortColumn(1);
                }

                if (GUILayout.Button(GetSortColumnTitle("Group", m_AssetSortColumn, 2, m_AssetSortAscending), EditorStyles.boldLabel, GUILayout.Width(120f)))
                {
                    SetAssetSortColumn(2);
                }

                if (GUILayout.Button(GetSortColumnTitle("Size", m_AssetSortColumn, 3, m_AssetSortAscending), EditorStyles.boldLabel, GUILayout.Width(100f)))
                {
                    SetAssetSortColumn(3);
                }

                if (GUILayout.Button(GetSortColumnTitle("Dep Size", m_AssetSortColumn, 4, m_AssetSortAscending), EditorStyles.boldLabel, GUILayout.Width(100f)))
                {
                    SetAssetSortColumn(4);
                }

                if (GUILayout.Button(GetSortColumnTitle("Deps", m_AssetSortColumn, 5, m_AssetSortAscending), EditorStyles.boldLabel, GUILayout.Width(80f)))
                {
                    SetAssetSortColumn(5);
                }

                if (GUILayout.Button(GetSortColumnTitle("Refs", m_AssetSortColumn, 6, m_AssetSortAscending), EditorStyles.boldLabel, GUILayout.Width(80f)))
                {
                    SetAssetSortColumn(6);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAssetRow(ResourceVersionAnalyzerController.AssetInfo asset)
        {
            bool isSelected = m_SelectedAsset == asset;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(asset.Name, GUILayout.MinWidth(250f));
                EditorGUILayout.LabelField(asset.ResourceFullName ?? string.Empty, GUILayout.Width(200f));
                EditorGUILayout.LabelField(GetResourceGroupsByAssetName(asset.Name), GUILayout.Width(120f));
                EditorGUILayout.LabelField(GetSizeString(GetResourceLengthByAssetName(asset.Name)), GUILayout.Width(100f));
                EditorGUILayout.LabelField(GetSizeString(GetDependencyResourceTotalSize(asset.Name)), GUILayout.Width(100f));
                EditorGUILayout.LabelField(asset.DependencyCount.ToString(), GUILayout.Width(80f));
                EditorGUILayout.LabelField(asset.DependentCount.ToString(), GUILayout.Width(80f));
            }
            EditorGUILayout.EndHorizontal();

            Rect rowRect = GUILayoutUtility.GetLastRect();
            if (isSelected && Event.current.type == EventType.Repaint)
            {
                EditorGUI.DrawRect(rowRect, SelectionColor);
            }

            if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
            {
                m_SelectedAsset = isSelected ? null : asset;
                Event.current.Use();
            }
        }

        private void DrawAssetDetail()
        {
            EditorGUILayout.LabelField("Asset Detail", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("Name");
                    DrawAssetNameLink(m_SelectedAsset.Name);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Resource", m_SelectedAsset.ResourceFullName ?? string.Empty);
                EditorGUILayout.LabelField("Resource Size", GetSizeString(GetResourceLengthByAssetName(m_SelectedAsset.Name)));
                EditorGUILayout.LabelField("Dependency Resource Total Size", GetSizeString(GetDependencyResourceTotalSize(m_SelectedAsset.Name)));

                m_AssetDetailScroll = EditorGUILayout.BeginScrollView(m_AssetDetailScroll);
                {
                    EditorGUILayout.LabelField(Utility.Text.Format("Dependencies ({0})", m_SelectedAsset.DependencyCount), EditorStyles.boldLabel);
                    if (m_SelectedAsset.DependencyCount > 0)
                    {
                        EditorGUI.indentLevel++;
                        for (int i = 0; i < m_SelectedAsset.DependencyAssetNames.Length; i++)
                        {
                            string depAssetName = m_SelectedAsset.DependencyAssetNames[i];
                            string depResource = GetResourceFullNameByAssetName(depAssetName);
                            int depResourceLength = GetResourceLengthByAssetName(depAssetName);
                            EditorGUILayout.BeginHorizontal();
                            {
                                DrawAssetNameLink(depAssetName, GUILayout.MinWidth(250f));
                                EditorGUILayout.LabelField(depResource, GUILayout.Width(200f));
                                EditorGUILayout.LabelField(GetSizeString(depResourceLength), GUILayout.Width(100f));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUI.indentLevel--;
                    }

                    GUILayout.Space(Spacing);
                    EditorGUILayout.LabelField(Utility.Text.Format("Dependents ({0})", m_SelectedAsset.DependentCount), EditorStyles.boldLabel);
                    if (m_SelectedAsset.DependentCount > 0)
                    {
                        EditorGUI.indentLevel++;
                        for (int i = 0; i < m_SelectedAsset.DependentAssetNames.Length; i++)
                        {
                            string refAssetName = m_SelectedAsset.DependentAssetNames[i];
                            string refResource = GetResourceFullNameByAssetName(refAssetName);
                            int refResourceLength = GetResourceLengthByAssetName(refAssetName);
                            EditorGUILayout.BeginHorizontal();
                            {
                                DrawAssetNameLink(refAssetName, GUILayout.MinWidth(250f));
                                EditorGUILayout.LabelField(refResource, GUILayout.Width(200f));
                                EditorGUILayout.LabelField(GetSizeString(refResourceLength), GUILayout.Width(100f));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void RefreshFilteredAssets()
        {
            if (m_VersionData == null)
            {
                m_FilteredAssets = null;
                return;
            }

            ResourceVersionAnalyzerController.AssetInfo[] assets = m_VersionData.AssetInfos;
            m_FilteredAssets = new List<ResourceVersionAnalyzerController.AssetInfo>();

            for (int i = 0; i < assets.Length; i++)
            {
                if (!string.IsNullOrEmpty(m_AssetSearchText) &&
                    assets[i].Name.IndexOf(m_AssetSearchText, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                m_FilteredAssets.Add(assets[i]);
            }

            SortFilteredAssets();
        }

        private void SetAssetSortColumn(int column)
        {
            if (m_AssetSortColumn == column)
            {
                m_AssetSortAscending = !m_AssetSortAscending;
            }
            else
            {
                m_AssetSortColumn = column;
                m_AssetSortAscending = true;
            }

            SortFilteredAssets();
        }

        private void SortFilteredAssets()
        {
            if (m_FilteredAssets == null)
            {
                return;
            }

            Comparison<ResourceVersionAnalyzerController.AssetInfo> comparison;
            switch (m_AssetSortColumn)
            {
                case 0:
                    comparison = (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal);
                    break;
                case 1:
                    comparison = (a, b) => string.Compare(a.ResourceFullName ?? string.Empty, b.ResourceFullName ?? string.Empty, StringComparison.Ordinal);
                    break;
                case 2:
                    comparison = (a, b) => string.Compare(GetResourceGroupsByAssetName(a.Name), GetResourceGroupsByAssetName(b.Name), StringComparison.Ordinal);
                    break;
                case 3:
                    comparison = (a, b) => GetResourceLengthByAssetName(a.Name).CompareTo(GetResourceLengthByAssetName(b.Name));
                    break;
                case 4:
                    comparison = (a, b) => GetDependencyResourceTotalSize(a.Name).CompareTo(GetDependencyResourceTotalSize(b.Name));
                    break;
                case 5:
                    comparison = (a, b) => a.DependencyCount.CompareTo(b.DependencyCount);
                    break;
                case 6:
                    comparison = (a, b) => a.DependentCount.CompareTo(b.DependentCount);
                    break;
                default:
                    comparison = (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal);
                    break;
            }

            if (m_AssetSortAscending)
            {
                m_FilteredAssets.Sort(comparison);
            }
            else
            {
                m_FilteredAssets.Sort((a, b) => comparison(b, a));
            }
        }
    }
}
