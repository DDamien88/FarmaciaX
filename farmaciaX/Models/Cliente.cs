using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace farmaciaX.Models
{
    [Table("clientes")]
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [RegularExpression(@"^\d{7,11}$", ErrorMessage = "El DNI debe tener entre 7 y 11 dígitos")]
        public string Dni { get; set; }


        [Required]
        [StringLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Solo se permiten números.")]
        public string Telefono { get; set; }


        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo electrónico inválido")]
        public string Email { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido} (DNI: {Dni})";

        public ICollection<Receta_Medica> Recetas { get; set; } = new List<Receta_Medica>();
        
        public List<Ventas> Ventas { get; set; } =  new List<Ventas>();


        public override string ToString()
        {
            return $"Id: {Id}, Nombre: {Nombre}, Apellido: {Apellido}, Dni: {Dni}";
        }
    }
}

