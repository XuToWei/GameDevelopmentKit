# [HybridCLR](https://github.com/focus-creative-games/hybridclr)

## 提示
由于热更只支持的业务ET或GameHot的业务Dll，需要按需开启Game/Define Symbol/Add UNITY_ET或Game/Define Symbol/Add UNITY_GAMEHOT

## 热更说明，以ET模块（Windows平台）为例

1. 参考[HybridCLR](https://github.com/focus-creative-games/hybridclr)说明

2. 执行菜单栏Game/Define Symbol/Add UNITY_HOT开启热更功能

3. 执行菜单栏Game/Define Symbol/Add UNITY_ET开启ET模块（或GameHot模块）

4. 执行菜单栏ET/Build Tool/BuildHotfixAndModel编译ET的热更dll资源（或GameHot的热更dll）

5. 执行菜单栏HybridCLR/Do All执行HybridCLR

6. 若有需要，根据Dll的依赖关系来调整Assets/Res/HybridCLR/HybridCLRConfig.asset中的AotDll的顺序

7. 执行[一键打包功能](../Book/%E4%B8%80%E9%94%AE%E6%89%93%E5%8C%85.md)

## 提示
1. 在桌面平台如果使用ET模块的Demo，CodeMode为Server或ClientServer会有部分服务器读取需要的文件，可以把根目录中的Config文件夹移动到Temp/Pkg下面即可，如果使用移动按需更改为Unity来加载

2. 编辑Assets/link.xml，执行菜单栏Game/Define Symbol/Refresh可以更具link.xml自动优化HybridCLRSetting配置，优化包体