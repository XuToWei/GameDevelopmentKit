using System.Collections.Generic;
using System.Threading;

namespace ET.Server
{
    [ChildOf(typeof(RobotCaseComponent))]
    public class RobotCase: Entity, IAwake, IDestroy
    {
        public CancellationTokenSource CancellationTokenSource;
        public string CommandLine;
        public HashSet<long> Scenes { get; } = new HashSet<long>();
    }
}