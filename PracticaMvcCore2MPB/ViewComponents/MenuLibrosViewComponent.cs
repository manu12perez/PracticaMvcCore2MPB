using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using PracticaMvcCore2MPB.Models;
using PracticaMvcCore2MPB.Repositories;

namespace PracticaMvcCore2MPB.ViewComponents
{
    public class MenuLibrosViewComponent: ViewComponent
    {
        private RepositoryLibros repo;

        public MenuLibrosViewComponent(RepositoryLibros repo)
        {
            this.repo = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Genero> generos = await this.repo.GetGenerosAsync();
            return View(generos);
        }
    }
}
