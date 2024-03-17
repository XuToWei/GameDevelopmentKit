# 变体剔除工具

### 1. 开启界面

&emsp;&emsp;菜单栏Tools/Soco/ShaderVariantsStripper/OpenStripperWindow打开窗口。

![](Images/1.%E5%BC%80%E5%90%AF%E7%95%8C%E9%9D%A2.png)

### 2. 创建配置文件

&emsp;&emsp;在Project窗口右键Create/Soco/ShaderVariantsStripper/Create Config创建配置文件，根据需要可以创建若干了，例如我创建了Global（对全局应用）、Effect（特效Shader）、Scene（场景Shader）、Role（角色Shader）、General（其他Shader）。

![](Images/2.%E5%88%9B%E5%BB%BA%E9%85%8D%E7%BD%AE%E6%96%87%E4%BB%B6.png)

### 3. 全局配置

&emsp;&emsp;选择好配置文件后，左侧Global Setting View和Shader View出现内容。点击Global Setting View中的全局设置按钮，右侧会出现全局设置栏，可添加全局条件。

![](Images/3.%E5%85%A8%E5%B1%80%E8%AE%BE%E7%BD%AE.png)

&emsp;&emsp;此工具的设计中，当前配置文件中Shader View没有Shader时，全局设置应用于项目所有Shader；当前配置文件中Shader View有Shader时，全局设置应用于当前配置文件中Shader View中的Shader。如果需要修改逻辑，可以查看ShaderVariantsStripperCode.cs

### 4. Shader配置

&emsp;&emsp;左侧下方可单独对shader配置，首先添加shader，可以用直接选择Shader，然后点添加按钮。

![](Images/4.%E9%80%89%E6%8B%A9Shader1.png)

&emsp;&emsp;为了方便，也可以通过输入ShaderName添加Shader。

![](Images/5.%E6%B7%BB%E5%8A%A0Shader2.png)

&emsp;&emsp;点击Shader后，右侧会出现当前Shader的剔除or保留条件。

### 5. 条件窗口

##### 5.1 条件类型
###### 5.1.1 PassType是

&emsp;&emsp;条件窗口中可选择和添加条件，例如我们选 __“PassType是”__ 这一条件，然后选择添加：

![](Images/6.%E6%B7%BB%E5%8A%A0%E6%9D%A1%E4%BB%B6.png)
![](Images/7.%E6%B7%BB%E5%8A%A0%E6%9D%A1%E4%BB%B62.png)

&emsp;&emsp;在新出现的选项中点击绿色的 __“保留”__ 按钮，变成红色的 __“剔除”__ 按钮，然后点击中间最大的 __“当Pass类型是Normal时”__ 按钮，在弹出的新窗口中，将PassType选择为 __“Deferred”__ 。
![](Images/8.%E4%BF%AE%E6%94%B9%E6%9D%A1%E4%BB%B6.png)

&emsp;&emsp;现在的含义是，当满足pass是Deferred这一条件时，剔除当前变体。

###### 5.1.2 包含Keyword或集合

&emsp;&emsp;可以选择包含或不包含，也可以限定PassType。

&emsp;&emsp;除此外还有其他条件，包含Keyword或集合，例如我们明确打包的物体不需要支持Instance，即可添加一个条件，当包含INSTANCING_ON这一keyword时，直接剔除变体：

![](Images/9.%E5%BD%93%E5%8C%85%E5%90%ABkeyword%E6%97%B6.png)

&emsp;&emsp;点击“当前keyword”下方的按钮可删除keyword。

&emsp;&emsp;此条件可以添加多个Keyword，用于满足同时存在多个Keyword变体的情况，例如如果我们希望“中质量”关键字和“法线纹理”关键字不会同时存在，则将两个关键字都加入到条件中，然后选择剔除。

###### 5.1.3 多个条件

![](Images/13.%E6%B7%BB%E5%8A%A0%E6%9D%A1%E4%BB%B63.png)

&emsp;&emsp;同时满足多个条件才会生效的复合条件；

&emsp;&emsp;比如确定GBufferPass不需要法线压缩，如果将条件分开为：当前Pass是GBufferPass和当前Pass有法线压缩，则无法判断上述情况，所以需要一个复合条件来判断。

&emsp;&emsp;复合条件可以有多个。

##### 5.2 优先级

&emsp;&emsp;如果按照上面说，将项目中包含instance关键字的变体全部剔除，但对于草的shader又希望保留，则利用优先级功能：

![](Images/10.%E4%BC%98%E5%85%88%E7%BA%A7.png)

&emsp;&emsp;剔除/保留按钮后面的数字就是优先级，默认是0，数字越大，优先级越高，优先级为5的shader设置覆盖了优先级为0的全局设置，使变体得以保留。

&emsp;&emsp;优先级只会覆盖相同的条件，而剔除时，只要存在有剔除的条件没有被保留覆盖，就会剔除掉变体。

### 6. 剔除检查

&emsp;&emsp;当配置和条件变得繁杂时，可能无法快速判断一个变体是否会被此工具剔除掉，因此提供这个功能。

&emsp;&emsp;当配置文件没有选中时，配置选择下方会出现 __“剔除检查”__ 按钮，单击后右侧会出现对应选项。

![](Images/12.%E5%89%94%E9%99%A4%E6%A3%80%E6%9F%A5.png)

&emsp;&emsp;选择好Shader、ShaderType、PassType，以及组成变体的Keyword，点击测试剔除，下方会显示所有配置中满足条件的项，点击后可以在工程中定位到Config文件。

### 7. 自定义条件

&emsp;&emsp;此工具的优势就是可拓展性，可以自定义条件。

&emsp;&emsp;需要继承自`ShaderVariantsStripperCondition`接口，实现以下方法：

<hr>

`public bool Completion(Shader shader, ShaderVariantsData data)`

&emsp;&emsp;`Completion`方法用于判断当前变体是否满足条件，传入的`ShaderVariantsData`对象包含当前变体的信息。

`public bool EqualTo(ShaderVariantsStripperCondition other)`

&emsp;&emsp;`EqualTo`方法用于判断两个条件是否相同，用于做优先级覆盖。

`public string Overview()`

&emsp;&emsp;`Overview`方法用于概览条件信息，也就是条件窗口上按钮上显示的字样。

`OnGUI(ShaderVariantsStripperConditionOnGUIContext context)`

&emsp;&emsp;`OnGUI`方法用于点击按钮后出现的窗口如何显示UI。

`public string GetName()`

&emsp;&emsp;`GetName`方法用于，当选择添加条件时，出现的字样。

<hr>

&emsp;&emsp;当实现上述接口后，重新打开窗口，编辑器会自动根据反射信息获取条件，出现在条件选择添加的下拉菜单中。

### 8. 类名、名空间、Assembly修改

&emsp;&emsp;有时开发者希望迁移代码，例如从Assets Package化，可能会修改`ShaderVariantsStripperCondition`及实现类的类名、修改namespace、改变Assembly。由于序列化接口实现对象需要用到`SerializeReference`，当修改这些后，已经编辑好的Config文件可能无法匹配前后的类型（至少Unity2019-2021都是如此），如果在编辑器Project窗口点击Config，甚至会导致编辑器卡死。

&emsp;&emsp;如果你的项目资产是文本类型，可以直接用文本方式编辑资产中记录的类名、namespace、Assembly，如果项目资产是二进制，工具提供配置导出、导入为Json功能：

![](Images/11.%E8%AF%BB%E5%86%99Json%E5%BA%8F%E5%88%97%E5%8C%96%E6%96%87%E4%BB%B6.png)

&emsp;&emsp;序列化Config会将Json序列化数据保存到Config同目录同名Json文件中。

&emsp;&emsp;选择读取路径后，下方会显示当前读取路径，选择读取Json会将Json反序列化到当前Config中，这是覆盖保存，所以注意提前备份好Config。

&emsp;&emsp;对于修改类名等情况，可以提前Json序列化好配置文件，Json中会保存`SerializeReference`对象的类型、名空间、Assembly，可以用修改Json文件中相关名称，然后修改Unity中类型名称，之后从Json文件中反序列化。

### 9. 优化注意事项

&emsp;&emsp;因为找不到Unity Build的事件函数，无法在Build之前只进行一次Config读取，因此每次进入`OnProcessShader`函数时，都会调用`LoadConfigs`，来保证功能不出错；这个方法的逻辑，是从硬盘中读取Config文件，消耗不大，但依旧会对打包时间造成不少影响。

&emsp;&emsp;如果你们有Build Bundle相关代码，可以将`LoadConfigs`从`OnProcessShader`中注释掉，然后在Build Bundle前，在外部调用`ShaderVariantsStripperCode.LoadConfigs()`方法。

&emsp;&emsp;需要注意的是，如果注释掉`LoadConfigs`，会导致工程用Unity默认的File>Build Settings>build不走这一套剔除程序，可以在`OnProcessShader`增加一些判断逻辑，比如`sConfigs`为空时LoadConfigs，这样直接Build不会实时更新config文件，但依旧可以正常走剔除。