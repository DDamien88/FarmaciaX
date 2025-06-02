namespace farmaciaX.Models
{
    public interface IRepositorioRecetaProducto
    {
        int Alta(RecetaProducto recetaProductos);
        List<RecetaProducto> ObtenerPorReceta(int recetaId);
        List<Productos> ObtenerTodos();
        void EliminarTodosPorReceta(int recetaId);

        List<RecetaProducto> ObtenerRecetasYProductos();
        
    }
}