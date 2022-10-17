using UnityEditor;
using UnityEngine;

namespace UGF.Editor
{
    public static class UGuiFormTool
    {
        private static readonly string UGuiFormTemplate = "Assets/Res/Editor/UI/UGuiForm.prefab";
        
        [MenuItem("GameObject/UI/UGuiForm")]
        static void CreateForm()
        {
            GameObject obj = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(UGuiFormTemplate));
            obj.name = "Form";
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
