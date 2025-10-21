# Git 快捷助手 (GitHelper)

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)](https://www.microsoft.com/windows)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)

> 一款简单易用的 Windows 桌面 Git 版本管理工具，专为个人项目和日常代码管理设计。

## ✨ 特性

- 🎯 **简单易用** - 无需学习复杂的 Git 命令，点击按钮即可完成操作
- 📁 **智能目录管理** - 自动记住最近使用的 5 个项目，快速切换
- ⏰ **自动保存** - 支持定时自动提交代码变更（2/3/5/10/15 分钟可选）
- 🔔 **系统托盘支持** - 可最小化到托盘后台运行，配合自动保存使用更佳
- 💾 **智能检测** - 自动保存只在有文件变化时才执行，节省空间
- 📝 **完整历史** - 所有提交历史永久保存，随时查看和回溯
- 🚀 **轻量高效** - 单文件运行，占用资源少

## 📸 功能展示

### 主界面功能
- **查看当前状态** - 快速了解文件改动情况
- **保存所有更改** - 一键添加并提交所有修改
- **查看提交历史** - 图形化显示提交记录
- **恢复单个文件** - 将文件恢复到指定版本
- **回退到旧版本** - 整个项目回退（危险操作）

### 目录管理
- 启动时自动加载上次使用的项目
- 下拉菜单快速切换历史项目
- 浏览选择新项目文件夹

### 自动保存
- 智能检测文件变化，只在有改动时保存
- 可自定义时间间隔
- 后台静默运行，不打扰工作

## 🚀 快速开始

### 系统要求

- Windows 10/11 (64-bit)
- .NET 8.0 Runtime（程序自带，无需额外安装）

### 安装

1. 从 [Releases](https://github.com/zczy-k/githelper/releases) 页面下载最新版本
2. 解压到任意目录
3. 双击 `GitHelper.exe` 运行

### 首次使用

1. **启动程序** → 选择要管理的项目文件夹
2. 如果该文件夹未初始化 Git → 点击"是"自动初始化
3. 完成！开始使用

## 📖 使用指南

### 基础操作

#### 手动保存代码
```
1. 修改代码文件
2. 点击 "2. 保存所有更改"
3. 输入提交说明（描述本次修改内容）
4. 点击 "确定"
```

#### 查看提交历史
```
点击 "3. 查看提交历史" → 查看所有提交记录和图形分支
```

#### 切换项目
```
方式1：点击 "▼ 最近目录" → 从历史记录中选择
方式2：点击 "浏览新目录" → 选择新的项目文件夹
```

### 高级功能

#### 启用自动保存
```
1. 勾选 "自动保存 - 启用"
2. 选择时间间隔（建议 5 分钟）
3. 专心写代码，程序会自动保存变更
```

#### 恢复误删/修改的文件
```
1. 点击 "3. 查看提交历史"，复制目标版本的 commit 哈希值
2. 点击 "4. 恢复单个文件"
3. 输入 commit 哈希值和文件路径
4. 确认恢复
```

#### 系统托盘使用
- **最小化到托盘** - 点击关闭按钮(X)选择"最小化到托盘"，或直接点击最小化按钮
- **恢复窗口** - 双击托盘图标
- **快捷菜单** - 右键托盘图标 → 显示窗口/退出

## 💡 使用建议

### ✅ 适合场景
- 个人项目代码管理
- 学习笔记版本控制
- 配置文件自动备份
- 文档写作历史保存
- 小型团队本地开发

### ✅ 最佳实践
- **重要节点手动保存** - 完成一个功能后手动保存并写清楚说明
- **日常开启自动保存** - 建议设置 5 分钟间隔，防止意外丢失代码
- **定期查看历史** - 了解项目进展，必要时可回溯
- **切换项目关闭自动保存** - 避免误操作

### ⚠️ 注意事项
- ⚠️ "回退到旧版本" 是**危险操作**，会永久删除后续所有提交
- 💡 自动保存会生成较多提交记录，但 Git 采用增量存储，实际占用空间很小
- 📌 本工具只管理**本地 Git 仓库**，不涉及远程仓库（GitHub/GitLab）推送
- 🔒 所有数据保存在项目的 `.git` 文件夹中，删除项目即删除历史

## 📁 数据存储说明

| 数据类型 | 存储位置 | 说明 |
|---------|---------|------|
| Git 提交历史 | `项目目录/.git/` | 永久保存，随项目移动 |
| 应用配置 | `程序目录/config.json` | 保存最近使用的目录 |
| 图标资源 | `程序目录/Resources/icon.ico` | 应用图标文件 |

## 🛠️ 技术栈

- **开发语言**: C# 
- **框架**: .NET 8.0 (Windows Forms)
- **版本控制**: Git CLI
- **打包方式**: 单文件发布 (Self-Contained)

## 🔧 开发指南

### 环境准备
```bash
# 需要安装
- Visual Studio 2022 或 Visual Studio Code
- .NET 8.0 SDK
- Git for Windows
```

### 克隆项目
```bash
git clone https://github.com/zczy-k/githelper.git
cd githelper
```

### 编译运行
```bash
# 调试运行
dotnet run --project GitHelper/GitHelper.csproj

# 编译发布（单文件）
dotnet publish GitHelper/GitHelper.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### 项目结构
```
GitHelper/
├── GitHelper/
│   ├── MainForm.cs           # 主窗体
│   ├── ConfigManager.cs      # 配置管理
│   ├── Program.cs            # 程序入口
│   ├── Resources/
│   │   └── icon.ico          # 应用图标
│   └── GitHelper.csproj      # 项目配置
└── README.md                 # 本文档
```

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

### 贡献流程
1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 提交 Pull Request

## 📋 路线图

- [x] 基础 Git 操作（status, add, commit, log）
- [x] 目录历史记忆
- [x] 自动保存功能
- [x] 系统托盘支持
- [ ] 分支管理（创建、切换、合并）
- [ ] 远程仓库推送/拉取
- [ ] 可视化差异对比
- [ ] 多语言支持（英文）

## 📝 更新日志

### v1.0.0 (2025-10-21)
- ✨ 首次发布
- ✅ 实现基础 Git 操作
- ✅ 目录历史记忆功能
- ✅ 智能自动保存
- ✅ 系统托盘支持

## 📄 许可证

本项目采用 MIT 许可证 - 详见 [LICENSE](LICENSE) 文件

## 👨‍💻 作者

**zczy-k**

- GitHub: [@zczy-k](https://github.com/zczy-k)

## 🙏 致谢

感谢所有为本项目提供建议和反馈的朋友们！

---

⭐ 如果这个项目对你有帮助，请给个 Star 支持一下！
