# 发布指南

本文档说明如何发布 GitHelper 的新版本到 GitHub Releases。

## 📋 发布前检查清单

- [ ] 代码已测试通过
- [ ] README.md 已更新
- [ ] 版本号已确定（遵循 [语义化版本](https://semver.org/lang/zh-CN/)）
- [ ] 准备好更新日志

## 🚀 自动发布流程

项目配置了 GitHub Actions 自动发布工作流，只需推送版本标签即可自动构建和发布。

### 步骤

1. **确定版本号**
   ```bash
   # 格式：v主版本号.次版本号.修订号
   # 例如：v1.0.0, v1.1.0, v2.0.0
   ```

2. **创建并推送标签**
   ```bash
   # 创建标签（附带说明）
   git tag -a v1.0.0 -m "Release v1.0.0"
   
   # 推送标签到远程仓库
   git push origin v1.0.0
   ```

3. **等待自动构建**
   - GitHub Actions 会自动触发构建流程
   - 查看进度：https://github.com/zczy-k/githelper/actions
   - 构建时间约 2-5 分钟

4. **完善 Release 说明**
   - 构建完成后，访问 [Releases](https://github.com/zczy-k/githelper/releases)
   - 点击刚发布的版本
   - 点击 "Edit release"
   - 在 `📝 更新内容` 部分添加详细的更新日志
   - 保存

## 📝 版本号规范

采用语义化版本 (Semantic Versioning)：

- **主版本号 (Major)**: 不兼容的 API 修改
- **次版本号 (Minor)**: 向下兼容的功能性新增
- **修订号 (Patch)**: 向下兼容的问题修正

### 示例

- `v1.0.0` - 首次正式发布
- `v1.1.0` - 新增功能（如：添加分支管理）
- `v1.1.1` - 修复 bug（如：修复托盘图标显示问题）
- `v2.0.0` - 重大更新（如：界面重构）

## 📦 发布产物

自动发布流程会生成以下文件：

```
GitHelper-vX.X.X-win-x64.zip
├── GitHelper.exe           # 主程序（单文件）
├── Resources/
│   └── icon.ico           # 应用图标
└── 使用说明.txt            # 简要说明
```

## 🔧 手动发布（备选方案）

如果自动发布失败，可以手动发布：

### 1. 本地编译

```bash
# 编译 Release 版本
dotnet publish GitHelper/GitHelper.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

### 2. 打包

```powershell
# 创建发布目录
New-Item -ItemType Directory -Force -Path ./release

# 复制文件
Copy-Item ./publish/GitHelper.exe ./release/
Copy-Item -Recurse ./GitHelper/Resources ./release/

# 创建说明文件
@"
Git 快捷助手 vX.X.X

使用方法：
1. 双击 GitHelper.exe 运行
2. 选择要管理的项目文件夹
3. 开始使用！

完整文档：https://github.com/zczy-k/githelper
"@ | Out-File -FilePath ./release/使用说明.txt -Encoding UTF8

# 打包
Compress-Archive -Path ./release/* -DestinationPath GitHelper-vX.X.X-win-x64.zip
```

### 3. 创建 Release

1. 访问 https://github.com/zczy-k/githelper/releases/new
2. 填写信息：
   - **Tag**: 选择或创建标签（如 v1.0.0）
   - **Title**: 版本号（如 v1.0.0）
   - **Description**: 参考自动发布的模板
3. 上传 `GitHelper-vX.X.X-win-x64.zip`
4. 点击 "Publish release"

## 📊 发布后工作

- [ ] 测试下载链接是否正常
- [ ] 测试下载的程序是否能正常运行
- [ ] 在社交媒体或相关社区宣传新版本
- [ ] 关闭已修复的 Issues

## ❓ 常见问题

### Q: GitHub Actions 构建失败怎么办？

A: 
1. 查看 Actions 页面的错误日志
2. 常见问题：
   - 依赖下载失败 → 重新运行工作流
   - 编译错误 → 检查代码是否有语法错误
   - 权限问题 → 检查 GitHub Token 权限

### Q: 如何删除错误的 Release？

A:
1. 访问 Releases 页面
2. 点击要删除的 Release
3. 点击 "Delete"
4. 删除对应的 Git 标签：
   ```bash
   # 删除本地标签
   git tag -d v1.0.0
   
   # 删除远程标签
   git push origin :refs/tags/v1.0.0
   ```

### Q: 如何发布预览版本？

A:
```bash
# 使用 beta, alpha, rc 等后缀
git tag -a v1.1.0-beta.1 -m "Beta release"
git push origin v1.1.0-beta.1
```

然后在 Release 页面勾选 "This is a pre-release"。

## 📚 参考资料

- [GitHub Actions 文档](https://docs.github.com/en/actions)
- [语义化版本规范](https://semver.org/lang/zh-CN/)
- [.NET 发布文档](https://learn.microsoft.com/zh-cn/dotnet/core/deploying/)
