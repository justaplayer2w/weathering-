using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace SimpleWeatherApp
{
    public partial class MainWindow : Window
    {
        // Konstante für den API-Schlüssel von OpenWeatherMap
        // (API-Schlüssel sollte idealerweise in einer sicheren Konfigurationsdatei gespeichert werden. Liebe Grüße, Joline lol)
        private const string ApiKey = "370f13c86910dede30176925b7011172";

        // HttpClient als statische Instanz, um Socket-Erschöpfung zu vermeiden
        private static readonly HttpClient _httpClient = new HttpClient();

        // Konstanten für UI-Texte
        private const string PlaceholderText = "Stadt eingeben";

        /// <summary>
        /// Konstruktor für das Hauptfenster
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Initialisierung der Benutzeroberfläche mit Platzhaltertext
            SetupUI();
        }

        /// <summary>
        /// Initialisiert die Benutzeroberfläche mit Standardwerten
        /// </summary>
        private void SetupUI()
        {
            // Platzhaltertext für Textfeld setzen und Fokus darauf setzen
            if (CityTextBox != null)
            {
                // Platzhaltertext auf Deutsch setzen
                CityTextBox.Text = PlaceholderText;

                CityTextBox.GotFocus += (s, e) =>
                {
                    if (CityTextBox.Text == PlaceholderText)
                        CityTextBox.Text = string.Empty;
                };

                CityTextBox.LostFocus += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(CityTextBox.Text))
                        CityTextBox.Text = PlaceholderText;
                };

                // Event-Handler für die Enter-Taste hinzufügen
                CityTextBox.KeyDown += (s, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        GetWeather_Click(this, new RoutedEventArgs());
                    }
                };
            }
        }

        /// <summary>
        /// Ereignishandler für den "Wetter abrufen"-Button
        /// </summary>
        /// <param name="sender">Das Ereignis auslösende Objekt</param>
        /// <param name="e">Ereignisdaten</param>
        private async void GetWeather_Click(object sender, RoutedEventArgs e)
        {
            // Stadt aus Textfeld auslesen
            string city = CityTextBox?.Text?.Trim();

            // Überprüfen, ob eine gültige Stadt eingegeben wurde
            if (string.IsNullOrWhiteSpace(city) || city == PlaceholderText)
            {
                ResultTextBlock.Text = "Bitte geben Sie eine Stadt ein.";
                return;
            }

            // Ladezustand anzeigen
            ResultTextBlock.Text = "Wird geladen...";

            try
            {
                // Wetterdaten asynchron abrufen
                string weather = await GetWeatherAsync(city);
                ResultTextBlock.Text = weather;
            }
            catch (HttpRequestException ex)
            {
                // Fehler bei der Netzwerkanfrage
                ResultTextBlock.Text = $"Netzwerkfehler: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Allgemeiner Fehler
                ResultTextBlock.Text = "Etwas ist schiefgelaufen. Bitte überprüfen Sie den Stadtnamen und versuchen Sie es erneut.";
            }
        }

        /// <summary>
        /// Ruft Wetterdaten von der OpenWeatherMap API ab
        /// </summary>
        /// <param name="city">Name der Stadt, für die Wetterdaten abgerufen werden sollen</param>
        /// <returns>Formatierte Wetterdaten als Zeichenfolge</returns>
        private async Task<string> GetWeatherAsync(string city)
        {
            // URL für die API-Anfrage erstellen
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(city)}&appid={ApiKey}&units=metric&lang=de";

            // HTTP-Anfrage senden und Antwort empfangen
            string response = await _httpClient.GetStringAsync(url);

            // JSON-Antwort analysieren
            JObject json = JObject.Parse(response);

            // Relevante Wetterdaten extrahieren
            double temp = json["main"]["temp"].Value<double>();
            string description = json["weather"][0]["description"].Value<string>();
            string cityName = json["name"].Value<string>();
            string country = json["sys"]["country"].Value<string>();
            double humidity = json["main"]["humidity"].Value<double>();

            // Formatierte Wetterdaten zurückgeben
            return $"Stadt: {cityName}, {country}\nTemperatur: {temp:F1}°C\nBeschreibung: {description}\nLuftfeuchtigkeit: {humidity}%";
        }
    }
}