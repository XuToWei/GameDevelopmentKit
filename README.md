# GameDevelopmentKit的介绍：
客户端以[UnityGameFramework框架（GF）](https://github.com/EllanJiang/UnityGameFramework)为基础，将[ET框架](https://github.com/egametang/ET)子模块化入GF并完善ET的客户端功能，配置工具使用[Luban](https://github.com/focus-creative-games/luban)，努力提供方便开发工具的缝合怪

## 概要
1.以[GFUI](Unity/Assets/Scripts/Game/ET/Loader/UGF/UIForm)为基础的[ETUI](Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Module/UI)

2.以[GFEntity](Unity/Assets/Scripts/Game/ET/Loader/UGF/UIForm)为基础的[ETEntity](Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Module/Entity)

3.[代码绑定工具](Unity/Assets/Scripts/Library/CodeBind/Doc/README.md)，使用简单人性化！

4.ET模块化，ET逻辑与GF逻辑，热更和非热更[切换](Book/Project%E7%BB%93%E6%9E%84.md)简单

5.项目全面使用[UniTask](Unity/Assets/Scripts/Library/UniTask)，已替换ETTask，对非ET的部分支持更全面，推荐使用

6.[基于Luban优化过后的导表工具](Book/Luban%E9%85%8D%E7%BD%AE.md)，省去大量的bat，sh配置，方便生成多项目多程序集的配置

7.[ET代码生成工具](Book/ET%E4%BB%A3%E7%A0%81%E7%94%9F%E6%88%90%E5%B7%A5%E5%85%B7.md)，可以很方便的生成ETUI和GFEntity的代码

8.[自定义Toolbar工具](Unity/Assets/Scripts/Library/ToolbarExtender/)

![](Book/png/toolbar.png)

9.[ET动态事件](Book/ET%E5%8A%A8%E6%80%81%E4%BA%8B%E4%BB%B6.md)

等等

## 运行步骤

1.安装 [.net6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)，服务器功能需要安装[MonogoDB](https://www.mongodb.com/)

2.打开Unity项目，等待Unity编译完成

3.打开 Kit.sln 编译（导表，代码分析等功能需要用到）

4.点击Unity编辑器运行按钮旁的Launcher按钮即可运行ET的Demo

# TODO && Features

- [X] ET中的Time模块支持Unity编辑器下加速功能（目前是DateTime）
- [X] 加入HotReload
- [X] 资源服务器，资源打包上传配套工具（包含dll，pack等）
- [ ] 多语言工具（包含图片，资源等）
- [X] 完善HybridCLR工作流，编辑器
- [X] 完善非热更时包体优化功能（优化link.xml）
- [X] 带代码定位的Log
- [X] UGui扩展：~~滑动列表（包括ET）~~，UI特效 , 编辑器点击选取UI优化 等等
- [ ] 资源热更后台下载（方便玩家更新）
- [ ] Demo
- [ ] 远程打包，通知，分发等
- [ ] 拆分代码绑定库，方便自取
- [ ] 拆分Luban优化版，方便自取
- [ ] 拆分ET动态事件，方便自取
- [ ] ET的ETTask运行统计编辑器


# 引用库 致谢
[UnityGameFramework](https://github.com/EllanJiang/UnityGameFramework)

[ET](https://github.com/egametang/ET)（版本：[6ec07f8](https://github.com/egametang/ET/commit/6ec07f84e4bc9052707858362eec7b3047cd2b0b)）

[Luban](https://github.com/focus-creative-games/luban)

[UniTask](https://github.com/Cysharp/UniTask)

[UGFExtensions](https://github.com/FingerCaster/UGFExtensions)


# 记录
1.ET的Proto对象添加缓存池，减少GC

2.远程打包
