using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    public static class UGuiFormCreateTool
    {
        private static readonly string UGuiFormTemplate = "Assets/Res/Editor/UI/UGuiTemplateForm-ET.prefab";
        
        [MenuItem("GameObject/UI/UGuiForm-ET")]
        public static void CreateUGuiForm()
        {
            GameObject obj = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(UGuiFormTemplate));
            obj.name = "UIForm";
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            rectTransform.SetParent(Selection.activeTransform);
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.anchoredPosition = Vector3.zero;
            rectTransform.sizeDelta = Vector3.zero;
        }
    }
}
