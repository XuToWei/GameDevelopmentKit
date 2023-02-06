# Luban工具

## 方便的处理多Luban工程，只用关心配置，不用去处理繁琐的生成命令
#
1.  Luban的Excel工程目录目录必须是Design/Excel下，对目录命名不做要求

2.  Luban的Excel工程目录中需要有GenConfig.xml文件才能生效，方便多程序集配置使用（热更，非热更等等）

3.  GenConfig.xml配置
   - Output_Code_Dirs        输出代码目录，可以填多个
   - Output_Data_Dirs        输出配置目录，可以填多个
   - Gen_Type_Code_Data      输出代码类型，参考[Luban中的命名参数](https://focus-creative-games.github.io/luban/command_tools/#gen-types-%E5%8F%82%E6%95%B0%E4%BB%8B%E7%BB%8D)
   - Gen_Group               分组导出参数，参考[Luban中的group](https://focus-creative-games.github.io/luban/define/#group)

## 支持单个表的Reload
#
1.  调用 Tables.Instance.GetDataTable("TableName").LoadAsync(); 即可