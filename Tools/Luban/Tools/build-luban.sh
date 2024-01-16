#!/bin/bash

[ -d Luban ] && rm -rf Luban

dotnet build  ../Luban-Extension/src/Luban/Luban.csproj -c Release -o Luban