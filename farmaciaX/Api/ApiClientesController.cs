using System.Text.Json;
using farmaciaX.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("api/clientes")]
[ApiController]
public class ApiClientesController : ControllerBase
{
    private readonly DataContext context;

    public ApiClientesController(DataContext context)
    {
        this.context = context;
    }
    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar(string termino)
    {
        var clientes = await context.Clientes
            .Where(c => c.Nombre.Contains(termino) || c.Apellido.Contains(termino) || c.Dni.Contains(termino))
            .ToListAsync();

        return Ok(clientes);
    }

}
