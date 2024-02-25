using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.UXText;

namespace UnityEngine.UI
{
    public class UXTextLocaleProcesser
    {
        protected UXText LocaleText;
        protected string OriginString;
        public LocalizationHelper.LanguageType LocalizationType { get; protected set; }
        public UXTextLocaleProcesser(UXText text)
        {
            LocalizationType = LocalizationHelper.LanguageType.ChineseSimplified;
            LocaleText = text;
        }

        /// <summary>
        /// 把 原始的多语言文本 处理成优化后的多语言文本
        /// 比如 换行/添加音调符号/字形修正 等
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual string GenLocaleRenderedString(string text)
        {
            return text;
        }
        public virtual void FixVerts(IList<UIVertex> verts)
        {

        }

        /// <summary>
        /// 修改 Localization 时的Text属性
        /// </summary>
        public virtual void ModifyLocaleTextSettings()
        {
            LocaleText.alignment = LocaleText.originAlignment;
        }
    }

    public static class LocaleProcesserFactory
    {
        public static UXTextLocaleProcesser Create(UXText text)
        {
            switch (LocalizationHelper.GetLanguage())
            {
                case LocalizationHelper.LanguageType.ChineseSimplified:
                    return new UXTextLocaleProcesser(text);
                case LocalizationHelper.LanguageType.Thai:
                    return new UXTextLocaleProcesser_Thai(text);
                case LocalizationHelper.LanguageType.Arabic:
                    return new UXTextLocaleProcesser_Arabic(text);
                default:
                    return new UXTextLocaleProcesser(text);
            }
        }
    }


}
