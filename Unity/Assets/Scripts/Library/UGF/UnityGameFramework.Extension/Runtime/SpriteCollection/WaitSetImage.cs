using System;
using GameFramework;
#if !ODIN_INSPECTOR && UNITY_EDITOR
using UnityEditor;
#endif

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
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
        public Sprite CurSprite { get; private set; }

        public void SetSprite(Sprite sprite)
        {
            if (m_Image != null)
            {
                m_Image.sprite = sprite;
                CurSprite = sprite;
            }
        }

        public bool IsCanRelease()
        {
            return m_Image == null || m_Image.sprite != CurSprite && CurSprite != null;
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
            CurSprite = null;
        }
    }
}