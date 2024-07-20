using UnityEngine;

namespace ThunderFireUnityEx
{
    public static class TransformEx
    {
        public static Transform FindChildRecursive(this Transform root, string name, bool selfCalling = false)
        {
            if (root == null)
                return null;
            if (!selfCalling)
                UnityEngine.Profiling.Profiler.BeginSample($"TransformEx.FindChildRecursive [{root.gameObject.name}][{name}]");
            Transform _temp = null;
            for (int index = 0; index < root.childCount; index++)
            {
                Transform child = root.GetChild(index);
                if (child.gameObject.name.Equals(name))
                {
                    //Debug.Log("child =" + child.gameObject.name);
                    _temp = child;
                }


                if (_temp)
                {
                    break;
                }
                _temp = FindChildRecursive(child, name, true);
            }
            if (!selfCalling)
                UnityEngine.Profiling.Profiler.EndSample();

            return _temp;
        }

        public static Transform FindChildDirect(this Transform root, string name)
        {
            Transform[] ts = root.transform.GetComponentsInChildren<Transform>();
            for (int index = 0; index < ts.Length; ++index)
            {
                Transform t = ts[index];
                if (t.gameObject.name == name)
                {
                    return t;
                }
            }
            return null;
        }

        public static void ChangeLayersRecursively(this Transform trans, int layer)
        {
            trans.gameObject.layer = layer;
            foreach (Transform child in trans)
            {
                child.ChangeLayersRecursively(layer);
            }
        }

        public static string GetTransPath(this Transform trans)
        {
            if (trans == null)
            {
                return "";
            }
            if (trans.parent != null)
            {
                return trans.parent.GetTransPath() + "/" + trans.name;
            }
            else
            {
                return trans.name;
            }
        }

        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            var com = obj.GetComponent<T>();
            if (com == null)
            {
                com = obj.AddComponent<T>();
            }
            return com;
        }


        public static Component GetOrAddComponent(this GameObject obj, System.Type type)
        {
            var com = obj.GetComponent(type);
            if (com == null)
            {
                com = obj.gameObject.AddComponent(type);
            }
            return com;
        }


        public static T GetOrAddComponent<T>(this Component behaviour) where T : Component
        {
            var com = behaviour.GetComponent<T>();
            if (com == null)
            {
                com = behaviour.gameObject.AddComponent<T>();
            }
            return com;
        }


        public static Component GetOrAddComponent(this Component behaviour, System.Type type)
        {
            var com = behaviour.GetComponent(type);
            if (com == null)
            {
                com = behaviour.gameObject.AddComponent(type);
            }
            return com;
        }


        public static void ResetValue(this Transform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }

        public static void LookAtPos(this Transform trans, float x, float y, float z)
        {
            trans.LookAt(new Vector3(x, y, z));
        }


        public static string PathFromRoot(this Transform t)
        {
            if (t == null)
            {
                return "";
            }

            if (t.parent == null)
            {
                return t.name;
            }

            return string.Format("{0}/{1}", PathFromRoot(t.parent), t.name);
        }

        public static void AddChild(this Transform trans, GameObject childGameObject)
        {
            if (childGameObject != null)
            {
                AddChild(trans, childGameObject.transform);
            }
        }

        public static void AddChild(this Transform trans, Transform childTrans)
        {
            if (trans == null || childTrans == null)
            {
                return;
            }
            childTrans.SetParent(trans);
            childTrans.localPosition = Vector3.zero;
            childTrans.localRotation = Quaternion.identity;
            childTrans.localScale = Vector3.one;
        }

        public static Vector3 InverseTransformPointWithoutScale(this Transform transform, Vector3 point)
        {
            var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
            return worldToLocalMatrix.MultiplyPoint3x4(point);
        }

        public static Vector3 InverseTransformPoint(this Transform transform, Vector3 point)
        {
            var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale).inverse;
            return worldToLocalMatrix.MultiplyPoint3x4(point);
        }

        public static T TryAddComponent<T>(this GameObject target) where T : Component
        {
            T[] components = target.GetComponents<T>();

            bool hasT = false;
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].GetType().FullName == typeof(T).FullName)
                {
                    hasT = true;
                }
            }

            if (!hasT)
            {
                T component = target.AddComponent<T>();
                return component;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ���ص�һ����ͬ����(���ּ̳й�ϵ)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T TryGetComponent<T>(this GameObject target) where T : Component
        {
            T[] components = target.GetComponents<T>();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].GetType().FullName == typeof(T).FullName)
                {
                    return components[i];
                }
            }
            return null;
        }

        public static bool HasComponent<T>(this GameObject target) where T : Component
        {
            T[] components = target.GetComponents<T>();

            bool hasT = false;
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].GetType().FullName == typeof(T).FullName)
                {
                    hasT = true;
                }
            }
            return hasT;
        }

    }
}
