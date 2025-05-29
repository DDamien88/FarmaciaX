using System;
using System.Data;
using farmaciaX.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace farmaciaX.Models
{
    public class RepositorioDetalleVentas : RepositorioBase, IRepositorioDetalleVentas
    {

        private readonly string connectionString;
        private readonly DataContext context;
        public RepositorioDetalleVentas(IConfiguration configuration) : base(configuration)
        {
            connectionString = configuration.GetConnectionString("MySql");
        }

        public int Guardar(DetalleVentas detalle)
        {
            int res = -1;
            using var connection = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO detalleventas (venta_id, productoId, cantidad, subTotal) VALUES (@venta_id, @productoId, @cantidad, @subTotal);";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@venta_id", detalle.Venta_Id);
            command.Parameters.AddWithValue("@productoId", detalle.ProductoId);
            command.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
            command.Parameters.AddWithValue("@subTotal", detalle.SubTotal);
            connection.Open();
            res = command.ExecuteNonQuery();
            connection.Close();
            return res;
        }



        public int Modificar(DetalleVentas detalle)
        {
            int res = -1;
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE detalleventas SET venta_id=@venta_id, productoId=@productoId, cantidad=@cantidad, subTotal=@subTotal WHERE Id = @id;";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", detalle.Id);
            command.Parameters.AddWithValue("@venta_id", detalle.Venta_Id);
            command.Parameters.AddWithValue("@productoId", detalle.ProductoId);
            command.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
            command.Parameters.AddWithValue("@subTotal", detalle.SubTotal);
            connection.Open();
            res = command.ExecuteNonQuery();
            connection.Close();
            return res;
        }   


    }
}