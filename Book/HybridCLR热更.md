# [HybridCLR](https://github.com/focus-creative-games/hybridclr)

## 提示
由于热更只支持的业务ET或GameHot的业务Dll，需要按需开启Tools/Define Symbol/Add UNITY_ET或Tools/Define Symbol/Add UNITY_GAMEHOT

## 热更说明，以ET模块为例

1. 参考[HybridCLR](https://github.com/focus-creative-games/hybridclr)说明

2. 执行菜单栏Tools/Define Symbol/Add UNITY_HOT开启热更功能

3. 执行菜单栏Tools/Define Symbol/Add UNITY_ET开启ET模块（或GameHot模块）

4. 执行菜单栏ET/Build Tool/BuildHotfixAndModel编译ET的热更dll资源（或GameHot的热更dll）

5. 执行菜单栏HybridCLR/Generate/All执行HybridCLR

6. 执行菜单栏HybridCLR/CopyAotDlls拷贝AotDlls到Assets/Res/HybridCLR目录

7. 若有需要，根据Dll的依赖关系来调整AotDll的顺序

8. 执行[一键打包功能](../Book/%E4%B8%80%E9%94%AE%E6%89%93%E5%8C%85.md)