using QFSW.QC.QGUI;
using UnityEditor;
using UnityEngine;

namespace QFSW.QC.Editor
{
    public static class EditorHelpers
    {
        private struct SupportItem
        {
            public string Name;
            public string Tooltip;
            public string Url;
        }

        private static readonly SupportItem[] _supportItems =
        {
            new SupportItem
            {
                Name = "Docs",
                Tooltip = "Official and up to date documentation for Quantum Console.",
                Url = "https://qfsw.co.uk/docs/QC/"
            },
            new SupportItem
            {
                Name = "Email",
                Tooltip = "Email address for support and other inquiries.",
                Url = "mailto:support@qfsw.co.uk"
            },
            new SupportItem
            {
                Name = "Discord",
                Tooltip = "Discord server for customer support, WIPs and more.",
                Url = "https://discord.gg/g8SJ7X6"
            },
            new SupportItem
            {
                Name = "Twitter",
                Tooltip = "Get in touch or show off what you've made with QC.",
                Url = "https://twitter.com/QFSW1024"
            },
            new SupportItem
            {
                Name = "Review",
                Tooltip = "Leave a review to share your opinion and support Quantum Console!",
                Url = "https://assetstore.unity.com/packages/tools/utilities/quantum-console-211046#reviews"
            },
            new SupportItem
            {
                Name = "Survey",
                Tooltip = "A short survey to help me get feedback on Quantum Console and prioritize what needs the most focus.",
                Url = "https://forms.gle/TZbpg1t6hc6sypZA9"
            }
        };

        private static Rect[] _supportItemRects = new Rect[_supportItems.Length];

        public static void DrawBanner(Texture2D banner, float sizeMultiplier = 1f)
        {
            if (banner)
            {
                sizeMultiplier = Mathf.Clamp01(sizeMultiplier);
                Rect bannerRect = GUILayoutUtility.GetRect(0.0f, 0.0f);
                bannerRect.height = Screen.width / EditorGUIUtility.pixelsPerPoint * banner.height / banner.width;
                bannerRect.x += bannerRect.width * (1 - sizeMultiplier) / 2;
                bannerRect.width *= sizeMultiplier;
                bannerRect.height *= sizeMultiplier;

                GUILayout.Space(bannerRect.height);
                GUI.Label(bannerRect, banner);
            }
        }

        public static void DrawSupportRow()
        {
            LayoutController layout = new LayoutController(EditorGUILayout.GetControlRect());
            layout.SpliceRow(_supportItemRects.Length, ref _supportItemRects);

            for (int i = 0; i < _supportItems.Length; i++)
            {
                SupportItem item = _supportItems[i];
                if (GUI.Button(_supportItemRects[i], new GUIContent(item.Name, item.Tooltip)))
                {
                    Application.OpenURL(item.Url);
                }
            }
        }

        public static void DrawHeader(Texture2D banner)
        {
            DrawBanner(banner);
            DrawSupportRow();
        }
    }
}
