# GameDevelopmentKit
大体框架基于ET7二次开发
[客户端部分使用的GF](./Unity/Assets/Scripts/Library/UnityGameFramework)
[表格配置部分使用Luban](./Tools/luban)
[自定义代码绑定工具](./Unity/Assets/Scripts/Editor/UGF/Common/BaseBindInspector)

# 使用方式
配置表目录在Develop中
打开Unity/Assets/Luncher.unity场景运行

#自定义代码绑定工具介绍
Unity的MonoBehaviour的Inspector继承自BaseBindInspector即可，使用Hybridclr来实现对MonoBehaviour的热更即可（对ModelView即可）
[Hybridclr热更MonoBehaviour](https://focus-creative-games.github.io/hybridclr/monobehaviour/)

# 引用库，致谢
[ET](https://github.com/egametang/ET)
[GF](https://github.com/EllanJiang/UnityGameFramework)
[Luban](https://github.com/focus-creative-games/luban_examples)
[UGFExtensions](https://github.com/FingerCaster/UGFExtensions)