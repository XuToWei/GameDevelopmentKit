using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class UGuiFormCreateTool
    {
        private static readonly string UGuiFormTemplate = "Assets/Res/Editor/UI/UGuiTemplateForm.prefab";
        
        [MenuItem("GameObject/UI/UGuiForm")]
        public static void CreateUGuiForm()
        {
            GameObject obj = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(UGuiFormTemplate));
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
