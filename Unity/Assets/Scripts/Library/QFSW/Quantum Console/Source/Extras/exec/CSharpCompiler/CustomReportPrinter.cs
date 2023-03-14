#if NET_4_6 && !NET_STANDARD_2_0
#define QC_SUPPORTED
#endif

using Mono.CSharp;
using System.CodeDom.Compiler;

#if QC_SUPPORTED
namespace CSharpCompiler
{

    public class CustomReportPrinter : ReportPrinter
    {

        readonly CompilerResults compilerResults;
        #region Properties

        public new int ErrorsCount { get; protected set; }

        public new int WarningsCount { get; private set; }

        #endregion
        public CustomReportPrinter(CompilerResults compilerResults)
        {
            this.compilerResults = compilerResults;
        }

        public override void Print(AbstractMessage msg, bool showFullPath)
        {
            if (msg.IsWarning)
            {
                ++WarningsCount;
            }
            else
            {
                ++ErrorsCount;
            }
            compilerResults.Errors.Add(new CompilerError()
            {
                IsWarning = msg.IsWarning,
                Column = msg.Location.Column,
                Line = msg.Location.Row,
                ErrorNumber = msg.Code.ToString(),
                ErrorText = msg.Text,
                FileName = showFullPath ? msg.Location.SourceFile.FullPathName : msg.Location.SourceFile.Name,
                // msg.RelatedSymbols // extra info
            });
        }

    }

}
#endif
