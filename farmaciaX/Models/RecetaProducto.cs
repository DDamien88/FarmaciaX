using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace farmaciaX.Models
{
    [Table("recetaproductos")]
    public class RecetaProducto
    {
        public int RecetaId { get; set; }
        public Receta_Medica Receta { get; set; }

        public int ProductoId { get; set; }
        public Productos Producto { get; set; }

        public int Cantidad { get; set; }
    }

}