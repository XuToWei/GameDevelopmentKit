using System;
using System.Collections;
using UnityEngine;

namespace QFSW.QC.Extras
{
    [AddComponentMenu("")]
    public class CoroutineCommands : MonoBehaviour
    {
        [Command("start-coroutine", "starts the supplied command as a coroutine", MonoTargetType.Singleton)]
        private void StartCoroutineCommand(string coroutineCommand)
        {
            object coroutineReturn = QuantumConsoleProcessor.InvokeCommand(coroutineCommand);
            if (coroutineReturn is IEnumerator)
            {
                StartCoroutine(coroutineReturn as IEnumerator);
            }
            else
            {
                throw new ArgumentException($"{coroutineCommand} is not a coroutine");
            }
        }
    }
}
