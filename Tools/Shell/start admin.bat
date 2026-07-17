cd /d ../../Bin
start http://localhost:5200
dotnet App.dll --AppType=Admin --Process=100002 --StartConfig=Localhost --Console=1
pause
