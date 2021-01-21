using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleTCP;

using SimpleTCP.Server;

namespace Server
{
    public partial class Form1 : Form
    {
        //TextWriter _writer = null;
        public static SimpleTcpServer server;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            server = new SimpleTcpServer();
            server.Delimiter = 0x13;
            server.StringEncoder = Encoding.UTF8;
            server.DataReceived += Server_DataReceived;
        }
        void SendUserInfo()
        {
            List<string> data = listBox1.Items.Cast<String>().ToList();

            byte[] dataAsBytes = data
              .SelectMany(s => Encoding.UTF8.GetBytes("USERS|"+s))
              .ToArray();
            server.Broadcast(dataAsBytes);
        }

        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {
            stringLog.Invoke((MethodInvoker)delegate ()
            {
                if (e.MessageString.Contains("IMH|"))
                {
                    string[] tmp = e.MessageString.Replace("IMH|", "").Split(' ');

                    listBox1.Items.Add(tmp[0] + "  |-{|");
                   
                    SendUserInfo();
                }
                if (e.MessageString.Contains("IMO|"))
                {
                    string tmp = e.MessageString.Replace("IMO|", "");

                    for (int n = listBox1.Items.Count - 1; n >= 0; --n)
                    {

                        if (listBox1.Items[n].ToString() == (tmp) + "  |-{|")
                        {
                            listBox1.Items.RemoveAt(n);
                        }
                    }

                    SendUserInfo();
                }
                stringLog.Text += e.MessageString;
                stringLog.AppendText(Environment.NewLine);
                commandLog.Text = e.MessageString;
               
                server.Broadcast(e.MessageString); Thread.Sleep(10);
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Start")
            {
                System.Net.IPAddress ip = System.Net.IPAddress.Parse(stringHost.Text);
                server.Start(ip, Convert.ToInt32(stringPort.Text));
                stringLog.Text += "\nserver started - ip:" + server.GetIPAddresses().ToList().FirstOrDefault()+ "\n\n\n";
                button1.Text = "Stop";
            }
            else
            {
                button1.Text = "Start";
                stringLog.Text += "\nserver stoped";
                server.Stop();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void stringLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
        Commands commands = new Commands(server);
        private void commandLog_TextChanged(object sender, EventArgs e)
        {
            stringLog.Text += commands.ReadCommand(commandLog.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            server.AutoTrimStrings = true;
            server.Broadcast(textBox3.Text);
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                server.AutoTrimStrings = true;
                server.Broadcast(textBox3.Text);
            }
            
        }
    }
}
