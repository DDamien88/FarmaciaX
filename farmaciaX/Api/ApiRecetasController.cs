using farmaciaX.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//namespace farmaciaX.Api;
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//[ApiController]
public class ApiRecetasController : ControllerBase
{
    private readonly DataContext context;
    private readonly IRepositorioProductos repositorioProductos;
    private readonly IRepositorioRecetaProductos repositorioRecetaProductos;

    public ApiRecetasController(DataContext context, IRepositorioProductos repositorioProductos, IRepositorioRecetaProductos repositorioRecetaProductos)
    {
        this.context = context;
        this.repositorioProductos = repositorioProductos;
        this.repositorioRecetaProductos = repositorioRecetaProductos;
    }



    [HttpGet("traer-recetas")]
    public async Task<IActionResult> TraerRecetas()
    {
        try
        {
            var recetas = repositorioRecetaProductos.ObtenerRecetasYProductos();
            var json = System.Text.Json.JsonSerializer.Serialize(recetas);
            return Ok(json);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener las recetas", detalle = ex.Message });
        }
    }






    [HttpGet("{id}/productos")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRecetaProductos(int id)
    {
        try
        {
            var receta = await context.Receta
                .Include(r => r.RecetaProductos)
                    .ThenInclude(rp => rp.Producto)
                .FirstOrDefaultAsync(r => r.Activo && r.Id == id);

            if (receta == null)
                return NotFound();

            return Ok(receta.RecetaProductos.Select(p => new
            {
                id = p.Producto.Id,
                nombre = p.Producto.Nombre,
                precio = p.Producto.Precio,
                cantidad = p.Cantidad,
                laboratorio = p.Producto.Laboratorio,
                cantidad_Stock = p.Producto.Cantidad_Stock
            }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener los productos de la receta", detalle = ex.Message });
        }
    }



}

