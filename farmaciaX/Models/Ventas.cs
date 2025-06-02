using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace farmaciaX.Models
{
    public class Ventas
    {
        public int Id { get; set; }
        //[ForeignKey(nameof(Cliente_Id))]
        public int? Cliente_Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        [ForeignKey(nameof(RecetaId))]
        public int? RecetaId { get; set; }


        [ForeignKey(nameof(Cliente_Id))]
        public Cliente? Cliente { get; set; }

        public List<DetalleVentas> VentaProductos { get; set; } = new List<DetalleVentas>();

        public Receta_Medica? Receta { get; set; }


        public List<Pago> Pagos { get; set; } = new List<Pago>();

        public int UsuarioAltaId { get; set; }
        public Usuario UsuarioAlta { get; set; }

        public bool Activo { get; set; }

        public int? UsuarioBajaId { get; set; }
        public Usuario UsuarioBaja { get; set; }
        
        public override string ToString()
        {
            return $"Id: {Id}, Cliente_Id: {Cliente_Id}, Fecha: {Fecha}, Total: {Total}";
        }
    }
}
