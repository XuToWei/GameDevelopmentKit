using System.Collections.Generic;
using System.Linq;
using QFSW.QC.Utilities;

namespace QFSW.QC.Suggestors
{
    public struct CollapsedCommand
    {
        public CommandData Command;
        public int NumOptionalParams;

        public CollapsedCommand(CommandData command)
        {
            Command = command;
            NumOptionalParams = 0;
        }
    }

    public class CommandSuggestor : BasicCachedQcSuggestor<CollapsedCommand>
    {
        private readonly Dictionary<string, List<CommandData>> _commandGroups = new Dictionary<string, List<CommandData>>();
        private readonly Stack<CollapsedCommand> _commandCollector = new Stack<CollapsedCommand>();

        protected override bool CanProvideSuggestions(SuggestionContext context, SuggestorOptions options)
        {
            return context.Depth == 0;
        }

        protected override IQcSuggestion ItemToSuggestion(CollapsedCommand collapsedCommand)
        {
            return new CommandSuggestion(collapsedCommand.Command, collapsedCommand.NumOptionalParams);
        }

        protected override IEnumerable<CollapsedCommand> GetItems(SuggestionContext context, SuggestorOptions options)
        {
            string incompleteCommandName =
                context.Prompt
                    .SplitScopedFirst(' ')
                    .SplitFirst('<');

            IEnumerable<CommandData> commands = GetCommands(incompleteCommandName, options);
            return options.CollapseOverloads
                ? CollapseCommands(commands)
                : commands.Select(x => new CollapsedCommand(x));
        }

        public IEnumerable<CommandData> GetCommands(string incompleteCommandName, SuggestorOptions options)
        {
            if (string.IsNullOrWhiteSpace(incompleteCommandName))
            {
                return Enumerable.Empty<CommandData>();
            }

            return QuantumConsoleProcessor.GetAllCommands()
                .Where(command => SuggestorUtilities.IsCompatible(incompleteCommandName, command.CommandName, options));
        }

        protected override bool IsMatch(SuggestionContext context, IQcSuggestion suggestion, SuggestorOptions options)
        {
            // Perform filtering in GetCommands
            return true;
        }

        private IEnumerable<CollapsedCommand> CollapseCommands(IEnumerable<CommandData> commands)
        {
            // Reset the command groups but keep lists around for better memory performance
            foreach (List<CommandData> commandGroup in _commandGroups.Values)
            {
                commandGroup.Clear();
            }

            // Allocate commands to their groups
            foreach (CommandData command in commands)
            {
                if (!_commandGroups.TryGetValue(command.CommandName, out List<CommandData> commandGroup))
                {
                    commandGroup = new List<CommandData>();
                    _commandGroups[command.CommandName] = commandGroup;
                }

                commandGroup.Add(command);
            }

            // For each group, iterate over commands from least to most parameters
            // If the new candidate is the same as the previous candidate + 1 new parameter
            // Then absorb the previous command as an optional argument, otherwise keep both
            foreach (List<CommandData> commandGroup in _commandGroups.Values)
            {
                commandGroup.InsertionSortBy(x => x.ParamCount);
                _commandCollector.Clear();

                foreach (CommandData command in commandGroup)
                {
                    CollapsedCommand newCandidate = new CollapsedCommand(command);
                    if (_commandCollector.Count > 0)
                    {
                        CollapsedCommand prevCandidate = _commandCollector.Peek();
                        CommandData newCommand = newCandidate.Command;
                        CommandData prevCommand = prevCandidate.Command;

                        if (newCommand.ParamCount == prevCommand.ParamCount + 1)
                        {
                            if (newCommand.ParameterSignature.StartsWith(prevCommand.ParameterSignature))
                            {
                                _commandCollector.Pop();
                                newCandidate.NumOptionalParams += 1 + prevCandidate.NumOptionalParams;
                            }
                        }
                    }

                    _commandCollector.Push(newCandidate);
                }

                foreach (CollapsedCommand collapsedCommand in _commandCollector)
                {
                    yield return collapsedCommand;
                }
            }
        }
    }
}