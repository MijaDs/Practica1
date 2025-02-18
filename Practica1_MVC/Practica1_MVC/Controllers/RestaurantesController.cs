using Microsoft.AspNetCore.Mvc;
using Practica1_MVC.Models;

namespace Practica1_MVC.Controllers
{
    public class RestaurantesController : Controller
    {
        private readonly ApiClient _apiClient;

        public RestaurantesController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public async Task<IActionResult> Index()
        {
            var restaurantes = await _apiClient.GetRestaurantesAsync();
            return View(restaurantes);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Crear(Restaurantes restaurante)
        {
            if (ModelState.IsValid)
            {
                var result = await _apiClient.AgregarRestauranteAsync(restaurante);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No se pudo agregar el restaurante. Inténtelo de nuevo.");
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage); 
                }
            }

            return View(restaurante);
        }
        [HttpGet]
        public async Task <IActionResult> Editar(int id)
        {
            var restaurante = await _apiClient.ObtenerRestaurantePorIdAsync(id); 
            if (restaurante == null)
            {
                return NotFound();
            }
            return View(restaurante);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, Restaurantes restaurante)
        {
            if (ModelState.IsValid)
            {
                
                restaurante.Id = id;

                var result = await _apiClient.EditarRestauranteAsync(restaurante);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No se pudo editar el restaurante. Inténtelo de nuevo.");
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View(restaurante);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var restaurante = await _apiClient.ObtenerRestaurantePorIdAsync(id);
            if (restaurante == null)
            {
                return NotFound();
            }
            return View(restaurante);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            bool resultado = await _apiClient.EliminarRestauranteAsync(id);
            if (!resultado)
            {
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el restaurante.");
                // Vuelve a cargar el restaurante para mostrarlo en la vista de confirmación
                var restaurante = await _apiClient.ObtenerRestaurantePorIdAsync(id);
                return View(restaurante);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
