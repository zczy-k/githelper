using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace GitHelper
{
    public class MainForm : Form
    {
        private Label lblCurrentDir;
        private TextBox txtOutput;
        private Button btnStatus;
        private Button btnCommit;
        private Button btnLog;
        private Button btnRestoreFile;
        private Button btnReset;
        private Button btnExit;
        private Button btnChangeDir;
        private Button btnRecentDirs;
        private string currentDirectory;
        
        // 自动保存相关
        private CheckBox chkAutoSave;
        private ComboBox cmbInterval;
        private Label lblAutoSave;
        private System.Windows.Forms.Timer autoSaveTimer;
        
        // 系统托盘相关
        private NotifyIcon notifyIcon;
        private bool isReallyClosing = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeAutoSaveTimer();
            
            // 启动时优先加载上次使用的目录
            string lastDir = ConfigManager.GetLastDirectory();
            if (!string.IsNullOrEmpty(lastDir) && Directory.Exists(lastDir))
            {
                currentDirectory = lastDir;
            }
            else
            {
                // 如果没有历史记录，则选择目录
                if (!SelectDirectory())
                {
                    // 用户取消选择，使用当前目录
                    currentDirectory = Directory.GetCurrentDirectory();
                }
            }
            
            UpdateDirectoryDisplay();
            CheckAndInitGit();
        }

        private void InitializeComponent()
        {
            this.Text = "Git 快捷助手";
            this.Size = new System.Drawing.Size(800, 710);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            
            // 加载窗体图标（优先从文件加载，其次使用嵌入资源）
            System.Drawing.Icon appIcon = null;
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icon.ico");
                if (File.Exists(iconPath))
                {
                    appIcon = new System.Drawing.Icon(iconPath);
                }
            }
            catch { /* 忽略文件加载错误 */ }
            
            // 如果文件加载失败，尝试使用程序嵌入的图标
            if (appIcon == null)
            {
                try
                {
                    appIcon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                catch { /* 忽略嵌入资源加载错误 */ }
            }
            
            // 设置窗体图标
            if (appIcon != null)
            {
                this.Icon = appIcon;
            }

            // 当前目录标签
            lblCurrentDir = new Label
            {
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(480, 30),
                Font = new System.Drawing.Font("Microsoft YaHei", 9F),
                Text = "当前目录: 未选择",
                AutoEllipsis = true
            };

            // 历史记录按钮
            btnRecentDirs = new Button
            {
                Location = new System.Drawing.Point(510, 17),
                Size = new System.Drawing.Size(100, 35),
                Text = "▼ 最近目录",
                Font = new System.Drawing.Font("Microsoft YaHei", 9F),
                BackColor = System.Drawing.Color.LightGreen
            };
            btnRecentDirs.Click += BtnRecentDirs_Click;

            // 选择/切换目录按钮
            btnChangeDir = new Button
            {
                Location = new System.Drawing.Point(620, 17),
                Size = new System.Drawing.Size(140, 35),
                Text = "浏览新目录",
                Font = new System.Drawing.Font("Microsoft YaHei", 9F),
                BackColor = System.Drawing.Color.LightSkyBlue
            };
            btnChangeDir.Click += BtnChangeDir_Click;

            // 输出文本框
            txtOutput = new TextBox
            {
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(740, 300),
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Font = new System.Drawing.Font("Consolas", 9F),
                ReadOnly = true,
                BackColor = System.Drawing.Color.White
            };

            // 按钮
            int btnY = 380;
            int btnSpacing = 70;

            btnStatus = new Button
            {
                Location = new System.Drawing.Point(20, btnY),
                Size = new System.Drawing.Size(740, 40),
                Text = "1. 查看当前状态 (git status)",
                Font = new System.Drawing.Font("Microsoft YaHei", 10F)
            };
            btnStatus.Click += BtnStatus_Click;

            btnCommit = new Button
            {
                Location = new System.Drawing.Point(20, btnY + btnSpacing),
                Size = new System.Drawing.Size(740, 40),
                Text = "2. 保存所有更改 (git add . + commit)",
                Font = new System.Drawing.Font("Microsoft YaHei", 10F)
            };
            btnCommit.Click += BtnCommit_Click;

            btnLog = new Button
            {
                Location = new System.Drawing.Point(20, btnY + btnSpacing * 2),
                Size = new System.Drawing.Size(360, 40),
                Text = "3. 查看提交历史",
                Font = new System.Drawing.Font("Microsoft YaHei", 10F)
            };
            btnLog.Click += BtnLog_Click;

            btnRestoreFile = new Button
            {
                Location = new System.Drawing.Point(400, btnY + btnSpacing * 2),
                Size = new System.Drawing.Size(360, 40),
                Text = "4. 恢复单个文件",
                Font = new System.Drawing.Font("Microsoft YaHei", 10F)
            };
            btnRestoreFile.Click += BtnRestoreFile_Click;

            btnReset = new Button
            {
                Location = new System.Drawing.Point(20, btnY + btnSpacing * 3),
                Size = new System.Drawing.Size(360, 40),
                Text = "5. [危险] 回退到旧版本",
                Font = new System.Drawing.Font("Microsoft YaHei", 10F),
                BackColor = System.Drawing.Color.IndianRed,
                ForeColor = System.Drawing.Color.White
            };
            btnReset.Click += BtnReset_Click;

            btnExit = new Button
            {
                Location = new System.Drawing.Point(400, btnY + btnSpacing * 3),
                Size = new System.Drawing.Size(360, 40),
                Text = "退出",
                Font = new System.Drawing.Font("Microsoft YaHei", 10F)
            };
            btnExit.Click += (s, e) => this.Close();

            // 自动保存控件
            lblAutoSave = new Label
            {
                Location = new System.Drawing.Point(20, 640),
                Size = new System.Drawing.Size(80, 25),
                Text = "自动保存:",
                Font = new System.Drawing.Font("Microsoft YaHei", 9F),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            chkAutoSave = new CheckBox
            {
                Location = new System.Drawing.Point(100, 640),
                Size = new System.Drawing.Size(80, 25),
                Text = "启用",
                Font = new System.Drawing.Font("Microsoft YaHei", 9F),
                Checked = false
            };
            chkAutoSave.CheckedChanged += ChkAutoSave_CheckedChanged;

            cmbInterval = new ComboBox
            {
                Location = new System.Drawing.Point(190, 640),
                Size = new System.Drawing.Size(120, 25),
                Font = new System.Drawing.Font("Microsoft YaHei", 9F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbInterval.Items.AddRange(new object[] { "2分钟", "3分钟", "5分钟", "10分钟", "15分钟" });
            cmbInterval.SelectedIndex = 2; // 默认5分钟
            cmbInterval.SelectedIndexChanged += CmbInterval_SelectedIndexChanged;

            // 系统托盘图标（使用与窗体相同的图标）
            notifyIcon = new NotifyIcon
            {
                Text = "Git 快捷助手",
                Visible = false,
                Icon = this.Icon != null ? (System.Drawing.Icon)this.Icon.Clone() : System.Drawing.SystemIcons.Application
            };
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            // 托盘右键菜单
            var trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("显示窗口", null, (s, e) => ShowWindow());
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add("退出", null, (s, e) => ReallyClose());
            notifyIcon.ContextMenuStrip = trayMenu;

            // 添加关闭事件处理
            this.FormClosing += MainForm_FormClosing;
            this.Resize += MainForm_Resize;

            // 添加控件
            this.Controls.Add(lblCurrentDir);
            this.Controls.Add(btnRecentDirs);
            this.Controls.Add(btnChangeDir);
            this.Controls.Add(txtOutput);
            this.Controls.Add(btnStatus);
            this.Controls.Add(btnCommit);
            this.Controls.Add(btnLog);
            this.Controls.Add(btnRestoreFile);
            this.Controls.Add(btnReset);
            this.Controls.Add(btnExit);
            this.Controls.Add(lblAutoSave);
            this.Controls.Add(chkAutoSave);
            this.Controls.Add(cmbInterval);
        }

        private bool SelectDirectory()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "请选择要管理的项目文件夹";
                folderDialog.ShowNewFolderButton = true;
                
                if (!string.IsNullOrEmpty(currentDirectory) && Directory.Exists(currentDirectory))
                {
                    folderDialog.SelectedPath = currentDirectory;
                }
                else
                {
                    folderDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    currentDirectory = folderDialog.SelectedPath;
                    return true;
                }
                return false;
            }
        }

        private void UpdateDirectoryDisplay()
        {
            lblCurrentDir.Text = $"当前目录: {currentDirectory}";
            this.Text = $"Git 快捷助手 - {Path.GetFileName(currentDirectory)}";
        }

        private void BtnChangeDir_Click(object sender, EventArgs e)
        {
            bool wasAutoSaveEnabled = chkAutoSave.Checked;
            if (wasAutoSaveEnabled)
            {
                chkAutoSave.Checked = false;
            }

            if (SelectDirectory())
            {
                SwitchToDirectory(currentDirectory);
                
                if (wasAutoSaveEnabled)
                {
                    txtOutput.AppendText("提示: 自动保存已停止，如需要请重新启用" + Environment.NewLine);
                }
            }
        }

        private void BtnRecentDirs_Click(object sender, EventArgs e)
        {
            var recentDirs = ConfigManager.GetRecentDirectories();
            
            if (recentDirs.Count == 0)
            {
                MessageBox.Show("暂无历史记录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var menu = new ContextMenuStrip();
            menu.Font = new System.Drawing.Font("Microsoft YaHei", 9F);

            foreach (var dir in recentDirs)
            {
                var item = new ToolStripMenuItem
                {
                    Text = dir,
                    Tag = dir,
                    Checked = (dir == currentDirectory)
                };
                item.Click += (s, args) =>
                {
                    string selectedDir = (s as ToolStripMenuItem)?.Tag as string;
                    if (!string.IsNullOrEmpty(selectedDir) && selectedDir != currentDirectory)
                    {
                        bool wasAutoSaveEnabled = chkAutoSave.Checked;
                        if (wasAutoSaveEnabled)
                        {
                            chkAutoSave.Checked = false;
                        }
                        
                        SwitchToDirectory(selectedDir);
                        
                        if (wasAutoSaveEnabled)
                        {
                            txtOutput.AppendText("提示: 自动保存已停止，如需要请重新启用" + Environment.NewLine);
                        }
                    }
                };
                menu.Items.Add(item);
            }

            menu.Show(btnRecentDirs, new System.Drawing.Point(0, btnRecentDirs.Height));
        }

        private void SwitchToDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                MessageBox.Show("目录不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentDirectory = directory;
            ConfigManager.AddRecentDirectory(directory);
            UpdateDirectoryDisplay();
            CheckAndInitGit();
            txtOutput.Clear();
            txtOutput.AppendText($"已切换到目录: {currentDirectory}" + Environment.NewLine);
        }

        private void InitializeAutoSaveTimer()
        {
            autoSaveTimer = new System.Windows.Forms.Timer();
            autoSaveTimer.Tick += AutoSaveTimer_Tick;
            UpdateTimerInterval();
        }

        private void UpdateTimerInterval()
        {
            int[] intervals = { 2, 3, 5, 10, 15 };
            int selectedIndex = cmbInterval.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < intervals.Length)
            {
                autoSaveTimer.Interval = intervals[selectedIndex] * 60 * 1000;
            }
        }

        private void ChkAutoSave_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoSave.Checked)
            {
                autoSaveTimer.Start();
                txtOutput.AppendText(Environment.NewLine + $"[自动保存] 已启用，间隔: {cmbInterval.Text}" + Environment.NewLine);
            }
            else
            {
                autoSaveTimer.Stop();
                txtOutput.AppendText(Environment.NewLine + "[自动保存] 已禁用" + Environment.NewLine);
            }
        }

        private void CmbInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTimerInterval();
            if (chkAutoSave.Checked)
            {
                autoSaveTimer.Stop();
                autoSaveTimer.Start();
                txtOutput.AppendText(Environment.NewLine + $"[自动保存] 时间间隔已更改为: {cmbInterval.Text}" + Environment.NewLine);
            }
        }

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                var statusProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "git",
                        Arguments = "status --porcelain",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        WorkingDirectory = currentDirectory
                    }
                };

                statusProcess.Start();
                string statusOutput = statusProcess.StandardOutput.ReadToEnd();
                statusProcess.WaitForExit();

                if (!string.IsNullOrWhiteSpace(statusOutput))
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    RunGitCommandSilent("add .");
                    RunGitCommandSilent($"commit -m \"自动保存 - {timestamp}\"");
                    txtOutput.AppendText($"{timestamp} [自动保存] 更改已自动提交" + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtOutput.AppendText($"{timestamp} [自动保存错误] {ex.Message}" + Environment.NewLine);
            }
        }

        private void CheckAndInitGit()
        {
            if (!Directory.Exists(Path.Combine(currentDirectory, ".git")))
            {
                var result = MessageBox.Show(
                    "当前文件夹还没有初始化Git版本控制。\n是否要在当前文件夹初始化Git仓库？",
                    "Git 仓库初始化",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    RunGitCommand("init");
                    MessageBox.Show("Git仓库初始化成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("未初始化Git仓库，部分功能将无法使用。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnStatus_Click(object sender, EventArgs e)
        {
            RunGitCommand("status");
        }

        private void BtnCommit_Click(object sender, EventArgs e)
        {
            using (var inputForm = new Form())
            {
                inputForm.Text = "输入提交说明";
                inputForm.Size = new System.Drawing.Size(500, 180);
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;

                var label = new Label
                {
                    Location = new System.Drawing.Point(20, 20),
                    Size = new System.Drawing.Size(440, 20),
                    Text = "请输入本次提交的说明："
                };

                var textBox = new TextBox
                {
                    Location = new System.Drawing.Point(20, 50),
                    Size = new System.Drawing.Size(440, 30),
                    Font = new System.Drawing.Font("Microsoft YaHei", 10F)
                };

                var btnOK = new Button
                {
                    Location = new System.Drawing.Point(280, 100),
                    Size = new System.Drawing.Size(90, 30),
                    Text = "确定",
                    DialogResult = DialogResult.OK
                };

                var btnCancel = new Button
                {
                    Location = new System.Drawing.Point(380, 100),
                    Size = new System.Drawing.Size(80, 30),
                    Text = "取消",
                    DialogResult = DialogResult.Cancel
                };

                inputForm.Controls.AddRange(new Control[] { label, textBox, btnOK, btnCancel });
                inputForm.AcceptButton = btnOK;
                inputForm.CancelButton = btnCancel;

                if (inputForm.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(textBox.Text))
                {
                    txtOutput.AppendText("正在添加所有文件到暂存区...\r\n");
                    RunGitCommand("add .");
                    txtOutput.AppendText("\r\n正在提交更改...\r\n");
                    RunGitCommand($"commit -m \"{textBox.Text}\"");
                    txtOutput.AppendText("\r\n提交完成！\r\n");
                }
                else if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    MessageBox.Show("提交说明不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnLog_Click(object sender, EventArgs e)
        {
            RunGitCommand("log --graph --pretty=format:%h %s (%cr)");
        }

        private void BtnRestoreFile_Click(object sender, EventArgs e)
        {
            using (var inputForm = new Form())
            {
                inputForm.Text = "恢复文件到旧版本";
                inputForm.Size = new System.Drawing.Size(500, 250);
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;

                var label1 = new Label
                {
                    Location = new System.Drawing.Point(20, 20),
                    Size = new System.Drawing.Size(440, 20),
                    Text = "提交哈希值 (commit hash)："
                };

                var txtHash = new TextBox
                {
                    Location = new System.Drawing.Point(20, 50),
                    Size = new System.Drawing.Size(440, 25)
                };

                var label2 = new Label
                {
                    Location = new System.Drawing.Point(20, 90),
                    Size = new System.Drawing.Size(440, 20),
                    Text = "文件路径（例如: src/main.js）："
                };

                var txtFile = new TextBox
                {
                    Location = new System.Drawing.Point(20, 120),
                    Size = new System.Drawing.Size(440, 25)
                };

                var btnOK = new Button
                {
                    Location = new System.Drawing.Point(280, 170),
                    Size = new System.Drawing.Size(90, 30),
                    Text = "确定",
                    DialogResult = DialogResult.OK
                };

                var btnCancel = new Button
                {
                    Location = new System.Drawing.Point(380, 170),
                    Size = new System.Drawing.Size(80, 30),
                    Text = "取消",
                    DialogResult = DialogResult.Cancel
                };

                inputForm.Controls.AddRange(new Control[] { label1, txtHash, label2, txtFile, btnOK, btnCancel });
                inputForm.AcceptButton = btnOK;
                inputForm.CancelButton = btnCancel;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    if (!string.IsNullOrWhiteSpace(txtHash.Text) && !string.IsNullOrWhiteSpace(txtFile.Text))
                    {
                        var confirmResult = MessageBox.Show(
                            $"确定要将文件 '{txtFile.Text}' 恢复到版本 '{txtHash.Text}' 吗？",
                            "确认",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (confirmResult == DialogResult.Yes)
                        {
                            RunGitCommand($"checkout {txtHash.Text} -- {txtFile.Text}");
                            MessageBox.Show("文件已成功恢复。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("commit hash 和文件路径都不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var warning = MessageBox.Show(
                "警告：这是一个破坏性操作！\n它将永久删除目标版本之后的所有提交和代码更改！\n\n你确定要继续吗？",
                "危险操作警告",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (warning != DialogResult.Yes) return;

            using (var inputForm = new Form())
            {
                inputForm.Text = "回退到旧版本";
                inputForm.Size = new System.Drawing.Size(500, 180);
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;

                var label = new Label
                {
                    Location = new System.Drawing.Point(20, 20),
                    Size = new System.Drawing.Size(440, 20),
                    Text = "请输入要回退到的提交哈希值 (commit hash)："
                };

                var textBox = new TextBox
                {
                    Location = new System.Drawing.Point(20, 50),
                    Size = new System.Drawing.Size(440, 30)
                };

                var btnOK = new Button
                {
                    Location = new System.Drawing.Point(280, 100),
                    Size = new System.Drawing.Size(90, 30),
                    Text = "确定",
                    DialogResult = DialogResult.OK
                };

                var btnCancel = new Button
                {
                    Location = new System.Drawing.Point(380, 100),
                    Size = new System.Drawing.Size(80, 30),
                    Text = "取消",
                    DialogResult = DialogResult.Cancel
                };

                inputForm.Controls.AddRange(new Control[] { label, textBox, btnOK, btnCancel });
                inputForm.AcceptButton = btnOK;
                inputForm.CancelButton = btnCancel;

                if (inputForm.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(textBox.Text))
                {
                    var finalConfirm = MessageBox.Show(
                        $"最后确认：真的要将整个项目回退到版本 '{textBox.Text}' 吗？\n这无法撤销！\n\n点击'是'继续，点击'否'取消。",
                        "最终确认",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Stop
                    );

                    if (finalConfirm == DialogResult.Yes)
                    {
                        RunGitCommand($"reset --hard {textBox.Text}");
                        MessageBox.Show("项目已成功回退。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void RunGitCommand(string arguments)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "git",
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = currentDirectory,
                        StandardOutputEncoding = System.Text.Encoding.UTF8,
                        StandardErrorEncoding = System.Text.Encoding.UTF8
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                txtOutput.Clear();
                txtOutput.AppendText($"$ git {arguments}\r\n\r\n");
                
                if (!string.IsNullOrEmpty(output))
                {
                    txtOutput.AppendText(output);
                }
                
                if (!string.IsNullOrEmpty(error))
                {
                    txtOutput.AppendText("\r\n");
                    txtOutput.AppendText(error);
                }

                txtOutput.AppendText("\r\n\r\n执行完成。");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"执行Git命令时出错：\\n{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RunGitCommandSilent(string arguments)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "git",
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = currentDirectory,
                        StandardOutputEncoding = System.Text.Encoding.UTF8,
                        StandardErrorEncoding = System.Text.Encoding.UTF8
                    }
                };

                process.Start();
                process.WaitForExit();
            }
            catch
            {
                // 静默执行，不显示错误
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isReallyClosing)
            {
                e.Cancel = true;
                
                var result = new Form
                {
                    Text = "关闭选项",
                    Size = new System.Drawing.Size(350, 180),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                var label = new Label
                {
                    Location = new System.Drawing.Point(30, 20),
                    Size = new System.Drawing.Size(280, 40),
                    Text = "请选择关闭方式：",
                    Font = new System.Drawing.Font("Microsoft YaHei", 10F),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };

                var btnMinimize = new Button
                {
                    Location = new System.Drawing.Point(30, 70),
                    Size = new System.Drawing.Size(130, 40),
                    Text = "最小化到托盘",
                    Font = new System.Drawing.Font("Microsoft YaHei", 10F),
                    DialogResult = DialogResult.OK
                };

                var btnExit = new Button
                {
                    Location = new System.Drawing.Point(180, 70),
                    Size = new System.Drawing.Size(130, 40),
                    Text = "退出程序",
                    Font = new System.Drawing.Font("Microsoft YaHei", 10F),
                    DialogResult = DialogResult.Yes
                };

                result.Controls.AddRange(new Control[] { label, btnMinimize, btnExit });
                result.AcceptButton = btnMinimize;

                var dialogResult = result.ShowDialog();
                result.Dispose();

                if (dialogResult == DialogResult.Yes)
                {
                    ReallyClose();
                }
                else if (dialogResult == DialogResult.OK)
                {
                    MinimizeToTray();
                }
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                MinimizeToTray();
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowWindow();
        }

        private void ShowWindow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            notifyIcon.Visible = false;
        }

        private void MinimizeToTray()
        {
            this.Hide();
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(2000, "Git 快捷助手", "程序已最小化到系统托盘，双击托盘图标可恢复窗口。", ToolTipIcon.Info);
        }

        private void ReallyClose()
        {
            isReallyClosing = true;
            this.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                autoSaveTimer?.Stop();
                autoSaveTimer?.Dispose();
                notifyIcon?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
