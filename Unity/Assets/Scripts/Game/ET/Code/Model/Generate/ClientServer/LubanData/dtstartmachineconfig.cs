namespace ET { public partial class DTStartMachineConfig {
partial void PostConstructor() { KeyList = new System.Collections.Generic.List<( string, int)> {
( @"Benchmark", 1), ( @"Localhost", 1), ( @"Release", 1), ( @"RouterTest", 1), };
}
private bool InternalTryGetValue(( string, int) key, out DRStartMachineConfig value) { switch (key) { default: value = default; return false;
case ( @"Benchmark", 1): value = new ET.DRStartMachineConfig { StartConfig = @"Benchmark", Id = 1, InnerIP = @"127.0.0.1", OuterIP = @"127.0.0.1", WatcherPort = @"10000"} ; return true;
case ( @"Localhost", 1): value = new ET.DRStartMachineConfig { StartConfig = @"Localhost", Id = 1, InnerIP = @"127.0.0.1", OuterIP = @"127.0.0.1", WatcherPort = @"10000"} ; return true;
case ( @"Release", 1): value = new ET.DRStartMachineConfig { StartConfig = @"Release", Id = 1, InnerIP = @"127.0.0.1", OuterIP = @"127.0.0.1", WatcherPort = @"10000"} ; return true;
case ( @"RouterTest", 1): value = new ET.DRStartMachineConfig { StartConfig = @"RouterTest", Id = 1, InnerIP = @"127.0.0.1", OuterIP = @"127.0.0.1", WatcherPort = @"10000"} ; return true;
}}
}}