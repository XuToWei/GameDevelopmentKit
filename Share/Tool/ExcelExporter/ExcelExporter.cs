using System.IO;

namespace ET
{
    public static partial class ExcelExporter
    {
        private static readonly string WorkDir = Path.GetFullPath("../Bin");
        private static readonly string LocalizationExcelFile =  Path.GetFullPath($"{WorkDir}/../Design/Excel/Localization.xlsx");

        public static void Export()
        {
            ExcelExporter_Luban.Export();
            ExcelExporter_Localization.Export();
        }
    }
}