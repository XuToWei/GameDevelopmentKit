@echo off
setlocal
cd /d "%~dp0..\..\Share\Aspire" || exit /b 1
dotnet run --launch-profile http
pause
endlocal
