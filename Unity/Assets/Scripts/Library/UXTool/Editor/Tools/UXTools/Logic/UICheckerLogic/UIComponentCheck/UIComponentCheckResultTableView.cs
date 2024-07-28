#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine.UI;


namespace ThunderFireUITool
{
    //带数据的TreeViewItem
    public class UIComponentCheckResultViewItem : TreeViewItem
    {
        public GameObject prefabGo;
        public string prefabPath;
        public List<string> nodePaths;
        public List<long> nodeFileIds;
    }

    public enum ResultColumns2
    {
        Name,
        Path,
        Nodes,
        Modify
    }

    //结果展示
    public class UIComponentCheckResultTableView : EditorUIUtils.UIIMTreeView<UIComponentCheckResultViewItem>
    {
        private FieldInfo selectedFieldInfo;
        private UICommonScriptCheckWindow.ModificationMode currentModificationMode;
        private Type selectedScriptType;
        private int selectedOperation;
        public enum modificationScriptOperations
        {
            Add,
            Delete
        }
        public UIComponentCheckResultTableView() : base()
        { }

        public static long GetLocalIdentfierInFile(UnityEngine.Object obj)
        {
            PropertyInfo info = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
            SerializedObject sObj = new SerializedObject(obj);
            info.SetValue(sObj, InspectorMode.Debug, null);
            SerializedProperty localIdProp = sObj.FindProperty("m_LocalIdentfierInFile");
            return localIdProp.longValue;
        }

        //生成ColumnHeader
        public override List<MultiColumnHeaderState.Column> CreateDefaultMultiColumns()
        {
            var columns = new List<MultiColumnHeaderState.Column>
            {
                //图标+名称
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(UICommonScriptCheckWindow.CheckResult_NameString),
                    width = 200,
                    minWidth = 60,
                    allowToggleVisibility = false,
                },
                //路径
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(UICommonScriptCheckWindow.CheckResult_PathString),
                    width = 660,
                    minWidth = 60,
                    allowToggleVisibility = false,
                },
                //符合条件的节点
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(UICommonScriptCheckWindow.CheckResult_ObjectListString),
                    width = 220,
                    minWidth = 60,
                    allowToggleVisibility = true,
                    canSort = false
                },
                //修改节点
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(UICommonScriptCheckWindow.ModifyString),
                    width = 100,
                    minWidth = 60,
                    allowToggleVisibility = true,
                    canSort = false
                }
                
            };
            return columns;
        }
        
        //生成Tree
        public void UpdateTree(List<UIComponentCheckResultViewItem> checkResults)
        {
            AssetRoot.children.Clear();
            int elementCount = 0;
            foreach (var result in checkResults)
            {
                elementCount++;
                UIComponentCheckResultViewItem rs = new UIComponentCheckResultViewItem()
                {
                    id = elementCount,
                    depth = 0,
                    prefabGo = result.prefabGo,
                    prefabPath = result.prefabPath,
                    nodePaths = result.nodePaths,
                    nodeFileIds = result.nodeFileIds
                };
                AssetRoot.AddChild(rs);
            }

            Reload();
        }
        
        //响应双击事件
        protected override void DoubleClickedItem(int id)
        {
            var item = (UIComponentCheckResultViewItem)FindItem(id, rootItem);

            if (item != null)
            {
                var assetObject = AssetDatabase.LoadAssetAtPath(item.prefabPath, typeof(UnityEngine.Object));
                if (assetObject is GameObject)
                {
                    //是Prefab 打开prefab 并在Hierarchy中高亮引用资源的节点
                    AssetDatabase.OpenAsset(assetObject);
                    if (PrefabStageUtils.GetCurrentPrefabStage() != null)
                    {
                        var go = PrefabStageUtils.GetCurrentPrefabStage().prefabContentsRoot;
                        var transList = go.GetComponentsInChildren<Transform>().ToList();

                        UICommonScriptCheckWindow.checkResultGoTransList.Clear();
                        foreach (var trans in transList)
                        {
                            var fileId = GetLocalIdentfierInFile(trans.gameObject);

                            if (item.nodeFileIds.Contains(fileId))
                            {
                                UICommonScriptCheckWindow.checkResultGoTransList.Add(trans);
                            }
                        }
                    }
                }
                else
                {
                    //不是Prefab 在ProjectWindow中高亮双击资源
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = assetObject;
                    EditorGUIUtility.PingObject(assetObject);
                }
            }
        }

        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            UIComponentCheckResultViewItem e = item as UIComponentCheckResultViewItem;
            if (e == null ) return base.GetCustomRowHeight(row, item);

            if (e.nodePaths.Count > 0)
            {
                return EditorGUIUtility.singleLineHeight * e.nodePaths.Count + 5;
            }

            return base.GetCustomRowHeight(row, item);
        }

        protected override void CellGUI(Rect cellRect, UIComponentCheckResultViewItem item, int columnIndex, RowGUIArgs args)
        {
            var cellStyle = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal =
                {
                    textColor = Color.white
                }
            };

            switch ((ResultColumns2)args.GetColumn(columnIndex))
            {
                case ResultColumns2.Name:
                {
                    //CenterRectUsingSingleLineHeight(ref cellRect);
                    cellRect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.ObjectField(cellRect, item.prefabGo, typeof(GameObject), false);
                }
                    break;
                case ResultColumns2.Path:
                {
                    GUI.Label(cellRect, item.prefabPath, cellStyle);
                }
                    break;
                case ResultColumns2.Nodes:
                {
                    EditorGUILayout.BeginVertical();
                    for (int i = 0; i < item.nodePaths.Count; i++)
                    {
                        var rect = new Rect()
                        {
                            x = cellRect.x,
                            y = cellRect.y + EditorGUIUtility.singleLineHeight * i,
                            width = cellRect.width,
                            height = EditorGUIUtility.singleLineHeight
                        };
                        GUI.Label(rect, item.nodePaths[i], cellStyle);
                    }

                    EditorGUILayout.EndVertical();
                }
                    break;
                case ResultColumns2.Modify:
                {
                    var buttonRect = new Rect()
                        {
                            x = cellRect.x,
                            y = cellRect.y + (cellRect.height - EditorGUIUtility.singleLineHeight) / 2,
                            width = cellRect.width,
                            height = EditorGUIUtility.singleLineHeight
                        };
                    if (GUI.Button(buttonRect, UICommonScriptCheckWindow.ModifyString))
                    {
                        ModifySingleResult(item);
                    }
                }
                    break;
            }
        }

        //根据资源信息获取资源图标
        private Texture2D GetIcon(string path)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            if (obj != null)
            {
                Texture2D icon = AssetPreview.GetMiniThumbnail(obj);
                if (icon == null)
                    icon = AssetPreview.GetMiniTypeThumbnail(obj.GetType());
                return icon;
            }

            return null;
        }

        public void ModifyAllResults(FieldInfo selectedFieldInfo)
        {
            foreach (var item in rootItem.children)
            {
                var resultItem = (UIComponentCheckResultViewItem)item;
                if (resultItem != null)
                {
                    ModifyGameObject(resultItem, selectedFieldInfo);
                }
            }
        }

        public void ModifySelectedResults(FieldInfo selectedFieldInfo)
        {
            foreach (var item in GetSelection())
            {
                var resultItem = (UIComponentCheckResultViewItem)FindItem(item, rootItem);
                if (resultItem != null)
                {
                    ModifyGameObject(resultItem, selectedFieldInfo);
                }
            }
        }

        private void ModifyGameObject(UIComponentCheckResultViewItem resultData, FieldInfo selectedFieldInfo)
        {
            var prefabRoot = resultData.prefabGo;
            foreach (var nodeId in resultData.nodeFileIds)
            {
                var component = FindComponentByLocalId(prefabRoot, selectedFieldInfo.DeclaringType, nodeId);
                if (component != null)
                {
                    ModifyFieldValue(component, selectedFieldInfo);
                }
            }
            // 保存Prefab的修改
            PrefabUtility.SavePrefabAsset(prefabRoot);
        }

        public void ModifySingleResult(UIComponentCheckResultViewItem resultItem)
        {
            switch (currentModificationMode)
            {
                case UICommonScriptCheckWindow.ModificationMode.ModifyScriptValue:
                    ModifyGameObject(resultItem, selectedFieldInfo);
                    break;
                case UICommonScriptCheckWindow.ModificationMode.AddRemoveScript:
                    AddRemoveScript(resultItem, selectedScriptType, selectedOperation);
                    break;
            }
        }

        public void SetSelectedField(FieldInfo fieldInfo)
        {
            selectedFieldInfo = fieldInfo;
        }

        public void SetModificationMode(UICommonScriptCheckWindow.ModificationMode mode)
        {
            currentModificationMode = mode;
        }

        public void SetSelectedScriptType(Type scriptType, int operationIndex)
        {
            selectedScriptType = scriptType;
            selectedOperation = operationIndex;
        }


        private Component FindComponentByLocalId(GameObject root, Type componentType, long localId)
        {
            var allTransforms = root.GetComponentsInChildren<Transform>();
            foreach (var trans in allTransforms)
            {
                if (UIComponentCheckResultTableView.GetLocalIdentfierInFile(trans.gameObject) == localId)
                {
                    return trans.GetComponent(componentType);
                }
            }
            return null;
        }
        

        private void ModifyFieldValue(Component component, FieldInfo field)
        {
            Type fieldType = field.FieldType;
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type listElementType = fieldType.GetGenericArguments()[0];
                if (listElementType == typeof(int))
                {
                    field.SetValue(component,UICommonScriptCheckWindow.modifyField.intListValue);
                }
                else if (listElementType == typeof(float))
                {
                    field.SetValue(component,UICommonScriptCheckWindow.modifyField.floatListValue);
                }
                else if (listElementType == typeof(Color))
                {
                    field.SetValue(component,UICommonScriptCheckWindow.modifyField.colorListValue);
                }
            }
            else
            {
                var result = Convert.ChangeType(UICommonScriptCheckWindow.modifyField.result, fieldType);
                field.SetValue(component,result); 
            }
            
        }


        public void AddRemoveScriptAllResults(Type scriptType, int operationIndex)
        {
            foreach (var item in rootItem.children)
            {
                var resultItem = (UIComponentCheckResultViewItem)item;
                if (resultItem != null)
                {
                    AddRemoveScript(resultItem, scriptType, operationIndex);
                }
            }
        }

        public void AddRemoveScriptSelectedResults(Type scriptType, int operationIndex)
        {
            foreach (var item in GetSelection())
            {
                var resultItem = (UIComponentCheckResultViewItem)FindItem(item, rootItem);
                if (resultItem != null)
                {
                    AddRemoveScript(resultItem, scriptType, operationIndex);
                }
            }
        }

        private void AddRemoveScript(UIComponentCheckResultViewItem resultData, Type scriptType, int operationIndex)
        {
            var prefabRoot = resultData.prefabGo;
            var transforms = prefabRoot.GetComponentsInChildren<Transform>(true);
            foreach (var nodeId in resultData.nodeFileIds)
            {
                var component = FindComponentByLocalId(prefabRoot, scriptType, nodeId);
                if ((modificationScriptOperations)operationIndex == modificationScriptOperations.Add)
                {
                    
                    foreach (var trans in transforms)
                    {
                        if (component == null && UIComponentCheckResultTableView.GetLocalIdentfierInFile(trans.gameObject) == nodeId)
                        {
                            var newComponent = trans.gameObject.AddComponent(scriptType);
                            if (newComponent != null)
                            {
                                Debug.Log($"Added component {scriptType.Name} to {prefabRoot.name}");
                            }
                        }
                        
                    }
                }
                else if ((modificationScriptOperations)operationIndex == modificationScriptOperations.Delete)
                {
                    if (component != null)
                    {
                        UnityEngine.Object.DestroyImmediate(component, true);
                        Debug.Log($"Removed component {scriptType.Name} from {prefabRoot.name}");
                    }
                }
            }

            PrefabUtility.SavePrefabAsset(prefabRoot);
        }
    

    }
}
#endif