# 模块 12：平台与 SDK 适配

## 概述
平台模块通过**策略接口** `IPlatform` + 平台实现（Editor/Android/iOS）隔离原生 SDK 差异（广告、打点、返回键、设备信息等），业务侧统一走 `GameEntry.Platform`，无需关心当前平台。

## 核心类（`Game/Platform/`）
| 文件 | 职责 |
| --- | --- |
| `IPlatform.cs` | 平台能力接口（见下） |
| `PlatformComponent.cs` | GF 组件（挂 GameEntry），持有当前 IPlatform 实例 |
| `PlatformEditor.cs` | 编辑器/默认实现（空实现或模拟） |
| `PlatformAndroid.cs` | Android 实现（走 AndroidJavaObject 调用原生 SDK） |
| `PlatformIOS.cs` | iOS 实现 |

## IPlatform 接口
```csharp
public interface IPlatform
{
    void Init();                                    // SDK 初始化
    void ShowRewardAd(string tag);                  // 激励广告
    bool CanShowRewardAd();                         // 激励广告是否可播
    void ShowInteractionAd();                       // 插屏广告
    bool BannerAdIsShow();                          // Banner 是否展示
    void ShowBannerAd();                            // Banner 广告
    void OnPressEscape();                           // 返回键（Android）
    void TrackEvent(string eventName, Dictionary<string, object> properties); // 数据打点
    string GetPkgId();                              // 分包 id
    string GetDeviceId();                           // 设备 id
    void AppRate();                                 // App 评分
    bool CanAppRate();                              // 是否可评分
}
```

## 典型用法
```csharp
// 初始化（启动流程中调用）
GameEntry.Platform.Init();

// 激励广告（带 tag 标识场景）
if (GameEntry.Platform.CanShowRewardAd())
    GameEntry.Platform.ShowRewardAd("level_revive");

// 打点
GameEntry.Platform.TrackEvent("level_start", new Dictionary<string, object>
{
    { "level_id", 3 }
});

// Android 返回键统一处理
GameEntry.Platform.OnPressEscape();
```

## 设计要点
- **策略模式**：新增平台只需实现 `IPlatform` 并在 `PlatformComponent` 按平台切换实例，业务零改动。
- 广告能力按渠道实现：激励/插屏/Banner 行为由各平台 SDK 决定，编辑器下为空实现便于调试。
- 分包 id、设备 id 统一从平台层取，避免业务侧分散调用原生接口。
- 与 `GameEntry.Platform` 对接的是 `GameEntry.Game.cs` 中注册的 `PlatformComponent`。
