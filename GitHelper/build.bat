@echo off
echo ================================
echo Git Helper 本地编译脚本
echo ================================
echo.

cd /d "%~dp0"

echo [1/3] 还原依赖...
dotnet restore
if errorlevel 1 (
    echo 错误：依赖还原失败！
    pause
    exit /b 1
)

echo.
echo [2/3] 编译项目...
dotnet build -c Release
if errorlevel 1 (
    echo 错误：编译失败！
    pause
    exit /b 1
)

echo.
echo [3/3] 发布单文件可执行程序...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
if errorlevel 1 (
    echo 错误：发布失败！
    pause
    exit /b 1
)

echo.
echo ================================
echo 编译成功！
echo ================================
echo.
echo 可执行文件位于:
echo %CD%\bin\Release\net6.0-windows\win-x64\publish\GitHelper.exe
echo.
pause
