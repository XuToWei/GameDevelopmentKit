# luban_examples

更多文档请参见 [Document](https://focus-creative-games.github.io/luban/)

一个示例工程，包含

- Tools
  - Luban.Client
    - Luban.Client 客户端

  - Luban.ClientServer
    - Luban.Server 服务端
    - Luban.ClientServer 一体化程序。免去部属Luban.Server的麻烦

  - [LubanAssistant](https://focus-creative-games.github.io/luban/tools/#lubanassistant) Excel插件

    在Tools/LubanAssistant目录。

    让你可以将数据保存成json文件，但又同时能在excel里编辑。大型项目的利器。 双击LubanAssistant/LubanAssistant.vsto 即可安装。

    目前功能较简略，后续会持续改进。

    ![Excel2TextDiff](docs/images/e_10.jpg)

  - [Excel2TextDiff](https://github.com/focus-creative-games/Excel2TextDiff) 工具

    一个方便diff excel族(xls,xlsx,csv)文件的工具，在Tools/Excel2TextDiff目录。
    将excel文件转为文本文件，然后再调用diff程序，极其方便直观地对比excel文件的变化。非常有用！

    配置方式如图

    ![Excel2TextDiff](docs/images/a_1.jpg)

    使用效果如图

    ![pipeline](docs/images/d_70.jpg)

- DesignerConfigs

    示例游戏配置。

- MiniDesignerConfigsTemplate

  一个最简单的配置模板，用于快速创建一个新的配置项目

- Projects 一些覆盖常见平台、引擎、语言的示例项目

- Projects/CfgValidator

    一个高级自定义配置校验项目，策划提交数据前运行此项目，可以避免提交错误的错误。

    可以补充一些自定义的工具不能支持的高级检查，以单元测试的形式实现。重新完配置后运行 dotnet test -v m 即可检查配置合法性。

- Benchmark 性能测试项目

    [性能测试报告](Benchmark/benchmark.md)

- ProtoProjects

    luban也支持消息生成。这个目录是消息生成示例项目目录。

- githooks-demo

  设置git hooks的示例。策划提交配置前进行合法性验证。

## 准备工作

Projects里的项目都已经过测试，已经包含了生成代码和数据，可以直接运行。

默认Luban.Client使用云生成，需要启动Luban.Server（运行根目录下的run_luban.server.bat即可）。新上手用户，**推荐使用Luban.ClientServer替代Luban.Client**，因为它可以独立运行，不需要部属Luban.Server服务器。

将运行脚本中%LUBAN_CLIENT%变量的Luban.Client/Luban.Client 改为 Luban.ClientServer/Luban.ClientServer，同时删除 -h (--host ) 选项及其参数。

命令行的详细用法，请见 [luban命令工具](https://focus-creative-games.github.io/luban/command_tools/)

## 支持与联系

   发issue不一定能及时看到，有问题请尽量加QQ群: 692890842 （Luban开发交流群） 反馈，往往能快速得到解决。
