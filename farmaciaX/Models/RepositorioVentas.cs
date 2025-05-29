using System;
using System.Data;
using farmaciaX.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace farmaciaX.Models
{
    public class RepositorioVentas : RepositorioBase, IRepositorioVentas
    {
        private readonly string connectionString;
        private readonly DataContext context;
        public RepositorioVentas(IConfiguration configuration) : base(configuration)
        {
            connectionString = configuration.GetConnectionString("MySql");
        }

        public int Alta(Ventas ventas)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    INSERT INTO ventas(id, cliente_Id, fecha, total)
                    VALUES(@id, @cliente_Id, @fecha, @total)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", ventas.Id);
                    //El cliente puede ser null
                    command.Parameters.AddWithValue("@cliente_Id", ventas.Cliente_Id);
                    command.Parameters.AddWithValue("@fecha", ventas.Fecha);
                    command.Parameters.AddWithValue("@total", ventas.Total);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }



        /*public Ventas ObtenerPorId(int id)
        {
            return context.Ventas
                .Include(v => v.VentaProductos)
                    .ThenInclude(vp => vp.Producto)
                .FirstOrDefault(v => v.Id == id);
        }*/


        public Ventas ObtenerPorId(int id)
        {
            var ventas = new Ventas();
            using var connection = new MySqlConnection(connectionString);
            var sql = @"
                SELECT Id, cliente_Id, fecha, total
                FROM ventas
                WHERE Id = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ventas.Id = reader.GetInt32("Id");
                ventas.Cliente_Id = reader.GetInt32("cliente_Id");
                ventas.Fecha = reader.GetDateTime("fecha");
                ventas.Total = (decimal)(float)reader.GetDecimal("total");
            }
            return ventas;
        }


        public IList<Ventas> ObtenerTodos()
        {
            var ventas = new List<Ventas>();
            using var connection = new MySqlConnection(connectionString);
            var sql = @"
                SELECT Id, cliente_Id, fecha, total
                FROM ventas";
            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ventas.Add(new Ventas
                {
                    Id = reader.GetInt32("Id"),
                    Cliente_Id = reader.GetInt32("cliente_Id"),
                    Fecha = reader.GetDateTime("fecha"),
                    Total = (decimal)(float)reader.GetDecimal("total")
                });
            }
            return ventas;
        }

        //Método mysql modificar receta para editar con sql
        public int Modificar(Ventas ventas)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE ventas SET
                    cliente_Id=@cliente_Id,
                    fecha=@fecha,
                    total=@total
                    WHERE Id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", ventas.Id);
                    command.Parameters.AddWithValue("@cliente_Id", ventas.Cliente_Id);
                    command.Parameters.AddWithValue("@fecha", ventas.Fecha);
                    command.Parameters.AddWithValue("@total", ventas.Total);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }




        public IList<Ventas> BuscarPorCliente(string termino)
        {
            return context.Ventas
                .Include(r => r.Cliente)
                .Where(r => r.Cliente.Id == int.Parse(termino))
                .ToList();
        }


        public IEnumerable<Ventas> BuscarPorTexto(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return ObtenerTodos();

            return context.Ventas
                .Include(r => r.Cliente)
                .Where(r =>
                    r.Cliente.Id.ToString().Contains(termino) ||
                    r.Cliente.Nombre.Contains(termino) ||
                    r.Cliente.Apellido.Contains(termino) ||
                    r.Cliente.Dni.Contains(termino) ||
                    r.Fecha.ToString().Contains(termino) ||
                    r.Total.ToString().Contains(termino)
                )
                .ToList();
        }



    }
}