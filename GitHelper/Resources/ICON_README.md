# GitHelper 图标使用说明

## 图标设计

图标包含以下元素：
- **蓝色渐变背景**：现代、专业的外观
- **Git 分支图**：象征版本控制的核心概念
- **齿轮符号**：代表工具/助手的功能
- **"Git"文字**：清晰标识应用用途

## 将SVG转换为ICO格式

### 方法1：使用在线工具（推荐）

1. 打开 https://convertio.co/zh/svg-ico/
2. 上传 `icon.svg` 文件
3. 选择输出格式为 `.ico`
4. 下载转换后的 `icon.ico` 文件
5. 将 `icon.ico` 放在 `Resources` 文件夹中

### 方法2：使用CloudConvert

1. 访问 https://cloudconvert.com/svg-to-ico
2. 上传 `icon.svg`
3. 设置图标尺寸（推荐：256x256, 128x128, 64x64, 48x48, 32x32, 16x16）
4. 转换并下载

### 方法3：使用本地工具

#### ImageMagick (跨平台)
```bash
# 安装 ImageMagick
# Windows: choco install imagemagick
# macOS: brew install imagemagick
# Linux: sudo apt-get install imagemagick

# 转换命令
magick convert -background none icon.svg -define icon:auto-resize=256,128,64,48,32,16 icon.ico
```

#### Inkscape (跨平台)
1. 在Inkscape中打开 `icon.svg`
2. 文件 → 导出PNG → 导出为多个尺寸
3. 使用在线工具合并为ICO

## 应用图标到项目

转换完成后，`icon.ico` 文件应该位于：
```
GitHelper/
  Resources/
    icon.svg      (源文件)
    icon.ico      (编译使用)
    ICON_README.md
```

图标已自动配置在 `GitHelper.csproj` 文件中，编译时会自动嵌入。

## 查看效果

编译后的 `GitHelper.exe` 将显示这个图标：
- 在文件资源管理器中
- 在任务栏中
- 在快捷方式中
- 在应用窗口标题栏中

## 自定义图标

如果你想修改图标：
1. 编辑 `icon.svg` 文件（使用文本编辑器或矢量图形软件）
2. 重新转换为 `.ico` 格式
3. 替换 `Resources/icon.ico`
4. 重新编译项目
