@echo off
SETLOCAL
SET TS=%DATE:~3,2%%DATE:~0,2%%DATE:~8,4%%TIME:~0,2%%TIME:~3,2%%TIME:~6,2%
del "..\Blacksmiths.Text.Json\bin\Debug\*.nupkg"
del "bin\Debug\*.nupkg"
dotnet pack "..\Blacksmiths.Text.Json" --version-suffix:dev%TS%
dotnet pack --version-suffix:dev%TS%
pause
REM source should be called "github-blacksmiths" in the future
dotnet nuget push "..\Blacksmiths.Text.Json\bin\Debug\*.nupkg" --skip-duplicate --source "github-wolf"
dotnet nuget push "bin\Debug\*.nupkg" --skip-duplicate --source "github-wolf"
pause