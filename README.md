# GameDevelopmentKit的介绍：
一个以[UnityGameFramework框架（GF）](https://github.com/EllanJiang/UnityGameFramework)为基础，将[ET框架](https://github.com/egametang/ET)子模块化入前者，[Luban](https://github.com/focus-creative-games/luban)双端配置工具的一站式开发工具

# 特色
以[GFUI](https://github.com/XuToWei/GameDevelopmentKit/tree/master/Unity/Assets/Scripts/Game/ET/Loader/UGF/UIForm)为基础的[ETUI](https://github.com/XuToWei/GameDevelopmentKit/tree/master/Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Module/UI)

以[GFEntity](https://github.com/XuToWei/GameDevelopmentKit/tree/master/Unity/Assets/Scripts/Game/ET/Loader/UGF/UIForm)为基础的[ETEntity](https://github.com/XuToWei/GameDevelopmentKit/tree/master/Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Module/Entity)

[代码绑定工具](https://github.com/XuToWei/GameDevelopmentKit/tree/master/Unity/Assets/Scripts/Library/CodeBind)

分为两种：
继承MonoBehaviour的绑定：

添加特性MonoCodeBind即可，指定分隔符参数

CSCodeBindMono和ICSCodeBind组合：

CSCodeBindMono保存绑定的数据，ICSCodeBind是非MonoBehaviour代码，因此一般有热更需求时候可以用

命名规则：

变量名_脚本类型名

脚本类型名继承[ICodeBindNameTypeConfig](https://github.com/XuToWei/GameDevelopmentKit/blob/master/Unity/Assets/Scripts/Library/CodeBind/Editor/ICodeBindNameTypeConfig.cs)实现即可，可以定义多个，适用多个程序集，会自动收集

