using System.ComponentModel.DataAnnotations.Schema;

namespace farmaciaX.Models
{
    public class DetalleVentas
    {
        public int Id { get; set; }
        [Column("venta_id")]
        public int Venta_Id { get; set; }
        [ForeignKey(nameof(ProductoId))]
        public int ProductoId { get; set; }
        public Productos Producto { get; set; }

        public int Cantidad { get; set; }
        public decimal SubTotal { get; set; }

        public override string ToString()
        {
            return $"Producto: {Producto?.Nombre}, Cantidad: {Cantidad}, SubTotal: {SubTotal}";
        }

    }
}