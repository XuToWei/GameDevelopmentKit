using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFramework;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.U2D;
#endif

namespace UnityGameFramework.Extension
{
    [CreateAssetMenu(fileName = "SpriteCollection", menuName = "UGF/SpriteCollection")]
    public class SpriteCollection : SerializedScriptableObject
    {
        [OdinSerialize, Searchable] [DictionaryDrawerSettings(KeyLabel = "Path", ValueLabel = "Sprite", IsReadOnly = true)]
        private Dictionary<string, Sprite> m_Sprites = new Dictionary<string, Sprite>();

        public Sprite GetSprite(string path)
        {
            m_Sprites.TryGetValue(path, out Sprite sprite);
            return sprite;
        }
#if UNITY_EDITOR
        private void Awake()
        {
            Pack();
        }

        [InfoBox("Can drag to 'Objects'")]
        [OdinSerialize]
        [OnValueChanged("OnListChange", includeChildren: true)]
        [ListDrawerSettings(DraggableItems = false, IsReadOnly = false, HideAddButton = true)]
        [AssetsOnly]
        private List<Object> m_Objects = new List<Object>();
        
        [NonSerialized]
        private readonly Dictionary<string, Sprite> m_SpritesTemp = new Dictionary<string, Sprite>();

        private void OnListChange()
        {
            for (int i = m_Objects.Count - 1; i >= 0; i--)
            {
                if (!ObjectFilter(m_Objects[i]))
                {
                    m_Objects.RemoveAt(i);
                }
            }

            m_Objects = m_Objects.Distinct().ToList();
            Pack();
        }

        [Button("Pack Preview", Expanded = false, ButtonHeight = 18)]
        [HorizontalGroup("Pack Preview", width: 150)]
        [PropertySpace(16, 16)]
        public void Pack()
        {
            bool isDirty = false;

            int count = m_Objects.Count;
            for (int i = m_Objects.Count - 1; i >= 0; i--)
            {
                if (!ObjectFilter(m_Objects[i]))
                {
                    m_Objects.RemoveAt(i);
                }
            }
            m_Objects = m_Objects.Distinct().ToList();
            isDirty |= m_Objects.Count != count;
            m_SpritesTemp.Clear();
            for (int i = 0; i < m_Objects.Count; i++)
            {
                Object obj = m_Objects[i];
                HandlePackable(obj, m_SpritesTemp);
            }

            if (m_Sprites.Count != m_SpritesTemp.Count)
            {
                SetSprites(m_SpritesTemp);
                isDirty = true;
            }
            else
            {
                foreach (KeyValuePair<string, Sprite> item in m_Sprites)
                {
                    if (!m_SpritesTemp.TryGetValue(item.Key, out Sprite sprite) || sprite != item.Value)
                    {
                        SetSprites(m_SpritesTemp);
                        isDirty = true;
                        break;
                    }
                }
            }

            m_SpritesTemp.Clear();
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
        [OnValueChanged("AtlasFolderChanged")]
        private string m_AtlasFolder = "Assets/Res/Atlas";

        void AtlasFolderChanged()
        {
            if (!string.IsNullOrEmpty(m_AtlasFolder))
            {
                int index = m_AtlasFolder.IndexOf("Assets/", StringComparison.Ordinal);
                if (index == -1)
                {
                    m_AtlasFolder = "Assets/Res/Atlas";
                    EditorUtility.DisplayDialog("提示", $"图集生成文件夹必须在Assets目录下", "确定");
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
                EditorUtility.DisplayDialog("提示", $"请先选择图集生成文件夹！", "确定");
                return;
            }

            if (m_Objects.Find(_ => _ is SpriteAtlas) != null)
            {
                EditorUtility.DisplayDialog("提示", $"SpriteCollection 中存在Atlas 请检查!", "确定");
                return;
            }

            //创建图集
            string atlas = Utility.Path.GetRegularPath(Path.Combine(m_AtlasFolder, this.name + ".spriteatlas"));

            if (File.Exists(atlas))
            {
                bool result = EditorUtility.DisplayDialog("提示", $"存在同名图集,是否覆盖？", "确定", "取消");
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

            sa.Add(m_Objects.ToArray());

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
                    .Where(_ => !_.EndsWith(".meta")).Select(Utility.Path.GetRegularPath).ToArray();
                foreach (string file in files)
                {
                    Object[] objects = AssetDatabase.LoadAllAssetsAtPath(file);
                    if (objects.Length == 2)
                    {
                        result[file] = GetSprites(objects)[0];
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
#endif
    }
}