using System;
using System.Collections.Generic;
using GameFramework;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Extension.Editor
{
    internal sealed partial class ResourceVersionAnalyzerEditor
    {
        private void DrawVersionCompare()
        {
            EditorGUILayout.LabelField("Version A", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                m_CompareFilePathA = EditorGUILayout.TextField(m_CompareFilePathA);
                if (GUILayout.Button("Browse...", GUILayout.Width(ButtonWidth)))
                {
                    string path = EditorUtility.OpenFilePanel("Select Version A", GetDefaultBrowsePath(), "dat");
                    if (!string.IsNullOrEmpty(path))
                    {
                        m_CompareFilePathA = path;
                        m_CompareVersionA = null;
                        m_CompareResult = null;
                        m_FilteredCompareItems = null;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(Spacing);

            EditorGUILayout.LabelField("Version B", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                m_CompareFilePathB = EditorGUILayout.TextField(m_CompareFilePathB);
                if (GUILayout.Button("Browse...", GUILayout.Width(ButtonWidth)))
                {
                    string path = EditorUtility.OpenFilePanel("Select Version B", GetDefaultBrowsePath(), "dat");
                    if (!string.IsNullOrEmpty(path))
                    {
                        m_CompareFilePathB = path;
                        m_CompareVersionB = null;
                        m_CompareResult = null;
                        m_FilteredCompareItems = null;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(Spacing);

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_CompareFilePathA) || string.IsNullOrEmpty(m_CompareFilePathB));
            {
                if (GUILayout.Button("Compare", GUILayout.Height(30f)))
                {
                    ExecuteCompare();
                }
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(Spacing);

            if (m_CompareResult == null)
            {
                return;
            }

            DrawCompareSummary();

            GUILayout.Space(Spacing);

            int compareFilterIndex = GUILayout.Toolbar(m_CompareFilterIndex, CompareFilterNames, GUILayout.Height(24f));
            if (compareFilterIndex != m_CompareFilterIndex)
            {
                m_CompareFilterIndex = compareFilterIndex;
                RefreshFilteredCompareItems();
            }

            GUILayout.Space(2f);
            DrawCompareListHeader();

            m_CompareScroll = EditorGUILayout.BeginScrollView(m_CompareScroll);
            {
                if (m_FilteredCompareItems != null)
                {
                    for (int i = 0; i < m_FilteredCompareItems.Count; i++)
                    {
                        DrawCompareRow(m_FilteredCompareItems[i]);
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.LabelField(Utility.Text.Format("{0} items displayed.", m_FilteredCompareItems != null ? m_FilteredCompareItems.Count : 0));
        }

        private void DrawCompareSummary()
        {
            EditorGUILayout.LabelField("Comparison Summary", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            {
                if (m_CompareVersionA != null && m_CompareVersionB != null)
                {
                    EditorGUILayout.LabelField("Version A",
                        Utility.Text.Format("{0} (v{1}) - {2} resources",
                            m_CompareVersionA.ApplicableGameVersion, m_CompareVersionA.InternalResourceVersion, m_CompareVersionA.ResourceCount));
                    EditorGUILayout.LabelField("Version B",
                        Utility.Text.Format("{0} (v{1}) - {2} resources",
                            m_CompareVersionB.ApplicableGameVersion, m_CompareVersionB.InternalResourceVersion, m_CompareVersionB.ResourceCount));
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    Color originalColor = GUI.color;

                    GUI.color = AddedColor;
                    EditorGUILayout.LabelField(Utility.Text.Format("Added: {0}", m_CompareResult.AddedCount), GUILayout.Width(120f));
                    GUI.color = RemovedColor;
                    EditorGUILayout.LabelField(Utility.Text.Format("Removed: {0}", m_CompareResult.RemovedCount), GUILayout.Width(120f));
                    GUI.color = ModifiedColor;
                    EditorGUILayout.LabelField(Utility.Text.Format("Modified: {0}", m_CompareResult.ModifiedCount), GUILayout.Width(120f));
                    GUI.color = originalColor;
                    EditorGUILayout.LabelField(Utility.Text.Format("Unchanged: {0}", m_CompareResult.UnchangedCount), GUILayout.Width(120f));
                }
                EditorGUILayout.EndHorizontal();

                string sizePrefix = m_CompareResult.TotalSizeDifference >= 0 ? "+" : string.Empty;
                EditorGUILayout.LabelField("Total Size Difference",
                    Utility.Text.Format("{0}{1}", sizePrefix, GetSizeString(m_CompareResult.TotalSizeDifference)));
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCompareListHeader()
        {
            EditorGUILayout.BeginHorizontal("box");
            {
                if (GUILayout.Button(GetSortColumnTitle("Status", m_CompareSortColumn, 0, m_CompareSortAscending), EditorStyles.boldLabel, GUILayout.Width(80f)))
                {
                    SetCompareSortColumn(0);
                }

                if (GUILayout.Button(GetSortColumnTitle("Name", m_CompareSortColumn, 1, m_CompareSortAscending), EditorStyles.boldLabel, GUILayout.MinWidth(200f)))
                {
                    SetCompareSortColumn(1);
                }

                if (GUILayout.Button(GetSortColumnTitle("Size A", m_CompareSortColumn, 2, m_CompareSortAscending), EditorStyles.boldLabel, GUILayout.Width(100f)))
                {
                    SetCompareSortColumn(2);
                }

                if (GUILayout.Button(GetSortColumnTitle("Size B", m_CompareSortColumn, 3, m_CompareSortAscending), EditorStyles.boldLabel, GUILayout.Width(100f)))
                {
                    SetCompareSortColumn(3);
                }

                if (GUILayout.Button(GetSortColumnTitle("Difference", m_CompareSortColumn, 4, m_CompareSortAscending), EditorStyles.boldLabel, GUILayout.Width(100f)))
                {
                    SetCompareSortColumn(4);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCompareRow(ResourceVersionAnalyzerController.CompareItem item)
        {
            Color originalColor = GUI.color;
            switch (item.Status)
            {
                case ResourceVersionAnalyzerController.CompareStatus.Added:
                    GUI.color = AddedColor;
                    break;
                case ResourceVersionAnalyzerController.CompareStatus.Removed:
                    GUI.color = RemovedColor;
                    break;
                case ResourceVersionAnalyzerController.CompareStatus.Modified:
                    GUI.color = ModifiedColor;
                    break;
            }

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(item.Status.ToString(), GUILayout.Width(80f));
                EditorGUILayout.LabelField(item.FullName, GUILayout.MinWidth(200f));
                EditorGUILayout.LabelField(item.ResourceA != null ? GetSizeString(item.ResourceA.Length) : "-", GUILayout.Width(100f));
                EditorGUILayout.LabelField(item.ResourceB != null ? GetSizeString(item.ResourceB.Length) : "-", GUILayout.Width(100f));
                if (item.SizeDifference != 0)
                {
                    string prefix = item.SizeDifference > 0 ? "+" : string.Empty;
                    EditorGUILayout.LabelField(Utility.Text.Format("{0}{1}", prefix, GetSizeString(item.SizeDifference)), GUILayout.Width(100f));
                }
                else
                {
                    EditorGUILayout.LabelField("-", GUILayout.Width(100f));
                }
            }
            EditorGUILayout.EndHorizontal();

            GUI.color = originalColor;
        }

        private void ExecuteCompare()
        {
            try
            {
                if (m_CompareVersionA == null || m_CompareVersionA.FilePath != m_CompareFilePathA)
                {
                    m_CompareVersionA = m_Controller.Load(m_CompareFilePathA);
                }

                if (m_CompareVersionB == null || m_CompareVersionB.FilePath != m_CompareFilePathB)
                {
                    m_CompareVersionB = m_Controller.Load(m_CompareFilePathB);
                }

                m_CompareResult = m_Controller.Compare(m_CompareVersionA, m_CompareVersionB);
                m_CompareFilterIndex = 0;
                RefreshFilteredCompareItems();
                Debug.Log(Utility.Text.Format("Compare version lists success. Added: {0}, Removed: {1}, Modified: {2}, Unchanged: {3}.",
                    m_CompareResult.AddedCount, m_CompareResult.RemovedCount, m_CompareResult.ModifiedCount, m_CompareResult.UnchangedCount));
            }
            catch (Exception e)
            {
                m_CompareResult = null;
                m_FilteredCompareItems = null;
                Debug.LogError(Utility.Text.Format("Compare version lists failure. Exception: {0}", e.Message));
            }
        }

        private void RefreshFilteredCompareItems()
        {
            if (m_CompareResult == null)
            {
                m_FilteredCompareItems = null;
                return;
            }

            ResourceVersionAnalyzerController.CompareItem[] items = m_CompareResult.Items;
            m_FilteredCompareItems = new List<ResourceVersionAnalyzerController.CompareItem>();

            for (int i = 0; i < items.Length; i++)
            {
                switch (m_CompareFilterIndex)
                {
                    case 1:
                        if (items[i].Status != ResourceVersionAnalyzerController.CompareStatus.Added) continue;
                        break;
                    case 2:
                        if (items[i].Status != ResourceVersionAnalyzerController.CompareStatus.Removed) continue;
                        break;
                    case 3:
                        if (items[i].Status != ResourceVersionAnalyzerController.CompareStatus.Modified) continue;
                        break;
                }

                m_FilteredCompareItems.Add(items[i]);
            }

            SortFilteredCompareItems();
        }

        private void SetCompareSortColumn(int column)
        {
            if (m_CompareSortColumn == column)
            {
                m_CompareSortAscending = !m_CompareSortAscending;
            }
            else
            {
                m_CompareSortColumn = column;
                m_CompareSortAscending = true;
            }

            SortFilteredCompareItems();
        }

        private void SortFilteredCompareItems()
        {
            if (m_FilteredCompareItems == null)
            {
                return;
            }

            Comparison<ResourceVersionAnalyzerController.CompareItem> comparison;
            switch (m_CompareSortColumn)
            {
                case 0:
                    comparison = (a, b) => a.Status.CompareTo(b.Status);
                    break;
                case 1:
                    comparison = (a, b) => string.Compare(a.FullName, b.FullName, StringComparison.Ordinal);
                    break;
                case 2:
                    comparison = (a, b) => (a.ResourceA != null ? a.ResourceA.Length : 0).CompareTo(b.ResourceA != null ? b.ResourceA.Length : 0);
                    break;
                case 3:
                    comparison = (a, b) => (a.ResourceB != null ? a.ResourceB.Length : 0).CompareTo(b.ResourceB != null ? b.ResourceB.Length : 0);
                    break;
                case 4:
                    comparison = (a, b) => a.SizeDifference.CompareTo(b.SizeDifference);
                    break;
                default:
                    comparison = (a, b) => a.Status.CompareTo(b.Status);
                    break;
            }

            if (m_CompareSortAscending)
            {
                m_FilteredCompareItems.Sort(comparison);
            }
            else
            {
                m_FilteredCompareItems.Sort((a, b) => comparison(b, a));
            }
        }
    }
}
