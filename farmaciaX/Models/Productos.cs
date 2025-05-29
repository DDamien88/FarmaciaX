using System.ComponentModel.DataAnnotations.Schema;

namespace farmaciaX.Models;
public class Productos
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Tipo { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }
    public int Cantidad_Stock { get; set; }
    public bool Requiere_Receta { get; set; }
    public string Laboratorio { get; set; }
    public DateTime Fecha_Vencimiento { get; set; }
    public bool Activo { get; set; } = true;

    public override string ToString()
    {
        return $"Id: {Id}, Nombre: {Nombre}, Tipo: {Tipo}, Precio: {Precio}, Cantidad_Stock: {Cantidad_Stock}, Requiere_Receta: {Requiere_Receta}, Laboratorio: {Laboratorio}, Fecha_Vencimiento: {Fecha_Vencimiento}";
    }
}
