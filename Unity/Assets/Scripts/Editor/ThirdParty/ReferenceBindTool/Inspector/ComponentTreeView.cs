using System.Collections.Generic;
using ReferenceBindTool.Runtime;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    public class ComponentTreeView : TreeView
    {
        private Transform m_Transform;
        private Dictionary<int, bool> m_Select;
        
        public ComponentTreeView(TreeViewState state,Transform transform,Dictionary<int, bool> select) : base(state)
        {
            Reload(transform, select);
        }

        public void Reload(Transform transform,Dictionary<int, bool> select)
        {
            m_Transform = transform;
            m_Select = select;
            base.Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem {id = 0, depth = -1};
        }
        
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = GetRows () ?? new List<TreeViewItem> (200);
            rows.Clear ();
            
            var item = CreateTreeViewItemForGameObject (m_Transform.gameObject);
            root.AddChild (item);
            rows.Add (item);
        
            if (IsExpanded(item.id))
            {
                AddChildrenRecursive(m_Transform, item, rows);
            }
            else
            {
                item.children = CreateChildListForCollapsedParent ();
            }
            //
            
            SetupDepthsFromParentsAndChildren (root);
            return rows;
        }

        void AddComponent(Transform transform, TreeViewItem item, IList<TreeViewItem> rows)
        {
            int childCount = transform.childCount;
            var components = transform.GetComponents<Component>();
            item.children = new List<TreeViewItem>(components.Length+childCount+1);
            foreach (Component component in components)
            {
                // 跳过 missing 
                if (component == null )
                {
                    continue;
                }
                
                if (transform == m_Transform && component is ReferenceBindComponent)
                {
                    continue;
                }
                var componentItem = CreateTreeViewItemForComponent(component);
                item.AddChild(componentItem);
                rows.Add (componentItem);
            }
        }

        void AddChildrenRecursive (Transform transform, TreeViewItem item, IList<TreeViewItem> rows)
        {
            int childCount = transform.childCount;
            
            AddComponent(transform, item, rows);
            
            for (int i = 0; i < childCount; ++i)
            {
                var childTransform = transform.GetChild (i);
                var go = CreateTreeViewItemForGameObject (childTransform.gameObject);
                item.AddChild(go);
                rows.Add(go);
                if (IsExpanded (go.id))
                {
                    AddChildrenRecursive(childTransform, go, rows);
                }
                else
                {
                    go.children = CreateChildListForCollapsedParent ();
                }
                
            }
        }
        static TreeViewItem CreateTreeViewItemForComponent (Component component)
        {
            return new TreeViewItem(component.GetInstanceID(), -1, component.GetType().Name);
        }
        static TreeViewItem CreateTreeViewItemForGameObject (GameObject component)
        {
            return new TreeViewItem(component.GetInstanceID(), -1, component.name);
        }
        
        Object GetObject (int instanceID)
        {
            return (Object)EditorUtility.InstanceIDToObject(instanceID);
        }
        
        protected override IList<int> GetAncestors (int id)
        {
            // The backend needs to provide us with this info since the item with id
            // may not be present in the rows
            var obj = GetObject(id);
            Transform transform = null;
            if (obj is Component component )
            {
                if (component != null)
                {
                    transform = component.transform;
                }
               
            }

            if (obj is GameObject gameObject)
            {
                if (gameObject != null)
                {
                    transform = gameObject.transform;
                }
            }
            
            List<int> ancestors = new List<int> ();
            if (transform == null)
            {
                return ancestors;
            }
            while (transform.parent != null)
            {
                ancestors.Add (transform.parent.gameObject.GetInstanceID ());
                transform = transform.parent;
            }

            return ancestors;
        }
        
        protected override IList<int> GetDescendantsThatHaveChildren (int id)
        {
            Stack<Transform> stack = new Stack<Transform> ();
            var obj = GetObject(id);

            Transform transform = null;
            if (obj is Component component )
            {
                if (component != null)
                {
                    transform = component.transform;
                }
               
            }

            if (obj is GameObject gameObject)
            {
                if (gameObject != null)
                {
                    transform = gameObject.transform;
                }
            }
            stack.Push (transform);

            var parents = new List<int> ();
            while (stack.Count > 0)
            {
                Transform current = stack.Pop ();
                parents.Add (current.gameObject.GetInstanceID ());
                for (int i = 0; i < current.childCount; ++i)
                {
                    if (current.childCount > 0)
                        stack.Push (current.GetChild (i));
                }
            }

            return parents;
        }
        
        protected override void RowGUI (RowGUIArgs args)
        {
            Event evt = Event.current;
            extraSpaceBeforeIconAndLabel = 18f;

            var unityObj = GetObject(args.item.id);
            if (unityObj == null)
                return;

            if (unityObj is Component)
            {
                Rect toggleRect = args.rowRect;
                toggleRect.x += GetContentIndent(args.item);
                toggleRect.width = 16f;
                if (evt.type == EventType.MouseDown && toggleRect.Contains(evt.mousePosition))
                    SelectionClick(args.item, false);
                m_Select.TryGetValue(args.item.id, out bool isSelect);
                EditorGUI.BeginChangeCheck();
                isSelect = EditorGUI.Toggle(toggleRect, isSelect);
                if (EditorGUI.EndChangeCheck())
                {
                    m_Select[args.item.id] = isSelect;
                }
            }
            // Text
            base.RowGUI(args);
        }
    }
}

