using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PracticaMvcCore2MPB.Extensions;
using PracticaMvcCore2MPB.Filters;
using PracticaMvcCore2MPB.Models;
using PracticaMvcCore2MPB.Repositories;

namespace PracticaMvcCore2MPB.Controllers
{
    public class LibrosController : Controller
    {
        private RepositoryLibros repo;

        public LibrosController(RepositoryLibros repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index(int? idgenero)
        {
            if(idgenero != null)
            {
                List<Libro> libros = await this.repo.FindLibrosByGenero(idgenero.Value);
                return View(libros);
            }
            else
            {
                List<Libro> libros = await this.repo.GetLibrosAsync();
                return View(libros);
            }            
        }

        public async Task<IActionResult> Details(int idlibro)
        {
            Libro libro = await this.repo.FindLibroById(idlibro);
            return View(libro);
        }

        public async Task<IActionResult> AñadirCarrito(int idlibro)
        {
            if (idlibro != null)
            {
                List<int> listacarrito;
                if (HttpContext.Session.GetObject<List<int>>("CARRITO") == null)
                {
                    listacarrito = new List<int>();
                }
                else
                {
                    listacarrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
                }
                listacarrito.Add(idlibro);
                HttpContext.Session.SetObject("CARRITO", listacarrito);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Carrito()
        {
            List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
            List<Libro> libros = new List<Libro>();
            List<CarritoCompra> carritoCompra = new List<CarritoCompra>();
            VistaCompra vistaCompra = new VistaCompra();
            if (carrito == null)
            {
                carrito = new List<int>();
            }
            else
            {
                foreach (var libro in carrito)
                {
                    Libro libroCarrito = await this.repo.FindLibroById(libro);
                    libros.Add(libroCarrito);
                }
                carritoCompra = await this.repo.GetLibrosAgrupados(libros);
                vistaCompra.carritoCompra = carritoCompra;
                vistaCompra.total = carritoCompra.Sum(l => l.libro.Precio * l.cantidad);
            }
            return View(vistaCompra);
        }

        public IActionResult DeleteLibroCarrito(int idlibro)
        {
            List<int> listaIdsLibros = HttpContext.Session.GetObject<List<int>>("CARRITO");
            if (listaIdsLibros != null)
            {
                listaIdsLibros.Remove(idlibro);
                HttpContext.Session.SetObject("CARRITO", listaIdsLibros);
            }
            return RedirectToAction("Carrito");
        }

        public async Task<IActionResult> VaciarCarrito()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        //No conseguido..
        [AuthorizeLibros]
        [HttpPost]
        public async Task<IActionResult> ComprarCarrito(List<int> IdsLibros, List<int> cantidades)
        {
            var idUsuarioClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idUsuarioClaim == null)
            {
                return RedirectToAction("Login", "Managed");
            }

            int idUsuario = int.Parse(idUsuarioClaim.Value);

            List<int> listaIdsLibros = HttpContext.Session.GetObject<List<int>>("CARRITO");
            if (listaIdsLibros == null || listaIdsLibros.Count == 0)
            {
                return RedirectToAction("Carrito");
            }

            var cantidadPorLibros = listaIdsLibros.GroupBy(id => id)
                .ToDictionary(g => g.Key, g => g.Count());

            List<Pedido> pedidos = cantidadPorLibros
                .Select(kv => new Pedido
                {
                    IdLibro = kv.Key,                    
                    Cantidad = kv.Value
                }).ToList();

            await this.repo.GuardarCarrito(idUsuario, pedidos);

            HttpContext.Session.Remove("CARRITO");

            return RedirectToAction("PerfilUsuario", "Managed");
        }
    }
}
