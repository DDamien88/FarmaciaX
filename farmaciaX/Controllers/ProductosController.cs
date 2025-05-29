using Microsoft.AspNetCore.Mvc;
using farmaciaX.Models;
using Microsoft.AspNetCore.SignalR.Protocol;
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


        //Get: Productos/Create
        public IActionResult Create()
        {
            return View();
        }

        //Post: Productos/Create
        [HttpPost]
        public IActionResult Create(Productos producto)
        {

            if (producto.Fecha_Vencimiento < DateTime.Now)
            {
                TempData["Error"] = "La fecha de vencimiento no puede ser menor a la fecha actual.";
                return View();

            }
            repositorio.Alta(producto);
            return RedirectToAction("Index");
        }



        //Get Edit Productos
        public IActionResult Edit(int id)
        {
            var producto = repositorio.BuscarPorId(id);
            return View(producto);
        }

        //Post Edit Productos
        [HttpPost]
        public IActionResult Edit(Productos producto)
        {
            repositorio.Modificar(producto);
            return RedirectToAction("Index");
        }

        //Get Delete Productos
        public IActionResult Delete(int id)
        {
            var producto = repositorio.BuscarPorId(id);
            return View(producto);
        }

        //Post Delete Productos
        [HttpPost]
        public IActionResult Delete(Productos producto)
        {
            repositorio.Baja(producto.Id);
            return RedirectToAction("Index");
        }

        //Post Activar productos
        public IActionResult Activar(int id)
        {
            repositorio.Activar(id);
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var producto = repositorio.BuscarPorId(id);
            return View(producto);
        }



        // POST: Productos/GuardarAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarAjax(Productos producto)
        {
            if (producto.Id == 0)
            {
                repositorio.Alta(producto);
            }
            else
            {
                repositorio.Modificar(producto);
            }
            return Json(new { success = true });
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