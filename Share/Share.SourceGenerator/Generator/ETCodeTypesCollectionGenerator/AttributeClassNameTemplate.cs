// using System;
// using System.Collections.Generic;
//
// namespace ET.Generator
// {
//     public class AttributeClassNameTemplate
//     {
//         private Dictionary<string, string> templates = new Dictionary<string, string>();
//
//         public AttributeClassNameTemplate()
//         {
//             this.templates.Add("EntitySystem",
//                 "$argsTypesUnderLine$_$methodName$System");
//
//             this.templates.Add("LSEntitySystem",
//                 "$argsTypesUnderLine$_$methodName$System");
//
//             this.templates.Add("MessageHandler",
//                 "$className$_$methodName$_Handler");
//
//             this.templates.Add("ActorMessageHandler",
//                 "$className$_$methodName$_Handler");
//
//             this.templates.Add("ActorMessageLocationHandler",
//                 "$className$_$methodName$_Handler");
//
//             this.templates.Add("Event",
//                 "$argsTypes2$_$methodName$");
//         }
//
//         public string Get(string attributeType)
//         {
//             if (!this.templates.TryGetValue(attributeType, out string template))
//             {
//                 throw new Exception($"not class name config template: {attributeType}");
//             }
//
//             if (template == null)
//             {
//                 throw new Exception($"not class name config template: {attributeType}");
//             }
//
//             return template;
//         }
//
//         public bool Contains(string attributeType)
//         {
//             return this.templates.ContainsKey(attributeType);
//         }
//     }
// }