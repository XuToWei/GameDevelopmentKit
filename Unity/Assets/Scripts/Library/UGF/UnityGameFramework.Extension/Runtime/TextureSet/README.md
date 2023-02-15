# TextureExtension

加载图片的扩展   (文件系统,网络上,资源系统)

# 本系统的作用

1. 文件系统的作用

   [文件系统 | Game Framework](https://gameframework.cn/document/filesystem/)

2. 加载扩展的作用

   方便的加载功能。

   提供自动卸载机制，上层无需操心。

# 使用教程 

## TextureSetComponent设置

`m_FileSystemMaxFileLength`  文件系统最大文件数量 默认为64 可以自行预估设置一个恰当的值，如果文件数超过最大数量 会扩容为当前的两倍。

`m_InitBufferLength`  通过文件系统加载图片的缓存，用于消除GC。默认64k（1024*64） 如果加载文件大小超过 BufferLength 会扩容为当前的两倍。 

`m_CheckCanReleaseInterval` 配置检查可以释放对象的时间间隔 默认30s检查一次 可以自行在检视面板调整

`m_AutoReleaseInterval` 用于配置对象池自动释放时间间隔  默认60s回收一次 可以自行在检视面板调整

`m_CheckCanReleaseInterval`和`m_AutoReleaseInterval` 配合使用 检查到可回收对象会调用对象池 回收。 对象池到达释放时间会自动清理。
## Texture 的加载 与卸载

加载Texture 通过 `TextureSetComponent` 的`SetTextureByFileSystem` `SetTextureByNetwork` `SetTextureByResources`方法

`SetTexture` 三个方法都需要提供一个 实现了`ISetTexture2dObject` 接口的对象。 

当前扩展中实现了对`RawImage`的扩展 提供RawImage的扩展方法

加载Texture

```csharp
 rawImage.SetTextureByFileSystem("TestTexture.png");
 rawImage.SetTextureByNetwork("http://xxx/xxx.png","TestTexture.png");//第二个参数是将网络图片保存到文件系统中的地址。可以不填写。
 rawImage.SetTextureByResources("Assets/Res/TestTexture.png");
```

卸载Texture由扩展组件卸载。不需要用户手动控制。