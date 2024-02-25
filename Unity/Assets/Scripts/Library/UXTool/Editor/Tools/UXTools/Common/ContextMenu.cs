#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ThunderFireUITool
{
    static public class ContextMenu
    {
        static List<string> mEntries = new List<string>();
        static GenericMenu mMenu;

        static public void AddItem(string item, bool isChecked, GenericMenu.MenuFunction callback)
        {
            if (callback != null)
            {
                if (mMenu == null) mMenu = new GenericMenu();
                int count = 0;

                for (int i = 0; i < mEntries.Count; ++i)
                {
                    string str = mEntries[i];
                    if (str == item) ++count;
                }
                mEntries.Add(item);

                if (count > 0) item += " [" + count + "]";
                mMenu.AddItem(new GUIContent(item), isChecked, callback);
            }
            else
            {
                AddDisabledItem(item);
            }
        }


        static public void AddItemWithArge(string item, bool isChecked, GenericMenu.MenuFunction2 callback, object arge)
        {
            if (callback != null)
            {
                if (mMenu == null) mMenu = new GenericMenu();
                int count = 0;

                for (int i = 0; i < mEntries.Count; ++i)
                {
                    string str = mEntries[i];
                    if (str == item) ++count;
                }
                mEntries.Add(item);

                if (count > 0) item += " [" + count + "]";
                mMenu.AddItem(new GUIContent(item), isChecked, callback, arge);
            }
            else
            {
                AddDisabledItem(item);
            }
        }

        static public void Show()
        {
            if (mMenu != null)
            {
                mMenu.ShowAsContext();
                mMenu = null;
                mEntries.Clear();
            }
        }

        static public void Show(Vector2 pos)
        {
            if (mMenu != null)
            {
                mMenu.DropDown(new Rect(pos, Vector2.zero));
                mMenu = null;
                mEntries.Clear();
            }
        }


        //增加UI控件菜单
        static public void AddUIMenu()
        {
            //AddItem("添加控件/Empty", false, UIEditorHelper.CreateEmptyObj);
        }

        //增加UI组件菜单
        static public void AddUIComponentMenu()
        {
            // AddItem("添加组件/Image", false, UIEditorHelper.AddImageComponent);     
        }

        static public void AddCommonItems(GameObject[] targets)
        {
            AddItem(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_复制), false, () =>
            {
                Unsupported.CopyGameObjectsToPasteboard();
            });
            AddItem(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_粘贴), false, () =>
            {
                Unsupported.PasteGameObjectsFromPasteboard();
            });

            var prefabStage = PrefabStageUtils.GetCurrentPrefabStage();
            if (prefabStage != null && Selection.Contains(prefabStage.prefabContentsRoot))
            {
                AddItem(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_删除), false, null);
            }
            else
            {
                AddItem(EditorLocalization.GetLocalization(EditorLocalizationStorage.Def_删除), false, () =>
                {
                    Unsupported.DeleteGameObjectSelection();
                });
            }
        }

        static public void AddSeparator(string path)
        {
            if (mMenu == null) mMenu = new GenericMenu();

            if (Application.platform != RuntimePlatform.OSXEditor)
                mMenu.AddSeparator(path);
        }

        static public void AddDisabledItem(string item)
        {
            if (mMenu == null) mMenu = new GenericMenu();
            mMenu.AddDisabledItem(new GUIContent(item));
        }

        static public void GetAllObjects()
        {
            Vector3 mousePos = Input.mousePosition;

        }
        static public bool IsEmpty()
        {
            if (mMenu == null) return true;
            return false;
        }
    }
}
#endif