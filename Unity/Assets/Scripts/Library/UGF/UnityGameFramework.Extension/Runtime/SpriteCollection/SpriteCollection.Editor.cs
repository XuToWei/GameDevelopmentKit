#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;


namespace UnityGameFramework.Extension
{
    public partial class SpriteCollection
    {
        private void Awake()
        {
            if (EditorApplication.isUpdating ||
                EditorApplication.isCompiling ||
                !EditorApplication.isPlaying ||
                string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                return;
            }
            Pack();
        }

        [InfoBox("Can drag to 'Objects'")]
        [ShowInInspector, AssetsOnly, NonSerialized]
        [OnValueChanged(nameof(OnListChange), includeChildren: true)]
        [ListDrawerSettings(DraggableItems = false, IsReadOnly = false, HideAddButton = true)]
        [Tooltip("收集Sprite的对象列表（Sprite，Folder，SpriteAtlas）")]
        private List<Object> m_CollectionObjects = new List<Object>();

        [HideInInspector, SerializeField]
        private List<string> m_CollectionGUIDs = new List<string>();

        [NonSerialized]
        private readonly Dictionary<string, Sprite> m_SpriteDictTemp = new Dictionary<string, Sprite>();

        /// <summary>
        /// 收集Sprite的对象列表（Sprite，Folder，SpriteAtlas）
        /// </summary>
        public List<Object> Objects => m_CollectionObjects;

        private void OnValidate()
        {
            bool isDirty = RefreshCollectionObjects();
            if (isDirty)
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        private bool RefreshCollectionObjects()
        {
            bool isDirty = false;
            m_CollectionObjects.Clear();
            for (int i = m_CollectionGUIDs.Count - 1; i >= 0; i--)
            {
                string guid = m_CollectionGUIDs[i];
                if (string.IsNullOrEmpty(guid))
                {
                    isDirty = true;
                    m_CollectionGUIDs.RemoveAt(i);
                    continue;
                }
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(assetPath))
                {
                    isDirty = true;
                    m_CollectionGUIDs.RemoveAt(i);
                    continue;
                }
                Object collectionObject = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                if (collectionObject == null)
                {
                    isDirty = true;
                    m_CollectionGUIDs.RemoveAt(i);
                    continue;
                }
                bool found = false;
                for (int j = i + 1; j < m_CollectionGUIDs.Count; j++)
                {
                    if(guid == m_CollectionGUIDs[j])
                    {
                        found = true;
                        isDirty = true;
                        m_CollectionGUIDs.RemoveAt(i);
                        break;
                    }
                }
                if (found)
                {
                    continue;
                }
                m_CollectionObjects.Add(collectionObject);
            }
            m_CollectionObjects.Reverse();
            return isDirty;
        }

        private void OnListChange()
        {
            bool isDirty = false;
            m_CollectionGUIDs.Clear();
            for (int i = m_CollectionObjects.Count - 1; i >= 0; i--)
            {
                Object collectionObject = m_CollectionObjects[i];
                if (!ObjectFilter(collectionObject))
                {
                    isDirty = true;
                    m_CollectionObjects.RemoveAt(i);
                    continue;
                }
                string assetPath = AssetDatabase.GetAssetPath(collectionObject);
                if (string.IsNullOrEmpty(assetPath))
                {
                    isDirty = true;
                    m_CollectionObjects.RemoveAt(i);
                    continue;
                }
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                if (string.IsNullOrEmpty(guid))
                {
                    isDirty = true;
                    m_CollectionObjects.RemoveAt(i);
                    continue;
                }
                bool found = false;
                for (int j = i + 1; j < m_CollectionObjects.Count; j++)
                {
                    if (collectionObject == m_CollectionObjects[j])
                    {
                        found = true;
                        isDirty = true;
                        m_CollectionObjects.RemoveAt(i);
                        break;
                    }
                }
                if (found)
                {
                    continue;
                }
                m_CollectionGUIDs.Add(guid);
            }
            m_CollectionGUIDs.Reverse();
            if(isDirty)
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
            Pack();
        }

        [Button("Pack Preview", Expanded = false, ButtonHeight = 18)]
        [HorizontalGroup("Pack Preview", width: 150)]
        [PropertySpace(16, 16)]
        public void Pack()
        {
            bool isDirty = RefreshCollectionObjects();

            m_SpriteDictTemp.Clear();
            foreach (Object collectionObject in m_CollectionObjects)
            {
                HandlePackable(collectionObject, m_SpriteDictTemp);
            }

            if (m_Sprites.Count != m_SpriteDictTemp.Count)
            {
                isDirty = true;
                SetSprites(m_SpriteDictTemp);
            }
            else
            {
                foreach (KeyValuePair<string, Sprite> item in m_Sprites)
                {
                    if (!m_SpriteDictTemp.TryGetValue(item.Key, out Sprite sprite) || sprite != item.Value)
                    {
                        isDirty = true;
                        SetSprites(m_SpriteDictTemp);
                        break;
                    }
                }
            }

            m_SpriteDictTemp.Clear();
            if (isDirty)
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        [SerializeField]
        [FolderPath]
        [FoldoutGroup("Create Atlas", true)]
        [PropertyOrder(1)]
        [OnValueChanged(nameof(AtlasFolderChanged))]
        private string m_AtlasFolder = "Assets/Res/UI/UIAtlas";

        void AtlasFolderChanged()
        {
            if (!string.IsNullOrEmpty(m_AtlasFolder))
            {
                int index = m_AtlasFolder.IndexOf("Assets/", StringComparison.Ordinal);
                if (index == -1)
                {
                    m_AtlasFolder = "Assets/Res/UI/UIAtlas";
                    EditorUtility.DisplayDialog("提示", "图集生成文件夹必须在Assets目录下", "确定");
                    return;
                }

                m_AtlasFolder = m_AtlasFolder.Substring(index);
            }
        }

        [Button("Create Atlas")]
        [FoldoutGroup("Create Atlas")]
        [PropertyOrder(2)]
        void CreateAtlas()
        {
            if (string.IsNullOrEmpty(m_AtlasFolder))
            {
                EditorUtility.DisplayDialog("提示", "请先选择图集生成文件夹！", "确定");
                return;
            }

            if (m_CollectionGUIDs.Find(guid => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid)) is SpriteAtlas) != null)
            {
                EditorUtility.DisplayDialog("提示", "SpriteCollection 中存在Atlas 请检查!", "确定");
                return;
            }

            //创建图集
            string atlas = Utility.Path.GetRegularPath(Path.Combine(m_AtlasFolder, this.name + ".spriteatlas"));

            if (File.Exists(atlas))
            {
                bool result = EditorUtility.DisplayDialog("提示", "存在同名图集,是否覆盖？", "确定", "取消");
                if (!result)
                {
                    return;
                }
            }

            SpriteAtlas sa = new SpriteAtlas();

            SpriteAtlasPackingSettings packSet = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 8,
            };
            sa.SetPackingSettings(packSet);

            SpriteAtlasTextureSettings textureSet = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            sa.SetTextureSettings(textureSet);
            AssetDatabase.CreateAsset(sa, atlas);

            List<Object> collectionObjects = new List<Object>();
            foreach (var guid in m_CollectionGUIDs)
            {
                collectionObjects.Add(AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid)));
            }
            sa.Add(collectionObjects.ToArray());

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void SetSprites(Dictionary<string, Sprite> result)
        {
            m_Sprites.Clear();
            foreach (var (k, v) in result)
            {
                m_Sprites.Add(k, v);
            }
        }

        void HandlePackable(Object obj, Dictionary<string, Sprite> result)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (obj is Sprite sp)
            {
                Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
                if (objects.Length == 2)
                {
                    result[path] = sp;
                }
                else
                {
                    string regularPath = Utility.Path.GetRegularPath(Path.Combine(path, sp.name));
                    result[regularPath] = sp;
                }
            }
            else if (obj is Texture2D)
            {
                Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
                if (objects.Length == 2)
                {
                    result[path] = GetSprites(objects)[0];
                }
                else
                {
                    Sprite[] sprites = GetSprites(objects);
                    for (int j = 0; j < sprites.Length; j++)
                    {
                        string regularPath = Utility.Path.GetRegularPath(Path.Combine(path, sprites[j].name));
                        result[regularPath] = sprites[j];
                    }
                }
            }
            else if (obj is DefaultAsset && ProjectWindowUtil.IsFolder(obj.GetInstanceID()))
            {
                string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(fileName => !fileName.EndsWith(".meta")).Select(Utility.Path.GetRegularPath).ToArray();
                foreach (string file in files)
                {
                    Object[] objects = AssetDatabase.LoadAllAssetsAtPath(file);
                    if (objects.Length == 2)
                    {
                        Sprite[] sprites = GetSprites(objects);
                        if (sprites.Length > 0)
                        {
                            result[file] = sprites[0];
                        }
                    }
                    else
                    {
                        Sprite[] sprites = GetSprites(objects);
                        for (int j = 0; j < sprites.Length; j++)
                        {
                            string regularPath = Utility.Path.GetRegularPath(Path.Combine(file, sprites[j].name));
                            result[regularPath] = sprites[j];
                        }
                    }
                }
            }
            else if (obj is SpriteAtlas spriteAtlas)
            {
                Object[] objs = spriteAtlas.GetPackables();
                for (int i = 0; i < objs.Length; i++)
                {
                    HandlePackable(objs[i], result);
                }
            }
        }

        private bool ObjectFilter(Object o)
        {
            return o != null && (o is Sprite ||
                                 o is Texture2D ||
                                 o is DefaultAsset && ProjectWindowUtil.IsFolder(o.GetInstanceID()) ||
                                 o is SpriteAtlas);
        }

        private Sprite[] GetSprites(Object[] objects)
        {
            return objects.OfType<Sprite>().ToArray();
        }

        [MenuItem("Assets/Create/UGF/SpriteCollection By Selection #%F11")]
        static void CreateSpriteCollection()
        {
            Object[] targets = Selection.objects;
            if (targets == null)
            {
                Debug.LogWarning("SpriteCollection必须选中Sprite，Texture2D，Folder或SpriteAtlas来创建");
                return;
            }
            for (int i = 0; i < targets.Length; i++)
            {
                Object target = targets[i];
                if (target == null || !(target is Sprite ||
                                        target is Texture2D ||
                                        target is DefaultAsset && ProjectWindowUtil.IsFolder(target.GetInstanceID()) ||
                                        target is SpriteAtlas))
                {
                    Debug.LogWarning($"选中的[{AssetDatabase.GetAssetPath(target)}]不是Sprite，Texture2D，Folder或SpriteAtlas", target);
                    continue;
                }
                string assetPath = $"{Utility.Path.GetRegularPath(Path.GetDirectoryName(AssetDatabase.GetAssetPath(target)))}/{target.name}.asset";
                SpriteCollection spriteCollection = AssetDatabase.LoadAssetAtPath<SpriteCollection>(assetPath);
                string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(target));
                if (spriteCollection != null)
                {
                    if (!spriteCollection.m_CollectionGUIDs.Contains(guid))
                    {
                        spriteCollection.m_CollectionGUIDs.Add(guid);
                        spriteCollection.Pack();
                        EditorUtility.SetDirty(spriteCollection);
                        AssetDatabase.SaveAssetIfDirty(spriteCollection);
                        Debug.Log($"更新SpriteCollection:{assetPath}", spriteCollection);
                    }
                }
                else
                {
                    spriteCollection = CreateInstance<SpriteCollection>();
                    spriteCollection.m_CollectionGUIDs.Add(guid);
                    spriteCollection.Pack();
                    AssetDatabase.CreateAsset(spriteCollection, assetPath);
                    Debug.Log($"创建SpriteCollection:{assetPath}", spriteCollection);
                }
            }
        }
    }
}
#endif