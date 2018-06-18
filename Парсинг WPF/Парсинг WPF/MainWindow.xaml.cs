using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;
using System.Drawing;
using System.IO;

using System.ComponentModel;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;


namespace Парсинг_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer DT = new DispatcherTimer();
        ReceivedData rd = new ReceivedData();

        string urlAddress = @"https://maanimo.com/cryptocurrency/iota";
        string s2 = @"<span class=""number text-bold"" data";
        Regex rx = new Regex(@"\d+(\.\d{0,2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public MainWindow() => InitializeComponent();

        private int GetSalt() => (new Random()).Next(100, int.MaxValue);
        public string GetCode(string urlAddress)
        {
            string data = "";
            string Req = $"{urlAddress}?_salt_{GetSalt()}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Req);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }
                data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }
            return data;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DT.Tick += DT_Tick;
            DT.Interval = new TimeSpan(0,0,1);
            
            DT.IsEnabled = true;
            DT.Start();
        }
        private void DT_Tick(object sender, EventArgs e)
        {
            rd.setRequest(GetCode(urlAddress));
            

            string temp = String.Empty;
            int index = rd.s1[rd.s1.Count - 1].IndexOf(s2);
            for (int i = index; i < index + 185; i++)
            {
                temp += rd.s1[rd.s1.Count - 1][i];
            }
            MatchCollection matches = rx.Matches(temp);
            textBox.Text = Convert.ToString(matches[0]);
        }
    
    }

    class ReceivedData
    {
        public List<string> s1 = new List<string>();
        public void setRequest(string s1) => this.s1.Add(s1);
        
    }
}
