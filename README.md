# ToDo
Unity能双击定位异常的Log
UI，Entity命名和结构优化
绑定代码优化（去掉自定义生命编辑器，改为同一个方便使用)
接入稳定的HybridCLR
删掉清理ET中没用的客户端模块

# Future
整理修改GF，删除GF中没有模块（或者不暴露）

# GameDevelopmentKit

目前在新项目中开发使用进行优化，后续补上详细的使用说明

大体框架基于ET7二次开发

[客户端部分使用的GF](./Unity/Assets/Scripts/Library/UnityGameFramework)

[表格配置部分使用Luban](./Tools/luban)

[自定义代码绑定工具](./Unity/Assets/Scripts/Editor/UGF/Common/BaseBindInspector)

[GF与ET结合的UI](./Unity/Assets/Scripts/Codes/ModelView/Client/UGF/UI)

# 使用方式
配置表目录在Develop中

打开Unity/Assets/Luncher.unity场景运行

自定义绑定代码MonoBehaviour继承使用BaseBindInspector即可，使用Hybridclr来实现对MonoBehaviour的热更即可

[Hybridclr热更ModelView中的MonoBehaviour](https://focus-creative-games.github.io/hybridclr/monobehaviour/)

# 引用库，致谢
[ET](https://github.com/egametang/ET)

[GF](https://github.com/EllanJiang/UnityGameFramework)

[Luban](https://github.com/focus-creative-games/luban_examples)

[UGFExtensions](https://github.com/FingerCaster/UGFExtensions)
