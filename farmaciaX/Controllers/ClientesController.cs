using Microsoft.AspNetCore.Mvc;
using farmaciaX.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace farmaciaX.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly IRepositorioCliente repositorio;
        private readonly DataContext context;

        public ClientesController(IRepositorioCliente repositorio, DataContext context)
        {
            this.repositorio = repositorio;
            this.context = context;
        }


        // GET: Index - Clientes
        public IActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cliente cliente)
        {
            try
            {
                var existente = repositorio.BuscarPorDni(cliente.Dni);
                if (existente != null)
                {
                    ModelState.AddModelError("Dni", "Ya existe un cliente con este DNI.");
                }

                if (ModelState.IsValid)
                {
                    repositorio.Alta(cliente);
                    TempData["Mensaje"] = "Cliente creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear cliente: " + ex.Message;
                return View(cliente);
            }
        }





        //GET Edit Clientes
        public IActionResult Edit(int id)
        {
            var cliente = repositorio.BuscarPorId(id);
            return View(cliente);
        }

        //Post Edit clientes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Cliente cliente)
        {
            try
            {
                repositorio.Modificar(cliente);
                TempData["Mensaje"] = "Cliente modificado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al modificar cliente: " + ex.Message;
                return View(cliente);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Buscar(string termino, int pagina = 1, int tamanioPagina = 5)
        {
            var query = context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                query = query.Where(c =>
                    c.Nombre.Contains(termino) ||
                    c.Apellido.Contains(termino) ||
                    c.Dni.Contains(termino));
            }

            var total = await query.CountAsync();
            var clientes = await query
                .OrderBy(c => c.Apellido)
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .ToListAsync();

            return Json(new
            {
                clientes,
                total
            });
        }

        [Authorize(Policy = "Administrador")]

        public IActionResult Details(int id)
        {
            var cliente = repositorio.ObtenerPorId(id);

            if (cliente == null) return NotFound();

            return View(cliente);
        }



    }

}

