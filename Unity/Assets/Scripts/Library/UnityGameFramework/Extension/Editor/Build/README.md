# BuildExtension

对UnityGameFramework 的 打包进行扩展  version 配置生成

# 使用教程

1. 配置UGF打包需要的配置文件 及路径

   配置参考 [使用 AssetBundle 编辑工具 | Game Framework](https://gameframework.cn/uncategorized/使用-assetbundle-编辑器/)  不过由于E大文档年代久远，新版本资源系统有些许不同   `AssetBundleXXXX--> ResourceXXX `  

   [GameFrameworkConfigs](./Editor/GameFrameworkConfigs.cs) 配置UGF 资源配置文件路径

   ```csharp
   public static class GameFrameworkConfigs
   {
       [BuildSettingsConfigPath]
       public static string BuildSettingsConfig = GameFramework.Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Res/Configs/BuildSettings.xml"));
   
       [ResourceCollectionConfigPath]
       public static string ResourceCollectionConfig = GameFramework.Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Res/Configs/ResourceCollection.xml"));
   
       [ResourceEditorConfigPath]
       public static string ResourceEditorConfig = GameFramework.Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Res/Configs/ResourceEditor.xml"));
   
       [ResourceBuilderConfigPath]
       public static string ResourceBuilderConfig = GameFramework.Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Res/Configs/ResourceBuilder.xml"));
   ```

2. 设置打包事件

   Unity 菜单栏 `Game Framework/Resource Tools/Resource Builder`

   `BuildEventHandle` 设置为[UGFExtensions.Build.Editor.BuildEventHandle](./Editor/BuildEventHandle.cs) 

3. 配置VersionInfo部分参数

   1. 在`Res/Configs` 目录下右键 选择`Create/UGFExtensions/VersionInfoEditorData` 创建VersionInfoEditorData

   2. 设置VersionInfoData
      * VersionInfoData配置在VersionInfo List.
      * 添加VersionInfoData   `Key`为 Version 环境名 比如（测试服1,测试服2,正式服,xxx渠道）
        `value` 为VersionInfo 其中需要配置`ServerPath` 不同环境下 资源地址可能不一样 所以需要自己配置
      * `ResourcesVersion` 是根据 `ApplicableGameVersion` 和 `InternalResourceVersion`  替换其中`.`为`_`   例如`1_0_1` 
      * `Platform` 是当前打包平台 
      * 最终的`UpdatePrefixUri` 是根据 前面三项 合并而成 例如`http://127.0.0.1/resources/1_1_1/Windows`
      * VersionInfo 中 `InternalGameVersion` 每次打包自增 也可以自己手动设置后 手动生成version.txt
      * `Active` 是当前处于激活的环境  生成时根据当前激活的配置进行生成。
   3. 两种生成version的模式 

      * `IsGenerateToFullPath`  设置为True 时 

        自动将version.txt 生成到打包界面设置的FullPath下对应平台的目录

      * `IsGenerateToFullPath`  设置为False时

        可以自行选择生成路径 点击生成按钮生成

4. 打包

   打包前如果没有配置 `VersionInfoEditorData` 会自动生成 `VersionInfoEditorData`.  生成位置配置在 [BuildEventHandle](./Editor/BuildEventHandle.cs)  56行
   会自动生成一个名为Normal 的VersionInfoData 可以自行修改。
