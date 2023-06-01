using System.Threading;

namespace ET.Server
{
    [ChildOf(typeof(RobotCaseComponent))]
    public class RobotCase: Entity, IAwake, IDestroy
    {
        public CancellationTokenSource Cts;
        public string CommandLine;
    }
}