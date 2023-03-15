# 代码绑定工具

只需要添加简单的特性或脚本就能自动生成绑定的脚本的代码，方便快捷易用

基于节点命名规则自动生成，支持子节点嵌套（列表使用），自定义命名规则（使用CodeBindNameTypeAttribute（引擎不能修改的代码）和CodeBindNameAttribute（经常修改的代码）来实现规则注入）

说明：

节点名字识别支持模糊匹配，比如需要绑定一个变量名为Self的Transform组件，节点名字Self_Tr就可以识别Tr为Transform

节点名字支持绑定多个不同组件，用分隔符连接起来即可，例如：Self_Transform_Button，Self_Tr_But等

## 1.MonoBehaviour类型：

添加特性[MonoCodeBind](../Runtime/CSCodeBindAttribute.cs)即可，指定分隔符参数（可选）

### 使用方式：
```csharp
[MonoCodeBind('_')]
public partial class TestMono : MonoBehaviour
{

}
```
![](1.png)

按钮Generate Bind Code：生成绑定代码

按钮Generate Serialization：生成绑定数据

## 2.CSCodeBindMono和ICSCodeBind组合(可热更新)：

[CSCodeBindMono](../Runtime/CSCodeBindMono.cs)保存绑定的数据，绑定类实现[ICSCodeBind](../Runtime/ICSCodeBind.cs)（非MonoBehaviour代码），因此一般有热更需求时候可以用

### 使用方式：
![](2.png)

拖入创建的C#脚本到BindScript(-_-!再也不用填写烦人的脚本路径)

```csharp
public partial class TestCS : ICSCodeBind
{
    //something
}
gameObject.GetCSCodeBindObject<TestCS>();//自带对象缓存
```



