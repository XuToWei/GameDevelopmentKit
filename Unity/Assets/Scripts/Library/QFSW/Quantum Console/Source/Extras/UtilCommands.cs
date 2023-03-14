using QFSW.QC.Pooling;
using QFSW.QC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFSW.QC.Extras
{
    public static class UtilCommands
    {
        private static readonly ConcurrentStringBuilderPool _builderPool = new ConcurrentStringBuilderPool();

        [Command("get-object-info", "Finds the specified GameObject and displays its transform and component data")]
        private static string ExtractObjectInfo(GameObject target)
        {
            StringBuilder builder = _builderPool.GetStringBuilder();

            builder.AppendLine($"Extracted info for object '{target.name}'");
            builder.AppendLine("Transform data:");
            builder.AppendLine($"   - position: {target.transform.position}");
            builder.AppendLine($"   - rotation: {target.transform.localRotation}");
            builder.AppendLine($"   - scale: {target.transform.localScale}");
            if (target.transform.childCount > 0) { builder.AppendLine($"   - child count: {target.transform.childCount}"); }
            if (target.transform.parent) { builder.AppendLine($"   - parent: {target.transform.parent.name}"); }

            Component[] components = target.GetComponents<Component>().OrderBy(x => x.GetType().Name).ToArray();

            if (components.Length > 0)
            {
                builder.AppendLine("Component data:");
                for (int i = 0; i < components.Length; i++)
                {
                    int componentCount = 1;
                    Type componentType = components[i].GetType();
                    builder.AppendLine($"   - {componentType.Name}");
                    while (i + 1 < components.Length && components[i + 1].GetType() == componentType)
                    {
                        componentCount++;
                        i++;
                    }

                    if (componentCount > 1) { builder.Append($" ({componentCount})"); }
                }
            }

            if (target.transform.childCount > 0)
            {
                builder.AppendLine("Children:");

                int childCount = target.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    builder.AppendLine($"   - {target.transform.GetChild(i).name}");
                }
            }

            return _builderPool.ReleaseAndToString(builder);
        }

        [Command("get-scene-hierarchy", "Renders the GameObject hierarchy of the currently open scenes")]
        private static string GetSceneHierarchy()
        {
            List<GameObject> objects = new List<GameObject>();
            StringBuilder buffer = _builderPool.GetStringBuilder();

            foreach (Scene scene in SceneUtilities.GetLoadedScenes())
            {
                objects.Clear();
                scene.GetRootGameObjects(objects);

                buffer.AppendLine(scene.name);
                GetSceneHierarchy(objects.Select(x => x.transform).ToArray(), 0, buffer, new List<bool>());
            }

            return _builderPool.ReleaseAndToString(buffer);
        }

        private static IEnumerable<Transform> GetChildren(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                yield return transform.GetChild(i);
            }
        }

        private static void GetSceneHierarchy(IList<Transform> roots, int depth, StringBuilder buffer, IList<bool> drawVertical)
        {
            const char terminalSymbol = '|';
            const char verticalSplitSymbol = '|';
            const char verticalSymbol = '|';
            const char horizontalSymbol = '-';
            const int indentation = 3;

            for (int i = 0; i < roots.Count; i++)
            {
                Transform root = roots[i];

                for (int j = 0; j < depth; j++)
                {
                    buffer.Append(drawVertical[j] ? verticalSymbol : ' ');
                    buffer.Append(' ', indentation - 1);
                }

                bool terminal = i == roots.Count - 1;
                drawVertical.Add(!terminal);

                buffer.Append(terminal ? terminalSymbol : verticalSplitSymbol);
                buffer.Append(horizontalSymbol, indentation - 1);
                buffer.AppendLine(root.name);

                GetSceneHierarchy(root.GetChildren().ToList(), depth + 1, buffer, drawVertical);
                drawVertical.RemoveAt(drawVertical.Count - 1);
            }
        }

        [Command("add-component", "Adds a component of type T to the specified GameObject")]
        private static void AddComponent<T>(GameObject target) where T : Component { target.AddComponent<T>(); }

        [Command("destroy-component", "Destroys the component of type T on the specified GameObject")]
        private static void DestroyComponent<T>(T target) where T : Component { GameObject.Destroy(target); }

        [Command("destroy", "Destroys a GameObject")]
        private static void DestroyGO(GameObject target) { GameObject.Destroy(target); }

        [Command("instantiate", "Instantiates a GameObject")]
        private static void InstantiateGO(
            [CommandParameterDescription("The original GameObject to instantiate a copy of.")] GameObject original,
            [CommandParameterDescription("The position of the instantiated GameObject.")] Vector3 position,
            [CommandParameterDescription("The rotation of the instantiated GameObject.")] Quaternion rotation)
        {
            GameObject.Instantiate(original, position, rotation);
        }

        [Command("instantiate", "Instantiates a GameObject")]
        private static void InstantiateGO(GameObject original, Vector3 position) { GameObject.Instantiate(original).transform.position = position; }

        [Command("instantiate", "Instantiates a GameObject")]
        private static void InstantiateGO(GameObject original) { GameObject.Instantiate(original); }

        [Command("teleport", "Teleports a GameObject")]
        private static void TeleportGO(GameObject target, Vector3 position) { target.transform.position = position; }

        [Command("teleport-relative", "Teleports a GameObject by a relative offset to its current position")]
        private static void TeleportRelativeGO(GameObject target, Vector3 offset) { target.transform.Translate(offset); }

        [Command("rotate", "Rotates a GameObject")]
        private static void RotateGO(GameObject target, Quaternion rotation) { target.transform.Rotate(rotation.eulerAngles); }

        [Command("set-active", "Activates/deactivates a GameObject")]
        private static void SetGOActive(GameObject target, bool active) { target.SetActive(active); }

        [Command("set-parent", "Sets the parent of the targert transform.")]
        private static void SetGOParent(Transform target, Transform parentTarget) { target.SetParent(parentTarget); }

        [Command("send-message", "Calls the method named 'methodName' on every MonoBehaviour in the target GameObject")]
        private static void SendGOMessage(GameObject target, string methodName) { target.SendMessage(methodName); }
    }
}
