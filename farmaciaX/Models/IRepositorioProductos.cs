
namespace farmaciaX.Models
{
    public interface IRepositorioProductos
    {
        int Alta(Productos producto);
        IList<Productos> ObtenerTodos();
        Productos? BuscarPorId(int id);
        Productos Modificar(Productos producto);
        int Baja(int id);
        int Activar(int id);
        int ObtenerLista(int pagina, int cantidad);
        Productos BuscarPorNombre(string nombre);
        
    }
}