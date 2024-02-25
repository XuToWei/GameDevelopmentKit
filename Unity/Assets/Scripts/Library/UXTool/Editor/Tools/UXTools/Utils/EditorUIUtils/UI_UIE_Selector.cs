using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class SelectorItem
    {
        private bool m_selected;
        private VisualElement container;
        private VisualElement body;
        public SelectorItem(VisualElement container, VisualElement body, bool occupied = true)
        {
            this.container = container;
            this.body = body;
            UnSelected();
            body.RegisterCallback((MouseEnterEvent e) =>
            {
                if (!m_selected || !occupied)
                {
                    GoSelected();
                }
                
            });

            body.RegisterCallback((MouseLeaveEvent e) =>
            {
                if (!m_selected || !occupied)
                {
                    UnSelected();
                }
                
            });

            body.RegisterCallback((MouseDownEvent e) =>
            {
                if (e.button == 0)
                {
                    m_selected = true;
                    GoSelected();
                }
            });
        }

        public void GoSelected()
        {
            container.style.borderLeftColor = new Color(0.11f, 0.59f, 0.91f, 1);
            container.style.borderRightColor = new Color(0.11f, 0.59f, 0.91f, 1);
            container.style.borderTopColor = new Color(0.11f, 0.59f, 0.91f, 1);
            container.style.borderBottomColor = new Color(0.11f, 0.59f, 0.91f, 1);
        }

        public void UnSelected()
        {
            container.style.borderLeftColor = new Color(0.22f, 0.22f, 0.22f, 1);
            container.style.borderRightColor = new Color(0.22f, 0.22f, 0.22f, 1);
            container.style.borderTopColor = new Color(0.22f, 0.22f, 0.22f, 1);
            container.style.borderBottomColor = new Color(0.22f, 0.22f, 0.22f, 1);
            m_selected = false;

        }

    }
}