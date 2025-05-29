using System.Data;
using farmaciaX.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;


namespace farmaciaX.Models
{
	public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
	{
		public RepositorioUsuario(IConfiguration configuration) : base(configuration)
		{

		}

		public int Alta(Usuario e)
		{
			int res = -1;
			if (string.IsNullOrWhiteSpace(e.Nombre) || string.IsNullOrWhiteSpace(e.Email) || string.IsNullOrWhiteSpace(e.Clave))
				throw new ArgumentException("Nombre, email y clave son obligatorios.");

			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"INSERT INTO usuarios 
					(Nombre, Apellido, Email, Clave, Avatar, Rol, Activo) 
					VALUES (@nombre, @apellido, @email, @clave, @avatar, @rol, 1);
					SELECT LAST_INSERT_ID();";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", e.Nombre);
					command.Parameters.AddWithValue("@apellido", e.Apellido);
					command.Parameters.AddWithValue("@email", e.Email);
					command.Parameters.AddWithValue("@clave", e.Clave);
					command.Parameters.AddWithValue("@avatar", string.IsNullOrEmpty(e.Avatar) ? DBNull.Value : (object)e.Avatar);
					command.Parameters.AddWithValue("@rol", e.Rol);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					e.Id = res;
					connection.Close();
				}
			}
			return res;
		}


		public int Baja(int id)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "UPDATE usuarios SET Activo = 0 WHERE Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}




		public int Modificacion(Usuario e)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"UPDATE usuarios 
					SET Nombre=@nombre, Apellido=@apellido, Email=@email, Clave=@clave, Avatar=@avatar,  Rol=@rol
					WHERE Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", e.Nombre);
					command.Parameters.AddWithValue("@apellido", e.Apellido);
					command.Parameters.AddWithValue("@avatar", e.Avatar);
					command.Parameters.AddWithValue("@email", e.Email);
					command.Parameters.AddWithValue("@clave", e.Clave);
					command.Parameters.AddWithValue("@rol", e.Rol);
					command.Parameters.AddWithValue("@id", e.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Usuario> ObtenerTodos()
		{
			IList<Usuario> res = new List<Usuario>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"
					SELECT Id, Nombre, Apellido, Avatar, Email, Clave, Rol, Activo
					FROM usuarios";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Usuario e = new Usuario
						{
							Id = reader.GetInt32("Id"),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Avatar = reader["Avatar"] == DBNull.Value ? "" : reader.GetString("Avatar"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
							Rol = reader.GetInt32("Rol"),
							Activo = reader.GetBoolean("Activo"),
						};
						res.Add(e);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Usuario ObtenerPorId(int id)
		{
			Usuario? e = null;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT 
					Id, Nombre, Apellido, Avatar, Email, Clave, Rol, Activo
					FROM usuarios
					WHERE Id=@id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						e = new Usuario
						{
							Id = reader.GetInt32("Id"),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Avatar = reader["Avatar"] == DBNull.Value ? "" : reader.GetString("Avatar"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
							Rol = reader.GetInt32("Rol"),
							Activo = reader.GetBoolean("Activo"),
						};
					}
					connection.Close();
				}
			}
			return e;
		}

		public Usuario ObtenerPorEmail(string email)
		{
			Usuario? e = null;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT
                        Id, Nombre, Apellido, Email, Clave, Avatar, Rol 
						FROM usuarios
						WHERE Email=@email";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						e = new Usuario
						{
							Id = reader.GetInt32("Id"),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Avatar = reader["Avatar"] == DBNull.Value ? "" : reader.GetString("Avatar"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
							Rol = reader.GetInt32("Rol"),
						};
					}
					else
					{
						Console.WriteLine($"No se encontró un usuario con el email: {email}");
					}
					connection.Close();
				}
			}
			return e;
		}


		//Para uso vía postman
		public Usuario ObtenerPorEmailParaLogin(string email)
		{
			Usuario? e = null;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT
                        Id, Nombre, Apellido, Email, Clave, Rol 
						FROM usuarios
						WHERE Email=@email";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						e = new Usuario
						{
							Id = reader.GetInt32("Id"),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
							Rol = reader.GetInt32("Rol"),
						};
					}
					else
					{
						Console.WriteLine($"No se encontró un usuario con el email: {email}");
					}
					connection.Close();
				}
			}
			return e;
		}



		public int ObtenerPorIdDos(int id)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT 
			Id
			FROM usuarios
			WHERE Id=@id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						res = reader.GetInt32("Id");
					}
					connection.Close();
				}
			}
			return res;
		}

		public int Activar(int id)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $"UPDATE usuarios SET Activo = 1 WHERE Id = {id}";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
	}
}