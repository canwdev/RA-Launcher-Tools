﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ra_launcher_remaster
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            // Call Process.Start method to open a browser, with link text as URL
            System.Diagnostics.Process.Start(e.LinkText); // call default browser
        }

        void openUrlByDefaultBrowser(string url)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            openUrlByDefaultBrowser("https://github.com/canwdev/ra-launcher-tools");
        }

        private void btnCurDir_Click(object sender, EventArgs e)
        {
            string folderPath = Application.StartupPath;
            Process.Start(folderPath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int tabIndex;
            if (checkExeIsExist("ra3.exe") || checkExeIsExist("ra3ep1.exe"))
            {
                tabIndex = 1;
                // MessageBox.Show("RA3 is detected", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (checkExeIsExist("ra2.exe") || checkExeIsExist("ra2md.exe"))
            {
                tabIndex = 0;
            }
            else
            {
                tabIndex = 2;
                // MessageBox.Show("请将此程序放在红警2/3文件夹内！", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // Set the default tab
            tabControl1.SelectedTab = tabControl1.TabPages[tabIndex];

            // 设置RA3分辨率列表
            // Add items to the combo box
            ra3ResComboBox.Items.Add("");
            ra3ResComboBox.Items.Add("Auto");
            ra3ResComboBox.Items.Add("800x600");
            ra3ResComboBox.Items.Add("1024x768");
            ra3ResComboBox.Items.Add("1360x768");
            ra3ResComboBox.Items.Add("1920x1080");
            ra3ResComboBox.Items.Add("2560x1440");

            // Set the default selected item
            ra3ResComboBox.SelectedItem = "";
        }

        void fnStartCurDirProgram(string exeName, string args="")
        {
            try
            {
                string exePath = Path.Combine(Application.StartupPath, exeName);
                Process.Start(exePath, args);
            }
            catch (Exception ex)
            {
                string errorMessage = $"An error occurred: {ex.Message}";
                Console.WriteLine(errorMessage);
                MessageBox.Show(exeName + " " + args + "\n" + ex.Message, "启动失败！", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void KillProcess(string processname)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";   // 启动命令提示符
            startInfo.Arguments = "/c taskkill /f /im "+ processname;   // 执行 taskkill 命令，/c 参数表示执行完命令后关闭命令提示符
            startInfo.CreateNoWindow = true;  // 不创建进程窗口
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;  // 隐藏窗口样式
            Process.Start(startInfo);
        }

        private void btnRa2Launch_Click(object sender, EventArgs e)
        {
            fnStartCurDirProgram("ra2.exe", "-speedcontrol");
        }

        private void btnRa2LaunchWin_Click(object sender, EventArgs e)
        {
            fnStartCurDirProgram("ra2.exe", "-win -speedcontrol");
        }

        private void btnRa2Exit_Click(object sender, EventArgs e)
        {
            KillProcess("ra2.exe");
            KillProcess("game.exe");
        }

        void fnOptRa2Ini(string iniFileName = "ra2.ini")
        {
            Process.Start("notepad", $".\\{iniFileName}");

            int width = Screen.FromControl(this).Bounds.Width;
            int height = Screen.FromControl(this).Bounds.Height;
            string configString = $@"[Video]
AllowHiResModes=yes
VideoBackBuffer=no
AllowVRAMSidebar=no
ScreenWidth={width}
ScreenHeight={height}
StretchMovies=no
";
            FormTextDisplay form2 = new FormTextDisplay(configString, $"请手动复制并替换 {iniFileName} 中的 [Video] 配置值"); // 创建一个新的窗口对象，并传递参数
            form2.Show(); // 显示新的窗口

        }

        private void btnRa2IniOpt_Click(object sender, EventArgs e)
        {
            fnOptRa2Ini();
        }

        private void btnRa2YrIniOpt_Click_Click(object sender, EventArgs e)
        {
            fnOptRa2Ini("ra2md.ini");
        }

        private void btnRa2YrLaunch_Click(object sender, EventArgs e)
        {
            fnStartCurDirProgram("ra2md.exe", "-speedcontrol");
        }

        private void btnRa2YrLaunchWin_Click(object sender, EventArgs e)
        {
            fnStartCurDirProgram("ra2md.exe", "-win -speedcontrol");
        }

        private void btnRa2YrExit_Click(object sender, EventArgs e)
        {
            KillProcess("ra2md.exe");
            KillProcess("gamemd.exe");
        }

        string fnGetRa3StartParams(string para = "")
        {
            if (checkBoxIsRa3Ui.Checked)
            {
                para += " -ui";
            }
            string resValue = ra3ResComboBox.SelectedItem != null ? (string)ra3ResComboBox.SelectedItem : ra3ResComboBox.Text;
            if (resValue != "")
            {
                if (resValue == "Auto")
                {
                    int width = Screen.FromControl(this).Bounds.Width;
                    int height = Screen.FromControl(this).Bounds.Height;
                    para += $" -xres {width} -yres {height}";
                } else if (resValue.Contains("x"))
                {
                    // 字符串包含"x"，执行相关操作
                    string[] parts = resValue.Split('x');
                    int width = int.Parse(parts[0]);
                    int height = int.Parse(parts[1]);
                    para += $" -xres {width} -yres {height}";
                } else
                {
                    MessageBox.Show("应该使用类似于 1024x768 的格式！", "错误的输入格式", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return para;
        }

        private void btnRa3Launch_Click(object sender, EventArgs e)
        {
            fnStartCurDirProgram("ra3.exe", fnGetRa3StartParams());
        }

        private void btnRa3LaunchWin_Click(object sender, EventArgs e)
        {
            fnStartCurDirProgram("ra3.exe", fnGetRa3StartParams("-win"));
        }

        private void btnRa3Exit_Click(object sender, EventArgs e)
        {
            KillProcess("ra3.exe");
            KillProcess("ra3_1.12.game"); // TODO: Auto
        }

        bool checkExeIsExist(string exeName)
        {
            string filePath = Path.Combine(Application.StartupPath, exeName);

            return File.Exists(filePath);

        }

        private void btnRa3Ep1Launch_Click(object sender, EventArgs e)
        {
            fnStartCurDirProgram("ra3ep1.exe", fnGetRa3StartParams());
        }

        private void btnRa3Ep1LaunchWin_Click(object sender, EventArgs e)
        {
            fnStartCurDirProgram("ra3ep1.exe", fnGetRa3StartParams("-win"));
        }

        private void btnRa3Ep1Exit_Click(object sender, EventArgs e)
        {
            KillProcess("ra3ep1.exe");
            KillProcess("ra3ep1_1.0.game"); // TODO: Auto
        }

        private void btnDdrawPatch_Click(object sender, EventArgs e)
        {
            // 本地开发时，请下载最新版 ddraw.dll https://github.com/narzoul/DDrawCompat/releases
            // 并放置在 Resources 目录下

            // 获取要提取的dll文件路径
            string localNameSpace = this.GetType().Namespace; //获取工作空间
            string dllFilePath = localNameSpace + ".Resources.ddraw.dll";
            Console.WriteLine(dllFilePath);

            // 获取提取后dll文件的目标路径
            string targetFilePath = Path.Combine(Application.StartupPath, "ddraw.dll");
            Console.WriteLine(targetFilePath);


            // 从资源文件中读取dll文件并复制
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(dllFilePath))
            {
                using (FileStream fileStream = new FileStream(targetFilePath, FileMode.Create))
                {
                    stream.CopyTo(fileStream);
                }
            }

            MessageBox.Show($"补丁已放置在目录\n{targetFilePath}", "操作成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 加载提取后的dll文件
            // Assembly assembly = Assembly.LoadFile(targetFilePath);
            // 使用加载的dll文件
        }
    }
}
