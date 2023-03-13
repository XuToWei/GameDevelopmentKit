using System;
using System.Collections.Generic;
using System.Reflection;

namespace QFSW.QC
{
    /// <summary>
    /// Wraps a dynamic delegate such as Action and Func so that a lambda can be used as a command.
    /// </summary>
    public class LambdaCommandData : CommandData
    {
        private readonly object _lambdaTarget;

        /// <summary>
        /// Constructs the command data from the provided lambda.
        /// </summary>
        /// <param name="lambda">The lambda to create a command from. To work around type system limitation you may need to first store the lambda in a strong delegate type like Action or Func.</param>
        /// <param name="commandName">The name to use for the command.</param>
        /// <param name="commandDescription">The description, if any, for the command.</param>
        public LambdaCommandData(Delegate lambda, string commandName, string commandDescription = "") : base(lambda.Method, new CommandAttribute(commandName, commandDescription, MonoTargetType.Registry))
        {
            _lambdaTarget = lambda.Target;
        }

        protected override IEnumerable<object> GetInvocationTargets(MethodInfo invokingMethod)
        {
            yield return _lambdaTarget;
        }
    }
}