

namespace farmaciaX.Models
{
    public class Imagen
    {
        public int Id { get; set; }
        public int RecetaId { get; set; }
        public string Url { get; set; } = "";
        public IFormFile? Archivo { get; set; } = null;
    }
}