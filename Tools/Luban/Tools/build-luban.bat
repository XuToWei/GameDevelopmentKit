@echo off
setlocal
pushd "%~dp0"
dotnet build "../Luban-Extension/src/Luban/Luban.csproj" -c Release -o Luban
set "build_exit_code=%errorlevel%"
popd
exit /b %build_exit_code%
