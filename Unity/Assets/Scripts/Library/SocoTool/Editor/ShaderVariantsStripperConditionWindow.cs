using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Soco.ShaderVariantsStripper
{
    public class ShaderVariantsStripperConditionWindow : EditorWindow
    {
        private static ShaderVariantsStripperConditionWindow m_window;
        public static ShaderVariantsStripperConditionWindow Window
        {
            get
            {
                if (m_window == null)
                {
                    m_window = EditorWindow.GetWindow<ShaderVariantsStripperConditionWindow>("ShaderVariantsStripperConditionWindow");
                    m_window.minSize = new Vector2(480, 320);
                }
                return m_window;
            }
        }

        public ShaderVariantsStripperCondition mCondition;
        public ShaderVariantsStripperConditionOnGUIContext mContext;
        
        public void OnGUI()
        {
            if (mCondition != null)
            {
                //mContext.window = this;
                mCondition.OnGUI(mContext);
            }
        }
    }

}
