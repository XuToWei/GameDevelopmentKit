# 变体收集工具

### 1. 开启菜单

&emsp;&emsp;顶部菜单栏Tools/Soco/ShaderVariantCollectionTools/OpenWindow 开启窗口。
![](..Images/1.开启界面.png)
&emsp;&emsp;**变体收集文件**：Unity原生的变体收集文件，可以对Shader变体产生引用，使变体打入Bundle，可用于预热变体。在填入当前操作变体收集文件前，将显示红色。<br>
&emsp;&emsp;**工具配置文件**：当前工具的配置文件，方便保留材质收集、变体材质过滤等工具的参数，不需要每次打开都重新编写。工具打开时会自动寻找，且如果找不到时，会自动在工具目录下生成默认配置文件，并选择添加(上图的Default ShaderVariantCollection Tool Config)。

### 2. 功能选择

&emsp;&emsp;当变体收集文件不为空时，功能选择菜单启用。<br>
&emsp;&emsp;当前工具暂时支持三类主要功能：**快速浏览、收集工具、批处理工具**。

![](..Images/2.功能选择.png)

&emsp;&emsp;**快速浏览**：浏览变体收集文件的内容；相比Unity原生的变体收集UI，工具的浏览能更快捷的定位shader、pass所拥有的变体，概览变体个数。<br>
&emsp;&emsp;**项目收集工具**：利用项目中材质对变体的引用，收集变体，在过程中过滤材质或变体。<br>
&emsp;&emsp;**批处理工具**：对收集完的变体收集文件批量处理，例如排列组合添加材质不会收集到的multi_compile变体。

### 3. 快速浏览

&emsp;&emsp;点击快速浏览后，次级功能选项会变成Shader View，可显示当前变体收集文件中包含的Shader。<br>
&emsp;&emsp;通过选择项目Shader并点击添加，以及点击下方Shader名称后的减号，可以添加或删除Shader。通过“过滤”中的字符串，可以筛选Shader。<br>
&emsp;&emsp;点击列表中的Shader名称，可以查看收集文件中，Shader拥有哪些变体。变体在右侧浏览窗口，通过PassType分好组。可以通过加号和减少增减变体。

![](..Images/3.快速浏览.png)

### 4. 收集工具

#### 4.1 材质收集器
&emsp;&emsp;点击项目收集工具后，先进入材质收集器列表选项。<br>
&emsp;&emsp;材质收集器是对象，对象的类需要实现`Soco.ShaderVariantsCollection.IMaterialCollection`接口，并实现`AddMaterialBuildDependency`这个方法，将打包所需要的材质添加到`AddMaterialBuildDependency`传入的`List<Material>`中。<br>
&emsp;&emsp;下面是我所实现的事例`MaterialCollection_SceneDependency`，作用是获取所有打包场景依赖的材质：
```C#
namespace Soco.ShaderVariantsCollection
{
    //用于收集所有打包场景依赖的材质
    public class MaterialCollection_SceneDependency : IMaterialCollection
    {
        //是否只收集在EditorBuildSettings中enable的场景
        public bool collectOnlyEnable = true;
        public override void AddMaterialBuildDependency(IList<Material> buildDependencyList)
        {
            var sceneDependencyMaterials = EditorBuildSettings.scenes               //所有场景
                .Where(scene => !collectOnlyEnable || scene.enabled)                //是否enable
                .SelectMany(scene => AssetDatabase.GetDependencies(scene.path))     //获取场景依赖的所有资源
                .Where(dependencyAsset => dependencyAsset.EndsWith(".mat"))         //获取资源中的材质
                .Distinct()                                                         //去重
                .Select(matPath => AssetDatabase.LoadAssetAtPath<Material>(matPath));
            
            buildDependencyList.AddRange(sceneDependencyMaterials);
        }
    }
}
```
&emsp;&emsp;不同项目可按照需要实现接口，比如某些项目用资源表决定有哪些材质会打入包中，就可以实现一个类，专门读取资源表获取资源，然后获取资源引用的材质，或资源本身就是材质，下边是我们项目的实现：<br>
```C#
//用于获取资源表所引用的资源
//注意，这个类并不在Soco.ShaderVariantsCollection名空间下，因为理想中，这个类是依赖于工程，而非工具的功能，所以继承时用到了类包含namespace的全名
public sealed class MaterialCollection_DependRes : Soco.ShaderVariantsCollection.IMaterialCollection
{
    public override void AddMaterialBuildDependency(IList<Material> buildDependencyList)
    {
        //获取资源表中所有资源
        List<string> resList = ResConfigFileEnter.GetConfigFile();
        foreach (string res in resList)
        {
            //如果资源本身是材质，则直接添加到列表中
            if (res.EndsWith(".mat"))
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(res);
                if(mat != null)
                    buildDependencyList.Add(mat);
            }
            //如果不是材质，则找到资源所引用的材质添加到列表中
            else
            {
                foreach (string depRes in AssetDatabase.GetDependencies(res))
                {
                    if (depRes.EndsWith(".mat"))
                    {
                        Material mat = AssetDatabase.LoadAssetAtPath<Material>(depRes);
                        if(mat != null)
                            buildDependencyList.Add(mat);
                    }
                }
            }
        }
    }
}
```
&emsp;&emsp;工具打开时会获取所有实现`IMaterialCollection`接口的类，可以通过点击“添加收集材质回调”按钮，实例化实现类，对象和对象的参数会保存在工具配置文件中。<br>
&emsp;&emsp;“**删除**”按钮可以删除收集器对象，**使用**表示下次点击“**收集材质**”时，是否会用到这个收集器对象。<br>
&emsp;&emsp;下方绘制了实现类的成员，可通过修改数值使对象收集不同的材质，例如我的一个材质收集器实现类，可以收集某些路径下所有材质，成员变量是一个路径，通过修改成员的数值，就可以收集不同路径下的材质。
![](..Images/4.材质收集器列表.png)

#### 4.2 收集变体
&emsp;&emsp;搞定好收集器后，先确认是否要覆盖原有文件，还是在原有文件上添加内容，可通过次级功能选项“**Collection View**”下的复选框修改。<br>
&emsp;&emsp;收集变体分为三步：**收集材质、材质变体转化、写入收集文件**。在次级功能菜单**Collection View**中可以单步运行，也可以直接点击“一键变体收集”运行三步。<br>
&emsp;&emsp;**收集材质**是通过4.1描述的材质收集器，在项目中收集材质。<br>
&emsp;&emsp;**材质变体转化**是将材质存储的shader keyword，转换为一个或多个shader变体。<br>
&emsp;&emsp;**写入收集文件**是将转换得到的变体写入变体收集文件。

#### 4.3 材质、变体过滤
![](..Images/5.变体过滤器列表.png)
&emsp;&emsp;上述收集器会按照规则收集材质，然后转换变体，但有时会得到我们不希望得到的变体；<br>
&emsp;&emsp;例如一个URP管线的项目，肯定不希望得到`Standard`这个Shader实例化出来的材质，如果按照引用规则收集，很多模型默认的内嵌材质会导致收集到。<br>
&emsp;&emsp;或者一个Forward管线也可能收集到`Deferred`或`Meta`这些Pass的变体。<br>
&emsp;&emsp;理所当然的，可以在收集器的逻辑中，屏蔽掉某个Shader的所有材质，但如果这样，需要在每个收集器的逻辑里添加屏蔽代码，需知材质收集器不止一种，屏蔽需求也不止一个，所以我添加了过滤器这一功能，将收集和过滤分离开来。<br>
&emsp;&emsp;和收集器一样，需要实现接口来实现过滤器。材质过滤器需要继承`IMaterialFilter`接口，并实现`Filter`方法：
```C#
public abstract class IMaterialFilter : ScriptableObject
{
    //return true will save and false will strip
    public abstract bool Filter(Material material, List<IPreprocessShaderVariantCollection> collections);
}
```
&emsp;&emsp;方法将传入材质，以及收集到材质的收集器，方法需要返回bool类型参数，返回true这个材质将保留，false则会使材质被剔除。<br>
&emsp;&emsp;类似的，变体的过滤器需要继承`IVariantFilter`接口，并实现`Filter`方法：
```C#
public abstract class IVariantFilter : ScriptableObject
{
    //return true will save and false will strip
    public abstract bool Filter(ShaderVariantCollection.ShaderVariant variant);
}
```
&emsp;&emsp;工具自带实现了两个变体过滤器：可以剔除或只保留指定Shader的`VariantFilter_Shader`，可以剔除指定Pass的`VariantFilter_PassStrip`。<br>
&emsp;&emsp;材质过滤器将在材质收集阶段后应用，变体过滤器将在材质-变体转换阶段后应用。

&emsp;&emsp;注: 不需要的变体自然能通过Unity的变体剔除来剔除，这里的做法只是为了保证变体收集文件的整洁性。

#### 4.4 自带收集器与过滤器使用说明
材质收集器：

① `MaterialCollection_SceneDependency`获取所有在BuildSetting中的场景，所引用的材质。<br>
成员`collectOnlyEnable`指定是否只收集在BuildSetting中打勾的场景。

② `MaterialCollection_TotalMaterial`获取指定目录下的所有材质。<br>
有两种指定文件夹的形式，利用`pathMode`指定；<br>
其一是Asset，可以拖动文件夹到`mFolders`数组中，这样的缺陷是，`Assets`和`Packages`目录无法拖动；<br>
其二是String，可以将路径字符串指定到`mIncludePath`数组中，路径的起始从项目根路径开始，也就是类似`Assets\Res`这种形式。

③ `MaterialCollection_AssignMaterial`指定收集某几个材质<br>
将材质拖到materials数组中即可

变体过滤器：

① `VariantFilter_Shaderp`剔除或只保留指定shader。<br>
将需要操作的Shader指定到`mShaders`数组中，mode指定模式。<br>
当模式为Strip时，将剔除收集到变体中，shader处于`mShaders`数组中的变体。<br>
当模式为OnlyReserveContains时，将只保留shader处于`mShaders`数组中的变体。<br>
只保留模式可以用来只收集某一shader的变体，这样不会减少收集时间，但非覆盖模式下，可以只收集几个Shader，应对需要的场景。

② `VariantFilter_PassStrip`剔除指定Pass。<br>
将需要剔除的PassType指定到`mStripPasses`数组中即可

材质过滤器因没有需求，暂时没提供默认实现类。

### 5. 批处理工具
&emsp;&emsp;收集工具将变体收集、写入到变体文件中后，如果有对变体批量处理的需求，则会用到这一功能。<br>
&emsp;&emsp;通过功能选择的批处理工具按钮会进入到批处理工具页面，当前有三个功能：**批处理执行器列表**、**合并文件**

#### 5.1 批处理执行器列表
&emsp;&emsp;当需要自定义批处理功能时，可按照接口实现执行器。<br>
&emsp;&emsp;这里举例一个使用场景：前面收集器收集到的是所有材质记录的keywords，这个参数是通过Material.EnableKeyword添加，Material.DisableKeyword去除，多数情况下，需要材质来开启关闭的keyword，用shader_feature声明，而雾效、阴影这些全局效果关键字，用multi_compile声明。<br>
&emsp;&emsp;打包时的规则是，获取引用到的所有变体组合的shader_feature部分（材质和变体收集文件会对变体产生引用），然后和shader的multi_compile keyword排列组合；通过材质获取的keyword组合，能保证打包不会丢变体，但由于没有收集、组合multi_compile keyword，会导致无法正确预热。<br>
&emsp;&emsp;此时就需要用批处理工具，将multi_compile部分与已经收集的变体排列组合，并添加到变体收集文件中，使变体正确预热。

&emsp;&emsp;和材质收集器、材质变体过滤器类似，批处理执行器需要实现`IExecutable`接口，并实现`Execute`方法。
```C#
public abstract class IExecutable : ScriptableObject
{
    public abstract void Execute(ShaderVariantCollectionMapper mapper);
}
```
&emsp;&emsp;方法会将当前正在处理的变体收集文件的包装器传入（因为Unity原生的变体收集文件提供的接口太少了），这个包装器类提供了Shader、变体是否存在，以及添加、删除Shader和变体的接口。
![](..Images/7.执行器列表.png)
&emsp;&emsp;执行器的列表中，会多出“执行”和“全部执行”按钮；执行会执行当前执行器，不管执行器的“使用”选项是否勾选；而全部执行会执行所有勾选“使用”的执行器。

#### 5.2 自带执行器使用说明

① `VariantKeywordCombination`<br>
添加keyword声明组，排列组合后，与现有变体再组合，写入到变体收集文件中。<br>
变体声明组就是Shader中的`#pragma multi_compile _ A B`，这样默认keyword`_`和`A`、`B`就是一个声明组。
Shader参数指定需要组合的Shader。<br>
添加keyword声明组可以通过最下面的加号`+`，然后会出现新的一行，包含`+`和`-`，`-`会去掉当前变体声明组，`+`会添加keyword。<br>
添加的keyword会根据选择模式变化，选择模式共3个`Custom`、`Default`、`DeclareStatement`。<br>
当模式为`Custom`时，点击`+`会将选择模式右侧选择的变体添加到当前声明组中。<br>
当模式为`Default`时，点击`+`会将默认keyword`_`（代表全下划线keyword，无论shader中用了几个下划线声明都一样）添加到当前声明组中。<br>
当模式为`DeclareStatement`时，右侧可以输入声明字符串（类似`#pragma multi_compile _ A B`），点击`+`会解析字符串，并将解析出的声明组添加到当前声明组，这一选项方便直接从shader复制语句。<br>
声明字符串只支持`multi_compile`，无论是否有local，支持instance(`multi_compile_instancing`)、fog、particle这些build_in声明，暂不支持`multi_compile_fwdbase`等其他build_in声明，如需要可改代码。<br>
当shader不为空时，可以通过“尝试收集声明组”按钮，读取shader文件尝试解析文件内容，获取声明组，这样能加快设置速度，但需要人工排查是否正确收集，否则很可能收集到不想要的声明组。
![](..Images/8.变体声明组合.png)

② `SocoVariantStripAssociate`<br>
与[Soco变体剔除工具](https://github.com/crossous/SocoTools/tree/main/SocoShaderVariantsStripper)联动，可以利用剔除工具的逻辑，将变体收集文件中，不会打入bundle的变体剔除掉，精简变体收集文件。<br>
因为依赖于变体剔除工具，所以当变体剔除工具不存在时，可以将这个类去掉。

#### 5.3 合并文件
![](..Images/6.合并文件.png)
&emsp;&emsp;作用是将新放入的文件的内容，合并到左侧的变体收集文件中。

#### 5.4 分割文件
![](..Images/9.分割文件.png)
&emsp;&emsp;有时候会有需求将现有文件分割成多份，比如低端设备初次进入游戏，会在预热时卡住一段时间，希望能在预热时显示进度条，此时就会希望分割文件。<br>
&emsp;&emsp;首先设置路径，默认是源文件位置。然后设置每个文件最多多少个变体。<br>
&emsp;&emsp;然后选择切割模式，其一是每个文件固定变体数量。<br>
&emsp;&emsp;选框可以决定分割的最小单位，如果可以分割Shader也可以分割Pass，则最小单位就是变体；否则如果可以分割Shader但不能分割Shader中的Pass，则最小分割单位是Pass；最后就是不可分割Shader，则不会分割Shader。<br>
&emsp;&emsp;第二个切割模式是按照固定文件数量切割，变体均匀分布在每个文件中，这样方便配置资源表。


### 6. 自定义编辑器
&emsp;&emsp;可以发现材质收集器、材质过滤器、变体过滤器、执行器的界面大多相似，因为它们的实现方式都是继承自某个抽象类然后实现抽象方法。<br>
&emsp;&emsp;界面都是绘制各个对象的成员，如果希望自定义编辑器，工具提供了相关抽象。例如上面的执行器自身的定义是这样：
```C#
public class VariantKeywordCombination : IExecutable
{
    //something
}
```
&emsp;&emsp;那么自定义编辑器可以这样定义：
```C#
[ShaderVariantCollectionToolEditor(typeof(VariantKeywordCombination))]
class VariantKeywordCombinationEditor : ShaderVariantCollectionToolEditor
{
    public void OnEnable()
    {
        //something
    }

    public override void OnInspectorGUI()
    {
        //something
    }
}
```
&emsp;&emsp;`ShaderVariantCollectionToolEditor`这个类继承自Unity的`Editor`类，所以相关的事件方法都可以实现，界面打开时会自动寻找有没有实现当前对象类的编辑器，如果没有就绘制类的成员，如果有则按照实现的编辑器代码绘制。