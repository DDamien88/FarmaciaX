namespace farmaciaX.Models
{
    public interface IRepositorioDetalleVentas
    {
        int Guardar(DetalleVentas detalle);
        int Modificar(DetalleVentas detalle);
    }
}