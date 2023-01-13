using System;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CodeBind.Editor
{
    internal sealed class MonoCodeBinder : BaseCodeBinder
    {
        private readonly MonoBehaviour m_MonoObj;
        
        public MonoCodeBinder(MonoScript script, Transform rootTransform, char separatorChar): base(script, rootTransform, separatorChar)
        {
            this.m_MonoObj = rootTransform.GetComponent(script.GetClass()) as MonoBehaviour;
            if (this.m_MonoObj == null)
            {
                throw new Exception("MonoCodeBinder only can be used of MonoBehaviour!");
            }
        }
        
        protected override string GetGeneratorCode()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string indentation = string.Empty;
            bool needNameSpace = !string.IsNullOrEmpty(this.m_ScriptNameSpace);
            //命名空间
            if (needNameSpace)
            {
                stringBuilder.AppendLine($"namespace {this.m_ScriptNameSpace}");
                stringBuilder.AppendLine("{");
                indentation = "\t";
            }
            //类名
            stringBuilder.AppendLine($"{indentation}public partial class {this.m_ScriptClassName}");
            stringBuilder.AppendLine($"{indentation}{{");
            //组件字段
            foreach (CodeBindData bindData in this.m_BindDatas)
            {
                stringBuilder.AppendLine($"{indentation}\t[UnityEngine.SerializeField] private {bindData.BindType.FullName} _{bindData.BindName}{bindData.BindPrefix};");
                stringBuilder.AppendLine("");
            }
            foreach (CodeBindData bindData in this.m_BindDatas)
            {
                stringBuilder.AppendLine($"{indentation}\tpublic {bindData.BindType.FullName} {bindData.BindName}{bindData.BindPrefix} => _{bindData.BindName}{bindData.BindPrefix};");
                stringBuilder.AppendLine("");
            }
            stringBuilder.AppendLine($"{indentation}}}");
            if (needNameSpace)
            {
                stringBuilder.AppendLine("}");
            }
            return stringBuilder.ToString();
        }

        protected override void SetSerialization()
        {
            Type monoType = this.m_MonoObj.GetType();
            foreach (CodeBindData bindData in m_BindDatas)
            {
                FieldInfo fieldInfo = monoType.GetField($"_{bindData.BindName}{bindData.BindPrefix}", BindingFlags.NonPublic | BindingFlags.Instance);
                fieldInfo.SetValue(this.m_MonoObj, bindData.BindTransform.GetComponent(bindData.BindType));
            }
        }
    }
}
