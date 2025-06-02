using Microsoft.AspNetCore.Mvc;
using farmaciaX.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace farmaciaX.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private readonly IRepositorioVentas repositorioVentas;

        private readonly IRepositorioDetalleVentas repositorioDetalleVentas;
        private readonly IRepositorioReceta_Medica repositorio;
        private readonly IWebHostEnvironment env;

        private readonly IRepositorioCliente repositorioCliente;
        private readonly DataContext context;

        private readonly IRepositorioProductos repositorioProductos;
        private readonly IRepositorioUsuario repositorioUsuario;

        private readonly IRepositorioRecetaProducto repositorioRecetaProductos;

        public VentasController(IRepositorioReceta_Medica repositorio, IWebHostEnvironment env, IRepositorioCliente repositorioCliente, DataContext context, IRepositorioVentas repositorioVentas, IRepositorioDetalleVentas repositorioDetalleVentas, IRepositorioProductos repositorioProductos, IRepositorioUsuario repositorioUsuario, IRepositorioRecetaProducto repositorioRecetaProductos)
        {
            this.repositorio = repositorio;
            this.env = env;
            this.repositorioCliente = repositorioCliente;
            this.context = context;
            this.repositorioVentas = repositorioVentas;
            this.repositorioDetalleVentas = repositorioDetalleVentas;
            this.repositorioUsuario = repositorioUsuario;
            this.repositorioRecetaProductos = repositorioRecetaProductos;
            this.repositorioProductos = repositorioProductos;
        }

        //GET index
        public IActionResult Index()
        {
            return View();
        }

        //Get create ventas
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Post create ventas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ventas venta)
        {
            try
            {

                var email = User.Identity.Name;
                var usuario = repositorioUsuario.ObtenerPorEmail(email);
                if (usuario == null)
                {
                    return BadRequest("No se encontró el usuario autenticado.");
                }

                if (venta.VentaProductos == null)
                {
                    ModelState.AddModelError("", "Debe incluir al menos un producto.");
                    return View(venta);
                }

                decimal totalVenta = 0;
                bool requiereReceta = false;

                foreach (var detalle in venta.VentaProductos)
                {
                    var producto = context.Productos.Find(detalle.ProductoId);
                    if (producto == null)
                    {
                        ModelState.AddModelError("", "Producto no encontrado.");
                        return View(venta);
                    }

                    if (producto.Cantidad_Stock < detalle.Cantidad)
                    {
                        ModelState.AddModelError("", $"Stock insuficiente para el producto {producto.Nombre}.");
                        return View(venta);
                    }

                    producto.Cantidad_Stock -= detalle.Cantidad;
                    if (producto.Cantidad_Stock == 0)
                    {
                        producto.Activo = false;
                    }

                    detalle.Venta_Id = venta.Id;
                    detalle.SubTotal = producto.Precio * detalle.Cantidad;
                    totalVenta += detalle.SubTotal;

                    if (producto.Requiere_Receta)
                        requiereReceta = true;
                }

                // Validar si necesita receta y no se proporcionó
                if (requiereReceta && venta.RecetaId == null)
                {
                    ModelState.AddModelError("", "Uno o más productos requieren receta médica.");
                    return View(venta);
                }

                // Validar pagos
                if (venta.Pagos == null || !venta.Pagos.Any())
                {
                    ModelState.AddModelError("", "Debe constatar al menos un pago para la venta.");
                    return View(venta);
                }

                var totalPagado = venta.Pagos.Sum(p => p.Monto);
                if (totalPagado < totalVenta)
                {
                    ModelState.AddModelError("", "El monto total de los pagos debe ser igual o mayor al total de la venta.");
                    return View(venta);
                }

                foreach (var pago in venta.Pagos)
                {
                    pago.FechaPago = DateTime.Now;
                    pago.VentaId = venta.Id;


                }

                if (venta.RecetaId.HasValue)
                {
                    var receta = context.Receta.Find(venta.RecetaId);
                    if (receta.Fecha_Vencimiento < DateTime.Now)
                    {
                        ModelState.AddModelError("", "La receta médica ha caducado.");
                        return View(venta);
                    }
                }

                venta.Total = totalVenta;
                venta.Fecha = DateTime.Now;
                venta.UsuarioAltaId = usuario.Id;
                venta.Activo = true;
                venta.UsuarioBajaId = null;

                // Dar de baja a la receta (si aplica)
                if (venta.RecetaId != null)
                {
                    var receta = context.Receta.Find(venta.RecetaId);
                    if (receta != null)
                        receta.Activo = false;
                }
                context.Ventas.Add(venta);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(venta);
            }
        }












        //Buscar Ventas
        [HttpGet]
        public IActionResult Buscar(string termino = "", int page = 1, int pageSize = 5)
        {
            var query = context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Receta)
                .Include(v => v.VentaProductos)
                    .ThenInclude(pv => pv.Producto)
                .Where(v => string.IsNullOrEmpty(termino) ||
                            v.Cliente.Nombre.Contains(termino) ||
                            v.Receta.Medico.Contains(termino));

            var totalItems = query.Count();

            var ventas = query
                .OrderByDescending(v => v.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new
                {
                    id = v.Id,
                    cliente = v.Cliente.Nombre + " " + v.Cliente.Apellido,
                    fecha = v.Fecha,
                    fechaReceta = v.Receta != null ? v.Receta.Fecha_Emision : (DateTime?)null,
                    total = v.Total,
                    activo = v.Activo,
                    productos = v.VentaProductos.Select(pv => new
                    {
                        nombre = pv.Producto.Nombre,
                        cantidad = pv.Cantidad
                    }).ToList()
                })
                .ToList();

            return Json(new
            {
                ventas,
                totalItems
            });
        }










        [HttpGet]
        public IActionResult ObtenerProductosPorReceta(int recetaId)
        {
            var productos = context.RecetaProductos
                .Where(rp => rp.RecetaId == recetaId)
                .Include(rp => rp.Producto)
                .Select(rp => new
                {
                    productoId = rp.ProductoId,
                    nombre = rp.Producto.Nombre,
                    precio = rp.Producto.Precio,
                    cantidad = rp.Cantidad
                }).ToList();

            return Ok(productos);
        }




        // Opción para buscar productos 
        public IActionResult BuscarProductos(string filtro)
        {
            var productos = context.Productos
                .Where(p => p.Nombre.Contains(filtro) || p.Laboratorio.Contains(filtro))
                .Select(p => new { p.Id, p.Nombre, p.Precio, p.Laboratorio })
                .ToList();
            Console.WriteLine(productos);
            return Json(productos);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult AnularVenta(int ventaId)
        {
            var email = User.Identity.Name;
            var usuario = repositorioUsuario.ObtenerPorEmail(email);
            if (usuario == null)
            {
                return BadRequest("No se encontró el usuario autenticado.");
            }

            var venta = context.Ventas
                .Include(v => v.VentaProductos)
                    .ThenInclude(vp => vp.Producto)
                .Include(v => v.Receta)
                .FirstOrDefault(v => v.Id == ventaId);

            if (venta.Activo == false)
            {
                return BadRequest("La venta ya ha sido anulada.");
            }

            venta.Activo = false;
            venta.UsuarioBajaId = usuario.Id;

            context.SaveChanges();

            return RedirectToAction("Index");
        }


        [Authorize(Policy = "Administrador")]
        public IActionResult Details(int id)
        {
            var venta = context.Ventas
        .Include(v => v.Cliente)
        .Include(v => v.VentaProductos)
            .ThenInclude(vp => vp.Producto)
        .Include(v => v.Pagos)
        .Include(v => v.Receta)
        .Include(v => v.UsuarioAlta)
        .Include(v => v.UsuarioBaja)
        .FirstOrDefault(v => v.Id == id);

            if (venta == null)
                return NotFound();

            return View(venta);
        }


    }
}
