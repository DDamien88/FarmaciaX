using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace farmaciaX.Models
{
    [Table("pagos")]
    public class Pago
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Venta))]
        public int VentaId { get; set; }

        public Ventas Venta { get; set; }

        public decimal Monto { get; set; }

        [Required]
        [Column("metodoPago")]
        public string MetodoPago { get; set; }

        public DateTime FechaPago { get; set; }

        public override string ToString()
        {
            return $"Pago {Id} - {MetodoPago} - {Monto:C} el {FechaPago:dd/MM/yyyy}";
        }

    }
}
