namespace ET { public partial class DTUnitConfig {
partial void PostConstructor() { KeyList = new System.Collections.Generic.List<int> {
1001, 1002, 1003, 1004, };
}
private bool InternalTryGetValue(int key, out DRUnitConfig value) { switch (key) { default: value = default; return false;
case 1001: value = new ET.DRUnitConfig { Id = 1001, Type = 1, Name = @"米克尔", Desc = @"带有强力攻击技能", Position = 1, Height = 178, Weight = 68} ; return true;
case 1002: value = new ET.DRUnitConfig { Id = 1002, Type = 1, Name = @"米克尔2", Desc = @"带有强力攻击技能2", Position = 2, Height = 278, Weight = 78} ; return true;
case 1003: value = new ET.DRUnitConfig { Id = 1003, Type = 1, Name = @"米克尔3", Desc = @"带有强力攻击技能3", Position = 1, Height = 178, Weight = 68} ; return true;
case 1004: value = new ET.DRUnitConfig { Id = 1004, Type = 1, Name = @"米克尔4", Desc = @"带有强力攻击技能4", Position = 2, Height = 278, Weight = 78} ; return true;
}}}}