namespace ET { public partial class DTStartSceneConfig {
partial void PostConstructor() { KeyList = new System.Collections.Generic.List<( string, int)> {
( @"Benchmark", 1), ( @"Benchmark", 2), ( @"Benchmark", 3), ( @"Benchmark", 4), ( @"Benchmark", 5), ( @"Benchmark", 6), ( @"Benchmark", 7), ( @"Localhost", 1), ( @"Localhost", 2), ( @"Localhost", 3), ( @"Localhost", 4), ( @"Localhost", 5), ( @"Localhost", 6), ( @"Localhost", 7), ( @"Localhost", 300), ( @"Localhost", 301), ( @"Localhost", 302), ( @"Localhost", 303), ( @"Localhost", 304), ( @"Release", 1), ( @"Release", 2), ( @"Release", 3), ( @"Release", 4), ( @"Release", 5), ( @"Release", 6), ( @"Release", 7), ( @"Release", 300), ( @"Release", 301), ( @"Release", 302), ( @"Release", 303), ( @"Release", 304), ( @"RouterTest", 1), ( @"RouterTest", 2), ( @"RouterTest", 3), ( @"RouterTest", 4), ( @"RouterTest", 5), ( @"RouterTest", 6), ( @"RouterTest", 400), ( @"RouterTest", 401), ( @"RouterTest", 402), ( @"RouterTest", 403), ( @"RouterTest", 404), };
}
private bool InternalTryGetValue(( string, int) key, out DRStartSceneConfig value) { switch (key) { default: value = default; return false;
case ( @"Benchmark", 1): value = new ET.DRStartSceneConfig { StartConfig = @"Benchmark", Id = 1, Process = 1, Zone = 1, SceneType = @"BenchmarkServer", Name = @"BenchmarkServer", Port = 10001} ; return true;
case ( @"Benchmark", 2): value = new ET.DRStartSceneConfig { StartConfig = @"Benchmark", Id = 2, Process = 2, Zone = 1, SceneType = @"BenchmarkClient", Name = @"BenchmarkClient1", Port = 0} ; return true;
case ( @"Benchmark", 3): value = new ET.DRStartSceneConfig { StartConfig = @"Benchmark", Id = 3, Process = 3, Zone = 1, SceneType = @"BenchmarkClient", Name = @"BenchmarkClient2", Port = 0} ; return true;
case ( @"Benchmark", 4): value = new ET.DRStartSceneConfig { StartConfig = @"Benchmark", Id = 4, Process = 4, Zone = 1, SceneType = @"BenchmarkClient", Name = @"BenchmarkClient3", Port = 0} ; return true;
case ( @"Benchmark", 5): value = new ET.DRStartSceneConfig { StartConfig = @"Benchmark", Id = 5, Process = 5, Zone = 1, SceneType = @"BenchmarkClient", Name = @"BenchmarkClient4", Port = 0} ; return true;
case ( @"Benchmark", 6): value = new ET.DRStartSceneConfig { StartConfig = @"Benchmark", Id = 6, Process = 6, Zone = 1, SceneType = @"BenchmarkClient", Name = @"BenchmarkClient5", Port = 0} ; return true;
case ( @"Benchmark", 7): value = new ET.DRStartSceneConfig { StartConfig = @"Benchmark", Id = 7, Process = 7, Zone = 1, SceneType = @"BenchmarkClient", Name = @"BenchmarkClient6", Port = 0} ; return true;
case ( @"Localhost", 1): value = new ET.DRStartSceneConfig { StartConfig = @"Localhost", Id = 1, Process = 1, Zone = 1, SceneType = @"Realm", Name = @"Realm", Port = 30002} ; return true;
case ( @"Localhost", 2): value = new ET.DRStartSceneConfig { StartConfig = @"Localhost", Id = 2, Process = 1, Zone = 1, SceneType = @"Gate", Name = @"Gate1", Port = 30003} ; return true;
case ( @"Localhost", 3): value = new ET.DRStartSceneConfig { StartConfig = @"Localhost", Id = 3, Process = 1, Zone = 1, SceneType = @"Gate", Name = @"Gate2", Port = 30004} ; return true;
case ( @"Localhost", 4): value = new ET.DRStartSceneConfig { StartConfig = @"Localhost", Id = 4, Process = 1, Zone = 1, SceneType = @"Location", Name = @"Location", Port = 0} ; return true;
case ( @"Localhost", 5): value = new ET.DRStartSceneConfig { StartConfig = @"Localhost", Id = 5, Process = 1, Zone = 1, SceneType = @"Match", Name = @"Match", Port = 0} ; return true;
case ( @"Localhost", 6): value = new ET.DRStartSceneConfig { StartConfig = @"Localhost", Id = 6, Process = 1, Zone = 1, SceneType = @"Map", Name = @"Map1", Port = 0} ; return true;
case ( @"Localhost", 7): value = new ET.DRStartSceneConfig { StartConfig = @"Localhost", Id = 7, Process = 1, Zone = 1, SceneType = @"Map", Name = @"Map2", Port = 0} ; return true;
case ( @"Localhost", 300): value = new ET.DRStartSceneConfig { StartConfig = @"Localhost", Id = 300, Process = 1, Zone = 3, SceneType = @"RouterManager", Name = @"RouterManager", Port = 30300} ; return true;
case ( @"Localhost", 301): value = new ET.DRStartSceneConfig { StartConfig = @"Localhost", Id = 301, Process = 1, Zone = 3, SceneType = @"Router", Name = @"Router01", Port = 30301} ; return true;
case ( @"Localhost", 302): value = new ET.DRStartSceneConfig { StartConfig = @"Localhost", Id = 302, Process = 1, Zone = 3, SceneType = @"Router", Name = @"Router02", Port = 30302} ; return true;
case ( @"Localhost", 303): value = new ET.DRStartSceneConfig { StartConfig = @"Localhost", Id = 303, Process = 1, Zone = 3, SceneType = @"Router", Name = @"Router03", Port = 30303} ; return true;
case ( @"Localhost", 304): value = new ET.DRStartSceneConfig { StartConfig = @"Localhost", Id = 304, Process = 1, Zone = 3, SceneType = @"Router", Name = @"Router04", Port = 30304} ; return true;
case ( @"Release", 1): value = new ET.DRStartSceneConfig { StartConfig = @"Release", Id = 1, Process = 1, Zone = 1, SceneType = @"Realm", Name = @"Realm", Port = 30002} ; return true;
case ( @"Release", 2): value = new ET.DRStartSceneConfig { StartConfig = @"Release", Id = 2, Process = 1, Zone = 1, SceneType = @"Gate", Name = @"Gate1", Port = 30003} ; return true;
case ( @"Release", 3): value = new ET.DRStartSceneConfig { StartConfig = @"Release", Id = 3, Process = 1, Zone = 1, SceneType = @"Gate", Name = @"Gate2", Port = 30004} ; return true;
case ( @"Release", 4): value = new ET.DRStartSceneConfig { StartConfig = @"Release", Id = 4, Process = 1, Zone = 1, SceneType = @"Location", Name = @"Location", Port = 0} ; return true;
case ( @"Release", 5): value = new ET.DRStartSceneConfig { StartConfig = @"Release", Id = 5, Process = 1, Zone = 1, SceneType = @"Match", Name = @"Match", Port = 0} ; return true;
case ( @"Release", 6): value = new ET.DRStartSceneConfig { StartConfig = @"Release", Id = 6, Process = 1, Zone = 1, SceneType = @"Map", Name = @"Map1", Port = 0} ; return true;
case ( @"Release", 7): value = new ET.DRStartSceneConfig { StartConfig = @"Release", Id = 7, Process = 1, Zone = 1, SceneType = @"Map", Name = @"Map2", Port = 0} ; return true;
case ( @"Release", 300): value = new ET.DRStartSceneConfig { StartConfig = @"Release", Id = 300, Process = 1, Zone = 3, SceneType = @"RouterManager", Name = @"RouterManager", Port = 30300} ; return true;
case ( @"Release", 301): value = new ET.DRStartSceneConfig { StartConfig = @"Release", Id = 301, Process = 1, Zone = 3, SceneType = @"Router", Name = @"Router01", Port = 30301} ; return true;
case ( @"Release", 302): value = new ET.DRStartSceneConfig { StartConfig = @"Release", Id = 302, Process = 1, Zone = 3, SceneType = @"Router", Name = @"Router02", Port = 30302} ; return true;
case ( @"Release", 303): value = new ET.DRStartSceneConfig { StartConfig = @"Release", Id = 303, Process = 1, Zone = 3, SceneType = @"Router", Name = @"Router03", Port = 30303} ; return true;
case ( @"Release", 304): value = new ET.DRStartSceneConfig { StartConfig = @"Release", Id = 304, Process = 1, Zone = 3, SceneType = @"Router", Name = @"Router04", Port = 30304} ; return true;
case ( @"RouterTest", 1): value = new ET.DRStartSceneConfig { StartConfig = @"RouterTest", Id = 1, Process = 1, Zone = 1, SceneType = @"Realm", Name = @"Realm", Port = 30002} ; return true;
case ( @"RouterTest", 2): value = new ET.DRStartSceneConfig { StartConfig = @"RouterTest", Id = 2, Process = 1, Zone = 1, SceneType = @"Gate", Name = @"Gate1", Port = 30003} ; return true;
case ( @"RouterTest", 3): value = new ET.DRStartSceneConfig { StartConfig = @"RouterTest", Id = 3, Process = 1, Zone = 1, SceneType = @"Gate", Name = @"Gate2", Port = 30004} ; return true;
case ( @"RouterTest", 4): value = new ET.DRStartSceneConfig { StartConfig = @"RouterTest", Id = 4, Process = 1, Zone = 1, SceneType = @"Location", Name = @"Location", Port = 0} ; return true;
case ( @"RouterTest", 5): value = new ET.DRStartSceneConfig { StartConfig = @"RouterTest", Id = 5, Process = 1, Zone = 1, SceneType = @"Map", Name = @"Map1", Port = 0} ; return true;
case ( @"RouterTest", 6): value = new ET.DRStartSceneConfig { StartConfig = @"RouterTest", Id = 6, Process = 1, Zone = 1, SceneType = @"Map", Name = @"Map2", Port = 0} ; return true;
case ( @"RouterTest", 400): value = new ET.DRStartSceneConfig { StartConfig = @"RouterTest", Id = 400, Process = 3, Zone = 3, SceneType = @"RouterManager", Name = @"RouterManager", Port = 30300} ; return true;
case ( @"RouterTest", 401): value = new ET.DRStartSceneConfig { StartConfig = @"RouterTest", Id = 401, Process = 4, Zone = 3, SceneType = @"Router", Name = @"Router01", Port = 30301} ; return true;
case ( @"RouterTest", 402): value = new ET.DRStartSceneConfig { StartConfig = @"RouterTest", Id = 402, Process = 5, Zone = 3, SceneType = @"Router", Name = @"Router02", Port = 30302} ; return true;
case ( @"RouterTest", 403): value = new ET.DRStartSceneConfig { StartConfig = @"RouterTest", Id = 403, Process = 6, Zone = 3, SceneType = @"Router", Name = @"Router03", Port = 30303} ; return true;
case ( @"RouterTest", 404): value = new ET.DRStartSceneConfig { StartConfig = @"RouterTest", Id = 404, Process = 7, Zone = 3, SceneType = @"Router", Name = @"Router04", Port = 30304} ; return true;
}}
}}