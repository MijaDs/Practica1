using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Practica1_MVC.Models;
using System.Text;
using System.Text.Json;

namespace Practica1_MVC.Controllers
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Restaurantes>> GetRestaurantesAsync()
        {
            var response = await _httpClient.GetAsync("https://localhost:7113/api/Restaurantes");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var restaurantes = JsonSerializer.Deserialize<List<Restaurantes>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return restaurantes;
        }

        public async Task<bool> AgregarRestauranteAsync(Restaurantes restaurante)
        {
            var url = "https://localhost:7113/api/AgregarRestaurante";

            var json = JsonSerializer.Serialize(restaurante);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent); 
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }

        public async Task<Restaurantes> ObtenerRestaurantePorIdAsync(int id)
        {
            var url = $"https://localhost:7113/api/Restaurantes/{id}";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode(); 

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                throw new Exception("Respuesta vacía del servidor.");

            return JsonSerializer.Deserialize<Restaurantes>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? throw new JsonException("No se pudo deserializar la respuesta JSON.");
        }

        public async Task<bool> EditarRestauranteAsync(Restaurantes restaurante)
        {
          
            var url = $"https://localhost:7113/api/ActualizarRestaurante?id={restaurante.Id}";
            var json = JsonSerializer.Serialize(restaurante);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PutAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> EliminarRestauranteAsync(int id)
        {          
            var url = $"https://localhost:7113/api/EliminarRestaurante/{id}";
            try
            {             
                var response = await _httpClient.DeleteAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
