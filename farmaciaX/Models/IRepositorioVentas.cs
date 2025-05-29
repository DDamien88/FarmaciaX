
namespace farmaciaX.Models
{
    public interface IRepositorioVentas
    {
        IList<Ventas> ObtenerTodos();
        Ventas ObtenerPorId(int id);
        int Alta(Ventas ventas);
        int Modificar(Ventas ventas);
        IList<Ventas> BuscarPorCliente(string termino);
        IEnumerable<Ventas> BuscarPorTexto(string termino);
    }
}