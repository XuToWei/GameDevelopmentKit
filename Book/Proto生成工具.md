# Proto生成工具

## 说明

1.采用子目录分组生成，可以分别设置导出目录

2.子目录里必须有GenConfig.xml文件才会识别该目录的proto文件生成代码

3.GenConfig.xml参数说明

  - Open -- 是否启用
  - Start_Opcode -- opcode开始数值
  - Code_Name -- 生成代码的类名
  - Code_Output_Dirs -- 生成代码目录，可以填多个
  - Code_Type -- 生成的代码类型，目前支持了ET，UGF（GF使用），需要可以自定义添加扩展
  - Name_Space -- 生成的代码的命名空间

![](png/proto_genconfig.png)

3.执行proto2cs.bat（需要编译Kit.sln）