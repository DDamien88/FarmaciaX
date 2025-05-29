namespace farmaciaX.Models
{
    public interface IRepositorioRecetaProductos
    {
        int Alta(RecetaProductos recetaProductos);
        List<RecetaProductos> ObtenerPorReceta(int recetaId);
        List<Productos> ObtenerTodos();
        void EliminarTodosPorReceta(int recetaId);

        List<RecetaProductos> ObtenerRecetasYProductos();
    }
}