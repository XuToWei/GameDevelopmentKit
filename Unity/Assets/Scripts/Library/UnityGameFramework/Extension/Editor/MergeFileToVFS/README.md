# MergeFileTOVFS
合并文件到虚拟文件系统 的一个编辑器工具

## 使用方式
![image](https://tvax2.sinaimg.cn/large/e1b1a94bgy1h1kvsvivspj20fo06xwez.jpg)

ObjectForPacking : 需要打包合并的文件 支持拖放 文件或文件夹。

SearchPatterns :  搜索模式。 根据后缀匹配 支持通过 `(,;|)`分割 例如`bytes,txt`

FileSystemFolder : 存储VFS文件的文件夹地址

FileSystemName: 虚拟文件系统名称

Merge:合并

Save: 保存设置到一个ScriptableObject中 之后可以通过ScriptableObject 配置合并

代码设置 : `MergeAssetUtility.Merge` 方法 传入数据 合并。


## 扩展
**PS:当前只支持对TextAsset资源进行合并  其他资源可以自行扩展。**

扩展其他资源需要新增`AssetType`  并实现 `MergeAssetUtility.GetAssetType`  `MergeAssetUtility.Asset2Bytes`   中新增类型的处理
