
namespace farmaciaX.Models
{
    public interface IRepositorioReceta_Medica
    {
        Receta_Medica ObtenerPorId(int id);
        int Alta(Receta_Medica receta);
        void Modificar(Receta_Medica receta);
        int Eliminar(int id);
        int Activar(int id);
        IList<Receta_Medica> BuscarPorClienteOMedicoOFecha(string termino);
        IEnumerable<Receta_Medica> BuscarPorTexto(string termino);
        void AltaProductosReceta(int recetaId, List<ProductoRecetado> productos);
        public IList<Receta_Medica> BuscarPorCliente(int id);
        List<Receta_Medica> ObtenerTodasConProductos();

    }
}