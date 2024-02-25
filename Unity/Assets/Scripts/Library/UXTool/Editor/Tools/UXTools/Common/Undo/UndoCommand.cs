#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace ThunderFireUITool
{
    //所有需要执行撤销的操作 在这里定义Commond,通过Execute执行Undo记录
    //目的是为了不让Undo. 代码乱飞
    public abstract class UXUndoCommand
    {
        public abstract void Execute();
    }

    public class QuickCreateCommand : UXUndoCommand
    {
        private GameObject m_quickObj;

        public QuickCreateCommand(GameObject qucikObj)
        {
            m_quickObj = qucikObj;
        }

        public override void Execute()
        {
            Undo.IncrementCurrentGroup();
            Undo.RegisterCreatedObjectUndo(m_quickObj, "Create" + m_quickObj.name);
            Undo.IncrementCurrentGroup();
        }
    }


    public class CombineCommand : UXUndoCommand
    {
        private GameObject m_combineRoot;
        private RectTransform[] m_combineObjects;

        public CombineCommand(GameObject combineRoot, RectTransform[] combineObjects)
        {
            m_combineRoot = combineRoot;
            m_combineObjects = combineObjects;
        }

        public override void Execute()
        {
            //var id = Undo.GetCurrentGroup();
            Undo.IncrementCurrentGroup();

            Undo.RegisterCreatedObjectUndo(m_combineRoot, "Combine Operation");
            foreach (var rect in m_combineObjects)
            {
                Undo.SetTransformParent(rect.transform, m_combineRoot.transform, "Combine Operation");
            }

            Undo.IncrementCurrentGroup();
        }
    }

    public class AlignCommand : UXUndoCommand
    {
        private RectTransform[] m_alignObjects;
        public AlignCommand(RectTransform[] alignObjects)
        {
            m_alignObjects = alignObjects;
        }

        public override void Execute()
        {
            Undo.IncrementCurrentGroup();
            Undo.RecordObjects(m_alignObjects, "Align Operation");
        }

    }
    public class LocationLineCommand : UXUndoCommand
    {
        private static LocationLinesData m_LinesData;
        //private static TextAsset m_datajson;
        private string undoName;
        public LocationLineCommand(LocationLinesData linesData, string OperationName)
        {
            m_LinesData = linesData;
            undoName = OperationName;
            //m_datajson = AssetDatabase.LoadAssetAtPath<TextAsset>(ThunderFireUIToolConfig.LocationLinesDataPath);
        }
        public override void Execute()
        {
            Undo.IncrementCurrentGroup();
            Undo.RecordObject(m_LinesData, undoName);
            //Undo.RecordObject(m_datajson, undoName);
        }
    }
}
#endif