﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Security.Permissions;
using System.Security.Principal;
using System.Windows;
using MCam;
using System.Management;

namespace MCam
{
    public partial class Menu1 : Form
    {
        public int pid;
        public string pname;
        public bool Goto;
        const bool F = false;
        const bool T = true;
        public int fps = 30;
        public Menu1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e){}
        [DllImport("User32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            returnpath();
        }
        public string returnpath()
        {
            string repath = saveFileDialog1.FileName;
            return (repath);
        }

        public void button1_Click(object sender, EventArgs e)
        {
            CreateProc();
        }
        public void CreateProc()
        {
            if (returnpath() == "") {saveFileDialog1.ShowDialog();};
            if (returnpath() == "") {return;};
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            string cmd = "ffmpeg -y -f gdigrab -framerate " + fps + " -offset_x " + Form1.ffOffX + " -offset_y " + Form1.ffOffY + " -video_size " + Form1.ffHeight +"x" + Form1.ffWidth + " -i desktop -pix_fmt +yuv420p -threads "+ coreCount + " \"" + returnpath() + "\"";
            Console.WriteLine(cmd);
            startInfo.Arguments = "/C " + cmd;
            startInfo.UseShellExecute = true;
            process.StartInfo = startInfo;
            //* Start process and handlers
            process.Start();
            int procidint = process.Id;
            string procid = procidint.ToString();
            //* PostMessage(process.MainWindowHandle, 0x0101, 0x59, 0);
            pid = process.Id;
            pname = process.ProcessName;
            Goto=T; RecordCheck();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e) {
            //*CreateProc().WriteLine("q");
            //*PostMessage(CreateProc().MainWindowHandle, 0x0100, 0x51, 0); //*.MainWindowHandle
            try
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(pid);
                PostMessage(process.MainWindowHandle, 0x0100, 0x51, 0);
                PostMessage(process.MainWindowHandle, 0x0101, 0x51, 0);
                pid=0;Goto=F;label1.Visible=F;
            }
            catch (System.ArgumentException) {pid = 0;Goto=F;};
        }

        async Task RecordCheck()
        {
        RecCheck:
            label1.Visible = false;
            try
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(pid);
                if (process.ProcessName == pname) {label1.Visible = true;}
            }
            catch (System.ArgumentException) {pid=0;Goto=F;};
            await Task.Delay(2000);
            if (Goto==T) {goto RecCheck;}
        }

        private void label1_Click(object sender, EventArgs e){}
        string bf = "";
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try { fps = Convert.ToInt32(textBox1.Text); }
            catch (System.FormatException) { textBox1.Text = bf;};
            bf = textBox1.Text;
        }

        public class ScalePos
        {
            public float Health = 100.0f;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.InstanceRef = this;
            form1.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    if ((int)m.Result == HTCLIENT)
                        m.Result = (IntPtr)HTCAPTION;
                    return;
            }
            base.WndProc(ref m);
        }


    }
}