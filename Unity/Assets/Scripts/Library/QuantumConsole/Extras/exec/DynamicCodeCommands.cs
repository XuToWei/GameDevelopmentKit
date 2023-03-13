#if NET_4_6 && !NET_STANDARD_2_0
#define QC_SUPPORTED
#endif

#if QC_SUPPORTED
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace QFSW.QC
{
    public static class DynamicCodeCommands
    {
        private const Platform execAvailability = Platform.AllPlatforms ^ (Platform.WebGLPlayer | Platform.IPhonePlayer | Platform.XboxOne | Platform.PS4 | Platform.Switch);

        [CommandDescription("Loads the code at the specified file and compiles it to C# which will then be executed. Use with caution as no safety checks will be performed. Not supported in AOT (IL2CPP) builds." +
        "\n\nBy default, boiler plate code will NOT be inserted around the code you provide. Please see 'exec' for more information about boilerplate insertion")]
        [Command("exec-extern", execAvailability)]
        private static async Task ExecuteExternalArbitaryCodeAsync(string filePath, bool insertBoilerplate = false)
        {
            if (!File.Exists(filePath)) { throw new ArgumentException($"file at the specified path '{filePath}' did not exist."); }
            string code = File.ReadAllText(filePath);
            await ExecuteArbitaryCodeAsync(code.Replace("”", "\"").Replace("“", "\""), insertBoilerplate);
        }

        [CommandDescription("Compiles the given code to C# which will then be executed. Use with caution as no safety checks will be performed. Not supported in AOT (IL2CPP) builds." +
        "\n\nBy default, boiler plate code will be inserted around the code you provide. This means various namespaces will be included, and the main class and main function entry point will " +
        "provided. In this case, the code you provide should be code that would exist within the body of the main function, and thus cannot contain things such as class definition. If you " +
        "disable boiler plate insertion, you can write whatever code you want, however you must provide a static entry point called Main in a static class called Program")]
        [Command("exec", execAvailability)]
        private static async Task ExecuteArbitaryCodeAsync(string code, bool insertBoilerplate = true)
        {
#if !UNITY_EDITOR && ENABLE_IL2CPP
            await Task.FromException(new Exception("exec is not supported on AOT platforms such as IL2CPP and requires JIT (Mono)."));
#else
            MethodInfo entryPoint = await Task.Run(() =>
            {
                string fullCode = string.Empty;
                if (insertBoilerplate)
                {
                    string[] includedNamespaces = new string[] { "System", "System.Collections", "System.Collections.Generic",
                                                         "System.Reflection", "System.Linq", "System.Text", "System.Globalization",
                                                         "UnityEngine", "UnityEngine.Events", "UnityEngine.EventSystems", "UnityEngine.UI" };

                    for (int i = 0; i < includedNamespaces.Length; i++) { fullCode += $"using {includedNamespaces[i]};\n"; }
                    fullCode += @"
                        public class Program
                        {
                            public static void Main()
                            {"
                                     + code +
                                @"}
                        }";
                }
                else { fullCode = code; }

                Assembly assembly = CompileCode(fullCode);
                BindingFlags searchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                Type program = assembly.GetType("Program");
                if (program == null) { throw new ArgumentException("Code Execution Failure - required static class Program could not be found"); }
                entryPoint = program.GetMethod("Main", searchFlags);
                if (entryPoint == null) { throw new ArgumentException("Code Execution Failure - required static entry point Main could not be found"); }
                return entryPoint;
            });

            entryPoint.Invoke(null, null);
#endif
        }

        private static Assembly CompileCode(string code)
        {
#if !UNITY_EDITOR && ENABLE_IL2CPP
            throw new Exception("Code compilation is not supported on AOT platforms such as IL2CPP and requires JIT (Mono).");
#else
            CSharpCompiler.CodeCompiler compiler = new CSharpCompiler.CodeCompiler();
            CompilerParameters compilerParams = new CompilerParameters();
            Assembly[] allLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            compilerParams.GenerateExecutable = false;
            compilerParams.GenerateInMemory = true;
            for (int i = 0; i < allLoadedAssemblies.Length; i++)
            {
                if (!allLoadedAssemblies[i].IsDynamic)
                {
                    string dllName = allLoadedAssemblies[i].Location;
                    compilerParams.ReferencedAssemblies.Add(dllName);
                }
            }

            CompilerResults compiledCode = compiler.CompileAssemblyFromSource(compilerParams, code);

            if (compiledCode.Errors.HasErrors)
            {
                string errorMessage = "Code Compilation Failure";
                for (int i = 0; i < compiledCode.Errors.Count; i++)
                {
                    errorMessage += $"\n{compiledCode.Errors[i].ErrorNumber} - {compiledCode.Errors[i].ErrorText}";
                }

                throw new ArgumentException(errorMessage);
            }

            return compiledCode.CompiledAssembly;
#endif
        }
    }
}
#endif
