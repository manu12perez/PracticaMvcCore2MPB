using System.ComponentModel.DataAnnotations;

namespace PracticaMvcCore2MPB.Models
{
    public class CarritoCompra
    {
        [Key]
        public int id { get; set; }
        public Libro libro { get; set; }
        public int cantidad { get; set; }
    }
}
