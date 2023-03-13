using System;
using System.Collections.Generic;

namespace QFSW.QC
{
    /// <summary>
    /// The context to provide suggestions for.
    /// </summary>
    public struct SuggestionContext
    {
        /// <summary>
        /// The depth of the suggestion context.
        /// </summary>
        public int Depth;

        /// <summary>
        /// The prompt to generate suggestions for at this depth.
        /// </summary>
        public string Prompt;

        /// <summary>
        /// If any, a specific type to target when producing suggestions.
        /// </summary>
        public Type TargetType;

        /// <summary>
        /// Any tags added to the suggestion context that may be queried by suggestors.
        /// </summary>
        public IQcSuggestorTag[] Tags;

        /// <summary>
        /// Checks if the specified tag exists.
        /// </summary>
        /// <typeparam name="T">The tag to check.</typeparam>
        /// <returns>If the tag exists.</returns>
        public bool HasTag<T>() where T : IQcSuggestorTag
        {
            if (Tags == null)
            {
                return false;
            }

            // foreach loop has better performance
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (IQcSuggestorTag tag in Tags)
            {
                if (tag is T)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the specified tag from the context.
        /// </summary>
        /// <typeparam name="T">The tag to get from the context.</typeparam>
        /// <returns>The tag if it could be found, otherwise a KeyNotFoundException exception is thrown.</returns>
        public T GetTag<T>() where T : IQcSuggestorTag
        {
            if (Tags != null)
            {
                foreach (IQcSuggestorTag tag in Tags)
                {
                    if (tag is T foundTag)
                    {
                        return foundTag;
                    }
                }
            }

            throw new KeyNotFoundException($"No tags of type {typeof(T)} could be found.");
        }
    }
}