using System;
using System.Reflection;
using CommandLine;

namespace ET.Server
{
    internal static class Init
    {
        private static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error(e.ExceptionObject.ToString());
            };
            
            try
            {
                // 命令行参数
                Parser.Default.ParseArguments<Options>(args)
                        .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
                        .WithParsed((o) => World.Instance.AddSingleton(o));
                var log = new ConsoleLog();
                World.Instance.AddSingleton<Logger, ILog>(log);
                
                World.Instance.AddSingleton<CodeTypes, Assembly[]>(new[] { typeof (Init).Assembly });
                World.Instance.AddSingleton<EventSystem>();
                
                // 强制调用一下mongo，避免mongo库被裁剪
                MongoHelper.ToJson(1);

                if (Define.WorkDir.Contains(' '))
                {
                    Log.Error("工作目录不能包含空格");
                    return 1;
                }

                Log.Info($"server start........................ ");
                switch (Options.Instance.AppType)
                {
                    case AppType.ExcelExporter:
                    {
                        Options.Instance.Console = 1;
                        //Options: Customs
                        //GB2312: 使用GB2312编码解决中文乱码
                        //Json: luban导出json
                        //Check: 只检查，不导出
                        //ShowCmd: 显示cmd
                        ExcelExporter.ExportAll();
                        return 0;
                    }
                    case AppType.Proto2CS:
                    {
                        Options.Instance.Console = 1;
                        Proto2CS.Export();
                        return 0;
                    }
                    case AppType.LocalizationExporter:
                    {
                        Options.Instance.Console = 1;
                        ExcelExporter.ExportLocalization();
                        return 0;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }

            return 1;
        }
    }
}
