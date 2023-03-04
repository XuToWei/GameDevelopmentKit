using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace CodeBind.Editor
{
    public class MonoCodeBindPropertyProcessor<T> : OdinPropertyProcessor<T, MonoCodeBindAttribute>
    {
        public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
        {
            MonoCodeBindAttribute attribute = this.Property.GetAttribute<MonoCodeBindAttribute>();
            
            propertyInfos.AddDelegate("Code Binder", (Action) (() => { }), -100000f, new Attribute[2]
            {
                (Attribute) new InfoBoxAttribute($"Separator Char:{attribute.separatorChar}"),
                (Attribute) new OnInspectorGUIAttribute("@")
            });

            propertyInfos.AddDelegate("Generate Bind Code", TryGenerateBindCode, -100000f);
            
            propertyInfos.AddDelegate("Generate Serialization", TrySetSerialization, -100000f);
        }

        private void TryGenerateBindCode()
        {
            MonoCodeBindAttribute attribute = this.Property.GetAttribute<MonoCodeBindAttribute>();
            MonoBehaviour mono = this.ValueEntry.SmartValue as MonoBehaviour;
            if (mono == null)
            {
                throw new Exception($"{this.ValueEntry.SmartValue.GetType()} is not inherit from MonoBehaviour!");
            }
            MonoScript script = MonoScript.FromMonoBehaviour(mono);
            MonoCodeBinder codeBinder = new MonoCodeBinder(script, mono.transform, attribute.separatorChar);
            codeBinder.TryGenerateBindCode();
        }

        private void TrySetSerialization()
        {
            MonoCodeBindAttribute attribute = this.Property.GetAttribute<MonoCodeBindAttribute>();
            MonoBehaviour mono = this.ValueEntry.SmartValue as MonoBehaviour;
            if (mono == null)
            {
                throw new Exception($"{this.ValueEntry.SmartValue.GetType()} is not inherit from MonoBehaviour!");
            }
            MonoScript script = MonoScript.FromMonoBehaviour(mono);
            MonoCodeBinder codeBinder = new MonoCodeBinder(script, mono.transform, attribute.separatorChar);
            codeBinder.TrySetSerialization();
        }
    }
}
