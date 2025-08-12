using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeatherApp.Data;
using WeatherApp.Config;

namespace WeatherApp.Services
{
    public class WeatherApiClient : MonoBehaviour
    {
        [Header("API Configuration")] [SerializeField]
        private string baseUrl = "http://api.openweathermap.org/data/2.5/weather";

        public async Task<WeatherData> GetWeatherDataAsync(string city)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(city))
            {
                Debug.LogError("City name cannot be empty");
                return null;
            }
            
            if (!ApiConfig.IsApiKeyConfigured())
            {
                Debug.LogError("API key not configured. Please set up your config.json file in StreamingAssets folder.");
                return null;
            }

            string url = $"{baseUrl}?q={UnityWebRequest.EscapeURL(city)}&appid={ApiConfig.OpenWeatherMapApiKey}";
            
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                await request.SendWebRequest();
                
                switch (request.result)
                {
                    case UnityWebRequest.Result.Success:
                        try
                        {
                            var settings = new JsonSerializerSettings
                            {
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                                NullValueHandling = NullValueHandling.Ignore
                            };
                            return JsonConvert.DeserializeObject<WeatherData>(request.downloadHandler.text, settings);
                        }
                        catch (JsonException ex)
                        {
                            Debug.LogError($"JSON parsing failed: {ex.Message}");
                            return null;
                        }
                    case UnityWebRequest.Result.ConnectionError:
                            Debug.LogError($"Network connection failed: {request.error}");
                            return null;

                    case UnityWebRequest.Result.ProtocolError:
                            Debug.LogError($"HTTP Error {request.responseCode}: {request.error}");
                            return null;

                    case UnityWebRequest.Result.DataProcessingError:
                            Debug.LogError($"Data processing failed: {request.error}");
                            return null; 
                    default:
                        Debug.LogError("Unknown error occurred");
                        return null;
                }
            }
        }
    }
}