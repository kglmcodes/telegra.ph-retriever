using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace TelegraphRetreiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int num = 0;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        void Trademe_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;
            using (FileStream filestream = new FileStream($"page{num}.html", FileMode.Create))
            {
                var streamwriter = new StreamWriter(filestream);
                streamwriter.Write(e.Result);
                streamwriter.AutoFlush = true;
                Console.SetOut(streamwriter);
                Console.SetError(streamwriter);
            }
            string curDir = Directory.GetCurrentDirectory();
            //webBrowser.NavigateToString(e.Result);
            webBrowser.Navigate(new Uri(String.Format("file:///{0}/page" + num + ".html", curDir)));
            progressBar1.IsIndeterminate = false;
            progressBar1.Visibility = Visibility.Collapsed;
            num++;
        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            WebClient Trademe = new WebClient();
            Trademe.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Trademe_DownloadStringCompleted);
            Trademe.DownloadStringAsync(new Uri("http://telegra.ph/" + GetPostUri()));
            progressBar1.IsIndeterminate = true;
            progressBar1.Visibility = Visibility.Visible;
        }

        private string GetPostUri()
        {
            return "Hello-World-01-01";
        }
    }
}
