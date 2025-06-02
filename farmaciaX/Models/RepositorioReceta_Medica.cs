using System;
using System.Data;
using farmaciaX.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MySqlConnector;

namespace farmaciaX.Models
{
    public class RepositorioReceta_Medica : RepositorioBase, IRepositorioReceta_Medica
    {
        private readonly string connectionString;
        private readonly DataContext context;
        private readonly IRepositorioCliente repositorioCliente;
        public RepositorioReceta_Medica(IConfiguration configuration, IRepositorioCliente repositorioCliente, DataContext context) : base(configuration)
        {
            connectionString = configuration.GetConnectionString("MySql");
            this.repositorioCliente = repositorioCliente;
            this.context = context;
        }

        public int Alta(Receta_Medica receta)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO receta_medica 
                        (cliente_Id, Medico, fecha_emision, fecha_vencimiento, ImgReceta, Activo) 
                        VALUES 
                        (@cliente_Id, @medico, @fecha_emision, @fecha_vencimiento, @imgReceta, 1);
                        SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@cliente_Id", receta.ClienteId);
                    command.Parameters.AddWithValue("@medico", receta.Medico);
                    command.Parameters.AddWithValue("@fecha_emision", receta.Fecha_Emision);
                    command.Parameters.AddWithValue("@fecha_vencimiento", receta.Fecha_Vencimiento);
                    command.Parameters.AddWithValue("@imgReceta", string.IsNullOrEmpty(receta.ImgReceta) ? DBNull.Value : receta.ImgReceta);

                    command.CommandType = CommandType.Text;


                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    receta.Id = res;
                    connection.Close();
                }
            }
            return res;
        }


        public void AltaProductosReceta(int recetaId, List<ProductoRecetado> productos)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                foreach (var p in productos)
                {
                    string sql = @"INSERT INTO recetaproductos (RecetaId, ProductoId, Cantidad)
                        VALUES (@recetaId, @productoId, @cantidad);";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@recetaId", recetaId);
                        command.Parameters.AddWithValue("@productoId", p.ProductoId);
                        command.Parameters.AddWithValue("@cantidad", p.Cantidad);

                        command.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }
        }






        public Receta_Medica ObtenerPorId(int id)
        {
            Receta_Medica receta = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT id, cliente_Id, medico, fecha_emision, fecha_vencimiento, imgReceta, Activo
                    FROM receta_medica 
                    WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            receta = new Receta_Medica
                            {
                                Id = reader.GetInt32("id"),
                                ClienteId = reader.GetInt32("cliente_Id"),
                                Medico = reader.GetString("medico"),
                                Fecha_Emision = reader.GetDateTime("fecha_emision"),
                                Fecha_Vencimiento = reader.GetDateTime("fecha_vencimiento"),
                                ImgReceta = reader.IsDBNull(reader.GetOrdinal("imgReceta")) ? null : reader.GetString("imgReceta"),
                                Activo = reader.GetBoolean("Activo")
                            };
                        }
                    }
                }
            }

            if (receta != null)
            {
                // Obtener también el cliente correspondiente
                receta.Cliente = repositorioCliente.ObtenerPorId(receta.ClienteId);
            }

            return receta;
        }



        public IList<Receta_Medica> ObtenerTodos()
        {
            var lista = new List<Receta_Medica>();
            using var connection = new MySqlConnection(connectionString);
            var sql = @"
                        SELECT r.Id, r.cliente_Id, r.Medico, r.fecha_emision, r.fecha_vencimiento, r.ImgReceta, r.Activo,
                            c.Nombre, c.Apellido, c.Dni
                        FROM receta_medica r
                        LEFT JOIN cliente c ON r.cliente_Id = c.Id";
            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var receta = new Receta_Medica
                {
                    Id = reader.GetInt32("Id"),
                    ClienteId = reader.GetInt32("cliente_Id"),
                    Medico = reader.GetString("Medico"),
                    Fecha_Emision = reader.GetDateTime("fecha_emision"),
                    Fecha_Vencimiento = reader.GetDateTime("fecha_vencimiento"),
                    ImgReceta = reader.IsDBNull(reader.GetOrdinal("ImgReceta")) ? null : reader.GetString("ImgReceta"),
                    Activo = reader.GetBoolean("Activo"),
                    Cliente = new Cliente
                    {
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Dni = reader.GetString("Dni")
                    }
                };
                lista.Add(receta);
            }
            return lista;
        }

        public void Modificar(Receta_Medica receta)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE receta_medica 
                        SET cliente_Id = @cliente_Id,
                            medico = @medico,
                            fecha_emision = @fecha_emision,
                            fecha_vencimiento = @fecha_vencimiento,
                            imgReceta = @imgReceta
                        WHERE id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@cliente_Id", receta.ClienteId);
                    command.Parameters.AddWithValue("@medico", receta.Medico);
                    command.Parameters.AddWithValue("@fecha_emision", receta.Fecha_Emision);
                    command.Parameters.AddWithValue("@fecha_vencimiento", receta.Fecha_Vencimiento);
                    command.Parameters.AddWithValue("@imgReceta", string.IsNullOrEmpty(receta.ImgReceta) ? DBNull.Value : receta.ImgReceta);
                    command.Parameters.AddWithValue("@id", receta.Id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }



        public int Eliminar(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    DELETE FROM recetaproductos WHERE RecetaId = @id;
                    DELETE FROM receta_medica WHERE Id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }


        public int Activar(int id)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE receta_medica SET
                    Activo=1
                    WHERE Id = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Receta_Medica> BuscarPorClienteOMedicoOFecha(string termino)
        {
            var lista = new List<Receta_Medica>();
            using var connection = new MySqlConnection(connectionString);
            string sql = @"SELECT r.Id, r.cliente_Id, r.Medico, r.fecha_emision, r.fecha_vencimiento, r.ImgReceta, r.Activo,
            c.Nombre, c.Apellido, c.Dni FROM receta_medica r
            LEFT JOIN cliente c ON r.cliente_Id = c.Id
            WHERE r.Medico LIKE @termino OR c.Nombre LIKE @termino OR c.Apellido LIKE @termino OR r.fecha_emision LIKE @termino OR r.fecha_vencimiento LIKE @termino";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@termino", $"%{termino}%");
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var receta = new Receta_Medica
                {
                    Id = reader.GetInt32("Id"),
                    ClienteId = reader.GetInt32("cliente_Id"),
                    Medico = reader.GetString("Medico"),
                    Fecha_Emision = reader.GetDateTime("fecha_emision"),
                    Fecha_Vencimiento = reader.GetDateTime("fecha_vencimiento"),
                    ImgReceta = reader.IsDBNull(reader.GetOrdinal("ImgReceta")) ? null : reader.GetString("ImgReceta"),
                    Activo = reader.GetBoolean("Activo"),
                    Cliente = new Cliente
                    {
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Dni = reader.GetString("Dni")
                    }
                };
                lista.Add(receta);
            }
            return lista;
        }


        public IEnumerable<Receta_Medica> BuscarPorTexto(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return ObtenerTodos();

            return context.Receta
                .Include(r => r.Cliente)
                .Where(r =>
                    r.Medico.Contains(termino) ||
                    r.Cliente.Nombre.Contains(termino) ||
                    r.Cliente.Apellido.Contains(termino) ||
                    r.Cliente.Dni.Contains(termino) ||
                    r.Fecha_Emision.ToString().Contains(termino) ||
                    r.Fecha_Vencimiento.ToString().Contains(termino) ||
                    r.ImgReceta.Contains(termino)
                )
                .ToList();
        }



        public IList<Receta_Medica> BuscarPorCliente(int id)
        {
            var lista = new List<Receta_Medica>();
            using var connection = new MySqlConnection(connectionString);
            string sql = @"SELECT r.Id, r.cliente_Id, r.Medico, r.fecha_emision, r.fecha_vencimiento, r.ImgReceta, r.Activo,
            c.Nombre, c.Apellido, c.Dni FROM receta_medica r
            LEFT JOIN clientes c ON r.cliente_Id = c.Id
            WHERE r.cliente_Id = @id" + " AND r.Activo = 1";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var receta = new Receta_Medica
                {
                    Id = reader.GetInt32("Id"),
                    ClienteId = reader.GetInt32("cliente_Id"),
                    Medico = reader.GetString("Medico"),
                    Fecha_Emision = reader.GetDateTime("fecha_emision"),
                    Fecha_Vencimiento = reader.GetDateTime("fecha_vencimiento"),
                    ImgReceta = reader.IsDBNull(reader.GetOrdinal("ImgReceta")) ? null : reader.GetString("ImgReceta"),
                    Activo = reader.GetBoolean("Activo"),
                    Cliente = new Cliente
                    {
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Dni = reader.GetString("Dni")
                    }
                };
                lista.Add(receta);
            }
            return lista;
        }



        public List<Receta_Medica> ObtenerTodasConProductos()
        {
            return context.Receta
                .Include(r => r.Cliente)
                .Include(r => r.RecetaProductos)
                    .ThenInclude(rp => rp.Producto)
                .ToList();
        }





    }
}