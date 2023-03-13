using QFSW.QC.Suggestors.Tags;
using QFSW.QC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QFSW.QC
{
    public static partial class QuantumConsoleProcessor
    {

        private const string helpStr = "Welcome to Quantum Console! In order to see specific help about any specific command, " +
                "please use the 'man' command. Use 'man man' to see more about the man command. To see a full list of all " +
                "commands, use 'all-commands'.\n\n" +
                "mono-targets\nVarious commands may show a mono-target in their command signature.\n" +
                "This means they are not static commands, and instead requires instance(s) of the class in order to invoke the command." +
                "\nEach mono-target works differently as follows:" +
                "\n - single: uses the first instance of the type found in the scene" +
                "\n - all: uses all instances of the type found in the scene" +
                "\n - registry: uses all instances of the type found in the registry" +
                "\n - singleton: creates and manages a single instance automatically" +
                "\n\nThe registry is a part of the Quantum Registry that allows you to decide which specific instances of the class " +
                "should be used when invoking the command. In order to add an object to the registry, either use " +
                "QFSW.QC.QuantumRegistry.RegisterObject<T> or the runtime command 'register-object<T>'.";

        [Command("help", "Shows a basic help guide for Quantum Console")]
        private static string GetHelp()
        {
            return helpStr;
        }

        [Command("manual")]
        [Command("man")]
        private static string ManualHelp()
        {
            return "To use the man command, simply put the desired command name in front of it. For example, 'man my-command' will generate the manual for 'my-command'";
        }

        [CommandDescription("Generates a user manual for any given command, including built in ones. To use the man command, simply put the desired command name infront of it. For example, 'man my-command' will generate the manual for 'my-command'")]
        [Command("help")]
        [Command("manual")]
        [Command("man")]
        private static string GenerateCommandManual([CommandName] string commandName)
        {
            string[] matchingCommands = 
                _commandTable
                    .Keys
                    .Where(key => key.Split('(')[0] == commandName)
                    .OrderBy(key => key)
                    .ToArray();

            if (matchingCommands.Length == 0)
            {
                throw new ArgumentException($"No command with the name {commandName} was found.");
            }

            Dictionary<string, ParameterInfo> foundParams = new Dictionary<string, ParameterInfo>();
            Dictionary<string, Type> foundGenericArguments = new Dictionary<string, Type>();
            Dictionary<string, CommandParameterDescriptionAttribute> foundParamDescriptions = new Dictionary<string, CommandParameterDescriptionAttribute>();
            List<Type> declaringTypes = new List<Type>(1);

            string manual = $"Generated user manual for {commandName}\nAvailable command signatures:";

            for (int i = 0; i < matchingCommands.Length; i++)
            {
                CommandData currentCommand = _commandTable[matchingCommands[i]];
                declaringTypes.Add(currentCommand.MethodData.DeclaringType);

                manual += $"\n   - {currentCommand.CommandSignature}";
                if (!currentCommand.IsStatic) { manual += $" (mono-target = {currentCommand.MonoTarget.ToString().ToLower()})"; }
                for (int j = 0; j < currentCommand.ParamCount; j++)
                {
                    ParameterInfo param = currentCommand.MethodParamData[j];
                    if (!foundParams.ContainsKey(param.Name)) { foundParams.Add(param.Name, param); }
                    if (!foundParamDescriptions.ContainsKey(param.Name))
                    {
                        CommandParameterDescriptionAttribute descriptionAttribute = param.GetCustomAttribute<CommandParameterDescriptionAttribute>();
                        if (descriptionAttribute != null && descriptionAttribute.Valid) { foundParamDescriptions.Add(param.Name, descriptionAttribute); }
                    }
                }

                if (currentCommand.IsGeneric)
                {
                    Type[] genericArgs = currentCommand.GenericParamTypes;
                    for (int j = 0; j < genericArgs.Length; j++)
                    {
                        Type arg = genericArgs[j];
                        if (!foundGenericArguments.ContainsKey(arg.Name)) { foundGenericArguments.Add(arg.Name, arg); }
                    }
                }
            }

            if (foundParams.Count > 0)
            {
                manual += "\nParameter info:";
                ParameterInfo[] commandParams = foundParams.Values.ToArray();
                for (int i = 0; i < commandParams.Length; i++)
                {
                    ParameterInfo currentParam = commandParams[i];
                    manual += $"\n   - {currentParam.Name}: {currentParam.ParameterType.GetDisplayName()}";
                }
            }

            string genericConstraintInformation = "";
            if (foundGenericArguments.Count > 0)
            {
                Type[] genericArgs = foundGenericArguments.Values.ToArray();
                for (int i = 0; i < genericArgs.Length; i++)
                {
                    Type arg = genericArgs[i];
                    Type[] typeConstraints = arg.GetGenericParameterConstraints();
                    GenericParameterAttributes attributes = arg.GenericParameterAttributes;

                    List<string> formattedConstraints = new List<string>();
                    if (attributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint)) { formattedConstraints.Add("struct"); }
                    if (attributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint)) { formattedConstraints.Add("class"); }
                    for (int j = 0; j < typeConstraints.Length; j++) { formattedConstraints.Add(typeConstraints[i].GetDisplayName()); }
                    if (attributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint)) { formattedConstraints.Add("new()"); }

                    if (formattedConstraints.Count > 0)
                    {
                        genericConstraintInformation += $"\n   - {arg.Name}: {string.Join(", ", formattedConstraints)}";
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(genericConstraintInformation)) { manual += $"\nGeneric constraints:{genericConstraintInformation}"; }

            for (int i = 0; i < matchingCommands.Length; i++)
            {
                CommandData currentCommand = _commandTable[matchingCommands[i]];
                if (currentCommand.HasDescription)
                {
                    manual += $"\n\nCommand description:\n{currentCommand.CommandDescription}";
                    i = matchingCommands.Length;
                }
            }

            if (foundParamDescriptions.Count > 0)
            {
                manual += "\n\nParameter descriptions:";
                ParameterInfo[] commandParams = foundParams.Values.ToArray();
                for (int i = 0; i < commandParams.Length; i++)
                {
                    ParameterInfo currentParam = commandParams[i];
                    if (foundParamDescriptions.ContainsKey(currentParam.Name))
                    {
                        manual += $"\n - {currentParam.Name}: {foundParamDescriptions[currentParam.Name].Description}";
                    }
                }
            }

            declaringTypes = declaringTypes.Distinct().ToList();
            manual += "\n\nDeclared in";
            if (declaringTypes.Count == 1) { manual += $" {declaringTypes[0].GetDisplayName(true)}"; }
            else
            {
                manual += ":";
                foreach (Type type in declaringTypes)
                {
                    manual += $"\n   - {type.GetDisplayName(true)}";
                }
            }

            return manual;
        }

        /// <summary>
        /// Gets all loaded unique commands. Unique excludes multiple overloads of the same command from appearing.
        /// </summary>
        /// <returns>All loaded unique commands.</returns>
        public static IEnumerable<CommandData> GetUniqueCommands()
        {
            return GetAllCommands()
                .DistinctBy(x => x.CommandName)
                .OrderBy(x => x.CommandName);
        }

        [CommandDescription("Generates a list of all commands currently loaded by the Quantum Console Processor")]
        [Command("commands")]
        [Command("all-commands")]
        private static string GenerateCommandList()
        {
            string output = "List of all commands loaded by the Quantum Processor. Use 'man' on any command to see more:";
            foreach (CommandData command in GetUniqueCommands())
            {
                output += $"\n   - {command.CommandName}";
            }

            return output;
        }

        [Command("user-commands", "Generates a list of all commands added by the user")]
        private static IEnumerable<string> GenerateUserCommandList()
        {
            return GetUniqueCommands()
                .Where(x => !x.MethodData.DeclaringType.Assembly.FullName.StartsWith("QFSW.QC"))
                .Select(x => $"   - {x.CommandName}");
        }
    }
}
