using UnityEngine;

namespace QFSW.QC
{
    [CreateAssetMenu(fileName = "Untitled Localization", menuName = "Quantum Console/Localization")]
    public class QuantumLocalization : ScriptableObject
    {
        [SerializeField] public string Loading = "Loading...";
        [SerializeField] public string ExecutingAsyncCommand = "Executing async command...";
        [SerializeField] public string EnterCommand = "Enter Command...";

        [SerializeField] public string CommandError = "Error";
        [SerializeField] public string ConsoleError = "Quantum Processor Error";
        [SerializeField] public string MaxLogSizeExceeded = "Log of size {0} exceeded the maximum log size of {1}";

        [SerializeField]
        [TextArea]
        public string InitializationProgress =
            "Q:\\>Quantum Console Processor is initializing\n" +
            "Q:\\>Table generation under progress\n" +
            "Q:\\>{0} commands have been loaded";

        [SerializeField]
        [TextArea]
        public string InitializationComplete = "Q:\\>Quantum Console Processor ready";
    }
}
