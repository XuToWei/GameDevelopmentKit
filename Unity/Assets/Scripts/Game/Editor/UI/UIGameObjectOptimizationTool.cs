using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Editor
{
    public static class UIGameObjectOptimizationTool
    {
        private const string MItemName = "GameObject/Optimize Selected UI GameObjects";

        [MenuItem(MItemName, true)]
        private static bool EnableOptimizeSelectedGameObjects()
        {
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem(MItemName)]
        private static void OptimizeSelectedGameObjects()
        {
            var resultMessage = new StringBuilder();
            resultMessage.Append("Optimized");

            var selectedGameObjects = Selection.gameObjects;

            foreach (var selectedGameObject in selectedGameObjects)
            {
                selectedGameObject.TryGetComponent<Canvas>(out var canvas);
                bool hasScrollRect = false;

                foreach (Transform child in selectedGameObject.transform)
                {
                    DisableRaycastTargetForNonButtonImages(child);
                    LogWarningIfAnimatorComponentsAreFound(child, selectedGameObject.name);
                    AddARectMask2DToTheScrollRectIfItsMissingOne(child, out bool childHasScrollRect);
                    DisablePixelPerfectForCanvasContainingAScrollRect(ref canvas, ref hasScrollRect, child, childHasScrollRect);
                    ReplaceTransparentImageWithRaycastGraphic(child);
                }

                resultMessage.AppendFormat(" <b>{0}</b>,", selectedGameObject.name);
            }

            resultMessage.Length--;
            resultMessage.Append((selectedGameObjects.Length > 1) ? " GameObjects!" : " GameObject!");
            Debug.Log(resultMessage);
        }

        private static void DisableRaycastTargetForNonButtonImages(Transform child)
        {
            bool childHasInteractable = child.gameObject.TryGetComponent<Button>(out var childButton) || child.gameObject.TryGetComponent<Toggle>(out var childToggle);
            bool childHasImage = child.gameObject.TryGetComponent<Image>(out var childImage);

            if (!childHasInteractable && childHasImage)
            {
                childImage.raycastTarget = false;
            }
        }

        private static void LogWarningIfAnimatorComponentsAreFound(Transform child, string selectedGameObjectName)
        {
            bool childHasAnimator = child.gameObject.TryGetComponent<Animator>(out var childAnimator);

            if (childHasAnimator)
            {
                Debug.LogWarning($"Child <b>{child.name}</b> of <b>{selectedGameObjectName}</b> has an Animator component. Please consider using a DOTween Animation or ShowHideAnimator instead if possible.");
            }
        }

        private static void AddARectMask2DToTheScrollRectIfItsMissingOne(Transform child, out bool childHasScrollRect)
        {
            childHasScrollRect = child.gameObject.TryGetComponent<ScrollRect>(out var childScrollRect);
            bool childHasRectMask2D = child.gameObject.TryGetComponent<RectMask2D>(out var childRectMask2D);

            if (childHasScrollRect && !childHasRectMask2D)
            {
                child.gameObject.AddComponent<RectMask2D>();
            }
        }

        private static void DisablePixelPerfectForCanvasContainingAScrollRect(ref Canvas canvas, ref bool hasScrollRect, Transform child, bool childHasScrollRect)
        {
            bool childHasCanvas = child.gameObject.TryGetComponent<Canvas>(out var childCanvas);

            if (childHasCanvas)
            {
                canvas = childCanvas;
            }

            if (childHasScrollRect)
            {
                hasScrollRect = true;
            }

            if (hasScrollRect && canvas != null)
            {
                canvas.pixelPerfect = false;
            }
        }
        
        private static void ReplaceTransparentImageWithRaycastGraphic(Transform child)
        {
            var parentObj = child.parent.gameObject;
            bool childHasInteractable = parentObj.TryGetComponent<Button>(out var childButton) || parentObj.TryGetComponent<Toggle>(out var childToggle);
            bool childHasImage = child.gameObject.TryGetComponent<Image>(out var childImage);

            if (childHasInteractable && childHasImage && childImage.raycastTarget && childImage.color.a == 0)
            {
                Object.DestroyImmediate(childImage);
                child.gameObject.AddComponent<RaycastGraphic>();
            }
        }
    }
}