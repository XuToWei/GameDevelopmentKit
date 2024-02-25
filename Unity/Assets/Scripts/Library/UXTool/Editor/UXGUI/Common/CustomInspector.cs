using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;

[Obsolete]
/// <summary>
/// 尝试动态添加/修改 Attribute 失败了
/// </summary>
//[CustomEditor(typeof(EmptyTest), true)]
public class UXCustomEditor : Editor
{
    private Type L18nLabelAttrbuteType;
    private string L18nLabelAttrbuteField = "LabelName";

    private Dictionary<string, string> LocalizationPropertyDict;


    private void OnEnable()
    {
        L18nLabelAttrbuteType = typeof(LabelTextAttribute);

        LocalizationPropertyDict = new Dictionary<string, string>();
        LocalizationPropertyDict.Add("sat", "动态属性ID");

        using (var iterator = serializedObject.GetIterator())
        {
            if (iterator.NextVisible(true))
            {
                do
                {
                    if (LocalizationPropertyDict.ContainsKey(iterator.name))
                    {
                        Type targetType = serializedObject.targetObject.GetType();
                        FieldInfo fieldInfo = targetType.GetField(iterator.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        FieldInfo attributefieldInfo = L18nLabelAttrbuteType.GetField(L18nLabelAttrbuteField, BindingFlags.Instance | BindingFlags.Public);
                        
                        object[] attrs = fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
                        LabelTextAttribute t = attrs.Select(e => e as LabelTextAttribute).First();
                        attributefieldInfo.SetValue(t, LocalizationPropertyDict[iterator.name]);

                        object[] attrs1 = fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), true);

                        //TypeDescriptor只能操作property, 不能操作field
                        //PropertyDescriptorCollection allProperties = TypeDescriptor.GetProperties(targetType);
                        //var t1 = allProperties[iterator.name.ToUpper()].Attributes[L18nLabelAttrbuteType];
                        //attributefieldInfo.SetValue(t1, LocalizationPropertyDict[iterator.name]);

                        //Type memberDescriptorType = typeof(MemberDescriptor);
                        //FieldInfo memberDescriptorFieldInfo = memberDescriptorType.GetField("displayName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
                        //memberDescriptorFieldInfo.SetValue(allProperties["name"], LocalizationPropertyDict[iterator.name]);
                    }
                }
                while (iterator.NextVisible(false));
            }
        }
    }
}
