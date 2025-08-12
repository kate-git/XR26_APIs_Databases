using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeatherApp.Services;
using WeatherApp.Data;
using System;
using System.Threading.Tasks;

namespace WeatherApp.UI
{
    public class WeatherUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField cityInputField;
        [SerializeField] private Button getWeatherButton;
        [SerializeField] private TextMeshProUGUI weatherDisplayText;
        [SerializeField] private TextMeshProUGUI statusText;
        
        [Header("API Client")]
        [SerializeField] private WeatherApiClient apiClient;
        
        private void Start()
        {
            // Set up button click listener
            getWeatherButton.onClick.AddListener(() => _ = OnGetWeatherClicked());

            // Initialize UI state
            SetStatusText("Enter a city name and click Get Weather");
        }
        
        private async Task OnGetWeatherClicked()
        {
            // Get city name from input field
            string cityName = cityInputField.text;
            
            // Validate input
            if (string.IsNullOrWhiteSpace(cityName))
            {
                SetStatusText("Please enter a city name");
                return;
            }
            
            // Disable button and show loading state
            getWeatherButton.interactable = false;
            SetStatusText("Loading weather data...");
            weatherDisplayText.text = "";
            
            try
            {
                var data = await apiClient.GetWeatherDataAsync(cityName);
                if (data != null && data.IsValid)
                {
                    DisplayWeatherData(data);
                    SetStatusText("Weather data loaded successfully");
                }
                else
                {
                    SetStatusText("City not found or invalid data");
                }
            }
            catch (System.Exception ex)
            {
                // Handle exceptions
                Debug.LogError($"Error getting weather data: {ex.Message}");
                SetStatusText("An error occurred. Please try again.");
            }
            finally
            {
                // Re-enable button
                getWeatherButton.interactable = true;
            }
        }
        
        private void DisplayWeatherData(WeatherData weatherData)
        {
            string displayText = $"City: {weatherData.CityName}\n" +
            $"Temperature: {weatherData.TemperatureInCelsius:F1}°C (Feels like: {weatherData.Main.FeelsLike - 273.15f:F1}°C)\n" +
                $"Description: {weatherData.PrimaryDescription}\n" +
                $"Humidity: {weatherData.Main.Humidity}%\n" +
                $"Pressure: {weatherData.Main.Pressure} hPa";
            weatherDisplayText.text = displayText;
            
        }
        
        private void SetStatusText(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }
        
        public void ClearDisplay()
        {
            weatherDisplayText.text = "";
            cityInputField.text = "";
            SetStatusText("Enter a city name and click Get Weather");
        }
    }
}