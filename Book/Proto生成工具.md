# [Proto生成工具]

## 说明

1.打开Design/Proto/GenConfig.xml填写生成规则

2.参数说明

  - Open -- 是否启用
  - Proto_File -- proto文件名
  - Start_Opcode -- opcode开始数值
  - Code_Name -- 生成代码的类名
  - Code_Output_Dirs -- 生成代码目录
  - Code_Type -- 生成的代码类型，目前支持了ET，UGF（GF使用）

***

3.执行proto2cs.bat（需要编译Kit.sln）