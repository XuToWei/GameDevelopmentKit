using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QFSW.QC.Utilities;
using UnityEngine;

namespace QFSW.QC.Actions
{
    /// <summary>
    /// Give the user a selection of choices which can be made by using the arrow keys and enter key.
    /// </summary>
    /// <typeparam name="T">The type of the choices.</typeparam>
    public class Choice<T> : Composite
    {
        /// <summary>
        /// Configuration for the Choice action.
        /// </summary>
        public struct Config
        {
            public string ItemFormat;
            public string Delimiter;
            public Color SelectedColor;

            public static readonly Config Default = new Config
            {
                ItemFormat = "{0} [{1}]",
                Delimiter = " ",
                SelectedColor = Color.green
            };
        }

        /// <param name="choices">The choices to select between.</param>
        /// <param name="onSelect">Action to invoke when a selection is made.</param>
        public Choice(IEnumerable<T> choices, Action<T> onSelect)
            : this(choices, onSelect, Config.Default)
        { }

        /// <param name="choices">The choices to select between.</param>
        /// <param name="onSelect">Action to invoke when a selection is made.</param>
        /// <param name="config">The configuration to be used.</param>
        public Choice(IEnumerable<T> choices, Action<T> onSelect, Config config)
            : base(Generate(choices, onSelect, config))
        { }

        private static IEnumerator<ICommandAction> Generate(IEnumerable<T> choices, Action<T> onSelect, Config config)
        {
            QuantumConsole console = null;
            StringBuilder builder = new StringBuilder();

            IReadOnlyList<T> choiceList = choices as IReadOnlyList<T> ?? choices.ToList();
            KeyCode key = KeyCode.None;
            int choice = 0;

            yield return new GetContext(ctx => console = ctx.Console);

            ICommandAction DrawRow()
            {
                builder.Clear();
                for (int i = 0; i < choiceList.Count; i++)
                {
                    string item = console.Serialize(choiceList[i]);
                    builder.Append(i == choice
                        ? string.Format(config.ItemFormat, item, 'x').ColorText(config.SelectedColor)
                        : string.Format(config.ItemFormat, item, ' '));

                    if (i != choiceList.Count - 1)
                    {
                        builder.Append(config.Delimiter);
                    }
                }

                return new Value(builder.ToString());
            }

            yield return DrawRow();
            while (key != KeyCode.Return)
            {
                yield return new GetKey(k => key = k);

                switch (key)
                {
                    case KeyCode.LeftArrow: choice--; break;
                    case KeyCode.RightArrow: choice++; break;
                    case KeyCode.DownArrow: choice++; break;
                    case KeyCode.UpArrow: choice--; break;
                }

                choice = (choice + choiceList.Count) % choiceList.Count;
                yield return new RemoveLog();
                yield return DrawRow();
            }

            onSelect(choiceList[choice]);
        }
    }
}