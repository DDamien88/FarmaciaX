using Microsoft.AspNetCore.Mvc;
using farmaciaX.Models;
using Microsoft.AspNetCore.Authorization;

namespace farmaciaX.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly IRepositorioProductos repositorio;
        private readonly DataContext context;
        public ProductosController(IRepositorioProductos repositorio, DataContext context)
        {
            this.repositorio = repositorio;
            this.context = context;
        }

        public IActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }



        //Buscar Productos
        [HttpGet]
        public IActionResult Buscar(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Json(new List<Productos>());

            try
            {
                var productos = context.Productos
                    .Where(p => p.Nombre != null && p.Nombre.ToLower().Contains(nombre.ToLower()) && p.Activo)
                    .Select(p => new
                    {
                        p.Id,
                        p.Nombre,
                        p.Precio,
                        p.Requiere_Receta,
                        p.Cantidad_Stock
                    })
                    .ToList();

                return Json(productos);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR en Buscar Productos: " + ex.Message);
                Console.WriteLine("STACK TRACE: " + ex.StackTrace);
                return StatusCode(500, $"Error interno: {ex.Message} - {ex.StackTrace}");
            }

        }





    }
}