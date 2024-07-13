#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    /// <summary>
    /// 组件库界面元素
    /// </summary>
    public class AssetItemPrefabRepository : AssetItemPrefab
    {
        public readonly string[] Labels;

        private readonly Action _refreshWindow;

        public AssetItemPrefabRepository(FileInfo fileInfo, bool isPrefabRecent, Action refresh = null, float scale = 1) : base(fileInfo, scale)
        {
            Labels = AssetDatabase.GetLabels(AssetObj);
            ResourceType = ResourceType.Prefab;

            Row.RegisterCallback<MouseDownEvent, bool>(OnClick, isPrefabRecent);

            _refreshWindow = refresh;
        }

        private void OnClick(MouseDownEvent e, bool y)
        {
            switch (e.button)
            {
                case 0:
                    {
                        if (y) PrefabRecentWindow.clickFlag = true;
                        else WidgetRepositoryWindow.clickFlag = true;

                        if (e.clickCount == 2)
                        {
                            Utils.OpenPrefab(FilePath);
                        }

                        break;
                    }
                case 1:
                    {
                        //Right Mouse Button
                        var menu = new GenericMenu();

                        if (y)
                        {
                            menu.AddItem(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_删除)),
                                false, OpenDeleteRecent);
                        }
                        else
                        {
                            menu.AddItem(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_修改信息)), false, () => { PrefabModifyWindow.OpenWindow((GameObject)AssetObj, _refreshWindow); });
                            menu.AddItem(new GUIContent(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_删除)),
                                false, OpenDeleteSettled);
                        }

                        menu.ShowAsContext();
                        break;
                    }
            }
        }

        public void OpenDeleteRecent()
        {
            var message = new DeleteWindowStruct
            {
                MessageTitle = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_删除),
                MessageDelete = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确认从列表中删除),
                MessageDeleteLocal = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_同时删除本地文件)
            };

            DeleteWindow.OpenWindow(message,
                () =>
                {
                    var recentOpened = JsonAssetManager.GetAssets<PrefabOpenedSetting>();
                    string guid = AssetDatabase.AssetPathToGUID(FilePath);
                    var recentList = recentOpened.List;
                    if (recentList.Contains(guid))
                    {
                        recentOpened.Remove(guid);
                        //PrefabRecentWindow.GetInstance().RefreshWindow();
                        Debug.Log("Delete Successfully");
                    }
                },
                () =>
                {
                    AssetDatabase.DeleteAsset(FilePath);
                    AssetDatabase.Refresh();
                    Debug.Log("Delete Successfully");
                });
        }

        private void OpenDeleteSettled()
        {
            var message = new DeleteWindowStruct
            {
                MessageTitle = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_删除),
                MessageDelete = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_确认从列表中删除),
                MessageDeleteLocal = EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_同时删除本地文件)
            };
            DeleteWindow.OpenWindow(message,
                () =>
                {
                    var widgetListSetting = JsonAssetManager.GetAssets<WidgetListSetting>();
                    string guid = AssetDatabase.AssetPathToGUID(FilePath);
                    var widgetList = widgetListSetting.List;
                    if (widgetList.Contains(guid))
                    {
                        widgetListSetting.Remove(guid);
                        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(FilePath);
                        if (asset != null)
                        {
                            AssetDatabase.ClearLabels(asset);
                        }

                        Debug.Log("Delete Successfully");
                    }
                },
                () =>
                {
                    AssetDatabase.DeleteAsset(FilePath);
                    AssetDatabase.Refresh();
                    Debug.Log("Delete Successfully");
                });
        }
    }
}
#endif