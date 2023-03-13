using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace QFSW.QC.Suggestors
{
    public class CommandSuggestion : IQcSuggestion
    {
        private readonly CommandData _command;
        private readonly string[] _paramNames;
        private readonly int _numOptionalParams;

        private readonly Dictionary<string, Type[]> _genericSignatureCache = new Dictionary<string, Type[]>();
        private readonly Dictionary<ParameterInfo, IQcSuggestorTag[]> _parameterTagsCache = new Dictionary<ParameterInfo, IQcSuggestorTag[]>();
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        private struct ParsedCommandNameInfo
        {
            public string RawName;
            public string CommandName;
            public string GenericSignature;
            public string[] GenericArgNames;
        }

        private ParsedCommandNameInfo _currentCommandNameCache;

        public string FullSignature => _command.CommandSignature;
        public string PrimarySignature => _command.CommandName;
        public string SecondarySignature { get; }
        public CommandData Command => _command;

        public CommandSuggestion(CommandData command, int numOptionalParams = 0)
        {
            _command = command;
            _paramNames = _command.ParameterSignature.Split(' ');

            _numOptionalParams = numOptionalParams;
            for (int i = _paramNames.Length - _numOptionalParams; i < _paramNames.Length; i++)
            {
                _paramNames[i] = $"[{_paramNames[i]}]";
            }

            SecondarySignature = $"{_command.GenericSignature} {string.Join(" ", _paramNames)}";
        }

        public bool MatchesPrompt(string prompt)
        {
            UpdateCurrentCache(prompt);
            return _currentCommandNameCache.CommandName == _command.CommandName;
        }

        public string GetCompletion(string prompt)
        {
            return _command.CommandName;
        }

        public string GetCompletionTail(string prompt)
        {
            UpdateCurrentCache(prompt);
            _stringBuilder.Clear();

            int numParamsInPrompt = prompt
                .SplitScoped(' ')
                .Count(x => !string.IsNullOrWhiteSpace(x)) - 1;

            // Can't be less than zero params in the prompt
            numParamsInPrompt = Mathf.Max(numParamsInPrompt, 0);

            int numParamsToPrint = _command.ParamCount - numParamsInPrompt;

            if (prompt == _currentCommandNameCache.CommandName)
            {
                _stringBuilder.Append(_command.GenericSignature);
            }

            // Print out the params not in the prompt
            for (int i = 0; i < numParamsToPrint; i++)
            {
                // Add a space only if there's no whitespace trail already
                if (i > 0 || !prompt.EndsWith(" "))
                {
                    _stringBuilder.Append(' ');
                }

                int paramIdx = i + numParamsInPrompt;
                _stringBuilder.Append(_paramNames[paramIdx]);
            }

            return _stringBuilder.ToString();
        }

        public SuggestionContext? GetInnerSuggestionContext(SuggestionContext context)
        {
            UpdateCurrentCache(context.Prompt);

            bool emptyPromptEnd = context.Prompt.EndsWith(" ");
            string[] promptParts = context.Prompt
                .SplitScoped(' ')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            int promptArgs = promptParts.Length - 1;
            if (emptyPromptEnd)
            {
                promptArgs++;
            }

            if (promptArgs <= 0 || promptArgs > _command.ParamCount)
            {
                return null;
            }

            int paramIndex = promptArgs - 1;
            SuggestionContext newContext = context;
            newContext.Depth++;
            newContext.TargetType = GetParameterType(paramIndex);
            newContext.Tags = GetParameterTags(paramIndex);
            newContext.Prompt = emptyPromptEnd
                ? string.Empty
                : promptParts.LastOrDefault();

            return newContext;
        }

        private void UpdateCurrentCache(string prompt)
        {
            string rawName = prompt.SplitScopedFirst(' ');
            if (rawName != _currentCommandNameCache.RawName)
            {
                _currentCommandNameCache = ParseCommandNameInfo(rawName);
            }
        }

        private ParsedCommandNameInfo ParseCommandNameInfo(string rawName)
        {
            string[] commandNameParts = rawName.Split(new[] { '<' }, 2);

            ParsedCommandNameInfo info = new ParsedCommandNameInfo();
            info.RawName = rawName;
            info.CommandName = commandNameParts[0];

            if (_command.IsGeneric)
            {
                info.GenericSignature = commandNameParts.Length > 1 ? $"<{commandNameParts[1]}" : "";
                info.GenericArgNames = info.GenericSignature
                    .ReduceScope('<', '>')
                    .SplitScoped(',');
            }

            return info;
        }

        private Type[] ParseGenericTypes(ParsedCommandNameInfo commandNameInfo)
        {
            return commandNameInfo
                .GenericArgNames
                .Select(QuantumParser.ParseType)
                .ToArray();
        }

        private Type[] GetParameterTypes(ParsedCommandNameInfo commandNameInfo)
        {
            // Return normal types if not generic
            if (!_command.IsGeneric)
            {
                return _command.ParamTypes;
            }

            // Return cached if available
            if (_genericSignatureCache.TryGetValue(commandNameInfo.GenericSignature, out Type[] paramTypes))
            {
                return paramTypes;
            }

            try
            {
                // Build types from generic types
                Type[] genericTypes = ParseGenericTypes(_currentCommandNameCache);
                paramTypes = _command.MakeGenericArguments(genericTypes);
            }
            catch
            {
                // Use normal types if unable to process generics
                paramTypes = _command.ParamTypes;
            }

            return _genericSignatureCache[commandNameInfo.GenericSignature] = paramTypes;
        }

        private Type GetParameterType(int paramIndex)
        {
            Type[] paramTypes = GetParameterTypes(_currentCommandNameCache);
            return paramTypes[paramIndex];
        }

        private IQcSuggestorTag[] GetParameterTags(int paramIndex)
        {
            ParameterInfo parameter = _command.MethodParamData[paramIndex];
            if (_parameterTagsCache.TryGetValue(parameter, out IQcSuggestorTag[] tags))
            {
                return tags;
            }

            return _parameterTagsCache[parameter] =
                parameter
                    .GetCustomAttributes<SuggestorTagAttribute>()
                    .SelectMany(x => x.GetSuggestorTags())
                    .ToArray();
        }
    }
}