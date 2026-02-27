@echo off
:: Workaround: DCP does not support Windows directory junctions.
:: Set NUGET_PACKAGES to the real path to avoid junction resolution issues.
set NUGET_PACKAGES=D:\AppData\.nuget\packages
cd /d ../../Share/Aspire
start http://localhost:15088
dotnet run --launch-profile http
pause