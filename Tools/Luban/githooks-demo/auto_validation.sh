#!/bin/bash

GEN_CLIENT=Tools/Luban.ClientServer/Luban.ClientServer.dll

dotnet ${GEN_CLIENT} -t TplsServer -j cfg --generateonly --\
 -d Data/Defines/Root.xml \
 --input_data_dir Data/Excels \
 --output_code_dir C#/All \
 --output_data_dir Json/All \
 --gen_types code_cs_json,data_json2 \
 --naming_convention:bean_member under_scores \
 --cs:use_unity_vector \
 -s all 