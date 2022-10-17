#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;

namespace ReferenceBindTool.Runtime
{
    public static class CodeGeneratorUtility
    {
        /// <summary>
        /// 获取绑定的组件使用到的命名空间
        /// </summary>
        /// <param name="bindDataList">绑定数据</param>
        /// <returns>绑定的组件使用到的命名空间</returns>
        public static List<string> GetNameSpaces(List<ReferenceBindComponent.BindObjectData> bindDataList)
        {
            List<string> nameSpaces = new List<string>
            {
                nameof(UnityEngine),
                typeof(ReferenceBindComponent).Namespace
            };
            foreach (var bindCom in bindDataList)
            {
                string nameSpace = bindCom.BindObject.GetType().Namespace;
                if (!string.IsNullOrEmpty(nameSpace))
                {
                    nameSpaces.Add(nameSpace);
                }
            }

            return nameSpaces.Distinct().ToList();
        }
    }

    public interface ICodeGeneratorRuleHelper : IRuleHelper
    {
        /// <summary>
        /// 获取生成代码字符串
        /// </summary>
        /// <param name="bindDataList">绑定数据</param>
        /// <param name="nameSpace">生成代码的命名空间</param>
        /// <param name="className">脚本名</param>
        /// <param name="userData">自定义数据</param>
        /// <returns>生成的代码字符串</returns>
        string GetGeneratorCode(List<ReferenceBindComponent.BindObjectData> bindDataList, string nameSpace,string className,object userData);


        /// <summary>
        /// 生成代码并写入文件
        /// </summary>
        /// <param name="bindDataList">绑定数据</param>
        /// <param name="nameSpace">生成代码的命名空间</param>
        /// <param name="className">脚本名</param>
        /// <param name="folderPath">文件夹地址</param>
        /// <param name="userData">自定义数据</param>
        /// <returns>生成代码写入文件成功</returns>
        bool GeneratorCodeAndWriteToFile(List<ReferenceBindComponent.BindObjectData> bindDataList, string nameSpace,string className,string folderPath,object userData);
    }
}
#endif