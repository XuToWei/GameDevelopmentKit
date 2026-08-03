# 模块 25：Recast 寻路链路

## 概述
GDK 使用 **Recast/Detour** 实现寻路，数据从 Unity 侧导出、服务端计算、客户端消费：Unity 从场景 NavMesh 导出 `.bin` → 配置到 `Config/Recast` → 原生库（RecastDll）解析 → 服务端 `RecastFileReader` 读取 → 服务端 Pathfinding 计算路径 → 发消息给客户端执行移动。

## 完整链路
```
1. 导出（Unity 编辑器）
   Unity/Assets/.../ThirdParty/Editor/RecastNavDataExporter/NavMeshExporter.cs
   └─ 从 Unity NavMesh 导出寻路数据（参考 Tools/RecastNavExportor/RecastDemo.exe + solo_navmesh.bin）

2. 配置存放
   Config/Recast/Map1、Map2（.bin 数据，当前为空目录）

3. 原生库（Share/Libs）
   RecastDll/（C++ CMake + make_*.sh/bat 多平台构建）
   Kcp/（ikcp.c，网络传输用，非寻路）

4. 服务端读取
   DotNet/Loader/RecastFileReader.cs（Invoke 读 ../Config/Recast）

5. 服务端计算
   NavmeshComponent（加载缓存字节）
   PathfindingComponent（模型）+ PathfindingComponentSystem（DtMeshSetReader 解析、FindNearestPoly/FindPath/FindStraightPath）
   C2M_PathfindingResultHandler（服务端处理客户端寻路请求）

6. 客户端执行
   M2C_PathfindingResultHandler → 客户端 MoveToAsync 移动
```

## 关键文件速查
| 环节 | 文件 |
| --- | --- |
| Unity 导出 | `Unity/Assets/.../ThirdParty/Editor/RecastNavDataExporter/NavMeshExporter.cs` |
| 参考 demo | `Tools/RecastNavExportor/`（RecastDemo.exe、solo_navmesh.bin） |
| 数据配置 | `Config/Recast/Map1、Map2` |
| 原生库 | `Share/Libs/RecastDll/`（CMake + 多平台脚本）、`Share/Libs/Kcp/` |
| 服务端读取 | `DotNet/Loader/RecastFileReader.cs` |
| 网格组件 | `Library/ET/Core/Runtime/Fiber/Module/Navmesh/NavmeshComponent.cs` |
| 寻路模型 | `Unity/.../Hotfix/Share/Module/Recast/PathfindingComponentSystem.cs`（FindNearestPoly/FindPath/FindStraightPath） |
| 客户端消息 | `M2C_PathfindingResultHandler`（MoveToAsync） |

## 设计要点
- 寻路计算在**服务端**完成（权威性，配合帧同步/防作弊），客户端只负责表现移动。
- RecastDll 为 C++ 原生库，需按平台编译（`make_*.sh/bat`）；Kcp 为服务端网络传输用（见模块 21）。
- `Config/Recast` 当前为空目录：地图数据需先用 Unity 导出工具生成。
- 客户端 MoveToAsync 与服务端路径结果通过 `C2M/M2C_PathfindingResult` 消息闭环（见模块 18 协议生成）。

## 官方文档
- `Book/Project结构.md`（Tools/Config 职责）
- `Book/动态扩容.md`（服务端拓扑相关，非寻路）
