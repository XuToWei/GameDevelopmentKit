namespace ET { public partial class DTAIConfig {
partial void PostConstructor() { KeyList = new System.Collections.Generic.List<int> {
101, 102, 201, };
}
private bool InternalTryGetValue(int key, out DRAIConfig value) { switch (key) { default: value = default; return false;
case 101: value = new ET.DRAIConfig { Id = 101, AIConfigId = 1, Order = 1, Name = @"AI_Attack", Desc = @"攻击", NodeParams = new System.Collections.Generic.List<int> { }} ; return true;
case 102: value = new ET.DRAIConfig { Id = 102, AIConfigId = 1, Order = 2, Name = @"AI_XunLuo", Desc = @"巡逻", NodeParams = new System.Collections.Generic.List<int> { }} ; return true;
case 201: value = new ET.DRAIConfig { Id = 201, AIConfigId = 2, Order = 1, Name = @"AI_XunLuo", Desc = @"巡逻", NodeParams = new System.Collections.Generic.List<int> { }} ; return true;
}}}}