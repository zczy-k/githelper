# Git Helper - 轻量级 Git 图形界面工具

一个极简的 Windows 桌面应用，提供友好的图形界面来管理 Git 版本控制。

## 功能特性

✅ **自动检测和初始化** Git 仓库  
✅ **查看当前状态** (`git status`)  
✅ **快速提交更改** (`git add . && git commit`)  
✅ **查看提交历史** (`git log`)  
✅ **恢复单个文件**到旧版本  
✅ **回退整个项目**到旧版本  
✨ **自动保存**功能，可设置 2/3/5/10/15 分钟自动提交更改  
✨ **目录选择**功能，可随时切换不同项目，无需复制exe到项目文件夹

## 使用方法

1. 将 `GitHelper.exe` 放在任意位置（例如桌面或工具文件夹）
2. 双击运行 `GitHelper.exe`
3. **选择项目目录**：启动时会自动弹出目录选择对话框，选择你要管理的项目文件夹
4. 如果是新项目，程序会自动询问是否初始化 Git 仓库
5. 使用图形界面进行 Git 操作
6. 需要管理其他项目？点击“选择/切换目录”按钮即可切换

## 本地编译

### 前置要求
- .NET 8.0 SDK 或更高版本
- Windows 操作系统

### 编译步骤

```bash
# 进入项目目录
cd GitHelper

# 还原依赖
dotnet restore

# 编译成单个可执行文件
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# 编译后的文件位于:
# bin/Release/net8.0-windows/win-x64/publish/GitHelper.exe
```

## GitHub Actions 自动编译

本项目已配置 GitHub Actions 工作流，每次推送到 `main` 或 `master` 分支时会自动编译。

编译完成后，可以在 GitHub Actions 页面下载编译好的 `GitHelper.exe`。

## 系统要求

- Windows 10 或更高版本
- 已安装 Git（需要在系统 PATH 中）

## 文件大小

编译后的单文件 exe 大小约为 **50-60 MB**（包含 .NET 运行时）。

如果你的系统已经安装了 .NET 8.0 运行时，可以编译成依赖框架的版本（约 200 KB）：

```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

## 许可证

MIT License
