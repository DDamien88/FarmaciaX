using System.Collections.Generic;

namespace farmaciaX.Models
{
    public interface IRepositorioCliente
    {
        void Alta(Cliente cliente);
        IList<Cliente> ObtenerTodos();
        Cliente? BuscarPorDni(String dni);
        Cliente? ObtenerPorId(int id);
        Cliente Modificar(Cliente cliente);
        IList<Cliente> BuscarPorNombreODni(string termino);
        Cliente BuscarPorId(int id);
        List<Receta_Medica> ObtenerRecetasPorClienteId(int clienteId);
        List<Productos> ObtenerProductosPorRecetaId(int recetaId);
        List<Ventas> ObtenerVentasPorClienteId(int clienteId);
        List<DetalleVentas> ObtenerDetallesPorVentaId(int ventaId);
        List<DetalleVentas> ObtenerDetallesConProducto(int ventaId);
        List<Ventas> ObtenerVentasConDetalles(int clienteId);
        List<Receta_Medica> ObtenerRecetasConProductos(int clienteId);
    }
}
