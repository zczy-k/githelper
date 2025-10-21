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
        private string currentDirectory;

        public MainForm()
        {
            InitializeComponent();
            currentDirectory = Directory.GetCurrentDirectory();
            CheckAndInitGit();
        }

        private void InitializeComponent()
        {
            this.Text = "Git 快捷助手";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // 当前目录标签
            lblCurrentDir = new Label
            {
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(740, 30),
                Font = new System.Drawing.Font("Microsoft YaHei", 10F),
                Text = $"当前目录: {currentDirectory}"
            };

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

            // 添加控件
            this.Controls.Add(lblCurrentDir);
            this.Controls.Add(txtOutput);
            this.Controls.Add(btnStatus);
            this.Controls.Add(btnCommit);
            this.Controls.Add(btnLog);
            this.Controls.Add(btnRestoreFile);
            this.Controls.Add(btnReset);
            this.Controls.Add(btnExit);
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
            RunGitCommand("log --graph --oneline --decorate --all");
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
                        $"最后确认：真的要将整个项目回退到版本 '{textBox.Text}' 吗？\n这无法撤销！\n\n点击"是"继续，点击"否"取消。",
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
                MessageBox.Show($"执行Git命令时出错：\n{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
