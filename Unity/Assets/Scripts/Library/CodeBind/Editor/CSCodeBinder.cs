using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CodeBind.Editor
{
    internal sealed class CSCodeBinder : BaseCodeBinder
    {
        private readonly CSCodeBindMono mCsCodeBindMono;
        
        public CSCodeBinder(MonoScript script, Transform rootTransform, char separatorChar): base(script, rootTransform, separatorChar)
        {
            this.mCsCodeBindMono = rootTransform.GetComponent<CSCodeBindMono>();
            if (this.mCsCodeBindMono == null)
            {
                throw new Exception($"PureCSCodeBinder init fail! {rootTransform} has no MonoBind!");
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
            stringBuilder.AppendLine($"{indentation}public partial class {this.m_ScriptClassName} : CodeBind.ICSCodeBind");
            stringBuilder.AppendLine($"{indentation}{{");
            //组件字段
            stringBuilder.AppendLine($"{indentation}{indentation}public CodeBind.CSCodeBindMono mono {{ get; private set; }}");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"{indentation}{indentation}public UnityEngine.Transform transform {{ get; private set; }}");
            stringBuilder.AppendLine("");
            foreach (CodeBindData bindData in this.m_BindDatas)
            {
                stringBuilder.AppendLine($"{indentation}{indentation}public {bindData.BindType.FullName} {bindData.BindName}{bindData.BindPrefix} {{ get; private set; }}");
                stringBuilder.AppendLine("");
            }
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
            this.mCsCodeBindMono.bindComponents.Clear();
            this.mCsCodeBindMono.bindComponentNames.Clear();
            foreach (CodeBindData bindData in m_BindDatas)
            {
                this.mCsCodeBindMono.bindComponents.Add(bindData.BindTransform.GetComponent(bindData.BindType));
                this.mCsCodeBindMono.bindComponentNames.Add(bindData.BindName);
            }
        }
    }
}
