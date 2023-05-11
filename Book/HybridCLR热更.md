# [HybridCLR](https://github.com/focus-creative-games/hybridclr)

## 说明

1. 参考[HybridCLR](https://github.com/focus-creative-games/hybridclr)说明

2. 执行菜单栏HybridCLR/CopyAotDlls，拷贝AotDlls到Assets/Res/HybridCLR目录

3. Assets/Res/HybridCLR/HybridCLRConfig.asset的Inspector上点击“自动链接AotDlls”

4. 更具Dll的依赖关系来调整AotDll的顺序

5. 打开菜单栏Tools/Define Symbol/Add UNITY_HOT开启热更功能

6. 由于热更只支持的业务ET或GameHot的业务Dll，需要按需开启Tools/Define Symbol/Add UNITY_ET或Tools/Define Symbol/Add UNITY_GameHot