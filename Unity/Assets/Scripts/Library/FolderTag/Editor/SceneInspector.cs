using UnityEditor;
using UnityEngine;

namespace FolderTag
{
    [CustomEditor(typeof(SceneAsset))]
    public class SceneInspector : Editor
    {
        private static bool showPreview;

        public override void OnInspectorGUI()
        {
            if (Selection.assetGUIDs.Length != 1 || !FolderSettings.Opt_EnableSceneTag.Value)
                return;

            var guid = Selection.assetGUIDs[0];
            var path = AssetDatabase.GUIDToAssetPath(guid);

            var folderData = FolderSettings.GetFolderData(guid, path, out bool subFolder);

            GUI.enabled = true;
            bool create = folderData == null;
            if (create)
            {
                folderData = FolderSettings.CreateFolderData(true);
                folderData._guid = guid;
                folderData._tag = string.Empty;
                folderData._desc = string.Empty;
            }

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Tag");
            var strTag = EditorGUILayout.TextField(folderData._tag, EditorStyles.textField);

            // Limit tag length to 50 characters
            if (strTag.Length > 50)
            {
                folderData._tag = strTag.Substring(0, 50);
            }
            else
            {
                folderData._tag = strTag;
            }

            GUILayout.Space(5);

            EditorGUILayout.LabelField("Description");
            folderData._desc = EditorGUILayout.TextArea(folderData._desc, EditorStyles.textArea, GUILayout.MinHeight(300));

            if (EditorGUI.EndChangeCheck())
            {
                if (create) FolderSettings.AddFoldersList(folderData);

                EditorApplication.RepaintProjectWindow();
                FolderSettings.SaveProjectPrefs();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Clean Empty Data"))
            {
                FolderSettings.CleanEmptyData();
            }

            GUILayout.Space(10);

            string title = showPreview ? " ∧ Hide Tags Preview" : " ∨ Show Tags Preview";
            showPreview = EditorGUILayout.BeginFoldoutHeaderGroup(showPreview, title);
            if (showPreview)
            {
                FolderSettings.GetFoldersList(isScene: true).DoLayoutList();
            }

            GUI.enabled = false;
        }
    }
}