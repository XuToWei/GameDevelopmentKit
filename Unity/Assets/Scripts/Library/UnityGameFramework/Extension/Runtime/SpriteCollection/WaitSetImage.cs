using System;
using GameFramework;
using UnityEditor;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameFramework.Extension
{
    [Serializable]
    public class WaitSetImage : ISetSpriteObject
    {
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        private Image m_Image;

        public static WaitSetImage Create(Image obj, string collection, string spriteName)
        {
            WaitSetImage waitSetImage = ReferencePool.Acquire<WaitSetImage>();
            waitSetImage.m_Image = obj;
            waitSetImage.SpritePath = spriteName;
            waitSetImage.CollectionPath = collection;
            int index1 = waitSetImage.SpritePath.LastIndexOf("/", StringComparison.Ordinal);
            int index2 = waitSetImage.SpritePath.LastIndexOf(".", StringComparison.Ordinal);
            waitSetImage.SpriteName = index2 < index1
                ? waitSetImage.SpritePath.Substring(index1 + 1)
                : waitSetImage.SpritePath.Substring(index1 + 1, index2 - index1 - 1);
            return waitSetImage;
        }
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public string SpritePath { get; private set; }
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public string CollectionPath { get; private set; }
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        private string SpriteName { get; set; }

        public void SetSprite(Sprite sprite)
        {
            if (m_Image != null)
            {
                m_Image.sprite = sprite;
            }
        }

        public bool IsCanRelease()
        {
            return m_Image == null || m_Image.sprite == null || m_Image.sprite.name != SpriteName;
        }
#if !ODIN_INSPECTOR && UNITY_EDITOR
        public Rect DrawSetSpriteObject(Rect rect)
        {
            EditorGUI.ObjectField(rect, "Image", m_Image, typeof(Image), true);
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.TextField(rect, "CollectionPath", CollectionPath);
            rect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.TextField(rect, "SpritePath", SpritePath);
            rect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.TextField(rect, "CollectionPath", CollectionPath);
            rect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.TextField(rect, "SpriteName", SpriteName);
            rect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.Toggle(rect, "IsCanRelease", IsCanRelease());
            return rect;
        }
#endif
        public void Clear()
        {
            m_Image = null;
            SpritePath = null;
            CollectionPath = null;
            SpriteName = null;
        }
    }
}