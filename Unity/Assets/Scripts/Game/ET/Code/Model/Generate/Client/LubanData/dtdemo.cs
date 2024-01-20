namespace ET { public partial class DTDemo {
partial void PostConstructor() { KeyList = new System.Collections.Generic.List<int> {
11, 12, 13, 14, 15, 16, 17, 18, };
}
private bool InternalTryGetValue(int key, out DRDemo value) { switch (key) { default: value = default; return false;
case 11: value = new ET.DRDemo { Id = 11, Key = @"/apple"} ; return true;
case 12: value = new ET.DRDemo { Id = 12, Key = @"/apple"} ; return true;
case 13: value = new ET.DRDemo { Id = 13, Key = @"/apple"} ; return true;
case 14: value = new ET.DRDemo { Id = 14, Key = @"/apple"} ; return true;
case 15: value = new ET.DRDemo { Id = 15, Key = @"/apple"} ; return true;
case 16: value = new ET.DRDemo { Id = 16, Key = @"/apple"} ; return true;
case 17: value = new ET.DRDemo { Id = 17, Key = @"/apple"} ; return true;
case 18: value = new ET.DRDemo { Id = 18, Key = @"/apple_bad"} ; return true;
}}}}