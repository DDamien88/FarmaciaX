using farmaciaX.Models;
using MySqlConnector;

namespace farmaciaX.Models
{
    public class RepositorioProductos : RepositorioBase, IRepositorioProductos
    {

        private readonly string connectionString;

        public RepositorioProductos(IConfiguration configuration) : base(configuration)
        {
            connectionString = configuration.GetConnectionString("MySql");
        }

        public int Alta(Productos producto)
        {
            int res = -1;
            using var connection = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO productos (nombre, tipo, precio, cantidad_stock, requiere_receta, laboratorio, fecha_vencimiento, activo)
                        VALUES (@nombre, @tipo, @precio, @cantidad_Stock, @requiere_receta, @laboratorio, @fecha_vencimiento, 1);
                        SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@nombre", producto.Nombre);
            command.Parameters.AddWithValue("@tipo", producto.Tipo);
            command.Parameters.AddWithValue("@precio", producto.Precio);
            command.Parameters.AddWithValue("@cantidad_Stock", producto.Cantidad_Stock);
            command.Parameters.AddWithValue("@requiere_receta", producto.Requiere_Receta);
            command.Parameters.AddWithValue("@laboratorio", producto.Laboratorio);
            command.Parameters.AddWithValue("@fecha_vencimiento", producto.Fecha_Vencimiento);
            connection.Open();
            res = command.ExecuteNonQuery();
            connection.Close();
            return res;
        }



        public IList<Productos> ObtenerTodos()
        {
            var lista = new List<Productos>();
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT id, nombre, tipo, precio, cantidad_stock, requiere_receta, laboratorio, fecha_vencimiento, activo FROM productos";
            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Productos
                {
                    Id = reader.GetInt32("id"),
                    Nombre = reader.GetString("nombre"),
                    Tipo = reader.GetString("tipo"),
                    Precio = (decimal)(float)reader.GetDecimal("precio"),
                    Cantidad_Stock = reader.GetInt32("cantidad_stock"),
                    Requiere_Receta = reader.GetBoolean("requiere_receta"),
                    Laboratorio = reader.GetString("laboratorio"),
                    Fecha_Vencimiento = reader.GetDateTime("fecha_vencimiento"),
                    Activo = reader.GetBoolean("activo")
                });
            }
            connection.Close();
            return lista;
        }


        public Productos BuscarPorId(int id)
        {
            Productos producto = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT id, nombre, tipo, precio, cantidad_stock, requiere_receta, laboratorio, fecha_vencimiento, activo FROM productos WHERE id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Productos
                            {
                                Id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre"),
                                Tipo = reader.GetString("tipo"),
                                Precio = (decimal)(float)reader.GetDecimal("precio"),
                                Cantidad_Stock = reader.GetInt32("cantidad_stock"),
                                Requiere_Receta = reader.GetBoolean("requiere_receta"),
                                Laboratorio = reader.GetString("laboratorio"),
                                Fecha_Vencimiento = reader.GetDateTime("fecha_vencimiento"),
                                Activo = reader.GetBoolean("activo")
                            };
                        }
                    }
                }
            }
            return producto;
        }

        public Productos BuscarPorNombre(string nombre)
        {
            Productos producto = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT id, nombre, tipo, precio, cantidad_stock, requiere_receta, laboratorio, fecha_vencimiento, activo FROM productos WHERE nombre = @nombre";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", nombre);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Productos
                            {
                                Id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre"),
                                Tipo = reader.GetString("tipo"),
                                Precio = (decimal)(float)reader.GetDecimal("precio"),
                                Cantidad_Stock = reader.GetInt32("cantidad_stock"),
                                Requiere_Receta = reader.GetBoolean("requiere_receta"),
                                Laboratorio = reader.GetString("laboratorio"),
                                Fecha_Vencimiento = reader.GetDateTime("fecha_vencimiento"),
                                Activo = reader.GetBoolean("activo")
                            };
                        }
                    }
                }
            }
            return producto;
        }


        public Productos Modificar(Productos producto)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE productos SET nombre = @nombre, tipo = @tipo, precio = @precio, cantidad_stock = @cantidad_stock,
                        requiere_receta = @requiere_receta, laboratorio = @laboratorio, fecha_vencimiento = @fecha_vencimiento WHERE id = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", producto.Id);
            command.Parameters.AddWithValue("@nombre", producto.Nombre);
            command.Parameters.AddWithValue("@tipo", producto.Tipo);
            command.Parameters.AddWithValue("@precio", producto.Precio);
            command.Parameters.AddWithValue("@cantidad_stock", producto.Cantidad_Stock);
            command.Parameters.AddWithValue("@requiere_receta", producto.Requiere_Receta);
            command.Parameters.AddWithValue("@laboratorio", producto.Laboratorio);
            command.Parameters.AddWithValue("@fecha_vencimiento", producto.Fecha_Vencimiento);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            return producto;
        }


        public int Baja(int id)
        {
            int res = -1;
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE productos SET activo = 0 WHERE id = @id;";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            res = command.ExecuteNonQuery();
            connection.Close();
            return res;
        }


        public int Activar(int id)
        {
            int res = -1;
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE productos SET activo = 1 WHERE id = @id;";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            res = command.ExecuteNonQuery();
            connection.Close();
            return res;
        }


        public int ObtenerLista(int pagina, int cantidad)
        {
            int res = -1;
            using var connection = new MySqlConnection(connectionString);
            var sql = @"SELECT id, nombre, tipo, precio, cantidad_stock, requiere_receta, laboratorio, fecha_vencimiento, activo FROM productos LIMIT @cantidad OFFSET @offset";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@cantidad", cantidad);
            command.Parameters.AddWithValue("@offset", (pagina - 1) * cantidad);
            connection.Open();
            res = command.ExecuteNonQuery();
            connection.Close();
            return res;
        }

    }
}