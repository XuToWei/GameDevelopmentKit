using System;
using GameFramework;

#if UNITY_EDITOR
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
    public class SetRawImage : ISetTexture2dObject
    {
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        private RawImage m_RawImage;
#if ODIN_INSPECTOR
        [ShowInInspector] 
#endif
        private Texture2D Texture2D { get; set; }
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public string Texture2dFilePath { get; private set; }

        public void SetTexture(Texture2D texture)
        {
            m_RawImage.texture = texture;
            Texture2D = texture;
        }

        public bool IsCanRelease()
        {
            return m_RawImage == null || m_RawImage.texture == null || m_RawImage.texture != Texture2D;
        }

        public static SetRawImage Create(RawImage rawImage, string filePath)
        {
            SetRawImage item = ReferencePool.Acquire<SetRawImage>();
            item.m_RawImage = rawImage;
            item.Texture2dFilePath = filePath;
            return item;
        }

        public void Clear()
        {
            m_RawImage = null;
            Texture2D = null;
            Texture2dFilePath = null;
        }
        
#if !ODIN_INSPECTOR && UNITY_EDITOR
        public Rect DrawSetTextureObject(Rect rect)
        {
            EditorGUI.ObjectField(rect, "RawImage", m_RawImage, typeof(RawImage), true);
            rect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.TextField(rect, "Texture2dFilePath", Texture2dFilePath);
            rect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.ObjectField(rect, "Texture", Texture2D, typeof(Texture2D), false);
            rect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.Toggle(rect, "IsCanRelease", IsCanRelease());
            return rect;
        }
#endif
    }
}