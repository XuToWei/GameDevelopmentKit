using UnityEditor;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    /// <summary>
    /// 页码工具类
    /// </summary>
    public class Page
    {
        private int m_CurrentPage = 0;
        private int m_ShowCount = 0;

        public int CurrentPage => m_CurrentPage;

        public int AllPage => m_AllPage;

        public int ShowCount => m_ShowCount;

        private int m_AllPage = 0;

        private string m_PageField;

        public Page(int showCount,int allCount)
        {
            m_ShowCount = showCount;
            SetAllCount(allCount);
        }

        public void Draw()
        {
            EditorGUILayout.BeginHorizontal();
            if (m_CurrentPage <= 0)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("上一页"))
            {
                m_CurrentPage--;
            }

            GUI.enabled = true;

            m_PageField = m_CurrentPage.ToString();
            m_PageField = EditorGUILayout.TextField(m_PageField, new GUIStyle("TextField")
            {
                alignment = TextAnchor.MiddleCenter
            }, GUILayout.MaxWidth(50));

            if (int.TryParse(m_PageField, out int page) && page < m_AllPage - 1)
            {
                m_CurrentPage = page;
            }
            else
            {
                m_PageField = m_CurrentPage.ToString();
            }

            if (m_CurrentPage >= (m_AllPage - 1))
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("下一页"))
            {
                m_CurrentPage++;
            }

            GUI.enabled = true;


            EditorGUILayout.EndHorizontal();
        }
        public void SetAllCount(int count)
        {
            m_AllPage = count / m_ShowCount;
            if (count % m_ShowCount != 0)
            {
                m_AllPage += 1;
            }

            if (m_CurrentPage >= m_AllPage - 1)
            {
                m_CurrentPage = m_AllPage - 1 > 0? m_AllPage - 1 :0;
            }
            m_PageField = m_CurrentPage.ToString();
        }
    }
}