using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor.Rendering;
#endif

namespace Soco.ShaderVariantsStripper
{
    public enum ShaderVariantsDataShaderType
    {
        Vertex = 1,
        Fragment = 2,
        Geometry = 3,
        Hull = 4,
        Domain = 5,
        Surface = 6,
        Count = 7,
        RayTracing = 7,
    }

    public struct ShaderVariantsData
    {
        //ShaderSnipperData
        public ShaderVariantsDataShaderType shaderType;
        public UnityEngine.Rendering.PassType passType;
        public string passName;

        public bool inStripCallback;

        //ShaderCompilerData
        private UnityEngine.Rendering.ShaderKeywordSet shaderKeywordSet;
        private UnityEngine.Rendering.PlatformKeywordSet platformKeywordSet;

        private List<string> _shaderKeywordList;

        private List<string> shaderKeywordList
        {
            get
            {
                if (_shaderKeywordList == null)
                    _shaderKeywordList = new List<string>();
                return _shaderKeywordList;
            }
        }

#if UNITY_EDITOR
        public static ShaderVariantsData GetDefaultShaderVariantsData()
        {
            return new ShaderVariantsData()
            {
                inStripCallback = false,
                passName = ""
            };
        }

        public static ShaderVariantsData GetShaderVariantsData(ShaderSnippetData shaderSnippetData,
            ShaderCompilerData data)
        {
            return new ShaderVariantsData()
            {
                shaderType = (ShaderVariantsDataShaderType)shaderSnippetData.shaderType,
                passType = shaderSnippetData.passType,
                passName = shaderSnippetData.passName,
                shaderKeywordSet = data.shaderKeywordSet,
                platformKeywordSet = data.platformKeywordSet,
                inStripCallback = true
            };
        }
#endif

        public void EnableKeyword(ShaderKeyword keyword)
        {
            if (inStripCallback)
                shaderKeywordSet.Enable(keyword);
            else
            {
                if (shaderKeywordList.FindIndex(k => k == keyword.GetKeywordName()) < 0)
                {
                    shaderKeywordList.Add(keyword.GetKeywordName());
                    shaderKeywordList.Sort();
                }
            }
        }

        public void DisableKeyword(ShaderKeyword keyword)
        {
            if (inStripCallback)
                shaderKeywordSet.Disable(keyword);
            else
                shaderKeywordList.Remove(keyword.GetKeywordName());
        }

        public void DisableKeyword(string keyword)
        {
            if (inStripCallback)
            {
                shaderKeywordSet.Disable(new ShaderKeyword(keyword));
            }
            else
                shaderKeywordList.Remove(keyword);
        }

        public bool IsKeywordEnabled(ShaderKeyword keyword)
        {
            if (inStripCallback)
                return shaderKeywordSet.IsEnabled(keyword);
            else
                return shaderKeywordList.FindIndex(k => k == keyword.GetKeywordName()) >= 0;
        }

        public bool IsKeywordEnabled(string keyword, Shader shader = null)
        {
            if (inStripCallback)
                return shaderKeywordSet.IsEnabled(new ShaderKeyword(keyword)) ||
                       (shader != null && shaderKeywordSet.IsEnabled(new ShaderKeyword(shader, keyword)));
            else
                return shaderKeywordList.FindIndex(k => k == keyword) >= 0;
        }

        public string[] GetShaderKeywords()
        {
            if (inStripCallback)
                return (from sk in shaderKeywordSet.GetShaderKeywords() select sk.GetKeywordName()).ToArray();
            else
                return shaderKeywordList.ToArray();
        }

        public ShaderKeyword[] GetShaderKeywordsObjectArray()
        {
            if (inStripCallback)
                return shaderKeywordSet.GetShaderKeywords();
            else
            {
                Debug.LogError("ShaderKeywordSet cannot be used outside of IPreprocessShaders");
                return null;
            }
        }
    }
}