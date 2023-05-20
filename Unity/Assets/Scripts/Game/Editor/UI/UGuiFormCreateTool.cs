using UnityEditor;

namespace Game.Editor
{
    public static class UGuiFormCreateTool
    {
        private static readonly string UGuiFormTemplate = "Assets/Res/Editor/UI/UGuiTemplateForm.prefab";
        
        [MenuItem("GameObject/UI/UGuiForm", false, UIEditorDefine.MenuPriority)]
        public static void CreateUGuiForm()
        {
            UnityEngine.GameObject obj = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(UGuiFormTemplate));
            obj.name = "UIForm";
            UnityEngine.RectTransform rectTransform = obj.GetComponent<UnityEngine.RectTransform>();
            rectTransform.SetParent(Selection.activeTransform);
            rectTransform.localRotation = UnityEngine.Quaternion.identity;
            rectTransform.localScale = UnityEngine.Vector3.one;
            rectTransform.localPosition = UnityEngine.Vector3.zero;
            rectTransform.anchoredPosition = UnityEngine.Vector3.zero;
            rectTransform.sizeDelta = UnityEngine.Vector3.zero;
        }
    }
}
