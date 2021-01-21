
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfVideo.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        string clientUsername;
        Commands command ;
        PlayerWindow player=null;

        public LoginWindow()
        {
            InitializeComponent();
        }
        bool isConnected = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (acceptBtn.Content.ToString() == "host")
            {

            }
            else
            {

            }

            try
            {
               // command = new Commands();
                //client.Connect(hostString.Text, Convert.ToInt32(portString.Text));
                //clientUsername = usernameString.Text;
                //client.WriteLineAndGetReply(command.Login(clientUsername), TimeSpan.Zero);
                //client.WriteLineAndGetReply("\n", TimeSpan.Zero);
                //
                this.Close();
            //    loginGrid.Visibility = Visibility.Collapsed;
            //    panelGrid.Visibility = Visibility.Visible;
                
            //    DispatcherTimer timer = new DispatcherTimer();
            //    timer.Interval = TimeSpan.FromSeconds(0.5);
            //    timer.Tick += timer_Tick;
            //    timer.Start();
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.ToString());
                System.Windows.MessageBox.Show("вронг");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //client = new SimpleTcpClient();
            //client.StringEncoder = Encoding.UTF8;
            //client.DataReceived += Client_DataReceived;

            


        }
        void timer_Tick(object sender, EventArgs e)
        {



            if (player != null)
            {
                //client.WriteLineAndGetReply("IMH|" + clientUsername + " " + player.IsPlaying.ToString() + "  \n", TimeSpan.FromSeconds(5));

                //client.WriteLineAndGetReply("PPB|"+ player.IsPlaying.ToString(),TimeSpan.FromSeconds(0.1));
                //if (player.Isst)
                //{

                //}

                //if (player.IsPositionChanged)
                //{
                //    client.WriteLineAndGetReply("VPC|" + player.VideoPosition, TimeSpan.FromSeconds(0.2));
                //}
            }




        }
        //private void Client_DataReceived(object sender, SimpleTCP.Message e)
        //{
        //    string usrn = e.MessageString.Split(' ')[0].Substring(0, e.MessageString.Split(' ')[0].Length - 1);
        //    stringChat.Dispatcher.Invoke((MethodInvoker)delegate ()
        //    {
        //        if (e.MessageString.Split('|')[0] == "MSG")
        //        {
        //            string message = e.MessageString.Replace("MSG|", "");
        //            //stringChat.AppendText(Environment.NewLine);
                    

        //                //   stringChat.Text += message.Split(' ')[0].Replace(":", "") // message.Substring(0, message.Length - 1).Replace(clientUsername, "_me");
        //                message = message.Replace(clientUsername + ':', "me");
        //                if (message.Contains("подлупился."))
        //                {
        //                    userList.Items.Add(clientUsername.Trim());
        //                    if (commandLine.Text =="/set")
        //                    {
        //                        client.WriteLineAndGetReply(commandLine.Text + " " + clientUsername + " " + fileNameTextBox.Text, TimeSpan.Zero);
        //                    }
        //                }
        //                stringChat.Text += message;
                  
        //        }
        //        if (e.MessageString.Split('|')[0] == "SET")
        //        {
        //            selectedVideo.Text = "";
        //            string message = e.MessageString.Replace("SET|", "");
        //            selectedVideo.Text = message;

        //        }
        //        if (e.MessageString.Split('|')[0] == "VPC")
        //        {
        //            selectedVideo.Text = "";
        //            string message = e.MessageString.Replace("VPC|", "");
        //            long p = long.Parse(message);
        //            player.VideoPositionChange(p);

        //        }

        //        //else
        //        //{
        //        //    client.WriteLineAndGetReply(command.ReadCommand(e.MessageString, clientUsername), TimeSpan.Zero);
        //        //}

        //        if (!userList.Items.Contains(command.GetUsers(e.MessageString)) && command.GetUsers(e.MessageString) != "")
        //        {
        //            userList.Items.Add(command.GetUsers(e.MessageString).Trim());

        //            string[] arr = new string[userList.Items.Count];
        //            userList.Items.CopyTo(arr, 0);

        //            var arr2 = arr.Distinct();

        //            userList.Items.Clear();
        //            foreach (string s in arr2)
        //            {
        //                userList.Items.Add(s);
        //            }

        //        }

        //        //stringChat.Focus();
        //        // Move the caret to the end of the text box
        //        stringChat.Select(stringChat.Text.Length, 0);
        //    });
        //}
        string videoPath;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFile = new OpenFileDialog();
            //openFile.ShowDialog();
            //videoPath = openFile.FileName.Replace(@"\", "/");
            //fileNameTextBox.Text = videoPath.Split('/')[videoPath.Split('/').Length-1]; ;
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            //player = new PlayerWindow(videoPath);
            //player.Show();
            //this.Hide();
            //player.playPause.Content="sex";
        }

        private void commandLine_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                //client.WriteLineAndGetReply(commandLine.Text + " "+clientUsername+" " + fileNameTextBox.Text, TimeSpan.Zero);
            }
        }

        private void hostBtn_Click(object sender, RoutedEventArgs e)
        {
            loginGrid.Visibility = Visibility.Visible;
            connectBtn.Visibility = Visibility.Collapsed;
            hostBtn.Visibility = Visibility.Collapsed;
            backBtn.Visibility = Visibility.Visible;
            acceptBtn.Visibility = Visibility.Visible;
            acceptBtn.Content = "host";
        }

        private void connectBtn_Click(object sender, RoutedEventArgs e)
        {
            loginGrid.Visibility = Visibility.Visible;
            connectBtn.Visibility = Visibility.Collapsed;
            hostBtn.Visibility = Visibility.Collapsed;
            backBtn.Visibility = Visibility.Visible;
            acceptBtn.Visibility = Visibility.Visible;
           acceptBtn.Content = "connect";
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            loginGrid.Visibility = Visibility.Collapsed;
            connectBtn.Visibility = Visibility.Visible;
            hostBtn.Visibility = Visibility.Visible;
            backBtn.Visibility = Visibility.Collapsed;
            acceptBtn.Visibility = Visibility.Collapsed;


        }
    }
}
