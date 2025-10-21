# 📦 GitHelper 图标设置指南

## 快速开始

GitHelper 已经包含了一个精心设计的SVG图标，你只需要将它转换为 `.ico` 格式即可。

## 🎨 图标设计预览

图标包含：
- 蓝色渐变圆形背景
- Git分支提交图案（3个分支，6个提交点）
- 右下角的齿轮符号（代表工具）
- 底部的"Git"文字

文件位置：`GitHelper/Resources/icon.svg`

## ⚡ 最快方法：使用在线工具（2分钟）

### 步骤：

1. **打开在线转换工具**
   - 推荐：https://convertio.co/zh/svg-ico/
   - 或者：https://cloudconvert.com/svg-to-ico

2. **上传文件**
   - 找到并上传 `GitHelper/Resources/icon.svg`

3. **转换并下载**
   - 点击"转换"按钮
   - 下载生成的 `icon.ico` 文件

4. **放置文件**
   - 将下载的 `icon.ico` 重命名（如果需要）
   - 放到 `GitHelper/Resources/icon.ico`

5. **编译项目**
   ```bash
   cd GitHelper
   .\build.bat
   ```

搞定！编译后的 `GitHelper.exe` 就会显示这个图标了。

## 🖥️ 方法2：使用PowerShell脚本（如果你有ImageMagick）

如果你的系统已安装 ImageMagick：

```powershell
# 进入Resources目录
cd GitHelper\Resources

# 转换图标（生成多尺寸）
magick convert -background none icon.svg -define icon:auto-resize=256,128,64,48,32,16 icon.ico
```

## 📁 文件结构

转换完成后，你的目录应该是这样的：

```
GitHelper/
├── Resources/
│   ├── icon.svg          ← 源SVG文件（已有）
│   ├── icon.ico          ← 需要你生成这个
│   └── ICON_README.md    ← 详细说明
├── MainForm.cs
├── Program.cs
├── GitHelper.csproj      ← 已配置好图标路径
└── build.bat
```

## ✅ 验证图标

编译后验证图标是否正确显示：

1. 在文件资源管理器中查看 `GitHelper.exe` - 应该显示蓝色Git图标
2. 运行程序后，在任务栏和窗口标题栏也会显示图标
3. 创建桌面快捷方式 - 快捷方式也会显示图标

## 🎯 不想自己转换？

你也可以选择：
- 暂时跳过图标，直接编译（exe会使用默认图标）
- 使用GitHub Actions自动编译（但仍需要先上传icon.ico文件）
- 让其他工具（如GIMP、Inkscape）帮你导出为ICO

## 🔧 自定义图标

如果你想修改图标样式：

1. 使用任何文本编辑器或SVG编辑器（如Inkscape）打开 `icon.svg`
2. 修改颜色、形状等（文件是标准SVG格式）
3. 重新转换为 `.ico`
4. 重新编译项目

### SVG颜色代码参考：
- 主蓝色：`#4A90E2`
- 深蓝色：`#357ABD`
- 白色：`#FFFFFF`
- 浅灰：`#F0F0F0`

## ❓ 常见问题

**Q: 我可以跳过图标直接编译吗？**
A: 可以！但编译时会有警告。删除 `.csproj` 文件中的 `<ApplicationIcon>` 行即可。

**Q: 图标看起来模糊？**
A: 确保转换时生成了多个尺寸（256, 128, 64, 48, 32, 16），这样在不同缩放比例下都会清晰。

**Q: GitHub Actions编译失败？**
A: 确保你已经提交了 `icon.ico` 文件到仓库。如果没有，临时删除 `.csproj` 中的图标配置。

## 🚀 完成后

运行编译脚本：
```bash
cd GitHelper
.\build.bat
```

然后在 `bin\Release\net8.0-windows\win-x64\publish\` 找到带图标的 `GitHelper.exe`！
