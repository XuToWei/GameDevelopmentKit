set WORKSPACE=..\..

set GEN_CLIENT=%WORKSPACE%\Tools\luban\Tools\Luban.ClientServer\Luban.ClientServer.exe
set CONF_ROOT=%WORKSPACE%\DevelopConfig\Excel
set GEN_TYPES=code_cs_unity_bin,data_bin

%GEN_CLIENT% -j cfg --^
 -d %CONF_ROOT%\Defines\__root__.xml ^
 --input_data_dir %CONF_ROOT%\Datas ^
 --output_code_dir %WORKSPACE%\Unity\Assets\Scripts\Hotfix\Model\Generate\Config ^
 --output_data_dir %WORKSPACE%\Unity\Assets\Res\Config\Hotfix ^
 --gen_types %GEN_TYPES% ^
 --external:selectors unity_cs ^
 -s all

pause