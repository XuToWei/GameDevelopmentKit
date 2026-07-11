# GameDevelopmentKit 文档

Book 解释“怎么做、为什么这样做、产物在哪里”。主 README 只负责项目定位和最短上手路径；具体流程以这里的专题文档为准。

## 推荐阅读路线

### 第一次运行

1. [快速开始](快速开始.md)
2. [项目结构与模式选择](Project结构.md)
3. 按所选模式阅读 [UI 开发](UI开发.md) 与 [Entity 开发](Entity开发.md)

### 配置与代码生成

1. [Luban 配置](Luban配置.md)
2. [多语言](多语言.md)
3. [Proto 生成工具](Proto生成工具.md)
4. [ET 代码生成工具](ET代码生成工具.md)

### 热更新与发布

1. [HybridCLR 热更新](HybridCLR热更.md)
2. [一键打包](一键打包.md)

## 文档索引

| 分类 | 文档 | 状态 | 解决的问题 |
| --- | --- | --- | --- |
| 上手 | [快速开始](快速开始.md) | 已实现 | 环境、编译、模式选择与运行 |
| 架构 | [项目结构与模式选择](Project结构.md) | 已实现 | 代码放哪里、模块如何切换 |
| 开发 | [UI 开发](UI开发.md) | 已实现 | GameHot UI 与 ETUI 的完整流程 |
| 开发 | [Entity 开发](Entity开发.md) | 已实现 | GameHot Entity 与 ETEntity 的完整流程 |
| 配置 | [Luban 配置](Luban配置.md) | 已实现 | 多工程并行导出、生成物与排错 |
| 配置 | [多语言](多语言.md) | 已实现 | Excel、运行时加载与编辑器预览 |
| 协议 | [Proto 生成工具](Proto生成工具.md) | 已实现 | ET/UGF 协议代码生成 |
| ET 工具 | [ET 代码生成工具](ET代码生成工具.md) | 已实现 | Component、UIForm、UIWidget、Entity 模板 |
| ET 扩展 | [ET 动态事件](ET动态事件.md) | 已实现 | 按实体类型和 SceneType 广播事件 |
| 编辑器 | [自定义 Toolbar](自定义Toolbar.md) | 已实现 | 向 Unity 主工具栏注册按钮 |
| 构建 | [HybridCLR 热更新](HybridCLR热更.md) | 已实现 | 热更新 DLL 与 AOT 元数据准备 |
| 构建 | [一键打包](一键打包.md) | 已实现 | 资源、安装包和本地资源服务器 |
| 设计 | [动态扩容设计](动态扩容.md) | 未实现 | ET 服务发现、路由与弹性伸缩方案 |

## 阅读约定

- `Unity/...`、`Design/...` 等路径相对仓库根目录。
- `Assets/...` 是 Unity 工程内路径，完整路径需加 `Unity/` 前缀。
- 关键代码表中的 `Game/...`、`Library/...` 相对 `Unity/Assets/Scripts/`。
- 标注“自动生成”的文件只用于核对产物，不应手工修改。
- 菜单名称、编译符号和输出目录以当前代码为准。
- `Code/` 通常可进入 HybridCLR 热更新程序集；`Loader/` 是稳定加载层，不参与业务 DLL 热更新。

## 代码入口

| 领域 | 入口 |
| --- | --- |
| Unity 启动流程 | `Unity/Assets/Scripts/Game/Procedure/` |
| ET 加载 | `Unity/Assets/Scripts/Game/ET/Loader/Init.cs` |
| GameHot 加载 | `Unity/Assets/Scripts/Game/Hot/Loader/Init.cs` |
| 配置导出 | `Share/Tool/ExcelExporter/` |
| Proto 生成 | `Share/Tool/Proto2CS/` |
| 构建工具 | `Unity/Assets/Scripts/Game/Editor/Build/` |
