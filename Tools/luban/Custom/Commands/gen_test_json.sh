#!/bin/zsh
WORKSPACE=../..

GEN_CLIENT=%WORKSPACE%/Tools/luban/Tools/Luban.ClientServer/Luban.ClientServer.dll
CONF_ROOT=%WORKSPACE%/DevelopConfig/Excel
GEN_TYPES=code_cs_unity_json,data_json

dotnet ${GEN_CLIENT} -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir ${WORKSPACE}/Unity/Assets/Scripts/Hotfix/Model/Generate/Config \
 --output_data_dir ${WORKSPACE}/Unity/Assets/Res/Config/Hotfix \
 --gen_types ${GEN_TYPES} \
 --external:selectors unity_cs \
 -s all
