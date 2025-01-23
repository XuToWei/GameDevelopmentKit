using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FolderTag
{
    public class FolderSettings : SettingsProvider
    {
        #region ============================== Definition ==============================

        private const string mPrefsFile = nameof(FolderTag) + "_Prefs.json";
        private const string mPrefsPath = "ProjectSettings\\" + mPrefsFile;
        private const int mGradientWidth = 16;

        public static EditorOption<bool> Opt_ShowGradient = new EditorOption<bool>(nameof(FolderTag) + "_ShowGradient", true);
        public static EditorOption<bool> Opt_InspectorEdit = new EditorOption<bool>(nameof(FolderTag) + "_InspectorEdit", true);
        public static EditorOption<Color> Opt_SubFoldersTint = new EditorOption<Color>(nameof(FolderTag) + "_SubFoldersTint", new Color(0.7f, 0.7f, 0.7f, 0.7f));
        private static EditorOption<Vector2> Opt_GradientScale = new EditorOption<Vector2>(nameof(FolderTag) + "_GradientScale", new Vector2(0.7f, 1f));
        private static Texture2D texGradient;

        public static Texture Gradient
        {
            get
            {
                if (texGradient == null)
                    UpdateGradient();

                return texGradient;
            }
        }


        public static Color FoldersDescColor { get; private set; }
        private static Color mFoldersDescTint = Color.white;

        private static Dictionary<string, FolderData> dicFoldersData;
        private static List<FolderData> listFoldersData;
        private static ReorderableList foldersList;

        #endregion

        #region ============================== Serializable ==============================

        [Serializable]
        private class JsonWrapper
        {
            public Color LabelColor;
            public DictionaryData<string, FolderData> FoldersData;

            [Serializable]
            public class DictionaryData<TKey, TValue>
            {
                public List<TKey> Keys;
                public List<TValue> Values;

                public IEnumerable<KeyValuePair<TKey, TValue>> Enumerate()
                {
                    if (Keys == null || Values == null)
                        yield break;

                    for (var n = 0; n < Keys.Count; n++)
                        yield return new KeyValuePair<TKey, TValue>(Keys[n], Values[n]);
                }

                public DictionaryData()
                    : this(new List<TKey>(), new List<TValue>())
                {
                }

                public DictionaryData(List<TKey> keys, List<TValue> values)
                {
                    Keys = keys;
                    Values = values;
                }

                public DictionaryData(IEnumerable<KeyValuePair<TKey, TValue>> data)
                {
                    var pairs = data as KeyValuePair<TKey, TValue>[] ?? data.ToArray();
                    Keys = pairs.Select(n => n.Key).ToList();
                    Values = pairs.Select(n => n.Value).ToList();
                }
            }
        }

        [Serializable]
        public class FolderData
        {
            public string _guid;
            public Color _color;
            public bool _recursive;
            public string _tag = "folder tag";
            public string _desc = "";
        }

        #endregion

        public FolderSettings(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new FolderSettings("Preferences/Folder Tag", SettingsScope.User);
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            if (File.Exists(mPrefsPath))
            {
                using var file = File.OpenText(mPrefsPath);
                try
                {
                    var data = JsonUtility.FromJson<JsonWrapper>(file.ReadToEnd());

                    listFoldersData = data.FoldersData
                        .Enumerate()
                        .Select(n => n.Value)
                        .ToList();

                    FoldersDescColor = data.LabelColor;
                }
                catch
                {
                    listFoldersData = new List<FolderData>();
                    FoldersDescColor = mFoldersDescTint;
                }
            }
            else
            {
                listFoldersData = new List<FolderData>();
                FoldersDescColor = mFoldersDescTint;
            }

            dicFoldersData = listFoldersData.ToDictionary(n => n._guid, n => n);

            UpdateGradient();
            EditorApplication.projectWindowItemOnGUI += FoldersBrowser.DrawFolderMemos;
        }

        public override void OnGUI(string searchContext)
        {
            // editor prefs variables
            EditorGUI.BeginChangeCheck();

            var showGradient = EditorGUILayout.Toggle("Show Gradient", Opt_ShowGradient.Value);

            var gradientScale = Opt_GradientScale.Value;
            EditorGUILayout.MinMaxSlider("Gradient Scale", ref gradientScale.x, ref gradientScale.y, 0f, 1f);

            var subFoldersTint = EditorGUILayout.ColorField("Sub Folders Tint", Opt_SubFoldersTint.Value);

            if (EditorGUI.EndChangeCheck())
            {
                Opt_ShowGradient.Value = showGradient;
                Opt_GradientScale.Value = gradientScale;
                Opt_SubFoldersTint.Value = subFoldersTint;

                EditorApplication.RepaintProjectWindow();
                UpdateGradient();
            }

            // project prefs variables
            EditorGUI.BeginChangeCheck();

            FoldersDescColor = EditorGUILayout.ColorField("Desc Color", FoldersDescColor);

            if (EditorGUI.EndChangeCheck())
            {
                EditorApplication.RepaintProjectWindow();
                SaveProjectPrefs();
            }

            GetFoldersList().DoLayoutList();
        }

        private static void UpdateGradient()
        {
            texGradient = new Texture2D(mGradientWidth, 1);
            texGradient.wrapMode = TextureWrapMode.Clamp;
            var range = Opt_GradientScale.Value;

            if (range == new Vector2(0, 1))
            {
                for (var x = 0; x < mGradientWidth; x++)
                    texGradient.SetPixel(x, 0, new Color(1, 1, 1, 1));
            }
            else
            {
                for (var x = 0; x < mGradientWidth; x++)
                    texGradient.SetPixel(x, 0, new Color(1, 1, 1, _getAlpha(x)));

                float _getAlpha(int xPixel)
                {
                    var xScale = xPixel / (mGradientWidth - 1f);

                    if (xScale >= range.x && xScale <= range.y)
                        return 1f;

                    var distance = xScale < range.x ? range.x - xScale : xScale - range.y;
                    return Mathf.Clamp01(1f - distance * 3f);
                }
            }

            texGradient.Apply();
        }

        public static void AddFoldersList(FolderData folderData)
        {
            listFoldersData.Add(folderData);
            GetFoldersList(true);
        }

        public static ReorderableList GetFoldersList(bool forceNew = false)
        {
            if (foldersList != null && !forceNew)
                return foldersList;

            float lineHeight = EditorGUIUtility.singleLineHeight;

            foldersList = new ReorderableList(listFoldersData, typeof(FolderData), true, true, true, true);
            foldersList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = listFoldersData[index];

                var refRect = new Rect(rect.position + new Vector2(0f, 1f),
                    new Vector2(rect.size.x * .5f - EditorGUIUtility.standardVerticalSpacing, lineHeight));
                var colorRect = new Rect(rect.position + new Vector2(rect.size.x * .5f, 1f),
                    new Vector2(rect.size.x * .5f - 18f - EditorGUIUtility.standardVerticalSpacing, lineHeight));
                var recRect = new Rect(rect.position + new Vector2(rect.size.x - 18f, 1f), new Vector2(18f, lineHeight));
                var tagRect = new Rect(rect.position + new Vector2(0f, lineHeight + 2f), new Vector2(rect.size.x, lineHeight));

                EditorGUI.BeginChangeCheck();
                var folder = EditorGUI.ObjectField(refRect,
                    GUIContent.none,
                    AssetDatabase.LoadAssetAtPath<DefaultAsset>(AssetDatabase.GUIDToAssetPath(element._guid)),
                    typeof(DefaultAsset),
                    false);

                element._color = EditorGUI.ColorField(colorRect, GUIContent.none, element._color);
                element._recursive = EditorGUI.Toggle(recRect, GUIContent.none, element._recursive);
                var strTag = EditorGUI.TextField(tagRect, GUIContent.none, element._tag);

                // Limit tag length to 50 characters
                if (strTag.Length > 50)
                {
                    element._tag = strTag.Substring(0, 50);
                }
                else
                {
                    element._tag = strTag;
                }

                if (EditorGUI.EndChangeCheck())
                {
                    var fodlerGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(folder));

                    if (element._guid != fodlerGuid)
                    {
                        // ignore non directory files
                        if (folder != null && !File.GetAttributes(AssetDatabase.GetAssetPath(folder)).HasFlag(FileAttributes.Directory))
                            folder = null;

                        // ignore if already contains
                        if (folder != null && listFoldersData.Any(n => n._guid == fodlerGuid))
                            folder = null;
                    }

                    element._guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(folder));
                    SaveProjectPrefs();

                    EditorApplication.RepaintProjectWindow();
                }
            };
            foldersList.elementHeight = lineHeight * 2 + 3;
            foldersList.onRemoveCallback = list =>
            {
                listFoldersData.RemoveAt(list.index);
                SaveProjectPrefs();
            };
            foldersList.onAddCallback = list => { listFoldersData.Add(CreateFolderData()); };
            foldersList.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, new GUIContent("Folders", "")); };

            return foldersList;
        }

        public static FolderData CreateFolderData()
        {
            var color = Color.HSVToRGB(Random.value, 0.7f, 0.7f);
            color.a = 0.7f;
            return new FolderData() { _color = color };
        }

        public static FolderData GetFolderData(string guid, string path, out bool subFolder)
        {
            subFolder = false;
            if (dicFoldersData.TryGetValue(guid, out var folderData))
                return folderData;

            subFolder = true;
            var searchPath = path;
            while (folderData == null)
            {
                searchPath = Path.GetDirectoryName(searchPath);
                if (string.IsNullOrEmpty(searchPath))
                    return null;

                var searchGuid = AssetDatabase.GUIDFromAssetPath(searchPath).ToString();

                dicFoldersData.TryGetValue(searchGuid, out folderData);
                if (folderData != null && !folderData._recursive)
                    return null;
            }

            return folderData;
        }

        public static void SaveProjectPrefs()
        {
            dicFoldersData = listFoldersData
                .Where(n => n != null && string.IsNullOrEmpty(n._guid) == false && n._guid != Guid.Empty.ToString())
                .ToDictionary(n => n._guid, n => n);

            var json = new JsonWrapper()
            {
                LabelColor = FoldersDescColor,
                FoldersData = new JsonWrapper.DictionaryData<string, FolderData>(dicFoldersData
                    .Values
                    .Select(n => new KeyValuePair<string, FolderData>(n._guid, n)))
            };

            File.WriteAllText(mPrefsPath, JsonUtility.ToJson(json));
        }
    }
}