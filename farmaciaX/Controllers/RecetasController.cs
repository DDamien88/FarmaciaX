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
            try
            {
                ViewBag.Clientes = repositorioCliente.ObtenerTodos();
                ViewBag.Productos = repositorioRecetaProductos.ObtenerTodos();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
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




        /*[HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var receta = repositorio.ObtenerPorId(id);
                receta.RecetaProductos = repositorioRecetaProductos.ObtenerPorReceta(id);
                ViewBag.TodosProductos = repositorioProductos.ObtenerTodos();
                return View(receta);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }

        }*/


        /* [HttpPost]
         public IActionResult Edit(Receta_Medica receta, IFormFile ImgRecetaFile, List<ProductoRecetado> productos)
         {
             Console.WriteLine("Form recibido");
             Console.WriteLine($"Productos recibidos: {receta.RecetaProductos?.Count ?? 0}");

             if (receta.RecetaProductos != null)
             {
                 foreach (var rp in receta.RecetaProductos)
                 {
                     Console.WriteLine($"ProductoId: {rp.ProductoId}, Cantidad: {rp.Cantidad}");
                 }
             }

             try
             {
                 var recetaOriginal = repositorio.ObtenerPorId(receta.Id);

                 if (receta.RecetaProductos == null || receta.RecetaProductos.Count == 0)
                 {
                     ModelState.AddModelError("", "Debe agregar al menos un producto a la receta.");

                     ViewBag.Clientes = recetaOriginal.Cliente;
                     ViewBag.Productos = recetaOriginal.RecetaProductos;
                     // ViewBag.RecetaProductos = recetaOriginal.RecetaProductos;
                     // ViewBag.TodosProductos = recetaOriginal.RecetaProductos;
                     receta.ImgReceta = recetaOriginal.ImgReceta;
                     return View(receta);
                 }


                 if (receta.Fecha_Vencimiento < DateTime.Now.Date)
                 {
                     ModelState.AddModelError("Fecha_Vencimiento", "La fecha de vencimiento debe ser posterior a la fecha actual.");

                     receta.Cliente = repositorioCliente.ObtenerPorId(receta.ClienteId);
                     receta.RecetaProductos = repositorioRecetaProductos.ObtenerPorReceta(receta.Id);
                     receta.ImgReceta = recetaOriginal.ImgReceta;

                     ViewBag.TodosProductos = repositorioProductos.ObtenerTodos();
                     return View(receta);
                 }

                 // Actualización de imagen
                 if (ImgRecetaFile != null)
                 {
                     var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Recetas");
                     Directory.CreateDirectory(uploadsPath);

                     var nombreNuevo = Guid.NewGuid() + Path.GetExtension(ImgRecetaFile.FileName);
                     var rutaNueva = Path.Combine(uploadsPath, nombreNuevo);

                     using (var stream = new FileStream(rutaNueva, FileMode.Create))
                     {
                         ImgRecetaFile.CopyTo(stream);
                     }

                     if (!string.IsNullOrEmpty(recetaOriginal.ImgReceta))
                     {
                         var rutaVieja = Path.Combine(uploadsPath, recetaOriginal.ImgReceta);
                         if (System.IO.File.Exists(rutaVieja))
                             System.IO.File.Delete(rutaVieja);
                     }

                     receta.ImgReceta = nombreNuevo;
                 }
                 else
                 {
                     receta.ImgReceta = recetaOriginal.ImgReceta;
                 }

                 // Actualizar productos asignados
                 repositorioRecetaProductos.EliminarTodosPorReceta(receta.Id);

                 repositorio.Modificar(receta);

                 // Alta productos asociados
                 repositorio.AltaProductosReceta(receta.Id, productos);


                 return RedirectToAction("Index");
             }
             catch (Exception ex)
             {
                 ViewBag.Error = ex.Message;
                 ViewBag.TodosProductos = repositorioProductos.ObtenerTodos();
                 return View(receta);
             }

         }*/


        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var receta = repositorio.ObtenerPorId(id);
                if (receta == null)
                {
                    return NotFound();
                }

                // Cargar productos recetados
                receta.RecetaProductos = repositorioRecetaProductos.ObtenerPorReceta(id);

                // Lista de todos los productos (para los nombres mostrados en el <td>)
                ViewBag.TodosProductos = repositorioRecetaProductos.ObtenerTodos();

                return View(receta);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public IActionResult Edit(Receta_Medica receta, string ProductosJson, IFormFile? ImgRecetaFile)
        {
            try
            {
                if (receta.Fecha_Vencimiento < receta.Fecha_Emision)
                {
                    ModelState.AddModelError("", "La fecha de vencimiento debe ser posterior a la fecha de emisión.");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.TodosProductos = repositorioRecetaProductos.ObtenerTodos();
                    receta.RecetaProductos = repositorioRecetaProductos.ObtenerPorReceta(receta.Id);
                    return View(receta);
                }

                // Procesar imagen nueva si se cargó
                if (ImgRecetaFile != null)
                {
                    var carpetaRelativa = Path.Combine("Uploads", "Recetas");
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", carpetaRelativa);
                    Directory.CreateDirectory(folderPath);

                    var uniqueName = Guid.NewGuid() + Path.GetExtension(ImgRecetaFile.FileName);
                    var filePath = Path.Combine(folderPath, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImgRecetaFile.CopyTo(stream);
                    }

                    receta.ImgReceta = uniqueName;
                }

                // Actualizar receta (excepto los productos)
                repositorio.Modificar(receta);

                // Procesar productos desde JSON
                var nuevosProductos = JsonSerializer.Deserialize<List<ProductoRecetado>>(ProductosJson);
                if (nuevosProductos != null)
                {
                    // Reemplazar productos existentes
                    repositorioRecetaProductos.EliminarTodosPorReceta(receta.Id);
                    repositorio.AltaProductosReceta(receta.Id, nuevosProductos);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.TodosProductos = repositorioRecetaProductos.ObtenerTodos();
                receta.RecetaProductos = repositorioRecetaProductos.ObtenerPorReceta(receta.Id);
                return View(receta);
            }
        }













        // Delete recetas
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id, Receta_Medica receta)
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
