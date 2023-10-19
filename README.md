# GameDevelopmentKit的介绍：
努力提供完善的双端开发工具

服务端以[ET8.0框架](https://github.com/egametang/ET)为基础

客户端以[UnityGameFramework框架（GF）](https://github.com/EllanJiang/UnityGameFramework)为基础，将ET框架子模块化入GF，完善ET的客户端功能

配置工具使用[Luban](https://github.com/focus-creative-games/luban)

使用[HybridCLR](https://github.com/focus-creative-games/hybridclr)热更新

***

# 细节

1.以[GFUI](Unity/Assets/Scripts/Game/ET/Loader/UGF/UIForm)为基础的[ETUI](Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Module/UI)

2.以[GFEntity](Unity/Assets/Scripts/Game/ET/Loader/UGF/UIForm)为基础的[ETEntity](Unity/Assets/Scripts/Game/ET/Code/ModelView/Client/Module/Entity)

3.使用极其灵活方便的[代码绑定工具](Unity/Assets/Scripts/Library/CodeBind/Doc/README.md)，解决代码与资源映射的最后一公里

4.[模块切换](Book/Project%E7%BB%93%E6%9E%84.md)方便，ET逻辑或GF逻辑，热更或非热更选择随心所欲，当然也可以只用GF

5.项目全面使用[UniTask](https://github.com/Cysharp/UniTask)异步方案，已替换ETTask，对非ET的部分支持更全面，扩展支持了GF，推荐使用

6.[基于Luban优化过后的导表工具](Book/Luban%E9%85%8D%E7%BD%AE.md)，简化Luban使用步骤，可以灵活的修改导出配置，支持多线程导表速度大幅提升

7.完善的[多语言](Book/%E5%A4%9A%E8%AF%AD%E8%A8%80.md)支持，导表自动生成多语言配置，支持编辑器配置和预览

8.完善的[热更新](Book/HybridCLR%E7%83%AD%E6%9B%B4.md)流程和工具支持，基于HybridCLR

9.[Proto生成工具](Book/Proto%E7%94%9F%E6%88%90%E5%B7%A5%E5%85%B7.md)，支持ET和GF两种格式的proto代码生成

10.[ET代码生成工具](Book/ET%E4%BB%A3%E7%A0%81%E7%94%9F%E6%88%90%E5%B7%A5%E5%85%B7.md)，可以很方便的生成ETUI和GFEntity的代码

11.[自定义Toolbar工具](Book/%E8%87%AA%E5%AE%9A%E4%B9%89Toolbar.md)

12.[ET动态事件](Book/ET%E5%8A%A8%E6%80%81%E4%BA%8B%E4%BB%B6.md)

13.[一键打包](Book/%E4%B8%80%E9%94%AE%E6%89%93%E5%8C%85.md)，上传资源服务器，方便开发期间出包测试

***

# 运行步骤

### Unity Editor

- 1.安装 [.net6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)，服务器功能需要安装[MonogoDB](https://www.mongodb.com/)

- 2.打开Unity项目，等待Unity编译完成

- 3.打开 Kit.sln 编译（导表，资源服务器，代码分析等功能需要用到）

- 4.点击Unity编辑器运行按钮旁的Launcher按钮即可运行ET的Demo

### Windows Build

- 1.[代码热更处理](Book/HybridCLR%E7%83%AD%E6%9B%B4.md)

- 2.[一键打包](../Book/%E4%B8%80%E9%94%AE%E6%89%93%E5%8C%85.md)，运行程序即可

***

# TODO && Features

- [X] Demo

***

# 引用库 致谢
[UnityGameFramework](https://github.com/EllanJiang/UnityGameFramework)

[ET](https://github.com/egametang/ET)（版本：[8.0](https://github.com/egametang/ET/commit/451b8bcb0204a4b129abb6f9f5eae27b7229058a)）

[Luban](https://github.com/focus-creative-games/luban)

[UniTask](https://github.com/Cysharp/UniTask)

[UGFExtensions](https://github.com/FingerCaster/UGFExtensions)
