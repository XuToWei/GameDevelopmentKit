#!/bin/bash
WORKSPACE=../..

GEN_CLIENT=${WORKSPACE}/Tools/luban/Tools/Luban.ClientServer/Luban.ClientServer.exe
CONF_ROOT=${WORKSPACE}/Develop/Excel

CUSTOM_TEMPLATE_DIR=${WORKSPACE}/Tools/luban/Custom/Templates
OUTPUT_CODE_DIR=${WORKSPACE}/Unity/Assets/Scripts/Codes/Model/Generate
OUTPUT_DATA_DIR=${WORKSPACE}/Config/Excel
UNITY_OUTPUT_DATA_DIR=${WORKSPACE}/Unity/Assets/Res/Config
GEN_TYPE_CODE_DATA=code_cs_unity_json,data_json
GEN_TYPE_DATA=data_bin

${GEN_CLIENT} --template_search_path ${CUSTOM_TEMPLATE_DIR}/LoadAsync -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir ${OUTPUT_CODE_DIR}/Client/Config \
 --output_data_dir ${OUTPUT_DATA_DIR}/Client \
 --gen_types ${GEN_TYPE_CODE_DATA} \
 -s client

${GEN_CLIENT} --template_search_path ${CUSTOM_TEMPLATE_DIR}/LoadAsync -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_data_dir ${UNITY_OUTPUT_DATA_DIR}/Client \
 --gen_types ${GEN_TYPE_DATA} \
 -s client

${GEN_CLIENT} --template_search_path ${CUSTOM_TEMPLATE_DIR}/Load -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir ${OUTPUT_CODE_DIR}/Server/Config \
 --output_data_dir ${OUTPUT_DATA_DIR}/Server \
 --gen_types ${GEN_TYPE_CODE_DATA} \
 -s server

${GEN_CLIENT} --template_search_path ${CUSTOM_TEMPLATE_DIR}/Load -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_data_dir ${UNITY_OUTPUT_DATA_DIR}/Server \
 --gen_types ${GEN_TYPE_DATA} \
 -s server

${GEN_CLIENT} --template_search_path ${CUSTOM_TEMPLATE_DIR}/LoadAsync -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir ${OUTPUT_CODE_DIR}/ClientServer/Config \
 --output_data_dir ${OUTPUT_DATA_DIR}/ClientServer \
 --gen_types ${GEN_TYPE_CODE_DATA} \
 -s clientserver

${GEN_CLIENT} --template_search_path ${CUSTOM_TEMPLATE_DIR}/LoadAsync -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_data_dir ${UNITY_OUTPUT_DATA_DIR}/ClientServer \
 --gen_types ${GEN_TYPE_DATA} \
 -s clientserver

${GEN_CLIENT} -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_code_dir ${WORKSPACE}/Unity/Assets/Scripts/Editor/Luban \
 --gen_types  code_cs_unity_editor_json \
 -s editor

${GEN_CLIENT} -j cfg --\
 -d ${CONF_ROOT}/Defines/__root__.xml \
 --input_data_dir ${CONF_ROOT}/Datas \
 --output_data_dir ${WORKSPACE}/Unity/Assets/Res/Editor/Luban \
 --gen_types data_json \
 -s editor