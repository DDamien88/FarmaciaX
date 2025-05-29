using farmaciaX.Models;

namespace farmaciaX.Models
{
    public interface IRepositorioImagen : IRepositorio<Imagen>
    {
        IList<Imagen> BuscarPorReceta(int recetaId);
    }
}