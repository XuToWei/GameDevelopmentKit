#!/bin/bash
set -euo pipefail
cd -- "$(dirname -- "${BASH_SOURCE[0]}")"
dotnet build ../Luban-Extension/src/Luban/Luban.csproj -c Release -o Luban
