using System.ComponentModel.DataAnnotations.Schema;

namespace farmaciaX.Models
{
    [Table("receta_medica")]
    public class Receta_Medica
    {
        public int Id { get; set; }

        [Column("cliente_Id")]
        public int ClienteId { get; set; }

        //[ForeignKey("ClienteId")]
        [NotMapped]
        public Cliente? Cliente { get; set; }

        public string Medico { get; set; }

        public DateTime Fecha_Emision { get; set; }

        public DateTime Fecha_Vencimiento { get; set; }

        public string? ImgReceta { get; set; }

        [NotMapped]
        public IFormFile? ImgRecetaFile { get; set; }

        public bool Activo { get; set; }
        public List<RecetaProducto> RecetaProductos { get; set; } = new List<RecetaProducto>();
        
        [NotMapped]
        public List<Productos> Productos { get; set; } = new List<Productos>();


        public override string ToString()
        {
            return $"Id: {Id}, ClienteId: {ClienteId}, Medico: {Medico}, Fecha_Emision: {Fecha_Emision}, Fecha_Vencimiento: {Fecha_Vencimiento}";
        }
    }
}
