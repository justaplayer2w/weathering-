using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace SimpleWeatherApp
{
    public partial class MainWindow : Window
    {
        private const string ApiKey = "370f13c86910dede30176925b7011172"; // Replace this with your actual OpenWeatherMap API key

        public MainWindow()
        {
            InitializeComponent();
        }

        // Event handler for when the "Get Weather" button is clicked
        private async void GetWeather_Click(object sender, RoutedEventArgs e)
        {
            string city = CityTextBox.Text;
            if (string.IsNullOrWhiteSpace(city) || city == "Enter city")
            {
                ResultTextBlock.Text = "Please enter a city.";
                return;
            }

            ResultTextBlock.Text = "Loading...";
            try
            {
                string weather = await GetWeatherAsync(city);
                ResultTextBlock.Text = weather;
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"something is wrong, please enter city name again";
            }
        }

        // Method to fetch weather data from OpenWeatherMap API
        private async Task<string> GetWeatherAsync(string city)
        {
            using HttpClient client = new HttpClient();
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={ApiKey}&units=metric";
            string response = await client.GetStringAsync(url);
            JObject json = JObject.Parse(response);
            double temp = json["main"]["temp"].Value<double>();
            string description = json["weather"][0]["description"].Value<string>();
            return $"Temp: {temp}°C\n{description}";
        }


        private void CityTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void CityTextBox_TextChanged_1(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
