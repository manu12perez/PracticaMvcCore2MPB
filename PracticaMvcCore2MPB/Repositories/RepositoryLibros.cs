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

        public async Task<List<Libro>> FindCubosByGenero(int idGenero)
        {
            return await this.context.Libros.Where(l => l.IdGenero == idGenero).ToListAsync();
        }

        public async Task<List<Genero>> GetGenerosAsync()
        {
            return await this.context.Generos
                .Distinct()
                .ToListAsync();
        }

        public async Task Carrito(int idUsuario, List<Pedido> pedidos)
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
    }
}
