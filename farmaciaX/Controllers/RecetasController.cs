using Microsoft.AspNetCore.Mvc;
using farmaciaX.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Collections.Generic;


namespace farmaciaX.Controllers
{
    [Authorize]
    public class RecetasController : Controller
    {
        private readonly IRepositorioReceta_Medica repositorio;
        private readonly IWebHostEnvironment env;

        private readonly IRepositorioCliente repositorioCliente;
        private readonly DataContext context;
        protected readonly IConfiguration configuration;

        private readonly IRepositorioRecetaProducto repositorioRecetaProductos;

        private readonly IRepositorioProductos repositorioProductos;

        public RecetasController(IRepositorioReceta_Medica repositorio, IWebHostEnvironment env, IRepositorioCliente repositorioCliente, DataContext context, IConfiguration configuration, IRepositorioRecetaProducto repositorioRecetaProductos, IRepositorioProductos repositorioProductos)
        {
            this.repositorio = repositorio;
            this.env = env;
            this.repositorioCliente = repositorioCliente;
            this.context = context;
            this.configuration = configuration;
            this.repositorioRecetaProductos = repositorioRecetaProductos;
            this.repositorioProductos = repositorioProductos;
        }

        //GET index
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Receta_Medica receta, List<ProductoRecetado> productos)
        {

            if (receta.Fecha_Vencimiento < DateTime.Today)
            {
                ModelState.AddModelError("", "La fecha de vencimiento debe ser posterior a la fecha actual.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = repositorioCliente.ObtenerTodos();
                ViewBag.Productos = repositorioRecetaProductos.ObtenerTodos();
                ViewBag.RecetaProductos = repositorioRecetaProductos.ObtenerTodos();
                return View(receta);
            }


            if (receta.ImgRecetaFile != null)
            {
                var carpetaRelativa = Path.Combine("Uploads", "Recetas");
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", carpetaRelativa);
                Directory.CreateDirectory(folderPath);

                var uniqueName = Guid.NewGuid() + Path.GetExtension(receta.ImgRecetaFile.FileName);
                var filePath = Path.Combine(folderPath, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    receta.ImgRecetaFile.CopyTo(stream);
                }

                // Guardar solo el nombre del archivo
                receta.ImgReceta = uniqueName;
            }


            // Alta receta
            int recetaId = repositorio.Alta(receta);

            // Alta productos asociados
            repositorio.AltaProductosReceta(recetaId, productos);

            return RedirectToAction("Index");
        }












        // Delete recetas
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            repositorio.Eliminar(id);
            TempData["Mensaje"] = "Receta eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }



        //Post activar recetas
        public IActionResult Activar(int id)
        {
            repositorio.Activar(id);
            return RedirectToAction("Index");
        }




        [HttpGet]
        public IActionResult Buscar(string termino = "", int page = 1)
        {
            int pageSize = 5;

            var recetas = repositorio.ObtenerTodasConProductos();

            if (!string.IsNullOrEmpty(termino))
            {
                termino = termino.ToLower();
                recetas = recetas.Where(r =>
                    r.Cliente.NombreCompleto.ToLower().Contains(termino) ||
                    r.Medico.ToString().ToLower().Contains(termino) ||
                    r.RecetaProductos.Any(rp => rp.Producto.Nombre.ToLower().Contains(termino))).ToList();
            }

            int totalRegistros = recetas.Count();

            var recetasPaginadas = recetas
                .OrderByDescending(r => r.Fecha_Emision)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new
                {
                    id = r.Id,
                    cliente = r.Cliente != null ? r.Cliente.Nombre + " " + r.Cliente.Apellido : "N/D",
                    medico = r.Medico,
                    fechaEmision = r.Fecha_Emision.ToString("dd/MM/yyyy"),
                    fechaVencimiento = r.Fecha_Vencimiento.ToString("dd/MM/yyyy"),
                    imgReceta = r.ImgReceta,
                    activo = r.Activo,
                    productos = r.RecetaProductos.Select(rp => new
                    {
                        id = rp.Producto.Id,
                        nombre = rp.Producto.Nombre,
                        precio = rp.Producto.Precio,
                        cantidad = rp.Cantidad
                    }).ToList()
                })
                .ToList();

            return Json(new
            {
                recetas = recetasPaginadas,
                totalRegistros,
                pageSize
            });
        }








        [HttpGet]
        public IActionResult BuscarRec(int id)
        {
            var recetas = repositorio.BuscarPorCliente(id);
            return Json(recetas);
        }



        [HttpGet("Recetas/Productos/{id}")]
        public async Task<IActionResult> ObtenerProductosPorReceta(int id)
        {
            var productos = await context.RecetaProductos
                .Where(rp => rp.RecetaId == id)
                .Include(rp => rp.Producto)
                .Select(rp => new
                {
                    id = rp.Producto.Id,
                    nombre = rp.Producto.Nombre,
                    precio = rp.Producto.Precio,
                    cantidad = rp.Cantidad
                })
                .ToListAsync();

            return Json(productos);
        }






    }
}
