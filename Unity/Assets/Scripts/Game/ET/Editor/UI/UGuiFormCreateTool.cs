using Game.Editor;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace ET.Editor
{
    public static class UGuiFormCreateTool
    {
        private const string UGuiFormTemplate = "Assets/Res/Editor/UI/UGuiTemplateForm-ET.prefab";
        
        [MenuItem("GameObject/UI/UGuiForm-ET", false, UIEditorDefine.MenuPriority)]
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

        public static GameObject CreateUGuiFormPrefab(string name, string savePath)
        {
            GameObject obj = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(UGuiFormTemplate));
            obj.name = name;
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(obj, savePath);
            UnityEngine.Object.DestroyImmediate(obj);
            return prefab;
        }
    }
}
