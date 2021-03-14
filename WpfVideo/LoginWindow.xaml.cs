
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

            if (acceptBtn.Content.ToString() == "Создать")
            {

            }
            else
            {

            }

            try
            {
 
                this.Close();
 
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.ToString());
                System.Windows.MessageBox.Show("вронг");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
 
        }
        void timer_Tick(object sender, EventArgs e)
        {



            if (player != null)
            {
 
            }




        }
         
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog openFile = new OpenFileDialog();
            //openFile.ShowDialog();
            //videoPath = openFile.FileName.Replace(@"\", "/");
            //fileNameTextBox.Text = videoPath.Split('/')[videoPath.Split('/').Length-1]; ;
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void commandLine_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
            }
        }

        private void hostBtn_Click(object sender, RoutedEventArgs e)
        {
            loginGrid.Visibility = Visibility.Visible;
            connectBtn.Visibility = Visibility.Collapsed;
            hostBtn.Visibility = Visibility.Collapsed;
            backBtn.Visibility = Visibility.Visible;
            acceptBtn.Visibility = Visibility.Visible;
            acceptBtn.Content = "Создать";
        }

        private void connectBtn_Click(object sender, RoutedEventArgs e)
        {
            loginGrid.Visibility = Visibility.Visible;
            connectBtn.Visibility = Visibility.Collapsed;
            hostBtn.Visibility = Visibility.Collapsed;
            backBtn.Visibility = Visibility.Visible;
            acceptBtn.Visibility = Visibility.Visible;
           acceptBtn.Content = "Подключиться";
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
