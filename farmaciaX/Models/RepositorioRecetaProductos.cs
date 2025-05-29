using System.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace farmaciaX.Models
{
    public class RepositorioRecetaProductos : RepositorioBase, IRepositorioRecetaProductos
    {
        private readonly string connectionString;
        private readonly IRepositorioReceta_Medica repositorioReceta_Medica;
        private readonly DataContext context;
        public RepositorioRecetaProductos(IConfiguration configuration, IRepositorioReceta_Medica repositorioReceta_Medica, DataContext context) : base(configuration)
        {
            connectionString = configuration.GetConnectionString("MySql");
            this.repositorioReceta_Medica = repositorioReceta_Medica;
            this.context = context;
        }

        public int Alta(RecetaProductos recetaProductos)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO recetaproductos (recetaId, productoId, cantidad)
                    VALUES (@receta_Id, @producto_Id, @cantidad);";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@receta_Id", recetaProductos.RecetaId);
                    command.Parameters.AddWithValue("@producto_Id", recetaProductos.ProductoId);
                    command.Parameters.AddWithValue("@cantidad", recetaProductos.Cantidad);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public List<Productos> ObtenerTodos()
        {
            List<Productos> productos = new List<Productos>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT id, nombre, tipo, precio, cantidad_stock, requiere_receta, laboratorio, fecha_vencimiento, activo FROM productos;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        productos.Add(new Productos
                        {
                            Id = reader.GetInt32("Id"),
                            Nombre = reader.GetString("Nombre"),
                            Tipo = reader.GetString("Tipo"),
                            Precio = reader.GetDecimal("Precio"),
                            Cantidad_Stock = reader.GetInt32("Cantidad_Stock"),
                            Requiere_Receta = reader.GetBoolean("Requiere_Receta"),
                            Laboratorio = reader.GetString("Laboratorio"),
                            Fecha_Vencimiento = reader.GetDateTime("Fecha_Vencimiento"),
                            Activo = reader.GetBoolean("Activo")
                        });
                    }
                    connection.Close();
                }
            }
            return productos;
        }




        public List<RecetaProductos> ObtenerPorReceta(int recetaId)
        {
            var lista = new List<RecetaProductos>();

            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT rp.RecetaId, rp.ProductoId, rp.Cantidad,
                    p.Nombre, p.Tipo, p.Precio, p.Cantidad_Stock, p.Requiere_Receta, p.Laboratorio, p.Fecha_Vencimiento
                    FROM recetaproductos rp
                    JOIN productos p ON p.Id = rp.ProductoId
                    WHERE rp.RecetaId = @recetaId";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@recetaId", recetaId);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new RecetaProductos
                            {
                                RecetaId = recetaId,
                                ProductoId = reader.GetInt32("ProductoId"),
                                Cantidad = reader.GetInt32("Cantidad"),
                                Producto = new Productos
                                {
                                    Id = reader.GetInt32("ProductoId"),
                                    Nombre = reader.GetString("Nombre"),
                                    Tipo = reader.GetString("Tipo"),
                                    Precio = (decimal)reader.GetFloat("Precio"),
                                    Cantidad_Stock = reader.GetInt32("Cantidad_Stock"),
                                    Requiere_Receta = reader.GetBoolean("Requiere_Receta"),
                                    Laboratorio = reader.GetString("Laboratorio"),
                                    Fecha_Vencimiento = reader.GetDateTime("Fecha_Vencimiento")
                                }
                            });
                        }
                    }

                    connection.Close();
                }
            }

            return lista;
        }




        public void EliminarTodosPorReceta(int recetaId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM recetaproductos WHERE RecetaId = @recetaId;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@recetaId", recetaId);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public List<RecetaProductos> ObtenerRecetasYProductos()
        {
            var recetasProductos = new List<RecetaProductos>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
            SELECT 
                rp.RecetaId, rp.ProductoId, rp.Cantidad,
                r.Fecha_Emision, r.Medico, r.Cliente_Id,
                p.Nombre, p.Tipo, p.Precio, p.Cantidad_Stock, p.Requiere_Receta, p.Laboratorio, p.Fecha_Vencimiento
            FROM recetaproductos rp
            JOIN productos p ON p.Id = rp.ProductoId
            JOIN receta_medica r ON r.Id = rp.RecetaId;
        ";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var receta = new Receta_Medica
                        {
                            Id = reader.GetInt32("RecetaId"),
                            Fecha_Emision = reader.GetDateTime("Fecha_Emision"),
                            Medico = reader.GetString("Medico"),
                            ClienteId = reader.GetInt32("Cliente_Id")
                        };

                        var producto = new Productos
                        {
                            Id = reader.GetInt32("ProductoId"),
                            Nombre = reader.GetString("Nombre"),
                            Tipo = reader.GetString("Tipo"),
                            Precio = (decimal)reader.GetFloat("Precio"),
                            Cantidad_Stock = reader.GetInt32("Cantidad_Stock"),
                            Requiere_Receta = reader.GetBoolean("Requiere_Receta"),
                            Laboratorio = reader.GetString("Laboratorio"),
                            Fecha_Vencimiento = reader.GetDateTime("Fecha_Vencimiento")
                        };

                        recetasProductos.Add(new RecetaProductos
                        {
                            RecetaId = receta.Id,
                            ProductoId = producto.Id,
                            Cantidad = reader.GetInt32("Cantidad"),
                            Receta = receta,
                            Producto = producto
                        });
                    }
                    connection.Close();
                }
            }
            return recetasProductos;
        }



    }
}