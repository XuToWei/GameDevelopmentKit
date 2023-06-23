//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.Localization;
using System;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game
{
    public class ProcedureLaunch : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            // 注册异步需要的事件
            Awaitable.SubscribeEvent();
            
            if (GameEntry.Debugger.ActiveWindow)
            {
                GameEntry.Debugger.RegisterDebuggerWindow("Tool", new ToolDebuggerWindow());
            }
            
            // 语言配置：设置当前使用的语言，如果不设置，则默认使用操作系统语言
            InitLanguageSettings();

            // 变体配置：根据使用的语言，通知底层加载对应的资源变体
            //InitCurrentVariant();

            // 声音配置：根据用户配置数据，设置即将使用的声音选项
            InitSoundSettings();
            
            // 此字典文件记录了资源更新前使用的各种语言的字符串，会随 App 一起发布，故不可更新
            GameEntry.BuiltinData.InitDefaultDictionary(GameEntry.Localization.Language);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            Awaitable.UnsubscribeEvent();
            base.OnDestroy(procedureOwner);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            // 运行一帧即切换到 Splash 展示流程
            ChangeState<ProcedureSplash>(procedureOwner);
        }

        private void InitLanguageSettings()
        {
            if (GameEntry.Base.EditorResourceMode && GameEntry.Base.EditorLanguage != Language.Unspecified)
            {
                // 编辑器资源模式直接使用 Inspector 上设置的语言
                return;
            }

            Language language = GameEntry.Localization.Language;
            if (GameEntry.Setting.HasSetting(Constant.Setting.Language))
            {
                try
                {
                    string languageString = GameEntry.Setting.GetString(Constant.Setting.Language);
                    language = (Language)Enum.Parse(typeof(Language), languageString);
                }
                catch
                {
                }
            }

            if (language != Language.English
                && language != Language.ChineseSimplified
                && language != Language.ChineseTraditional
                && language != Language.Korean)
            {
                // 若是暂不支持的语言，则使用英语
                language = Language.English;

                GameEntry.Setting.SetString(Constant.Setting.Language, language.ToString());
                GameEntry.Setting.Save();
            }

            GameEntry.Localization.Language = language;
            Log.Info("Init language settings complete, current language is '{0}'.", language.ToString());
        }

        private void InitCurrentVariant()
        {
            if (GameEntry.Base.EditorResourceMode)
            {
                // 编辑器资源模式不使用 AssetBundle，也就没有变体了
                return;
            }

            string currentVariant = null;
            switch (GameEntry.Localization.Language)
            {
                case Language.English:
                    currentVariant = "en-us";
                    break;

                case Language.ChineseSimplified:
                    currentVariant = "zh-cn";
                    break;

                case Language.ChineseTraditional:
                    currentVariant = "zh-tw";
                    break;

                case Language.Korean:
                    currentVariant = "ko-kr";
                    break;

                default:
                    currentVariant = "zh-cn";
                    break;
            }

            GameEntry.Resource.SetCurrentVariant(currentVariant);
            Log.Info("Init current variant complete.");
        }

        private void InitSoundSettings()
        {
            GameEntry.Sound.Mute("Music", GameEntry.Setting.GetBool(Constant.Setting.MusicMuted, false));
            GameEntry.Sound.SetVolume("Music", GameEntry.Setting.GetFloat(Constant.Setting.MusicVolume, 0.3f));
            GameEntry.Sound.Mute("UISound", GameEntry.Setting.GetBool(Constant.Setting.UISoundMuted, false));
            GameEntry.Sound.SetVolume("UISound", GameEntry.Setting.GetFloat(Constant.Setting.UISoundVolume, 1f));
            GameEntry.Sound.Mute("Sound.Default", GameEntry.Setting.GetBool(Constant.Setting.SoundMuted, false));
            GameEntry.Sound.SetVolume("Sound.Default", GameEntry.Setting.GetFloat(Constant.Setting.SoundVolume, 1f));
            Log.Info("Init sound settings complete.");
        }
    }
}
