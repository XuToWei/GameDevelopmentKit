# GameDevelopmentKit的介绍：
客户端以[UnityGameFramework框架（GF）](https://github.com/EllanJiang/UnityGameFramework)为基础，将[ET框架](https://github.com/egametang/ET)子模块化入GF并完善ET的客户端功能，配置工具使用[Luban](https://github.com/focus-creative-games/luban)，努力提供方便开发的常用工具

## 特色
1.以[GFUI](Unity/Assets/Scripts/Game/ET/Loader/UGF/UIForm)为基础的[ETUI](Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Module/UI)

2.以[GFEntity](Unity/Assets/Scripts/Game/ET/Loader/UGF/UIForm)为基础的[ETEntity](Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Module/Entity)

3.[代码绑定工具](Unity/Assets/Scripts/Library/CodeBind/Doc/README.md)，使用简单人性化

4.ET模块化，ET逻辑与GF逻辑，热更和非热更[切换](Book/Project%E7%BB%93%E6%9E%84.md)简单

5.项目全面使用[UniTask](Unity/Assets/Scripts/Library/UniTask)，已替换ETTask，对非ET的部分支持更全面，推荐使用

6.[基于Luban优化过后的导表工具](Book/Luban%E9%85%8D%E7%BD%AE.md)，方便生成多项目多程序集的配置

7.[自定义Toolbar工具](Unity/Assets/Scripts/Library/ToolbarExtender/)

![](Book/png/toolbar.png)

等等

## 运行步骤

1.安装 [.net6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)，服务器功能需要安装[MonogoDB](https://www.mongodb.com/)

2.打开 ET/ET.sln 编译

# 引用库 致谢
[UnityGameFramework](https://github.com/EllanJiang/UnityGameFramework)

[ET](https://github.com/egametang/ET)

[Luban](https://github.com/focus-creative-games/luban)

[UniTask](https://github.com/Cysharp/UniTask)

[UGFExtensions](https://github.com/FingerCaster/UGFExtensions)

