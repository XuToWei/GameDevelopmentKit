using UnityEditor;
using UnityEngine;

namespace FolderTag
{
    public static class FoldersBrowser
    {
        private static GUIStyle s_labelNormal;
        private static GUIStyle s_labelSelected;

        public static void DrawFolderMemos(string guid, Rect rect)
        {
            //var t = Stopwatch.StartNew();
            DrawFolderTag(guid, rect);
            //t.Stop();

            //if(t.ElapsedMilliseconds > 0)
            //    UnityEngine.Debug.Log($"DrawFolders: {t.ElapsedMilliseconds} ms");
        }

        private static void DrawFolderTag(string guid, Rect rect)
        {
            if (rect.width < rect.height)
                return;

            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (!FolderHelper.IsValidFolder(path) && !FolderHelper.IsValidScene(path))
                return;

            if (!FolderSettings.Opt_EnableSceneTag.Value && FolderHelper.IsValidScene(path))
                return;

            var data = FolderSettings.GetFolderData(guid, path, out var isSubFolder);
            if (data == null)
                return;

            bool curIsTreeView = (rect.x - 16) % 14 == 0;
            if (!curIsTreeView)
                rect.xMin += 3;

            Color tagColor = FolderSettings.FoldersDescColor;
            if (FolderSettings.Opt_ShowGradient.Value)
            {
                // draw background
                GUI.color = isSubFolder ? data._color * FolderSettings.Opt_SubFoldersTint.Value : data._color;
                GUI.DrawTexture(rect, FolderSettings.Gradient, ScaleMode.ScaleAndCrop);
            }
            else if (FolderSettings.FoldersDescColor == Color.white)
            {
                // use gradient color
                tagColor = EditorGUIUtility.isProSkin ? data._color * 1.5f : data._color;
            }

            // draw tag
            if (!string.IsNullOrEmpty(data._tag))
            {
                GUI.color = tagColor;
                GUIStyle style = GUI.skin.label;
                Vector2 englishSize = style.CalcSize(new GUIContent(data._tag));

                int x = rect.xMax - englishSize.x > 0 ? (int)(rect.xMax - englishSize.x) : 0;
                GUI.Label(new Rect(x, rect.y - 1, englishSize.x, englishSize.y), data._tag, _labelSkin());
            }

            GUI.color = Color.white;

            GUIStyle _labelSkin()
            {
                if (s_labelSelected == null || s_labelSelected.normal.textColor != FolderSettings.FoldersDescColor)
                    SetLabelTint();

                return FolderHelper.IsSelected(guid) ? s_labelSelected : s_labelNormal;
            }
        }

        private static void SetLabelTint()
        {
            s_labelSelected = new GUIStyle("Label");
            s_labelSelected.normal.textColor = FolderSettings.FoldersDescColor;
            s_labelSelected.hover.textColor = s_labelSelected.normal.textColor;

            s_labelNormal = new GUIStyle("Label");
            s_labelNormal.normal.textColor = new Color32(210, 210, 210, 255) * FolderSettings.FoldersDescColor;
            s_labelNormal.hover.textColor = s_labelNormal.normal.textColor;
        }
    }
}