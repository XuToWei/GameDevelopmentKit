using System.Linq;
using System.Reflection;
using Soco.ShaderVariantsCollection;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.Rendering;

// 用于联动变体收集 和 变体剔除工具
// 如果没有使用Soco变体剔除工具，此执行器应删除
public class SocoVariantStripAssociate : Soco.ShaderVariantsCollection.IExecutable
{
    public override void Execute(ShaderVariantCollectionMapper mapper)
    {
        var m_ShaderType = typeof(ShaderSnippetData).GetField("m_ShaderType", BindingFlags.Instance | BindingFlags.NonPublic);
            var m_PassType = typeof(ShaderSnippetData).GetField("m_PassType", BindingFlags.Instance | BindingFlags.NonPublic);
            var m_PassName = typeof(ShaderSnippetData).GetField("m_PassName", BindingFlags.Instance | BindingFlags.NonPublic);

            var stripConfigs =
                com.cyou.soco.ShaderVariantsStripper.ShaderVariantsStripperCode.LoadConfigs();
            
            foreach (var shader in mapper.shaders)
            {
                int variantIndex = 0;
                var variants = mapper.GetShaderVariants(shader).ToArray();
                foreach (var variant in variants)
                {
                    EditorUtility.DisplayProgressBar("变体剔除", $"正在处理{shader.name} {variantIndex}/{variants.Length}变体",
                        (float)variantIndex / (float)variants.Length);
                    variantIndex++;
                    
                    ShaderSnippetData snipperData = new ShaderSnippetData();
                    
                    //这里没对RayTracing、Mesh Shader的剔除做相关处理，默认所有Shader Pass都有VertexShader
                    m_ShaderType.SetValueDirect(__makeref(snipperData), ShaderType.Vertex);
                    m_PassType.SetValueDirect(__makeref(snipperData), variant.passType);
                    m_PassName.SetValueDirect(__makeref(snipperData), "");

                    ShaderCompilerData data = new ShaderCompilerData();
                    data.shaderKeywordSet = new ShaderKeywordSet();

                    foreach (var keyword in variant.keywords)
                    {
                        data.shaderKeywordSet.Enable(new ShaderKeyword(keyword));
                    }

                    if (com.cyou.soco.ShaderVariantsStripper.ShaderVariantsStripperCode.IsVariantStrip(
                            shader, snipperData, data, stripConfigs))
                    {
                        mapper.RemoveVariant(variant);
                    }
                }
            }
            
            EditorUtility.ClearProgressBar();
    }
}
