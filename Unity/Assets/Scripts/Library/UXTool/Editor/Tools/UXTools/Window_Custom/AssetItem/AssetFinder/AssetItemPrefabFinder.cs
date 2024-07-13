#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace ThunderFireUITool
{
    public class AssetItemPrefabFinder : AssetItemPrefab
    {

        public AssetItemPrefabFinder(FileInfo fileInfo, float scale = 1) : base(fileInfo, scale)
        {
            ResourceType = ResourceType.Prefab;
        }

        protected override void OnClick(MouseDownEvent e)
        {
            switch (e.button)
            {
                case 0:
                    {
                        // Left Mouse Button
                        if (e.clickCount == 2)
                        {
                            Selection.activeObject = AssetObj;
                        }

                        break;
                    }
                case 1:
                    break;
            }
        }
    }
}
#endif