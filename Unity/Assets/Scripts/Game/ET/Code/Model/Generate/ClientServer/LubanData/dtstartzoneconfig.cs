namespace ET { public partial class DTStartZoneConfig {
partial void PostConstructor() { KeyList = new System.Collections.Generic.List<( string, int)> {
( @"Benchmark", 1), ( @"Localhost", 1), ( @"Localhost", 2), ( @"Localhost", 3), ( @"Release", 1), ( @"Release", 2), ( @"Release", 3), ( @"RouterTest", 1), ( @"RouterTest", 2), ( @"RouterTest", 3), };
}
private bool InternalTryGetValue(( string, int) key, out DRStartZoneConfig value) { switch (key) { default: value = default; return false;
case ( @"Benchmark", 1): value = new ET.DRStartZoneConfig { StartConfig = @"Benchmark", Id = 1, DBConnection = @"mongodb://127.0.0.1", DBName = @"ET1", Desc = @"游戏区"} ; return true;
case ( @"Localhost", 1): value = new ET.DRStartZoneConfig { StartConfig = @"Localhost", Id = 1, DBConnection = @"mongodb://127.0.0.1", DBName = @"ET1", Desc = @"游戏区"} ; return true;
case ( @"Localhost", 2): value = new ET.DRStartZoneConfig { StartConfig = @"Localhost", Id = 2, DBConnection = @"", DBName = @"", Desc = @"机器人区"} ; return true;
case ( @"Localhost", 3): value = new ET.DRStartZoneConfig { StartConfig = @"Localhost", Id = 3, DBConnection = @"", DBName = @"", Desc = @"路由区"} ; return true;
case ( @"Release", 1): value = new ET.DRStartZoneConfig { StartConfig = @"Release", Id = 1, DBConnection = @"mongodb://127.0.0.1", DBName = @"ET1", Desc = @"游戏区"} ; return true;
case ( @"Release", 2): value = new ET.DRStartZoneConfig { StartConfig = @"Release", Id = 2, DBConnection = @"", DBName = @"", Desc = @"机器人区"} ; return true;
case ( @"Release", 3): value = new ET.DRStartZoneConfig { StartConfig = @"Release", Id = 3, DBConnection = @"", DBName = @"", Desc = @"路由区"} ; return true;
case ( @"RouterTest", 1): value = new ET.DRStartZoneConfig { StartConfig = @"RouterTest", Id = 1, DBConnection = @"mongodb://127.0.0.1", DBName = @"ET1", Desc = @"游戏区"} ; return true;
case ( @"RouterTest", 2): value = new ET.DRStartZoneConfig { StartConfig = @"RouterTest", Id = 2, DBConnection = @"", DBName = @"", Desc = @"机器人区"} ; return true;
case ( @"RouterTest", 3): value = new ET.DRStartZoneConfig { StartConfig = @"RouterTest", Id = 3, DBConnection = @"", DBName = @"", Desc = @"路由区"} ; return true;
}}
}}