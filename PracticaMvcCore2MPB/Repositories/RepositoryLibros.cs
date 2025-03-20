using Microsoft.EntityFrameworkCore;
using PracticaMvcCore2MPB.Data;
using PracticaMvcCore2MPB.Models;

namespace PracticaMvcCore2MPB.Repositories
{
    public class RepositoryLibros
    {
        private LibrosContext context;

        public RepositoryLibros(LibrosContext context)
        {
            this.context = context;
        }

        public async Task<List<Libro>> GetLibrosAsync()
        {
            return await this.context.Libros.ToListAsync();
        }

        public async Task<Libro> FindLibroById(int idLibro)
        {
            return await this.context.Libros.FirstOrDefaultAsync(l => l.IdLibro == idLibro);
        }

        public async Task<List<Libro>> FindLibrosByGenero(int idGenero)
        {
            return await this.context.Libros.Where(l => l.IdGenero == idGenero).ToListAsync();
        }

        public async Task<List<Genero>> GetGenerosAsync()
        {
            return await this.context.Generos
                .Distinct()
                .ToListAsync();
        }

        public async Task Pedido(int idUsuario, List<Pedido> pedidos)
        {
            foreach(var pedido in pedidos)
            {
                pedido.IdUsuario = idUsuario;
                pedido.Fecha = DateTime.Now;
                this.context.Pedidos.Add(pedido);
            }
            await this.context.SaveChangesAsync();
        }

        public async Task<List<Libro>> GetLibrosByIds(List<int> ids)
        {
            return await this.context.Libros
                .Where(e => ids.Contains(e.IdLibro))
                .ToListAsync();
        }

        public async Task<List<CarritoCompra>> GetLibrosAgrupados(List<Libro> libros)
        {
            var librosAgrupados = libros.GroupBy(x => x.IdLibro)
                .Select(grupo => new { Libro = grupo.First(), Cantidad = grupo.Count() })
                .ToList();

            List<CarritoCompra> carrito = new List<CarritoCompra>();
            foreach (var grupo in librosAgrupados)
            {
                CarritoCompra compra = new CarritoCompra();
                compra.libro = grupo.Libro;
                compra.cantidad = grupo.Cantidad;
                carrito.Add(compra);
            }

            return carrito;
        }

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            return await context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> FindUsuarioAsync(int idusuario)
        {
            return await this.context.Usuarios.FirstOrDefaultAsync(c => c.IdUsuario == idusuario);
        }

        public async Task<Usuario> LogInAsync(string email, string password)
        {
            return await context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public async Task<Usuario> FindUsuarioByIdAsync(int id)
        {
            return await context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task<List<Pedido>> GetPedidosAsync()
        {
            return await this.context.Pedidos.AsNoTracking().ToListAsync();
        }

        public async Task<int> GetCompraMaxId()
        {
            var consulta = from datos in this.context.Pedidos
                           where datos.IdPedido == this.context.Pedidos.Max(c => c.IdPedido)
                           select datos.IdPedido + 1;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<int> GetVistaCompraMaxId()
        {
            var consulta = from datos in this.context.VistaCompras
                           where datos.idVistaCompra == this.context.Pedidos.Max(c => c.IdPedido)
                           select datos.idVistaCompra + 1;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task InsertarCompraAsync(Pedido pedido)
        {
            await context.Pedidos.AddAsync(pedido);
            await context.SaveChangesAsync();
        }

        //No conseguido..
        public async Task GuardarCarrito(int idUsuario, List<Pedido> pedidos)
        {
            foreach (var pedido in pedidos)
            {
                pedido.IdUsuario = idUsuario;
                pedido.Fecha = DateTime.Now;
                this.context.Pedidos.Add(pedido);
            }
            await this.context.SaveChangesAsync();
        }
    }
}
