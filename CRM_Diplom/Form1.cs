using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleTCP;

namespace CRM_Diplom
{
    public partial class Form1 : Form
    {
        public static SimpleTcpClient client;
        string clientUsername;
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;


            client.Connect(stringHost.Text, Convert.ToInt32(stringPort.Text));
            //client.Start(ip, Convert.ToInt32(stringPort.Text));

            //stringLog.Text += "\nserver started - ip:" + server.GetIPAddresses().ToList().FirstOrDefault();
            //button1.Text = "Stop";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new SimpleTcpClient();
            client.StringEncoder = Encoding.UTF8;
            client.DataReceived += Client_DataReceived;
        }

        private void ClientDataReceiver(object sender, SimpleTCP.Message e)
        {
            stringChat.Text +='\n'+e.MessageString;
        }

        private void Client_DataReceived(object sender, SimpleTCP.Message e)
        {
            stringChat.Invoke((MethodInvoker)delegate ()
            {
                stringChat.AppendText(Environment.NewLine);
                if (e.MessageString.Contains(clientUsername))
                {
                    stringChat.Text += e.MessageString.Substring(0, e.MessageString.Length - 1).Replace(clientUsername, "_me:");
                }
                else
                {
                    stringChat.Text += e.MessageString.Substring(0, e.MessageString.Length - 1);
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.WriteLineAndGetReply(stringMessage.Text, TimeSpan.Zero);
        }
        Commands commands = new Commands(client);
        private void button1_Click_1(object sender, EventArgs e)
        {

           
        }
        void SendCommand()
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            clientUsername = textBox1.Text;
            client.WriteLineAndGetReply(commands.Login(textBox1.Text), TimeSpan.Zero);

        }

        private void recievedMessage_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
