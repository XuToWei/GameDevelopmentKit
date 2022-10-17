set WORKSPACE=..\..\..\..

set GEN_CLIENT=%WORKSPACE%\Tools\luban\Tools\Luban.ClientServer\Luban.ClientServer.exe
set CONF_ROOT=%WORKSPACE%\DevelopConfig\Excel
set GEN_TYPES=code_cs_unity_bin,data_bin
set OUTPUT_CODE_DIR=%WORKSPACE%\Unity\Assets\Scripts\Codes\Model\Generate\
set OUTPUT_DATA_DIR=%WORKSPACE%\Unity\Assets\Re\Luban\

%GEN_CLIENT% --template_search_path %WORKSPACE%\Tools\luban\Custom\Templates\LoadAsync -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %OUTPUT_CODE_DIR%\Client\Luban ^
 --output_data_dir %OUTPUT_DATA_DIR%\Client ^
 --gen_types %GEN_TYPES% ^
 -s client

%GEN_CLIENT% --template_search_path %WORKSPACE%\Tools\luban\Custom\Templates\Load -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %OUTPUT_CODE_DIR%\Server\Luban ^
 --output_data_dir %WORKSPACE%\Config\Excel ^
 --gen_types %GEN_TYPES% ^
 -s server

%GEN_CLIENT% -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %WORKSPACE%\Unity\Assets\Scripts\Editor\Codes\Luban\ ^
 --output_data_dir %WORKSPACE%\Unity\Assets\Res\Editor\Luban ^
 --gen_types %GEN_TYPES% ^
 -s editor

%GEN_CLIENT% --template_search_path %WORKSPACE%\Tools\luban\Custom\Templates\LoadAsync -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %OUTPUT_CODE_DIR%\ClientServer\Luban ^
 --output_data_dir %OUTPUT_DATA_DIR%\ClientServer ^
 --gen_types %GEN_TYPES% ^
 -s clientserver

pause