using Microsoft.Win32;
//using SimpleTCP;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Vlc.DotNet.Wpf;
using WpfVideo.Views;
using WpfVideo.Models;
using YoutubeExplode;
using NarutoPlayer;
using NarutoPlayer.Models;
using System.Net.Sockets;
using System.Threading;
using TcpClient = NetCoreServer.TcpClient;
//using TcpServer = NetCoreServer.TcpServer;
using NetCoreServer;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.Net;

namespace WpfVideo
{
    /// <summary>
    /// Логика взаимодействия для PlayerWindow.xaml
    /// </summary>
    /// 


    class ServerChatSession : TcpSession
    {
        public ServerChatSession(TcpServer server) : base(server) { }

        protected override void OnConnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} connected!");

            // Send invite message
            //string message = "Hello from TCP chat! Please send a message or '!' to disconnect the client!";
            //SendAsync(message);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            //Console.WriteLine("Incoming: " + message);
            
            // Multicast message to all connected sessions
            Server.Multicast(message);

            // If the buffer starts with '!' the disconnect the current session
            if (message == "!")
                Disconnect();
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP session caught an error with code {error}");
        }
    }

    class ChatServer : TcpServer
    {
        public ChatServer(IPAddress address, int port) : base(address, port) { }


        protected override TcpSession CreateSession() { return new ServerChatSession(this); }




        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP server caught an error with code {error}");
        }
    }

    public class ChatClient : TcpClient
    {
        ClientUser clientUser; string s;
        public ChatClient(string address, int port) : base(address, port) { }


        public void SetUser(ClientUser user)
        {
            clientUser = user;
            s = clientUser.ClientUserName;
        }

        public void DisconnectAndStop()
        {
            _stop = true;
            DisconnectAsync();
            while (IsConnected)
                Thread.Yield();
        }

        protected override void OnConnected()
        {

            SendAsync($"IMH|{clientUser.ClientUserName}");

        }
        public void WriteLine(string text)
        {
            SendAsync(text);

        }
        protected override void OnDisconnected()
        {

            if (!_stop)
                ConnectAsync();
        }
        protected virtual void OnRecievedMessageEvent(CustomEventArgs e)
        {

            EventHandler<CustomEventArgs> raiseEvent = RaiseCustomEvent;

            raiseEvent(this, e);

        }

        public event EventHandler<CustomEventArgs> RaiseCustomEvent;

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
          
            OnRecievedMessageEvent(new CustomEventArgs(buffer, offset, size));

        }



        protected override void OnError(SocketError error)
        {
            MessageBox.Show($"Chat TCP client caught an error with code {error}");
        }

        private bool _stop;

        internal void WriteLine(byte[] dataAsBytes)
        {
            SendAsync(dataAsBytes);
        }
    }

    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(byte[] buffer, long offset, long size)
        {
            Message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

        }


        public string Message { get; set; }
    }
    class Users
    {
        List<String> userlist = new List<string>(100);

        public void Add(string user)
        {
            userlist.Add(user);
        }
        public int Count()
        {
            return userlist.Count();
        }
        public List<String> GetUsers()
        {
            return userlist;
        }

    }
    public class ClientUser
    {
        public ClientUser(string message, string Id, string Ip)
        {
            ClientUserName = message;
            this.Id = Id;
            this.Ip = Ip;
        }


        public string ClientUserName { get; set; }
        public string Id { get; set; }
        public string Ip { get; set; }
    }

    public partial class PlayerWindow : Window
    {
        ClientUser clientUsername = new ClientUser("", "", "");

        private VlcControl control;
        public ChatClient _client;
        Commands command;
        DispatcherTimer timer = new DispatcherTimer();
        Room room = new Room();
        public PlayerWindow()
        {
            InitializeComponent();



            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

            var vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", /*IntPtr.Size == 4 ? "win-x86" : */"win-x64"));//ew DirectoryInfo(@"E:\Program Files\VideoLAN\VLC\");//new DirectoryInfo(System.IO.Path.Combine(".", "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));




            this.control?.Dispose();
            this.control = new VlcControl();
            this.controlPanel.Content = this.control;
            this.control.SourceProvider.CreatePlayer(vlcLibDirectory);
            logShowHide.IsEnabled = false;
            // appShortcutToDesktop();
            volumeSlider2.Maximum = 100;
            volumeSlider2.Value = 50;
            control.SourceProvider.MediaPlayer.Audio.Volume = 0;
            playPause.IsEnabled = false;
            speedPanel.Visibility = Visibility.Collapsed;
            //Submit.Visibility = Visibility.Collapsed;
            postionSlider2.AddHandler(Slider.PreviewMouseDownEvent, new MouseButtonEventHandler(SliderMauseDown), true);
            postionSlider2.AddHandler(Slider.PreviewMouseUpEvent, new MouseButtonEventHandler(SliderMauseUp), true);

        }
        private const string StreamParams = ":network-caching=2000";


        bool isPlayerStarted = false;
        void StartPlayer(string flnm = "null")
        {

            //string link = flnm;
            //foreach (var item in filesListbox.Items)
            //{
            //    if ((item as Video).FileName == flnm)
            //    {
            //        flnm = (item as Video).FullPath + "/" + (item as Video).FileName;

            //    }
            //}



            this.Title = selectedVideoName + " | " + login.hostString.Text + ":" + login.portString.Text + " " + clientUsername.ClientUserName;
            if (control.SourceProvider.MediaPlayer.IsPlaying() == true)
            {
                control.SourceProvider.MediaPlayer.Pause();
                isPlaying = false;
            }



            isPlayerStarted = true;
            novidGrid.Visibility = Visibility.Collapsed;
            logPanel.Visibility = Visibility.Collapsed;
            controlPanel.Visibility = Visibility.Visible;
            flnm = flnm.Replace("/", @"\");
            string fn = "";




            this.control.SourceProvider.MediaPlayer.Log += (_, args) =>
            {
                string message = $"libVlc : {args.Level} {args.Message} @ {args.Module}";
                System.Diagnostics.Debug.WriteLine(message);
                stringChat.Text += message + '\n';
                _client.WriteLine(message);
            };


            {

                //if (!string.IsNullOrEmpty(videoPath))
                //{
                //    fn = "file:///" + videoPath.Replace("/", @"\");

                //}
                //else

                if (flnm == "null" || File.Exists(videosFolder + "/" + selectedVideo.Text))
                {
                    fn = videosFolder + "/" + selectedVideo.Text;
                    fn = "file:///" + fn.Replace("/", @"\");
                    //MessageBox.Show(fn);
                }


                string subFile = flnm.Substring(0, flnm.Length - 3) + "srt";
                var options = new string[]
                    {
                         new string("--sub-File="+subFile)
               // VLC options can be given here. Please refer to the VLC command line documentation.
                      };
                if (File.Exists(subFile))
                {
                    control.SourceProvider.MediaPlayer.SetMedia(fn, options);
                }
                else
                {
                    control.SourceProvider.MediaPlayer.SetMedia(fn);
                    //MessageBox.Show(fn);
                }

                //  myVlcControl.SetMedia(@"file:///E:\movies\watchman.mp4", new string[] { @"--sub-file=E:\movies\watchman.srt" });
                //  control.SourceProvider.MediaPlayer.SubTitles.Current = control.SourceProvider.MediaPlayer.SubTitles.All.First(o => o.ID == 0);
            }
            playPause.IsEnabled = true;


            control.SourceProvider.MediaPlayer.Position = 0;
            control.SourceProvider.MediaPlayer.Pause();
            if (control.SourceProvider.MediaPlayer.Audio.Volume < 1)
            {
                control.SourceProvider.MediaPlayer.Audio.Volume = 50;
                volumeSlider2.Value = 50;
            }
            _client.SendAsync("PLAY|");

        }

        //private void 
        //{
        //    YoutubeClient youtube = new YoutubeClient();
        //    var trackManifest = async youtube.Videos.ClosedCaptions.GetManifestAsync("u_yIGGhubZs");

        //    Select a closed caption track in English
        //   var trackInfo = trackManifest.TryGetByLanguage("en");

        //    var track = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo);

        //    youtube.Videos.ClosedCaptions
        //       var youtubeVidId = YoutubeClient.ParseVideoId(flnm);
        //    var yt = new YoutubeClient();
        //    var video = await yt.GetVideoMediaStreamInfosAsync(youtubeVidId);
        //    var muxed = video.Muxed.WithHighestVideoQuality();

        //    // control.SourceProvider.MediaPlayer.SetMedia("http://sfrom.net/https://youtu.be/LPG4tA5vsU8", StreamParams);

        //}
        private async void YTStream(string link)
        {
            // var url = "https://www.youtube.com/watch?v=OBk3k-PKpKc";
            //var youtubeVidId = YoutubeClient.ParseVideoId(url);

            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();

            var yt = new YoutubeClient(httpClient);
            //yt.Videos.ClosedCaptions.GetAsync(url)
            //var video = await yt.Videos.GetAsync(url);//.GetVideoMediaStreamInfosAsync(youtubeVidId);
            var streamManifest = await yt.Videos.Streams.GetManifestAsync(link);
            var q720 = YoutubeExplode.Videos.Streams.VideoQuality.High720;
            var q1080 = YoutubeExplode.Videos.Streams.VideoQuality.High720;
            var streamInfo = "";

            if (streamManifest.GetMuxed().Where(x => x.VideoQuality == q1080).FirstOrDefault() != null)
            {
                streamInfo = streamManifest.GetMuxed().Where(x => x.VideoQuality == q1080).FirstOrDefault().Url;
            }
            else
            if (streamManifest.GetMuxed().Where(x => x.VideoQuality == q720).FirstOrDefault() != null)
            {
                streamInfo = streamManifest.GetMuxed().Where(x => x.VideoQuality == q720).FirstOrDefault().Url;
            }
            else
            {
                streamInfo = streamManifest.GetMuxed().FirstOrDefault().Url;
            }

            control.SourceProvider.MediaPlayer.SetMedia(new Uri(streamInfo));

        }

        bool changePos = false;


        bool isPlaying = false;
        public bool IsPlaying { get { return isPlaying; } set { isPlaying = value; } }


        double videoPosition;
        public double VideoPosition { get { return videoPosition; } set { videoPosition = value; } }
        public void VideoPositionChange(long position)
        {
            changePos = true;
            this.control.SourceProvider.MediaPlayer.Time = position;
            changePos = false;
        }

        string GetCorrectTime(long ms)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds);

        }
        int tickCounter = 0;
        void timer_Tick(object sender, EventArgs e)
        {
            //if (fileNameTextBox.Text != selectedVideo.Text)
            //    Submit.IsEnabled = false;
            //else 5
            //Submit.IsEnabled = true;
            //MessageBox.Show(tickCounter.ToString());
            if (++tickCounter > 15 && logPanel.Visibility != Visibility.Visible)
            {
                buttonsPanel.Visibility = Visibility.Hidden; this.Cursor = Cursors.None;
                tickCounter = 0;

            }

            tb1.Text = GetCorrectTime(this.control.SourceProvider.MediaPlayer.Length);//// this.control.SourceProvider.MediaPlayer.Length + " ms";

            this.postionSlider2.Maximum = this.postionSlider.Maximum = this.control.SourceProvider.MediaPlayer.Length;

            videoPosition = this.postionSlider2.Value = this.control.SourceProvider.MediaPlayer.Time;
            tb2.Text = GetCorrectTime(this.control.SourceProvider.MediaPlayer.Time);

            if (!changePos)
            {
                this.postionSlider2.Value = videoPosition;
            }
            else
            {
                this.control.SourceProvider.MediaPlayer.Time = (long)this.postionSlider2.Value;
            }

            if (isPlaying != this.control.SourceProvider.MediaPlayer.IsPlaying())
            {
                if (isPlaying)
                    this.control.SourceProvider.MediaPlayer.Play();
                else
                    this.control.SourceProvider.MediaPlayer.Pause();
            }

            ClientTick();

        }
        void ClientTick()
        {


            // MessageBox.Show("");


            //_client.SendAsync("PPB|"+ player.IsPlaying.ToString(),TimeSpan.FromSeconds(0.1));
            //if (player.Isst)
            //{

            //}

            //if (changePos)
            //{
            //    _client.SendAsync("VPC|" + VideoPosition, TimeSpan.FromSeconds(1));
            //}

        }
        private void playPause_Click(object sender, RoutedEventArgs e)
        {
            PlayPause();
        }
        void PlayPause()
        {
            if (this.control.SourceProvider.MediaPlayer.IsPlaying())
            {
                //this.control.SourceProvider.MediaPlayer.Pause();
                //isPlaying = false;
                //isPlayerStarted = false;
                _client.SendAsync("PAUSE|");

                _client.SendAsync("POS|" + Math.Round(postionSlider2.Value).ToString());

            }
            else
            {
                //this.control.SourceProvider.MediaPlayer.Play();
                //isPlaying = true;
                //isPlayerStarted = true;
                _client.SendAsync("PLAY|");

            }
        }
        private void postioonSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void postioonSlider_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void postioonSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }


        public int a = 0;
        public int c = 0;
        public delegate void UpdateControlsDelegate(); //Execute when video loads
        string lastCommand = "";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    _client.ReceivedMessage += Client_DataReceived2;
            //}
            //catch
            //{

            //}

        }

        //private void _client_RaiseCustomEvent(object sender, CustomEventArgs e)
        //{
        //    MessageBox.Show($"{clientUsername} received this message: {e.Message}");
        //}

        //private void _client_ThresholdReached(byte[] buffer)
        //{
        //    MessageBox.Show(Encoding.UTF8.GetString(buffer));

        //}

        //private void _client_ReceivedEvent(byte[] buffer, long offset, long size)
        //{
        //  //  throw new NotImplementedException();


        //}

        void SendUserInfo()
        {
            List<string> data = usershost;

            byte[] dataAsBytes = data
              .SelectMany(s => Encoding.UTF8.GetBytes("USERS|" + s))
              .ToArray();
            _server.Multicast(dataAsBytes);
        }
        List<String> usershost = new List<string>();
        private void _client_RaiseCustomEvent(object sender, CustomEventArgs e)
        {

            stringChat.Dispatcher.Invoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
                if (isHost)
                {
                    if (e.Message.Contains("IMH|"))
                    {
                        string[] tmp = e.Message.Replace("IMH|", "").Split(' ');

                        usershost.Add(tmp[0] + "  |-{|");

                        SendUserInfo();
                    }
                    if (e.Message.Contains("IMO|"))
                    {
                        string tmp = e.Message.Replace("IMO|", "");

                        for (int n = usershost.Count - 1; n >= 0; --n)
                        {

                            if (usershost[n].ToString() == (tmp) + "  |-{|")
                            {
                                usershost.RemoveAt(n);
                            }
                        }

                        SendUserInfo();
                    }
                }
                string usrn = e.Message.Split(' ')[0].Substring(0, e.Message.Split(' ')[0].Length);
                string message = string.Empty;
                switch (e.Message.Split('|')[0])
                {
                    case "USERS":

                        string[] tmp = e.Message.Replace("USERS|", "").Split("  |-{|").Where(c => c != null && c != "").ToArray();
                        if (true)
                        {
                            userList.ItemsSource = tmp;
                        }
                        break;
                    case "MSG":
                        message = e.Message.Replace("MSG|", "");

                        stringChat.Text = message;
                        break;
                    case "ADDLINK":

                        Video video = new Video("", e.Message.Split('|')[1]);
                        if (!filesListbox.Items.Contains(video))
                        {
                            filesListbox.Items.Add(video);
                        }

                        break;
                    case "START":
                        StartPlayer(selectedVideo.Text);

                        break;
                    case "PLAY":
                        if (isPlaying == false)
                        {
                            this.control.SourceProvider.MediaPlayer.Play();
                            isPlaying = true;
                            isPlayerStarted = true;
                        }
                        break;
                    case "PAUSE":
                        if (isPlaying)
                        {
                            this.control.SourceProvider.MediaPlayer.Pause();
                            isPlaying = false;
                            isPlayerStarted = false;
                        }
                        break;
                    case "RATE":
                        room.speed = e.Message.Split('|')[1];
                        this.control.SourceProvider.MediaPlayer.Rate = float.Parse(room.speed, CultureInfo.InvariantCulture.NumberFormat);

                        break;
                    case "POS":
                        if (!changePos && lastCommand != e.Message)
                        {
                            changePos = true;
                            // this.control.SourceProvider.MediaPlayer.Pause();
                            this.postionSlider2.Value = double.Parse(e.Message.Split('|')[1]);
                            if (this.control.SourceProvider.MediaPlayer.Time != (long)this.postionSlider2.Value)
                            {
                                this.control.SourceProvider.MediaPlayer.Time = (long)this.postionSlider2.Value;
                            }
                            changePos = false;
                        }
                        break;
                    case "ROOM":
                        room.SetRoom(e.Message);
                        if (!isPlayerStarted)
                        {
                            Opf();
                            selectedVideo.Text = room.selectedVideo;
                            this.Title = room.selectedVideo;
                            this.control.SourceProvider.MediaPlayer.Rate = float.Parse(room.speed, CultureInfo.InvariantCulture.NumberFormat);
                        }


                        break;

                    default:
                        break;
                }

                //MessageBox.Show(e.Message);

                if (e.Message.Split(' ')[0] == "/set")
                {

                    string msg = e.Message.Replace("/set ", "").Replace(e.Message.Split(' ')[1], "");
                    //selectedVideoName = string.Empty;
                    //selectedVideo.Text = string.Empty;
                    selectedVideoName = msg.Trim();
                    // this.Title = e.Message;
                    selectedVideo.Text = msg.Trim();




                }
                //if (e.Message.Split(' ')[0] == "/alert")
                //{
                //    message = e.Message.Replace("/alert", "");
                //    string msg = "";

                //    if (clientUsername.ClientUserName == message.Trim().Split(' ')[0])
                //    {
                //        for (int i = 2; i < e.Message.Split(' ').Length; i++)
                //        {
                //            msg += e.Message.Split(' ')[i].Trim() + " ";

                //        }
                //        MessageBox.Show(msg.Trim());
                //    }

                //}
                //if (e.Message.Split(' ')[0] == "/kick")
                //{
                //    message = e.Message.Replace("/kick", "");
                //    string msg = "";

                //    if (clientUsername.ClientUserName == message.Trim().Split(' ')[0])
                //    {

                //        for (int i = 2; i < e.Message.Split(' ').Length; i++)
                //        {
                //            msg += e.Message.Split(' ')[i].Trim() + " ";
                //        }
                //        MessageBox.Show(msg.Trim());
                //        this.Close();
                //    }
                //}


                stringChat.Select(stringChat.Text.Length, 0);
            });
        }


        string videoPath;
        string videosFolder = "";
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.control.SourceProvider.MediaPlayer.Pause();
            isPlaying = false;
            //isPlayerStarted = false; 
            //if (client.TcpClient != null)
            //{
            //    _client.SendAsync("PAUSE|" );
            //}

            Opf();


            //OpenFileDialog openFile = new OpenFileDialog();
            //openFile.ShowDialog();
            //videoPath = openFile.FileName.Replace(@"\", "/");
            //fileNameTextBox.Text = videoPath.Split('/')[videoPath.Split('/').Length - 1];
            //_client.SendAsync("
            // " + clientUsername + " " + fileNameTextBox.Text );


            //if (commandLine.Text == "set")
            //{
            //    _client.SendAsync("MSG|" + clientUsername + " поставил " + fileNameTextBox.Text );
            //}





        }
        void Opf()
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {


                System.Windows.Forms.DialogResult result = fbd.ShowDialog();


                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    videosFolder = fbd.SelectedPath;
                    string[] files = Directory.GetFiles(fbd.SelectedPath);
                    filesListbox.Items.Clear();
                    string str = "";
                    foreach (string a in files)
                    {
                        str = a.Replace(@"\", "/").Split('/')[a.Replace(@"\", "/").Split('/').Length - 1];
                        Video video = new Video(videosFolder, str);
                        if (str.Substring(str.Length - 3) == "mkv" || str.Substring(str.Length - 3) == "mp4")
                        {
                            filesListbox.Items.Add(video);
                        }

                    }
                    //filesListbox.ItemsSource = files;
                    //MessageBox.Show("Files found: " + files[1].ToString(), "Message");
                }
            }

        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            //player = new PlayerWindow(videoPath);
            //player.Show();
            //this.Hide();
            //player.playPause.Content="sex";


        }
        //void SubmitB()
        //{
        //    if ((filesListbox.SelectedItem as Video).FileName.Trim() == selectedVideo.Text.Trim() && filesListbox.Items.Count > 0)
        //    {



        //        //StartPlayer(videoPath);

        //    }
        //}
        private void commandLine_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (e.Key == Key.Return)
            //{
            //    _client.SendAsync("/" + commandLine.Text + " " + clientUsername + " " + selectedVideoName );
            //     
            //    if (commandLine.Text == "set")
            //    {
            //        _client.SendAsync("MSG|" + clientUsername + " поставил " + selectedVideoName ); 
            //         
            //    }
            //}
        }

        private void postioonSlider2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void SliderMauseDown(object sender, MouseButtonEventArgs e)
        {
            changePos = true;
            this.control.SourceProvider.MediaPlayer.Time = (long)Math.Round(postionSlider2.Value);
            // MessageBox.Show("Down");
        }
        string lstpos = "";
        private void SliderMauseUp(object sender, MouseButtonEventArgs e)
        {
            changePos = false;

            string str = "POS|" + Math.Round(postionSlider2.Value).ToString();

            //MessageBox.Show(lastCommand);
            this.control.SourceProvider.MediaPlayer.Time = (long)Math.Round(postionSlider2.Value);
            if (str != lastCommand)
            {
                if (lstpos != str)
                {
                    _client.SendAsync(str);

                }
                //   
            }
            lstpos = str;



            //MessageBox.Show("Up");
        }

        private void postioonSlider2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            //changePos = true;
            //tb1.Text = postioonSlider2.Value.ToString();

        }

        private void postioonSlider2_DragEnter(object sender, DragEventArgs e)
        {
            //    MessageBox.Show("");
        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {
            //  Forward();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            // Back();
            // SetCurrentTime.Content = this.control.SourceProvider.MediaPlayer.Time + " ms";
        }
        void Back()
        {
            if (this.control == null)
            {
                return;
            }
            changePos = false;
            if (this.control.SourceProvider.MediaPlayer.Time - 5000 > 0)
            {
                this.control.SourceProvider.MediaPlayer.Time = this.control.SourceProvider.MediaPlayer.Time - 5000;

            }
            else
            {
                this.control.SourceProvider.MediaPlayer.Time = 0;
            }
            changePos = true;

        }
        void Forward()
        {
            if (this.control == null)
            {
                return;
            }
            changePos = false;
            if (this.control.SourceProvider.MediaPlayer.Time + 5000 < this.control.SourceProvider.MediaPlayer.Length)
            {
                this.control.SourceProvider.MediaPlayer.Time = this.control.SourceProvider.MediaPlayer.Time + 5000;
            }
            else
            {
                this.control.SourceProvider.MediaPlayer.Time = this.control.SourceProvider.MediaPlayer.Length;
            }
            changePos = true;
        }
        bool isHost = false;
        private void Speed_Click(object sender, RoutedEventArgs e)
        {
            string s = (sender as Button).Content.ToString();

            //this.control.SourceProvider.MediaPlayer.Rate = float.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
            _client.SendAsync("RATE|" + s);

            speedPanel.Visibility = Visibility.Collapsed;

        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {

            if (speedPanel.Visibility == Visibility.Collapsed)
                speedPanel.Visibility = Visibility.Visible;
            else
                speedPanel.Visibility = Visibility.Collapsed;

        }
        LoginWindow login;
        ChatServer _server;
        private void connectBtn_Click(object sender, RoutedEventArgs e)
        {
            login = new LoginWindow();
            login.ShowDialog();




            if (login.acceptBtn.Content.ToString() == "host")
            {
                isHost = true;
                IPAddress iPAddress = IPAddress.Parse(login.hostString.Text);
                _server = new ChatServer(iPAddress, int.Parse(login.portString.Text));
                 //_client = new ChatClient(_server);
                
                _server.Start();
            }
            else
            {
            }



                _client = new ChatClient(login.hostString.Text, int.Parse(login.portString.Text));

            clientUsername = new ClientUser(login.usernameString.Text.Replace(" ", "_"), _client.Id.ToString(), _client.Endpoint.Address.ToString());
            _client.SetUser(clientUsername);

            _client.ConnectAsync();



            if (!_client.IsConnected)
            {
                _client.RaiseCustomEvent += _client_RaiseCustomEvent;
                connectBtn.Visibility = Visibility.Collapsed;
                logPanel.Visibility = Visibility.Visible;
                controlPanel.Visibility = Visibility.Collapsed;
                timer.Interval = TimeSpan.FromSeconds(0.1);
                timer.Tick += timer_Tick;
                logShowHide.IsEnabled = true;
                timer.Start();
            }
            else
            {
                System.Windows.MessageBox.Show("Сервер не отвечает");
            }





        }

        private void controlPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void stringChat_TextChanged(object sender, TextChangedEventArgs e)
        {
            //   stringChat.Text += "\n";
        }

        private void logShowHide_Click(object sender, RoutedEventArgs e)
        {
            if (logPanel.Visibility == Visibility.Visible)
                logPanel.Visibility = Visibility.Collapsed;
            else logPanel.Visibility = Visibility.Visible;
            //controlPanel.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            //controlPanel.Height = System.Windows.SystemParameters.PrimaryScreenHeight;



            // SyncDockingManager.MaximizeMode = MaximizeMode.FullScreen;

        }

        private void controlPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F)
            {
                MaximizeWindow();
            }
            if (e.Key == Key.Space)
            {
                PlayPause();
            }
            if (e.Key == Key.D1)
            {
                buttonsPanel.Visibility = Visibility.Visible;
            }
            if (e.Key == Key.D2)
            {
                buttonsPanel.Visibility = Visibility.Hidden;

            }
            if (e.Key == Key.Left)
            {
                _client.SendAsync("POS|" + (Math.Round(postionSlider2.Value) - 5000).ToString());
            }
            if (e.Key == Key.Right)
            {
                _client.SendAsync("POS|" + (Math.Round(postionSlider2.Value) + 5000).ToString());
            }
            // MessageBox.Show(e.Key.ToString());
        }

        private void controlPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.Cursor = Cursors.Arrow;
                tickCounter = 0;
                this.DragMove();
            }
            if (e.ChangedButton == MouseButton.XButton2)
            {
                //IsPlaying = false;
                //isPlayerStarted = false;
                //control.SourceProvider.MediaPlayer.Pause();
                //_client.SendAsync("PAUSE|");

                NextVideo();

            }
            //  MessageBox.Show( e.ChangedButton.ToString());
        }
        void NextVideo()
        {
            if (filesListbox.SelectedIndex + 1 <= filesListbox.Items.Count)
            {
                filesListbox.SelectedIndex = filesListbox.SelectedIndex + 1;
                //Setter();
                selectedVideoName = ((filesListbox).SelectedItem as Video).FileName;
                Thread.Sleep(1000);
                //selectedVideoPath = ((filesListbox).SelectedItem as Video).FullPath;
                _client.Send("/set " + clientUsername.ClientUserName + " " + selectedVideoName);
                //videoPath = selectedVideoPath + "/" + selectedVideoName;


                Thread.Sleep(1000);
                _client.SendAsync("START|");
                //PlayPause();
            }
        }
        void SubmitXB()
        {
            // StartPlayer(videoPath);

        }
        private void controlPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MaximizeWindow();
        }
        private void MaximizeWindow()
        {
            if (this.WindowStyle == WindowStyle.SingleBorderWindow)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;

            }
        }

        private void novidGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void volumeSlider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            control.SourceProvider.MediaPlayer.Audio.Volume = (int)volumeSlider2.Value;
            volumeTextBlock.Text = (int)volumeSlider2.Value + "%";

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // __client.SendAsync("IMO|" + clientUsername);
            _client.SendAsync("IMO|" + clientUsername.ClientUserName);
            _client.DisconnectAndStop();
            if (isHost)
            {
                _server.Multicast("!");

            }

        }

        private void fileNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void commandLine_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PlayPause();
        }

        private void controlPanel_MouseMove(object sender, MouseEventArgs e)
        {
            buttonsPanel.Visibility = Visibility.Visible;
            tickCounter = 0;
            this.Cursor = Cursors.Arrow;

        }

        private void filesListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // MessageBox.Show(videosFolder+ (sender as ListBox).SelectedItem.ToString());
            Setter();

        }

        private void setBtn_Click(object sender, RoutedEventArgs e)
        {


        }

        private void filesListbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {


            //SubmitB();
        }
        string selectedVideoName = "";
        string selectedVideoPath = "";
        void Setter()
        {
            selectedVideoName = ((filesListbox).SelectedItem as Video).FileName;
            selectedVideoPath = ((filesListbox).SelectedItem as Video).FullPath;
            videoPath = selectedVideoPath + "/" + selectedVideoName;

            _client.Send("/set " + clientUsername.ClientUserName + " " + selectedVideoName);


            //room.selectedVideo = selectedVideoName;

            //     _client.SendAsync("MSG|" + clientUsername + " поставил " + selectedVideoName );
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenLinkDialog openLinkDialog = new OpenLinkDialog();
            openLinkDialog.ShowDialog();
            Video video = new Video("", openLinkDialog.linkString.Text);

            _client.SendAsync("ADDLINK|" + video.FileName);

            // filesListbox.Items.Add(video);
        }

        private void MIStart_Click(object sender, RoutedEventArgs e)
        {
            _client.SendAsync("START|");
        }
    }
}
