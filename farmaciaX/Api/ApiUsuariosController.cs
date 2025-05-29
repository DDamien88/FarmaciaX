using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using farmaciaX.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace farmaciaX.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ApiUsuariosController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IConfiguration config;
        private readonly IRepositorioUsuario repositorio;
        public ApiUsuariosController(DataContext context, IConfiguration config, IRepositorioUsuario repositorio)
        {
            this.context = context;
            this.config = config;
            this.repositorio = repositorio;
        }



        // POST api/<controller>/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginView login)
        {
            try
            {

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: login.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                var e = await context.Usuarios.FirstOrDefaultAsync(x => x.Email == login.Usuario);

                if (e == null || e.Clave != hashed)
                {
                    return BadRequest("Nombre de usuario o clave incorrecta");
                }
                else
                {
                    var key = new SymmetricSecurityKey(
                        System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim("FullName", e.Nombre + " " + e.Apellido),
                        new Claim(ClaimTypes.Role, e.RolNombre),
                    };

                    var token = new JwtSecurityToken(
                            issuer: config["TokenAuthentication:Issuer"],
                            audience: config["TokenAuthentication:Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(60),
                            signingCredentials: credenciales
                        );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}