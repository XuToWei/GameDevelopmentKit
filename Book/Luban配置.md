# [Luban工具](https://focus-creative-games.github.io/luban-doc/#/)

##  1.方便的处理多Luban目录（多Luban目录可以用来区分热更非热更等）
##  2.只用关心配置，不用去处理繁琐的生成命令
##  3.由于生成功能在Share.Tool.csproj中执行可以方便添加定义需求

***

## 使用
1.  打开Kit.sln项目后，编辑整个项目
2.  运行Design/Excel/gen all.bat或Tools/Tool/ExcelExport即可

***

## 注意

1.  Luban的Excel工程目录目录必须是Design/Excel下，对目录命名不做要求

2.  Luban的Excel工程目录中需要有GenConfig.xml文件才能生效，方便多程序集配置使用（热更，非热更等等）

3.  GenConfig.xml配置
   - Open -- 是否开启
   - Output_Code_Dirs -- 输出代码目录，可以填多个
   - Output_Data_Dirs -- 输出配置目录，可以填多个
   - Gen_Type_Code_Data -- 输出代码类型，参考[Luban中的命名参数](https://focus-creative-games.github.io/luban-doc/#/manual/commandtools?id=gen_types-%e5%8f%82%e6%95%b0%e4%bb%8b%e7%bb%8d)
   - Gen_Group -- 分组导出参数，参考[Luban中的group](https://focus-creative-games.github.io/luban-doc/#/manual/generatecodedata?id=%e7%94%9f%e6%88%90%e4%bb%a3%e7%a0%81%e5%92%8c%e6%95%b0%e6%8d%ae)
   - Text_Field_Name -- [本地化](https://focus-creative-games.github.io/luban-doc/#/manual/l10n?id=lubanclient-%e5%91%bd%e4%bb%a4)Localization.xlsx默认的字段名，不填不执行本地化
   - Extra_Command --  补充执行的[命令](https://focus-creative-games.github.io/luban-doc/#/manual/commandtools?id=luban-client-%e4%bd%bf%e7%94%a8%e4%bb%8b%e7%bb%8d)

![](png/luban_genconfig.png)

4.  导表自动生成GFUI和GFEntity的Id代码，类似导表后需要处理的需求可以在Share.Tool.csproj项目中添加

## 支持Reload：
```csharp
Tables.Instance.GetDataTable("TableName").LoadAsync(); 
```