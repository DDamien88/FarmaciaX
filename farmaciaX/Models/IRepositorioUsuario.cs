using System;
using System.Collections.Generic;
using System.Linq;
using farmaciaX.Models;

namespace farmaciaX.Models
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        int Activar(int id);
        Usuario ObtenerPorEmail(string email);
        int ObtenerPorIdDos(int id);

        Usuario ObtenerPorEmailParaLogin(string email);
    }
}