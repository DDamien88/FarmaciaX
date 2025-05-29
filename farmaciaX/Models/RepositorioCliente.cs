using System.Data;
using farmaciaX.Models;
using MySqlConnector;


namespace farmaciaX.Models
{
    public class RepositorioCliente : RepositorioBase, IRepositorioCliente
    {
        private readonly string connectionString;

        public RepositorioCliente(IConfiguration configuration) : base(configuration)
        {
            connectionString = configuration.GetConnectionString("MySql");
        }

        public void Alta(Cliente cliente)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"INSERT INTO clientes (nombre, apellido, dni, telefono, email)
                        VALUES (@nombre, @apellido, @dni, @telefono, @correo);
                        SELECT LAST_INSERT_ID();";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@nombre", cliente.Nombre);
            command.Parameters.AddWithValue("@apellido", cliente.Apellido);
            command.Parameters.AddWithValue("@dni", cliente.Dni);
            command.Parameters.AddWithValue("@telefono", cliente.Telefono);
            command.Parameters.AddWithValue("@correo", cliente.Email);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        public IList<Cliente> ObtenerTodos()
        {
            var lista = new List<Cliente>();
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT id, nombre, apellido, dni, telefono, email FROM clientes";
            using var command = new MySqlCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Cliente
                {
                    Id = reader.GetInt32("id"),
                    Nombre = reader.GetString("nombre"),
                    Apellido = reader.GetString("apellido"),
                    Dni = reader.GetString("dni"),
                    Telefono = reader.GetString("telefono"),
                    Email = reader.GetString("email")
                });
            }
            connection.Close();
            return lista;
        }



        public Cliente? BuscarPorDni(String dni)
        {
            Cliente? cliente = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT id, nombre, apellido, dni, telefono, email FROM clientes WHERE dni = @dni";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", dni);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = new Cliente
                            {
                                Id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre"),
                                Apellido = reader.GetString("apellido"),
                                Dni = reader.GetString("dni"),
                                Telefono = reader.GetString("telefono"),
                                Email = reader.GetString("email")
                            };
                        }
                    }
                }
            }
            return cliente;
        }

        public Cliente ObtenerPorId(int id)
        {
            Cliente cliente = null;

            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT Id, Nombre, Apellido, Dni, Telefono, Email FROM clientes WHERE Id = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                cliente = new Cliente
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Dni = reader.GetString("Dni"),
                    Telefono = reader.GetString("Telefono"),
                    Email = reader.GetString("Email")
                };
            }
            connection.Close();

            if (cliente != null)
            {
                cliente.Recetas = ObtenerRecetasConProductos(cliente.Id);
                cliente.Ventas = ObtenerVentasConDetalles(cliente.Id);
            }

            return cliente;
        }




        public Cliente Modificar(Cliente cliente)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = @"UPDATE clientes SET nombre = @nombre, apellido = @apellido, dni = @dni, telefono = @telefono, email = @email
                        WHERE id = @id;";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", cliente.Id);
            command.Parameters.AddWithValue("@nombre", cliente.Nombre);
            command.Parameters.AddWithValue("@apellido", cliente.Apellido);
            command.Parameters.AddWithValue("@dni", cliente.Dni);
            command.Parameters.AddWithValue("@telefono", cliente.Telefono);
            command.Parameters.AddWithValue("@email", cliente.Email);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            return cliente;
        }


        public IList<Cliente> BuscarPorNombreODni(string termino)
        {
            var lista = new List<Cliente>();
            using var connection = new MySqlConnection(connectionString);
            string sql = @"SELECT id, nombre, apellido, dni, telefono, email FROM clientes 
                    WHERE Nombre LIKE @termino OR Dni LIKE @termino";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@termino", "%" + termino + "%");
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Cliente
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Dni = reader.GetString("Dni"),
                    Telefono = reader.GetString("Telefono"),
                    Email = reader.GetString("Email")
                });
            }
            return lista;
        }

        public Cliente BuscarPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            var sql = "SELECT id, nombre, apellido, dni, telefono, email FROM clientes WHERE id = @id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Cliente
                {
                    Id = reader.GetInt32("id"),
                    Nombre = reader.GetString("nombre"),
                    Apellido = reader.GetString("apellido"),
                    Dni = reader.GetString("dni"),
                    Telefono = reader.GetString("telefono"),
                    Email = reader.GetString("email")
                };
            }
            return null;
        }



        public List<Receta_Medica> ObtenerRecetasPorClienteId(int clienteId)
        {
            var recetas = new List<Receta_Medica>();

            using (var comando = new MySqlCommand(@"SELECT Id, cliente_Id, Medico, Fecha_Emision, Fecha_Vencimiento, ImgReceta, Activo
                                                FROM Receta_Medica
                                                WHERE cliente_Id = @clienteId"))
            {
                comando.Parameters.AddWithValue("@clienteId", clienteId);
                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        recetas.Add(new Receta_Medica
                        {
                            Id = reader.GetInt32("Id"),
                            ClienteId = reader.GetInt32("cliente_Id"),
                            Medico = reader.GetString("Medico"),
                            Fecha_Emision = reader.GetDateTime("Fecha_Emision"),
                            Fecha_Vencimiento = reader.GetDateTime("Fecha_Vencimiento"),
                            ImgReceta = reader.GetString("ImgReceta"),
                            Activo = reader.GetBoolean("Activo")
                        });
                    }
                }
            }

            return recetas;
        }



        public List<Productos> ObtenerProductosPorRecetaId(int recetaId)
        {
            var productos = new List<Productos>();

            using var connection = new MySqlConnection(connectionString);
            var sql = @"SELECT p.Id, p.Nombre, p.Precio, p.Requiere_Receta, p.Laboratorio, p.Tipo
                FROM Productos p
                INNER JOIN RecetaProductos rp ON p.Id = rp.ProductoId
                WHERE rp.RecetaId = @recetaId";

            using var comando = new MySqlCommand(sql, connection);
            comando.Parameters.AddWithValue("@recetaId", recetaId);
            connection.Open();

            using var reader = comando.ExecuteReader();
            while (reader.Read())
            {
                productos.Add(new Productos
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Precio = (decimal)reader.GetFloat("Precio"),
                    Requiere_Receta = reader.GetBoolean("Requiere_Receta"),
                    Laboratorio = reader.GetString("Laboratorio"),
                    Tipo = reader.GetString("Tipo")
                });
            }

            return productos;
        }




        public List<Ventas> ObtenerVentasPorClienteId(int clienteId)
        {
            var ventas = new List<Ventas>();

            using (var comando = new MySqlCommand(@"SELECT Id, cliente_Id, Fecha, Total, RecetaId
                                                FROM Ventas
                                                WHERE Cliente_Id = @clienteId"))
            {
                comando.Parameters.AddWithValue("@clienteId", clienteId);
                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ventas.Add(new Ventas
                        {
                            Id = reader.GetInt32("Id"),
                            Cliente_Id = reader.GetInt32("cliente_Id"),
                            Fecha = reader.GetDateTime("Fecha"),
                            Total = (decimal)reader.GetFloat("Total"),
                            RecetaId = reader.GetInt32("RecetaId")
                        });
                    }
                }
            }

            return ventas;
        }




        public List<DetalleVentas> ObtenerDetallesPorVentaId(int ventaId)
        {
            var detalles = new List<DetalleVentas>();

            using (var comando = new MySqlCommand(@"SELECT Id, Venta_Id, ProductoId, Cantidad, subTotal
                                                FROM DetalleVentas
                                                WHERE VentaId = @ventaId"))
            {
                comando.Parameters.AddWithValue("@ventaId", ventaId);
                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        detalles.Add(new DetalleVentas
                        {
                            Id = reader.GetInt32("Id"),
                            Venta_Id = reader.GetInt32("VentaId"),
                            ProductoId = reader.GetInt32("ProductoId"),
                            Cantidad = reader.GetInt32("Cantidad"),
                            SubTotal = (decimal)reader.GetFloat("subTotal")
                        });
                    }
                }
            }

            return detalles;
        }


        public List<Receta_Medica> ObtenerRecetasConProductos(int clienteId)
        {
            var recetas = new List<Receta_Medica>();

            using var connection = new MySqlConnection(connectionString);
            var sql = @"SELECT Id, Cliente_Id, Medico, Fecha_Emision, Fecha_Vencimiento, ImgReceta, Activo
                FROM Receta_Medica
                WHERE Cliente_Id = @cliente_Id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@cliente_Id", clienteId);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                recetas.Add(new Receta_Medica
                {
                    Id = reader.GetInt32("Id"),
                    ClienteId = reader.GetInt32("Cliente_Id"),
                    Medico = reader.GetString("Medico"),
                    Fecha_Emision = reader.GetDateTime("Fecha_Emision"),
                    Fecha_Vencimiento = reader.GetDateTime("Fecha_Vencimiento"),
                    ImgReceta = reader.GetString("ImgReceta"),
                    Activo = reader.GetBoolean("Activo"),
                    Productos = new List<Productos>()
                });
            }
            connection.Close();

            foreach (var receta in recetas)
            {
                receta.Productos = ObtenerProductosPorRecetaId(receta.Id);
            }

            return recetas;
        }




        public List<Ventas> ObtenerVentasConDetalles(int clienteId)
        {
            var ventas = new List<Ventas>();

            using var connection = new MySqlConnection(connectionString);
            var sql = @"SELECT Id, Cliente_Id, Fecha, Total, RecetaId
                FROM Ventas
                WHERE Cliente_Id = @clienteId";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@clienteId", clienteId);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ventas.Add(new Ventas
                {
                    Id = reader.GetInt32("Id"),
                    Cliente_Id = reader.GetInt32("Cliente_Id"),
                    Fecha = reader.GetDateTime("Fecha"),
                    Total = (decimal)reader.GetFloat("Total"),
                    RecetaId = reader.IsDBNull("RecetaId") ? (int?)null : reader.GetInt32("RecetaId"),
                    VentaProductos = new List<DetalleVentas>()
                });
            }
            connection.Close();

            foreach (var venta in ventas)
            {
                venta.VentaProductos = ObtenerDetallesConProducto(venta.Id);
            }

            return ventas;
        }

        public List<DetalleVentas> ObtenerDetallesConProducto(int ventaId)
        {
            var detalles = new List<DetalleVentas>();

            using var connection = new MySqlConnection(connectionString);
            var sql = @"SELECT dv.Id, dv.Venta_Id, dv.ProductoId, dv.Cantidad, dv.subTotal,
                    p.Nombre
                FROM DetalleVentas dv
                JOIN Productos p ON p.Id = dv.ProductoId
                WHERE dv.Venta_Id = @venta_Id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@venta_Id", ventaId);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                detalles.Add(new DetalleVentas
                {
                    Id = reader.GetInt32("Id"),
                    Venta_Id = reader.GetInt32("Venta_Id"),
                    ProductoId = reader.GetInt32("ProductoId"),
                    Cantidad = reader.GetInt32("Cantidad"),
                    SubTotal = (decimal)reader.GetFloat("subTotal"),
                    Producto = new Productos
                    {
                        Id = reader.GetInt32("ProductoId"),
                        Nombre = reader.GetString("Nombre")
                    }
                });
            }
            connection.Close();

            return detalles;
        }



    }
}
