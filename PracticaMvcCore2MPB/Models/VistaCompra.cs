using System.ComponentModel.DataAnnotations;

namespace PracticaMvcCore2MPB.Models
{
    public class VistaCompra
    {
        [Key]
        public int idVistaCompra { get; set; }
        public List<CarritoCompra> carritoCompra { get; set; }
        public int total { get; set; }
    }
}
