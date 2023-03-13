#if NET_4_6 && !NET_STANDARD_2_0
#define QC_SUPPORTED
#endif

using Mono.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection.Emit;
using System.Text;

#if QC_SUPPORTED
namespace CSharpCompiler
{
    public class CodeCompiler : ICodeCompiler
    {
        static long assemblyCounter = 0;

        public CompilerResults CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit compilationUnit)
        {
            return CompileAssemblyFromDomBatch(options, new[] { compilationUnit });
        }

        public CompilerResults CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            try
            {
                return CompileFromDomBatch(options, ea);
            }
            finally
            {
                options.TempFiles.Delete();
            }
        }

        private CompilerResults CompileFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
        {
            throw new NotImplementedException("sorry ICodeGenerator is not implemented, feel free to fix it and request merge");
        }

        public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName)
        {
            return CompileAssemblyFromFileBatch(options, new[] { fileName });
        }

        public CompilerResults CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames)
        {
            var settings = ParamsToSettings(options);

            foreach (var fileName in fileNames)
            {
                string path = Path.GetFullPath(fileName);
                var unit = new SourceFile(fileName, path, settings.SourceFiles.Count + 1);
                settings.SourceFiles.Add(unit);
            }

            return CompileFromCompilerSettings(settings, options.GenerateInMemory);
        }

        public CompilerResults CompileAssemblyFromSource(CompilerParameters options, string source)
        {
            return CompileAssemblyFromSourceBatch(options, new[] { source });
        }

        public CompilerResults CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
        {
            var settings = ParamsToSettings(options);

            int i = 0;
            foreach (var _source in sources)
            {
                var source = _source;
                Func<Stream> getStream = () => { return new MemoryStream(Encoding.UTF8.GetBytes(source ?? "")); };
                var fileName = i.ToString();
                var unit = new SourceFile(fileName, fileName, settings.SourceFiles.Count + 1, getStream);
                settings.SourceFiles.Add(unit);
                i++;
            }

            return CompileFromCompilerSettings(settings, options.GenerateInMemory);
        }


        CompilerResults CompileFromCompilerSettings(CompilerSettings settings, bool generateInMemory)
        {
            var compilerResults = new CompilerResults(new TempFileCollection(Path.GetTempPath()));
            var driver = new CustomDynamicDriver(new CompilerContext(settings, new CustomReportPrinter(compilerResults)));

            AssemblyBuilder outAssembly = null;
            try
            {
                driver.Compile(out outAssembly, AppDomain.CurrentDomain, generateInMemory);
            }
            catch (Exception e)
            {
                compilerResults.Errors.Add(new CompilerError()
                {
                    IsWarning = false,
                    ErrorText = e.Message,
                });
            }
            compilerResults.CompiledAssembly = outAssembly;

            return compilerResults;
        }


        CompilerSettings ParamsToSettings(CompilerParameters parameters)
        {
            var settings = new CompilerSettings();


            foreach (var assembly in parameters.ReferencedAssemblies) settings.AssemblyReferences.Add(assembly);

            settings.Encoding = System.Text.Encoding.UTF8;
            settings.GenerateDebugInfo = parameters.IncludeDebugInformation;
            settings.MainClass = parameters.MainClass;
            settings.Platform = Platform.AnyCPU;
            settings.StdLibRuntimeVersion = RuntimeVersion.v4;
            if (parameters.GenerateExecutable)
            {
                settings.Target = Target.Exe;
                settings.TargetExt = ".exe";
            }
            else
            {
                settings.Target = Target.Library;
                settings.TargetExt = ".dll";
            }
            if (parameters.GenerateInMemory) settings.Target = Target.Library;

            if (string.IsNullOrEmpty(parameters.OutputAssembly))
            {
                parameters.OutputAssembly = settings.OutputFile = "DynamicAssembly_" + assemblyCounter + settings.TargetExt;
                assemblyCounter++;
            }
            settings.OutputFile = parameters.OutputAssembly; // if it is not being outputted, we use this to set name of the dynamic assembly

            settings.Version = LanguageVersion.Default;
            settings.WarningLevel = parameters.WarningLevel;
            settings.WarningsAreErrors = parameters.TreatWarningsAsErrors;

            return settings;
        }
    }
}
#endif
