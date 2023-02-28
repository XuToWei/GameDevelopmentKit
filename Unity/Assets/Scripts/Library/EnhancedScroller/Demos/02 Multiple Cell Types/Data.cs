using UnityEngine;
using System.Collections;

namespace EnhancedScrollerDemos.MultipleCellTypesDemo
{
    /// <summary>
    /// This set of classes store information about the different rows.
    /// The base data class has no members, but is useful for polymorphism.
    /// </summary>
    public class Data
    {
    }

    /// <summary>
    /// This is the data that the header rows will use. It only contains a category
    /// field.
    /// </summary>
    public class HeaderData : Data
    {
        /// <summary>
        /// The category of header for the group
        /// </summary>
        public string category;
    }

    /// <summary>
    /// This is the data that will store information about users within a group
    /// </summary>
    public class RowData : Data
    {
        /// <summary>
        /// The name of the user
        /// </summary>
        public string userName;

        /// <summary>
        /// The user avatar's path to the sprite resource
        /// </summary>
        public string userAvatarSpritePath;

        /// <summary>
        /// The user's high score
        /// </summary>
        public ulong userHighScore;
    }

    /// <summary>
    /// This is data for the footer. No actual fields are used in this class,
    /// but we set it up for completeness of the example.
    /// </summary>
    public class FooterData : Data
    {
    }
}