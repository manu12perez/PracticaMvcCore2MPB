using Microsoft.AspNetCore.Mvc;
using PracticaMvcCore2MPB.Extensions;
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
                List<Libro> libros = await this.repo.FindCubosByGenero(idgenero.Value);
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
            List<int> listaIdsLibros = HttpContext.Session.GetObject<List<int>>("CARRITO");
            if (listaIdsLibros == null)
            {
                listaIdsLibros = new List<int>();
            }

            var cantidadPorLibro = listaIdsLibros.GroupBy(id => id)
                .ToDictionary(g => g.Key, g => g.Count());

            List<Libro> librosAñadidos = await this.repo.GetLibrosByIds(cantidadPorLibro.Keys.ToList());

            ViewBag.CantidadPorLibro = cantidadPorLibro;

            return View(librosAñadidos);
        }

        public IActionResult EliminarLibro(int idlibro)
        {
            List<int> listaIdsLibros = HttpContext.Session.GetObject<List<int>>("CARRITO");
            if (listaIdsLibros != null)
            {
                listaIdsLibros.Remove(idlibro);
                HttpContext.Session.SetObject("CARRITO", listaIdsLibros);
            }
            return RedirectToAction("Carrito");
        }
    }
}
