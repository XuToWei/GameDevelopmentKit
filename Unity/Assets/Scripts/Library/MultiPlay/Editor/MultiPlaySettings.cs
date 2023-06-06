using UnityEngine;
using UnityEngine.Serialization;

namespace MultiPlay
{
    //[CreateAssetMenu(menuName = "MultiPlay/Settings")] //Uncomment to create one object to control the global settings.
    internal class MultiPlaySettings : ScriptableObject
    {
        [Range(1, 30)]
        [Tooltip("Maximum number of clients")]
        public int maxNumberOfClones = 3;

        [Tooltip("Default Project Clones Path")]
        public string clonesPath = "../Temp";

        [Tooltip("Enabeling this will increase the project size but will transfer project data like startup scene")]
        public bool linkLibrary = false;
    }
}