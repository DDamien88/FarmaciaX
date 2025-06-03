using farmaciaX.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("api/productos")]
[ApiController]
public class ProductosApiController : ControllerBase
{
    private readonly IRepositorioProductos repo;
    private readonly DataContext context;

    public ProductosApiController(IRepositorioProductos repo, DataContext context)
    {
        this.repo = repo;
        this.context = context;
    }

    [HttpGet]
    public IActionResult Get() => Ok(repo.ObtenerTodos());

    [HttpPost]
    public IActionResult Post([FromBody] Productos p)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("ModelState no es valido.");
        }

        if (p.Precio <= 0)
        {
            return BadRequest("El precio debe ser mayor que cero.");
        }

        if (p.Cantidad_Stock <= 0)
        {
            return BadRequest("La cantidad de stock debe ser mayor que cero.");
        }

        if (p.Fecha_Vencimiento < DateTime.Now)
        {
            return BadRequest("La fecha de vencimiento no puede ser menor a la fecha actual.");
        }


        if (repo.BuscarPorNombre(p.Nombre.ToLower()) != null)
        {
            return BadRequest("El producto ya existe. Lo puede editar en el listado.");
        }

        repo.Alta(p);
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] Productos p)
    {
        p.Id = id;
        if (!ModelState.IsValid)
        {
            return BadRequest("ModelState no es valido.");
        }

        if (p.Precio <= 0)
        {
            return BadRequest("El precio debe ser mayor que cero.");
        }

        if (p.Cantidad_Stock <= 0)
        {
            return BadRequest("La cantidad de stock debe ser mayor que cero.");
        }

        if (p.Fecha_Vencimiento < DateTime.Now)
        {
            return BadRequest("La fecha de vencimiento no puede ser menor a la fecha actual.");

        }
        repo.Modificar(p);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        repo.Baja(id);
        return Ok();
    }

    [HttpPost("activar/{id}")]
    public IActionResult Activar(int id)
    {
        repo.Activar(id);
        return Ok();
    }

    // Para uso en EDIT DE VENTAS
    [Route("recetas/por-cliente/{clienteId}")]
    [HttpGet]
    public IActionResult ObtenerRecetasPorCliente(int clienteId)
    {
        var recetas = context.Receta
            .Where(r => r.ClienteId == clienteId && r.Activo)
            .Select(r => new
            {
                id = r.Id,
                descripcion = $"Receta: - Médico: {r.Medico} - Fecha de emisión: {r.Fecha_Emision.ToShortDateString()} - Fecha de vencimiento: {r.Fecha_Vencimiento.ToShortDateString()}"
            })
            .ToList();

        return Ok(recetas);
    }



    [HttpGet("obtener-todos-los-productos")]
    public async Task<IActionResult> Buscar(string termino)
    {
        var productos = await context.Productos
            .Where(p => p.Nombre.Contains(termino) && p.Activo)
            .ToListAsync();

        return Ok(productos);
    }



}
