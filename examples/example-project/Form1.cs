using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WhatsDotLib;

namespace WDexample1
{
    public partial class Form1 : Form
    {
        private WhatsDot myWhats;
        private Timer myTimer;
        public Form1()
        {
            InitializeComponent();
        }

        private string isLoggedIn = "";
        private async void Form1_Load(object sender, EventArgs e)
        {
            sendMsgPanel.Visible = false;
            sendMsgBtn.Visible = false;
            statusLabel.Text = "";
            connectBtn.Visible = false;
            disconnectBtn.Visible = false;
            await mainWebView.EnsureCoreWebView2Async();
            myWhats = new WhatsDot(mainWebView);
            myTimer = new Timer();
            myTimer.Interval = 1000;
            myTimer.Tick += async (sender1, args1) =>
            {
                isLoggedIn = await myWhats.checkLogin();
                statusLabel.Text = isLoggedIn;
                if (isLoggedIn == "connected")
                {
                    disconnectBtn.Visible = true;
                    connectBtn.Visible = false;
                    sendMsgBtn.Visible = true;
                }
                else if (isLoggedIn == "disconnected")
                {
                    disconnectBtn.Visible = false;
                    connectBtn.Visible = true;
                    sendMsgBtn.Visible= false;
                }
                else
                {
                    disconnectBtn.Visible = false;
                    connectBtn.Visible = false;
                    sendMsgBtn.Visible = false;
                }
            };
            myTimer.Start();
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            connectBtn.Visible=false;
            myWhats.connectWhatsapp();
        }

        private void disconnectBtn_Click(object sender, EventArgs e)
        {
            disconnectBtn.Visible=false;
            myWhats.logoutWhatsapp();
        }


        private void sendMsgBtn_Click(object sender, EventArgs e)
        {
            sendMsgPanel.Visible=true;
            
        }

        private  void sendBtn_Click(object sender, EventArgs e)
        {
            if (phnNumBox.Text == "" || msgTextBox.Text == "")
            {
                MessageBox.Show("Please Enter all fields");
            }
            else
            {
                myWhats.sendMessage(phnNumBox.Text, msgTextBox.Text);
                sendMsgPanel.Visible = false;
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            sendMsgPanel.Visible = false;
        }
    }
}
