using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace CodeBind.Editor
{
    internal sealed class CSCodeBinder : BaseCodeBinder
    {
        private readonly CSCodeBindMono m_CsCodeBindMono;
        
        public CSCodeBinder(MonoScript script, Transform rootTransform, char separatorChar): base(script, rootTransform, separatorChar)
        {
            this.m_CsCodeBindMono = rootTransform.GetComponent<CSCodeBindMono>();
            if (this.m_CsCodeBindMono == null)
            {
                throw new Exception($"PureCSCodeBinder init fail! {rootTransform} has no MonoBind!");
            }
        }

        protected override string GetGeneratorCode()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("// This is an automatically generated class by CodeBind. Please do not modify it.");
            stringBuilder.AppendLine("");
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
            stringBuilder.AppendLine($"{indentation}public partial class {this.m_ScriptClassName} : CodeBind.ICSCodeBind");
            stringBuilder.AppendLine($"{indentation}{{");
            //组件字段
            stringBuilder.AppendLine($"{indentation}{indentation}public CodeBind.CSCodeBindMono mono {{ get; private set; }}");
            stringBuilder.AppendLine($"{indentation}{indentation}public UnityEngine.Transform transform {{ get; private set; }}");
            stringBuilder.AppendLine("");
            foreach (CodeBindData bindData in this.m_BindDatas)
            {
                stringBuilder.AppendLine($"{indentation}{indentation}public {bindData.BindType.FullName} {bindData.BindName}{bindData.BindPrefix} {{ get; private set; }}");
            }
            stringBuilder.AppendLine("");
            foreach (KeyValuePair<string, List<CodeBindData>> kv in this.m_BindArrayDataDict)
            {
                stringBuilder.AppendLine($"{indentation}{indentation}public {kv.Value[0].BindType.FullName}[] {kv.Key}Array {{ get; private set; }}");
            }
            stringBuilder.AppendLine("");
            //InitBind方法
            stringBuilder.AppendLine($"{indentation}{indentation}public void InitBind(CodeBind.CSCodeBindMono mono)");
            stringBuilder.AppendLine($"{indentation}{indentation}{{");
            stringBuilder.AppendLine($"{indentation}{indentation}{indentation}this.mono = mono;");
            stringBuilder.AppendLine($"{indentation}{indentation}{indentation}this.transform = mono.transform;");
            for (int i = 0; i < this.m_BindDatas.Count; i++)
            {
                CodeBindData bindData = this.m_BindDatas[i];
                stringBuilder.AppendLine($"{indentation}{indentation}{indentation}this.{bindData.BindName}{bindData.BindPrefix} = this.mono.bindComponents[{i}] as {bindData.BindType.FullName};");
            }
            foreach (KeyValuePair<string, List<CodeBindData>> kv in this.m_BindArrayDataDict)
            {
                stringBuilder.AppendLine($"{indentation}{indentation}{indentation}this.{kv.Key}Array = new {kv.Value[0].BindType.FullName}[{kv.Value.Count}]");
                stringBuilder.AppendLine($"{indentation}{indentation}{indentation}{{");
                for (int i = 0; i < kv.Value.Count; i++)
                {
                    CodeBindData bindData = kv.Value[i];
                    stringBuilder.AppendLine($"{indentation}{indentation}{indentation}{indentation}this.mono.bindComponents[{this.m_BindArrayDatas.IndexOf(bindData) + this.m_BindDatas.Count}] as {bindData.BindType.FullName},");
                }
                stringBuilder.AppendLine($"{indentation}{indentation}{indentation}}};");
            }
            stringBuilder.AppendLine($"{indentation}{indentation}}}");
            //Clear方法
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"{indentation}{indentation}public void ClearBind()");
            stringBuilder.AppendLine($"{indentation}{indentation}{{");
            stringBuilder.AppendLine($"{indentation}{indentation}{indentation}this.mono = null;");
            stringBuilder.AppendLine($"{indentation}{indentation}{indentation}this.transform = null;");
            for (int i = 0; i < this.m_BindDatas.Count; i++)
            {
                CodeBindData bindData = this.m_BindDatas[i];
                stringBuilder.AppendLine($"{indentation}{indentation}{indentation}this.{bindData.BindName}{bindData.BindPrefix} = null;");
            }
            foreach (KeyValuePair<string, List<CodeBindData>> kv in this.m_BindArrayDataDict)
            {
                stringBuilder.AppendLine($"{indentation}{indentation}{indentation}this.{kv.Key}Array = null;");
            }
            stringBuilder.AppendLine($"{indentation}{indentation}}}");
            
            stringBuilder.AppendLine($"{indentation}}}");
            if (needNameSpace)
            {
                stringBuilder.AppendLine("}");
            }
            return stringBuilder.ToString();
        }

        protected override void SetSerialization()
        {
            List<string> bindNames = new List<string>();
            List<Component> bindComponents = new List<Component>();
            foreach (CodeBindData bindData in this.m_BindDatas)
            {
                bindNames.Add(bindData.BindName + bindData.BindPrefix);
                bindComponents.Add(bindData.BindTransform.GetComponent(bindData.BindType));
            }
            foreach (CodeBindData bindData in this.m_BindArrayDatas)
            {
                bindNames.Add($"{bindData.BindName}{bindData.BindPrefix}Array");
                bindComponents.Add(bindData.BindTransform.GetComponent(bindData.BindType));
            }
            this.m_CsCodeBindMono.SetBindComponents(bindNames.ToArray(), bindComponents.ToArray());
        }
    }
}
