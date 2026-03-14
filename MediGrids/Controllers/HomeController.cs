using MediGrids.EntityFramework;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace MediGrids.Controllers
{
    public class HomeController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult BibliotecaEjercicios(int? idTerapia, int? idCategoria)
        {
            var ejercicios = db.Ejercicio
                .Include(e => e.CategoriaClinica)
                .AsQueryable();

            ViewBag.Terapias = db.TipoTerapia.ToList();

            if (idTerapia.HasValue)
            {
                ViewBag.Categorias = db.CategoriaClinica
                    .Where(c => c.id_terapia == idTerapia.Value)
                    .ToList();

                ejercicios = ejercicios.Where(e => e.id_terapia == idTerapia.Value);
            }
            else
            {
                ViewBag.Categorias = db.CategoriaClinica.ToList();
            }

            if (idCategoria.HasValue)
            {
                ejercicios = ejercicios.Where(e => e.id_categoria == idCategoria.Value);
            }

            ViewBag.IdTerapia = idTerapia;
            ViewBag.IdCategoria = idCategoria;

            return View(ejercicios.ToList());
        }
    }
}