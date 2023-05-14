# [HybridCLR](https://github.com/focus-creative-games/hybridclr)

## 热更说明

1. 参考[HybridCLR](https://github.com/focus-creative-games/hybridclr)说明

3. 执行菜单栏Tools/Define Symbol/Add UNITY_HOT开启热更功能

2. 执行菜单栏HybridCLR/Generate/All执行HybridCLR

4. 执行菜单栏HybridCLR/CopyAotDlls拷贝AotDlls到Assets/Res/HybridCLR目录

5. Assets/Res/HybridCLR/HybridCLRConfig.asset的Inspector上点击“自动链接AotDlls”

6. 若有需要，根据Dll的依赖关系来调整AotDll的顺序

7. 由于热更只支持的业务ET或GameHot的业务Dll，需要按需开启Tools/Define Symbol/Add UNITY_ET或Tools/Define Symbol/Add UNITY_GameHot

8. 执行菜单栏ET/Build Tool/BuildHotfixAndModel编译ET的热更dll资源

9. 执行[一键打包功能](../Book/%E4%B8%80%E9%94%AE%E6%89%93%E5%8C%85.md)