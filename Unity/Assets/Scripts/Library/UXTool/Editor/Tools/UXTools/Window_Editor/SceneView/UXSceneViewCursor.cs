#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ThunderFireUITool
{
    //统一控制UXTools中鼠标样式

    public enum UXCursorType
    {
        None,
        Crosshair,
        Updown,
        Leftright
    }

    public class UXSceneViewCursor : UXSingleton<UXSceneViewCursor>
    {
        Dictionary<UXCursorType, Texture2D> CursorTexturesDic;
        Texture2D CurCursorTexture;

        public override void Init()
        {
            UXCustomSceneView.AddDelegate(OnSceneGUI);
            InitCursorTexture();
        }

        private void InitCursorTexture()
        {
            //Todo load from assets
            CursorTexturesDic = new Dictionary<UXCursorType, Texture2D>();
            CursorTexturesDic.Add(UXCursorType.Crosshair, ToolUtils.GetIcon("PointerCrosshair") as Texture2D);
            CursorTexturesDic.Add(UXCursorType.Updown, ToolUtils.GetIcon("SplitResizeUpDown") as Texture2D);
            CursorTexturesDic.Add(UXCursorType.Leftright, ToolUtils.GetIcon("SplitResizeLeftRight") as Texture2D);
        }

        private Texture2D GetCursorTexture(UXCursorType cursorType)
        {
            if (CursorTexturesDic != null && CursorTexturesDic.ContainsKey(cursorType))
            {
                return CursorTexturesDic[cursorType];
            }
            else
            {
                return null;
            }
        }

        public void SetCursor(UXCursorType cursorType)
        {
            if (cursorType == UXCursorType.None)
            {
                CurCursorTexture = null;
                //Cursor.SetCursor(null, new Vector2(16, 16), CursorMode.Auto);
                Utils.ClearCurrentViewCursor();
            }
            else
            {
                CurCursorTexture = GetCursorTexture(cursorType);
            }

        }
        private void OnSceneGUI(SceneView sceneView)
        {
            if (CurCursorTexture != null)
            {
                Utils.SetCursor(CurCursorTexture);
                //Cursor.SetCursor(CurCursorTexture, new Vector2(16, 16), CursorMode.Auto);
                HandleUtility.AddDefaultControl(0);
            }
        }
    }
}
#endif


