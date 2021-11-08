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
using log4net;

namespace TelegraphRetreiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int CurrentTries
        {
            get { return (int)GetValue(CurrentTriesProperty); }
            set { SetValue(CurrentTriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTries.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTriesProperty =
            DependencyProperty.Register("CurrentTries", typeof(int), typeof(MainWindow), new PropertyMetadata(0));



        public int TotalTries
        {
            get { return (int)GetValue(TotalTriesProperty); }
            set { SetValue(TotalTriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalTries.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalTriesProperty =
            DependencyProperty.Register("TotalTries", typeof(int), typeof(MainWindow), new PropertyMetadata(0));


        private int wordCount = 2;
        private Uri randomWordURI = new Uri($"https://random-word-api.herokuapp.com/word?number=2");
        private int num = 0;
        public MainWindow()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            btn_next_Click(this, new RoutedEventArgs());
        }

        void Trademe_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                btn_next_Click(this, new RoutedEventArgs());
                TotalTries++;
                CurrentTries++;
                return;
            }
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
            CurrentTries = 0;
            btn_next_Click(this, new RoutedEventArgs());
        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            WebClient Trademe = new WebClient();
            Trademe.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Trademe_DownloadStringCompleted);
            Uri finalUri = new Uri("http://telegra.ph/" + GetPostUri().Result);
            log.Info(finalUri);
            Trademe.DownloadStringAsync(finalUri);
            progressBar1.IsIndeterminate = true;
            progressBar1.Visibility = Visibility.Visible;
        }

        private async Task<string> GetPostUri()
        {
            WebClient getRandomWord = new WebClient();
            string randomwords = getRandomWord.DownloadString(randomWordURI);
            //getRandomWord.DownloadStringCompleted += GetRandomWord_DownloadStringCompleted;
            //string randomwordsAsync = await Task.Run(async () => { await getRandomWord.DownloadStringTaskAsync(randomWordURI);});

            //string randomwordsAsync = "";
            //await Task.Run(async () => { randomwordsAsync = await getRandomWord.DownloadStringTaskAsync(randomWordURI); });


            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '"', '[', ']', '/' };
            string[] vs = randomwords.Split(delimiterChars);
            string result = "";
            foreach (var word in vs)
            {
                if (!(string.IsNullOrEmpty(word) || string.IsNullOrWhiteSpace(word)))
                {
                    result += word + "-";
                }
            }
            result += "01-01";
            //Console.WriteLine(randomwordsAsync);
            return result;
        }

    }
}