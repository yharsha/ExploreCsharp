using Newtonsoft.Json;
using StockAnalyzer.Core;
using StockAnalyzer.Core.Domain;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;

namespace StockAnalyzer.Windows
{
    public partial class MainWindow : Window
    {
        private static string API_URL = "https://ps-async.fekberg.com/api/stocks";
        private Stopwatch stopwatch = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
        }



        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            BeforeLoadingStockData();

            //async read from web API
            using (var client = new HttpClient())
            {
                var responseTask =  client.GetAsync($"{API_URL}/{StockIdentifier.Text}");

                var response = await responseTask;

                var content = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<IEnumerable<StockPrice>>(content);

                Stocks.ItemsSource = data;
            }
           

            //async read from file 
            /**
            var store = new DataStore();
            var pricesTask =  store.GetStockPrices(StockIdentifier.Text);
            Stocks.ItemsSource = await pricesTask;
            **/

            /**
            //sync logic
            var client = new WebClient();

            var content = client.DownloadString($"{API_URL}/{StockIdentifier.Text}");

            // Simulate that the web call takes a very long time
            Thread.Sleep(10000);
            
            var data = JsonConvert.DeserializeObject<IEnumerable<StockPrice>>(content);

            Stocks.ItemsSource = data;
            **/
            AfterLoadingStockData();
        }








        private void BeforeLoadingStockData()
        {
            stopwatch.Restart();
            StockProgress.Visibility = Visibility.Visible;
            StockProgress.IsIndeterminate = true;
        }

        private void AfterLoadingStockData()
        {
            StocksStatus.Text = $"Loaded stocks for {StockIdentifier.Text} in {stopwatch.ElapsedMilliseconds}ms";
            StockProgress.Visibility = Visibility.Hidden;
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
