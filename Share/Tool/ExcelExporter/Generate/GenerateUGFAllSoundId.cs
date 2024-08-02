using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Game.Editor;
using SimpleJSON;

namespace ET
{
    public static class GenerateUGFAllSoundId
    {
        private static readonly string s_LubanMusicAsset = Path.GetFullPath("../Unity/Assets/Res/Editor/Luban/dtmusic.json");
        private static readonly string s_LubanUISoundAsset = Path.GetFullPath("../Unity/Assets/Res/Editor/Luban/dtuisound.json");
        private static readonly string s_LubanSoundAsset = Path.GetFullPath("../Unity/Assets/Res/Editor/Luban/dtsound.json");

        public static void GenerateCode()
        {
            if (ExcelExporter.ExcelExporter_Luban.IsEnableET)
            {
                GenerateCS_Music("ET.Client", "UGFMusicId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Generate/UGF/UGFMusicId.cs"));
                GenerateCS_UISound("ET.Client", "UGFUISoundId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Generate/UGF/UGFUISoundId.cs"));
                GenerateCS_Sound("ET.Client", "UGFSoundId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Generate/UGF/UGFSoundId.cs"));
            }

            if (ExcelExporter.ExcelExporter_Luban.IsEnableGameHot)
            {
                GenerateCS_Music("Game.Hot", "MusicId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/Hot/Code/Runtime/Generate/UGF/MusicId.cs"));
                GenerateCS_UISound("Game.Hot", "UISoundId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/Hot/Code/Runtime/Generate/UGF/UISoundId.cs"));
                GenerateCS_Sound("Game.Hot", "SoundId",
                    Path.GetFullPath("../Unity/Assets/Scripts/Game/Hot/Code/Runtime/Generate/UGF/SoundId.cs"));
            }
        }
        
        private static void GenerateCS_Music(string nameSpaceName, string className, string codeFile)
        {
            if (string.IsNullOrEmpty(nameSpaceName))
            {
                throw new Exception($"Generate UGFMusicId code fail, namespace is empty.");
            }
            if (string.IsNullOrEmpty(className))
            {
                throw new Exception($"Generate UGFMusicId code fail, class name is empty.");
            }
            if (string.IsNullOrEmpty(codeFile))
            {
                throw new Exception($"Generate UGFMusicId code fail, code file is empty.");
            }
            
            JSONNode jsonNode = JSONNode.Parse(File.ReadAllText(s_LubanMusicAsset));
            List<DRMusic> drMusics = new List<DRMusic>();
            foreach (var childNode in jsonNode.Children)
            {
                DRMusic drMusic = DRMusic.LoadJsonDRMusic(childNode);
                drMusics.Add(drMusic);
            }

            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("// This is an automatically generated class by Share.Tool. Please do not modify it.");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"namespace {nameSpaceName}");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    /// <summary>");
            stringBuilder.AppendLine("    /// 音乐编号");
            stringBuilder.AppendLine("    /// </summary>");
            stringBuilder.AppendLine($"    public static class {className}");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        public const int Undefined = 0;");
            foreach (DRMusic drMusic in drMusics)
            {
                if (string.IsNullOrEmpty(drMusic.CSName))
                {
                    throw new Exception($"MusicId {drMusic.Id} CSName is empty!");
                }
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine($"        /// {drMusic.Desc}");
                stringBuilder.AppendLine("        /// </summary>");
                stringBuilder.AppendLine($"        public const int {drMusic.CSName} = {drMusic.Id};");
            }

            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");
            string codeContent = stringBuilder.ToString();
            string dir = Path.GetDirectoryName(codeFile);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(codeFile) || !string.Equals(codeContent, File.ReadAllText(codeFile)))
            {
                File.WriteAllText(codeFile, codeContent);
                Log.Info($"Generate code : {codeFile}!");
            }
        }
        
        private static void GenerateCS_UISound(string nameSpaceName, string className, string codeFile)
        {
            if (string.IsNullOrEmpty(nameSpaceName))
            {
                throw new Exception($"Generate UGFUISoundId code fail, namespace is empty.");
            }
            if (string.IsNullOrEmpty(className))
            {
                throw new Exception($"Generate UGFUISoundId code fail, class name is empty.");
            }
            if (string.IsNullOrEmpty(codeFile))
            {
                throw new Exception($"Generate UGFUISoundId code fail, code file is empty.");
            }
            
            JSONNode jsonNode = JSONNode.Parse(File.ReadAllText(s_LubanUISoundAsset));
            List<DRUISound> drUISounds = new List<DRUISound>();
            foreach (var childNode in jsonNode.Children)
            {
                DRUISound drUISound = DRUISound.LoadJsonDRUISound(childNode);
                drUISounds.Add(drUISound);
            }

            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("// This is an automatically generated class by Share.Tool. Please do not modify it.");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"namespace {nameSpaceName}");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    /// <summary>");
            stringBuilder.AppendLine("    /// UI音乐编号。");
            stringBuilder.AppendLine("    /// </summary>");
            stringBuilder.AppendLine($"    public static class {className}");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        public const int Undefined = 0;");
            foreach (DRUISound drUISound in drUISounds)
            {
                if (string.IsNullOrEmpty(drUISound.CSName))
                {
                    throw new Exception($"UGFUISoundId {drUISound.Id} CSName is empty!");
                }
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine($"        /// {drUISound.Desc}。");
                stringBuilder.AppendLine("        /// </summary>");
                stringBuilder.AppendLine($"        public const int {drUISound.CSName} = {drUISound.Id};");
            }

            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");
            string codeContent = stringBuilder.ToString();
            string dir = Path.GetDirectoryName(codeFile);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(codeFile) || !string.Equals(codeContent, File.ReadAllText(codeFile)))
            {
                File.WriteAllText(codeFile, codeContent);
                Log.Info($"Generate code : {codeFile}!");
            }
        }
        
        private static void GenerateCS_Sound(string nameSpaceName, string className, string codeFile)
        {
            if (string.IsNullOrEmpty(nameSpaceName))
            {
                throw new Exception($"Generate UGFSoundId code fail, namespace is empty.");
            }
            if (string.IsNullOrEmpty(className))
            {
                throw new Exception($"Generate UGFSoundId code fail, class name is empty.");
            }
            if (string.IsNullOrEmpty(codeFile))
            {
                throw new Exception($"Generate UGFSoundId code fail, code file is empty.");
            }
            
            JSONNode jsonNode = JSONNode.Parse(File.ReadAllText(s_LubanSoundAsset));
            List<DRSound> drSounds = new List<DRSound>();
            foreach (var childNode in jsonNode.Children)
            {
                DRSound drUISound = DRSound.LoadJsonDRSound(childNode);
                drSounds.Add(drUISound);
            }

            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("// This is an automatically generated class by Share.Tool. Please do not modify it.");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"namespace {nameSpaceName}");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    /// <summary>");
            stringBuilder.AppendLine("    /// 音乐编号。");
            stringBuilder.AppendLine("    /// </summary>");
            stringBuilder.AppendLine($"    public static class {className}");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        public const int Undefined = 0;");
            foreach (DRSound drSound in drSounds)
            {
                if (string.IsNullOrEmpty(drSound.CSName))
                {
                    throw new Exception($"UGFSoundId {drSound.Id} CSName is empty!");
                }
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("        /// <summary>");
                stringBuilder.AppendLine($"        /// {drSound.Desc}。");
                stringBuilder.AppendLine("        /// </summary>");
                stringBuilder.AppendLine($"        public const int {drSound.CSName} = {drSound.Id};");
            }

            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");
            string codeContent = stringBuilder.ToString();
            string dir = Path.GetDirectoryName(codeFile);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(codeFile) || !string.Equals(codeContent, File.ReadAllText(codeFile)))
            {
                File.WriteAllText(codeFile, codeContent);
                Log.Info($"Generate code : {codeFile}!");
            }
        }
    }
}