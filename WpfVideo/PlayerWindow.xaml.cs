using NarutoPlayer;
using NarutoPlayer.Models;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VideoLibrary;
using Vlc.DotNet.Wpf;
using WpfVideo.Views;
using YoutubeExplode;
using Video = WpfVideo.Models.Video;

namespace WpfVideo
{
    internal class ServerChatSession : TcpSession
    {
        public ServerChatSession(TcpServer server) : base(server) { }

        protected override void OnConnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} connected!");

        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

            Server.Multicast(message);

            if (message == "!")
            {
                Disconnect();
            }
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP session caught an error with code {error}");
        }
    }

    internal class ChatServer : TcpServer
    {
        public ChatServer(IPAddress address, int port) : base(address, port) { }
        protected override TcpSession CreateSession() { return new ServerChatSession(this); }
        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"error code: {error}");
        }
    }

    public class ChatClient : NetCoreServer.TcpClient
    {

        public ChatClient(string address, int port) : base(address, port) { }

        private ClientUser clientUser;
        private string s;
        public void SetUser(ClientUser user)
        {
            clientUser = user;
            s = clientUser.ClientUserName;
        }
        protected override void OnConnected()
        {

            SendAsync($"IMH|{clientUser.ClientUserName}");

        }

        public void DisconnectAndStop()
        {
            _stop = true;
            Disconnect();
            while (IsConnected)
            {
                Thread.Yield();
            }
        }
        public void WriteLine(string text)
        {
            SendAsync(text);

        }
        protected override void OnDisconnected()
        {

            if (!_stop)
            {
                Connect();
            }
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
            ReceiveAsync();
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
        public CustomEventArgs(byte[] buffer)
        {
            Message = Encoding.UTF8.GetString(buffer);

        }
        public CustomEventArgs(string message)
        {
            Message = message;

        }
        public string Message { get; set; }
    }

    internal class Users
    {
        private readonly List<string> userlist = new List<string>(100);

        public void Add(string user)
        {
            userlist.Add(user);
        }
        public int Count()
        {
            return userlist.Count();
        }
        public List<string> GetUsers()
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
        private ClientUser clientUsername = new ClientUser("", "", "");

        private readonly VlcControl control;
        public ChatClient _client;
        private readonly Commands command;
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly Room room = new Room();
        public PlayerWindow()
        {
            InitializeComponent();

            Assembly currentAssembly = Assembly.GetEntryAssembly();
            string currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

            DirectoryInfo vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", /*IntPtr.Size == 4 ? "win-x86" : */"win-x64"));//ew DirectoryInfo(@"E:\Program Files\VideoLAN\VLC\");//new DirectoryInfo(System.IO.Path.Combine(".", "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            control = new VlcControl();
            controlPanel.Content = control;
            control.SourceProvider.CreatePlayer(vlcLibDirectory);
            logShowHide.IsEnabled = false;

            volumeSlider2.Maximum = 100;
            volumeSlider2.Value = 50;
            control.SourceProvider.MediaPlayer.Audio.Volume = 0;
            playPause.IsEnabled = false;
            speedPanel.Visibility = Visibility.Collapsed;

            postionSlider2.AddHandler(Slider.PreviewMouseDownEvent, new MouseButtonEventHandler(SliderMauseDown), true);
            postionSlider2.AddHandler(Slider.PreviewMouseUpEvent, new MouseButtonEventHandler(SliderMauseUp), true);

        }
        private const string StreamParams = ":network-caching=2000";
        private bool isPlayerStarted = false;

        private void StartPlayer(string flnm = "null")
        {
            Title = selectedVideoName + " | " + login.hostString.Text + ":" + login.portString.Text + " " + clientUsername.ClientUserName;
            if (control.SourceProvider.MediaPlayer.IsPlaying() == true)
            {
                control.SourceProvider.MediaPlayer.Pause();
                isPlaying = false;
            }

            isPlayerStarted = true;
            novidGrid.Visibility = Visibility.Collapsed;
            logPanel.Visibility = Visibility.Collapsed;
            controlPanel.Visibility = Visibility.Visible;
            if (flnm.Contains("https"))
            {
                if (flnm.Contains("youtube.com") || flnm.Contains("youtu.be"))
                {
                    YTStream(flnm);
                }
                else if (flnm.Contains("vimeo.com"))
                {
                    VimeoStream(flnm);
                }

            }
            else
            {

                flnm = flnm.Replace("/", @"\");
                string fn = "";

                if (flnm == "null" || File.Exists(videosFolder + "/" + selectedVideo.Text))
                {
                    fn = videosFolder + "/" + selectedVideo.Text;
                    fn = "file:///" + fn.Replace("/", @"\");
                }

                string subFile = flnm.Substring(0, flnm.Length - 3) + "srt";
                string[] options = new string[]
                    {
                         new string("--sub-File="+subFile)
                    };
                if (File.Exists(subFile))
                {
                    control.SourceProvider.MediaPlayer.SetMedia(fn, options);
                }
                else
                {
                    control.SourceProvider.MediaPlayer.SetMedia(fn);
                }
            }
            playPause.IsEnabled = true;
            control.SourceProvider.MediaPlayer.Position = 0;
            control.SourceProvider.MediaPlayer.Pause();
            if (control.SourceProvider.MediaPlayer.Audio.Volume < 1)
            {
                control.SourceProvider.MediaPlayer.Audio.Volume = 50;
                volumeSlider2.Value = 50;
            }
            client.Send("PLAY|");

        }

        private string tmp = "";
        private async void VimeoStream(string link)
        {

            tmp = link;

            await DoSomething();

        }
        public async Task DoSomething()
        {
            string x = await Application.Current.Dispatcher.Invoke<Task<string>>(
                DoSomethingWithUIAsync);

            control.SourceProvider.MediaPlayer.SetMedia(x.ToString());

        }

        public async Task<string> DoSomethingWithUIAsync()
        {
            DownloadItem downloadItem = new DownloadItem
            {
                SourceUrl = tmp
            };
            downloadItem.Start();
            while (string.IsNullOrWhiteSpace(downloadItem.Link))
            {
                await Task.Delay(10);
            }

            return downloadItem.Link;
        }
        private async void YTStream(string link)
        {

            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();

            YoutubeClient yt = new YoutubeClient(httpClient);
            YoutubeExplode.Videos.Streams.StreamManifest streamManifest;
            try
            {
                streamManifest = await yt.Videos.Streams.GetManifestAsync(link);
                YouTube youtube = YouTube.Default;
                YouTubeVideo vid = youtube.GetVideo(link);

                string vidUri = vid.Uri;
                YoutubeExplode.Videos.Streams.VideoQuality q720 = YoutubeExplode.Videos.Streams.VideoQuality.High720;
                YoutubeExplode.Videos.Streams.VideoQuality q1080 = YoutubeExplode.Videos.Streams.VideoQuality.High720;
                string streamInfo = "";
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
                control.SourceProvider.MediaPlayer.SetMedia(new Uri(vidUri));
            }
            catch (Exception)
            {
                throw;
            }

        }

        private bool changePos = false;
        private bool isPlaying = false;
        public bool IsPlaying { get => isPlaying; set => isPlaying = value; }

        private double videoPosition;
        public double VideoPosition { get => videoPosition; set => videoPosition = value; }
        public void VideoPositionChange(long position)
        {
            changePos = true;
            control.SourceProvider.MediaPlayer.Time = position;
            changePos = false;
        }

        private string GetCorrectTime(long ms)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds);

        }

        private int tickCounter = 0;
        private int saveCounter = 150;

        private void timer_Tick(object sender, EventArgs e)
        {
            if (++tickCounter > 15 && logPanel.Visibility != Visibility.Visible)
            {
                buttonsPanel.Visibility = Visibility.Hidden; Cursor = Cursors.None;
                tickCounter = 0;

            }
            if (saveCounter > 0)
            {
                float tmp = MathF.Round(saveCounter / 10) + 1;

                resetBtn.Visibility = Visibility.Visible;
                resetBtn.Content = "Восстановить сессию (" + tmp.ToString() + ")";
                saveCounter--;
            }
            else
            {
                resetBtn.Visibility = Visibility.Collapsed;
            }

            if (control.SourceProvider.MediaPlayer.Audio.Tracks.Count > 0)
            {
                ContextMenu context = new ContextMenu();
                context.PreviewKeyDown += Context_PreviewKeyDown;//= Context_MouseDown;
                List<Vlc.DotNet.Core.TrackDescription> s = control.SourceProvider.MediaPlayer.Audio.Tracks.All.ToList();

                MenuItem newMenuItem1 = new MenuItem
                {
                    Header = "Аудио"
                };
                context.Items.Add(newMenuItem1);
                foreach (Vlc.DotNet.Core.TrackDescription item in s)
                {
                    MenuItem newMenuItem2 = new MenuItem();
                    MenuItem newExistMenuItem = (MenuItem)context.Items[0];
                    newMenuItem2.Header = item.Name;
                    newMenuItem2.Tag = item.ID.ToString();
                    newMenuItem2.Click += Mi_Click;
                    newExistMenuItem.Items.Add(newMenuItem2);

                }
                controlPanel.ContextMenu = context;
                controlPanel.ContextMenu.MouseDown += Context_MouseDown;
            }
            tb1.Text = GetCorrectTime(control.SourceProvider.MediaPlayer.Length);//// this.control.SourceProvider.MediaPlayer.Length + " ms";

            postionSlider2.Maximum = postionSlider.Maximum = control.SourceProvider.MediaPlayer.Length;

            videoPosition = postionSlider2.Value = control.SourceProvider.MediaPlayer.Time;
            tb2.Text = GetCorrectTime(control.SourceProvider.MediaPlayer.Time);

            if (!changePos)
            {
                postionSlider2.Value = videoPosition;
            }
            else
            {
                control.SourceProvider.MediaPlayer.Time = (long)postionSlider2.Value;
            }

            if (isPlaying != control.SourceProvider.MediaPlayer.IsPlaying())
            {
                if (isPlaying)
                {
                    control.SourceProvider.MediaPlayer.Play();
                }
                else
                {
                    control.SourceProvider.MediaPlayer.Pause();
                }
            }
        }

        private void Mi_Click(object sender, RoutedEventArgs e)
        {
            MenuItem m = (MenuItem)sender;
            control.SourceProvider.MediaPlayer.Audio.Tracks.Current = control.SourceProvider.MediaPlayer.Audio.Tracks.All.Where(x => x.ID == int.Parse(m.Tag.ToString())).FirstOrDefault();
        }

        private void Context_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void Context_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ClientTick()
        {
        }
        private void playPause_Click(object sender, RoutedEventArgs e)
        {
            PlayPause();
        }

        private void PlayPause()
        {
            if (control.SourceProvider.MediaPlayer.IsPlaying())
            {
                client.Send("PAUSE|");

                SendPos();

            }
            else
            {
                client.Send("PLAY|");

            }

        }

        private void SendPos()
        {
            Thread.Sleep(50);
            client.Send("POS|" + Math.Round(postionSlider2.Value).ToString());
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

        private readonly string lastCommand = "";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string line;
            int counter = 0;
            if (File.Exists("LastSession.lsn"))
            {
                System.IO.StreamReader file =
               new System.IO.StreamReader("LastSession.lsn");
                while ((line = file.ReadLine()) != null)
                {
                    if (counter == 0 && !string.IsNullOrWhiteSpace(line))
                    {
                        GetFiles(line);
                    }
                    counter++;

                }
                file.Close();
            }
            else
            {
                resetBtn.Opacity = 0;
                resetBtn.IsEnabled = false;
            }
        }

        private void SendUserInfo()
        {
            List<string> data = usershost;

            byte[] dataAsBytes = data
              .SelectMany(s => Encoding.UTF8.GetBytes("USERS|" + s))
              .ToArray();
            _server.Multicast(dataAsBytes);
        }

        private readonly List<string> usershost = new List<string>();
        private string lastMessage;
        private void _client_RaiseCustomEvent(object sender, CustomEventArgs e)
        {
            lastMessage = e.Message;
            if (lastMessage.Length > 3)
            {
                string msg = lastMessage;
                try
                {

                    selectedVideo.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new System.Windows.Threading.DispatcherOperationCallback(delegate
                    {

                        if (isHost)
                        {
                            if (lastMessage.Contains("IMH|"))
                            {
                                string[] tmp = lastMessage.Replace("IMH|", "").Split(' ');

                                usershost.Add(tmp[0] + "  |-{|");

                                SendUserInfo();
                            }
                            if (lastMessage.Contains("IMO|"))
                            {
                                string tmp = lastMessage.Replace("IMO|", "");

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

                        if (lastMessage.Split(' ')[0] == "/set")
                        {
                            msg = lastMessage.Replace("/set ", "")/*.Replace(lastMessage.Split(' ')[1], "")*/;
                            Dispatcher.Invoke(() => selectedVideo.Text = msg.Trim());
                            Dispatcher.Invoke(() => selectedVideoName = msg.Trim());
                        }
                        else
                        {
                            switch (lastMessage.Split('|')[0])
                            {
                                case "USERS":

                                    string[] tmp = lastMessage.Replace("USERS|", "").Split("  |-{|").Where(c => c != null && c != "").ToArray();
                                    if (true)
                                    {
                                        userList.ItemsSource = tmp;
                                    }
                                    break;
                                case "ADDLINK":

                                    Models.Video video = new Models.Video("", lastMessage.Split('|')[1]);
                                    if (!filesListbox.Items.Contains(video))
                                    {
                                        filesListbox.Items.Add(video);
                                    }
                                    break;
                                case "START":
                                    StartPlayer(selectedVideo.Text);
                                    break;
                                case "PLAY":
                                    control.SourceProvider.MediaPlayer.Play();
                                    isPlaying = true;
                                    isPlayerStarted = true;

                                    if (!string.IsNullOrWhiteSpace(_position))
                                    {

                                        Thread.Sleep(100);
                                        client.WriteLine("POS|" + _position);
                                        _position = "";
                                    }
                                    break;
                                case "PAUSE":
                                    control.SourceProvider.MediaPlayer.Pause();
                                    isPlaying = false;
                                    isPlayerStarted = false;
                                    break;
                                case "RATE":
                                    room.speed = lastMessage.Split('|')[1];
                                    control.SourceProvider.MediaPlayer.Rate = float.Parse(room.speed, CultureInfo.InvariantCulture.NumberFormat);

                                    break;
                                case "POS":
                                    if (!changePos && lastCommand != lastMessage)
                                    {
                                        changePos = true;

                                        postionSlider2.Value = double.Parse(lastMessage.Split('|')[1]);
                                        if (control.SourceProvider.MediaPlayer.Time != (long)postionSlider2.Value)
                                        {
                                            control.SourceProvider.MediaPlayer.Time = (long)postionSlider2.Value;
                                        }
                                        changePos = false;
                                    }
                                    break;
                                case "ROOM":
                                    room.SetRoom(lastMessage);
                                    if (!isPlayerStarted)
                                    {
                                        Opf();
                                        selectedVideo.Text = room.selectedVideo;
                                        Title = room.selectedVideo;
                                        control.SourceProvider.MediaPlayer.Rate = float.Parse(room.speed, CultureInfo.InvariantCulture.NumberFormat);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        return null;

                    }), null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private string videoPath;
        private string videosFolder = "";
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            control.SourceProvider.MediaPlayer.Pause();
            isPlaying = false;
            Opf();

        }

        private void Opf()
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                GetFiles(fbd.SelectedPath);
            }

        }

        private void GetFiles(string SelectedPath)
        {
            if (!string.IsNullOrWhiteSpace(SelectedPath))
            {
                videosFolder = SelectedPath;
                string[] files = Directory.GetFiles(SelectedPath);
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
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
        }

        private void commandLine_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        }

        private void postioonSlider2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void SliderMauseDown(object sender, MouseButtonEventArgs e)
        {
            changePos = true;
            control.SourceProvider.MediaPlayer.Time = (long)Math.Round(postionSlider2.Value);

        }

        private string lstpos = "";
        private void SliderMauseUp(object sender, MouseButtonEventArgs e)
        {
            changePos = false;

            string str = "POS|" + Math.Round(postionSlider2.Value).ToString();

            control.SourceProvider.MediaPlayer.Time = (long)Math.Round(postionSlider2.Value);
            if (str != lastCommand)
            {
                if (lstpos != str)
                {
                    client.Send(str);

                }

            }
            lstpos = str;

        }

        private void postioonSlider2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void postioonSlider2_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {

        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
        }

        private bool isHost = false;
        private void Speed_Click(object sender, RoutedEventArgs e)
        {
            string s = (sender as Button).Content.ToString();

            client.Send("RATE|" + s);

            speedPanel.Visibility = Visibility.Collapsed;

        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {

            if (speedPanel.Visibility == Visibility.Collapsed)
            {
                speedPanel.Visibility = Visibility.Visible;
            }
            else
            {
                speedPanel.Visibility = Visibility.Collapsed;
            }
        }

        private LoginWindow login;
        private ChatServer _server;
        private ChatClient client;
        private bool host = false;
        private void connectBtn_Click(object sender, RoutedEventArgs e)
        {

            login = new LoginWindow();
            login.ShowDialog();
            if (login.acceptBtn.Content.ToString() == "host")
            {
                isHost = true;
                IPAddress iPAddress = IPAddress.Parse(login.hostString.Text);
                _server = new ChatServer(iPAddress, int.Parse(login.portString.Text));
                _server.Start();
                host = true;

            }
            else
            {

            }

            client = new ChatClient(login.hostString.Text, int.Parse(login.portString.Text));

            clientUsername = new ClientUser(login.usernameString.Text.Replace(" ", "_"), client.Id.ToString(), client.Endpoint.Address.ToString());
            client.SetUser(clientUsername);
            client.ConnectAsync();

            client.RaiseCustomEvent += _client_RaiseCustomEvent;
            connectBtn.Visibility = Visibility.Collapsed;
            logPanel.Visibility = Visibility.Visible;
            controlPanel.Visibility = Visibility.Collapsed;
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += timer_Tick;
            logShowHide.IsEnabled = true;
            timer.Start();
        }

        private void controlPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void stringChat_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void logShowHide_Click(object sender, RoutedEventArgs e)
        {
            if (logPanel.Visibility == Visibility.Visible)
            {
                logPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                logPanel.Visibility = Visibility.Visible;
            }
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
                client.Send("POS|" + (Math.Round(postionSlider2.Value) - 5000).ToString());
            }
            if (e.Key == Key.Right)
            {
                client.Send("POS|" + (Math.Round(postionSlider2.Value) + 5000).ToString());
            }

        }

        private void controlPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Cursor = Cursors.Arrow;
                tickCounter = 0;
                DragMove();
            }
            if (e.ChangedButton == MouseButton.XButton2)
            {

                NextVideo();

            }

        }

        private void NextVideo()
        {
            if (filesListbox.SelectedIndex + 1 <= filesListbox.Items.Count)
            {
                filesListbox.SelectedIndex = filesListbox.SelectedIndex + 1;

                selectedVideoName = ((filesListbox).SelectedItem as Video).FileName;
                Thread.Sleep(1000);

                client.Send("/set " /*+ clientUsername.ClientUserName + " " */+ selectedVideoName);

                Thread.Sleep(1000);
                client.Send("START|");

            }
        }

        private void SubmitXB()
        {
        }
        private void controlPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MaximizeWindow();
        }
        private void MaximizeWindow()
        {
            if (WindowStyle == WindowStyle.SingleBorderWindow)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;

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

        public async Task SaveAsync()
        {
            string text = videosFolder + '\n' + selectedVideo.Text + '\n' + Math.Round(postionSlider2.Value).ToString();

            await File.WriteAllTextAsync("LastSession.lsn", text);
        }

        public async Task ReadAsync()
        {
            string line;
            int counter = 0;

            if (File.Exists("LastSession.lsn"))
            {

                System.IO.StreamReader file =
                   new System.IO.StreamReader("LastSession.lsn");
                while ((line = file.ReadLine()) != null)
                {
                    if (counter == 1 && !string.IsNullOrWhiteSpace(line))
                    {
                        Thread.Sleep(50);
                        client.WriteLine("/set " + line);
                        Thread.Sleep(50);
                        client.WriteLine("START|");
                        Thread.Sleep(50);
                        client.WriteLine("");

                    }
                    if (counter == 2 && line != "0")
                    {

                        setPosition(line);
                    }
                    counter++;

                }

                file.Close();
            }
            else
            {
                resetBtn.Opacity = 0;
                resetBtn.IsEnabled = false;
            }
        }

        private string _position = "";

        private void setPosition(string line)
        {
            _position = line;

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveAsync();
            client.Send("IMO|" + clientUsername.ClientUserName);
            client.Disconnect();
            System.Diagnostics.Process.GetCurrentProcess().Kill();

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
            Cursor = Cursors.Arrow;

        }

        private void filesListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Setter();

        }

        private void setBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void filesListbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private string selectedVideoName = "";
        private string selectedVideoPath = "";

        private void Setter()
        {
            selectedVideoName = ((filesListbox).SelectedItem as Video).FileName;
            selectedVideoPath = ((filesListbox).SelectedItem as Video).FullPath;
            videoPath = selectedVideoPath + "/" + selectedVideoName;

            client.Send("/set " /*+ clientUsername.ClientUserName + " "*/ + selectedVideoName);

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenLinkDialog openLinkDialog = new OpenLinkDialog();
            openLinkDialog.ShowDialog();
            Video video = new Video("", openLinkDialog.linkString.Text);

            client.Send("ADDLINK|" + video.FileName);

        }

        private void MIStart_Click(object sender, RoutedEventArgs e)
        {
            client.Send("START|");
        }

        private async void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            await ReadAsync();
            resetBtn.Opacity = 0;
            resetBtn.IsEnabled = false;
        }
    }
}
