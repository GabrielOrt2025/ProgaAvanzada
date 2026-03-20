using MediGrids.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace MediGrids.Controllers
{
    public class HomeController : Controller
    {
        private Entities db = new Entities();

        [HttpGet]
        public ActionResult Index()
        {
            Session.Clear(); //limpia sesión
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Correo y contraseña son obligatorios.";
                return View();
            }

            var user = db.Usuario.FirstOrDefault(u => u.correo == email && u.activo == true);

            if (user == null)
            {
                ViewBag.Error = "Usuario no encontrado.";
                return View();
            }

            var storedPassword = user.password_hash ?? string.Empty;

            if (!VerifyPassword(password, storedPassword))
            {
                ViewBag.Error = "Credenciales inválidas. Por favor verifique e intente de nuevo.";
                return View();
            }

            Session["UserId"] = user.id_usuario;
            Session["UserEmail"] = user.correo;
            Session["Rol"] = user.id_rol;

            return RedirectToAction("PanelInterno");
        }

        [HttpGet]
        public ActionResult PanelInterno()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpGet]
        public ActionResult BibliotecaEjercicios(int? idTerapia, int? idCategoria)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

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

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        private bool VerifyPassword(string providedPassword, string storedHash)
        {
            if (storedHash == providedPassword)
                return true;

            var sha256 = ComputeSha256Hash(providedPassword);
            if (string.Equals(storedHash, sha256, System.StringComparison.OrdinalIgnoreCase))
                return true;

            var md5 = ComputeMd5Hash(providedPassword);
            if (string.Equals(storedHash, md5, System.StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private string ComputeSha256Hash(string raw)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private string ComputeMd5Hash(string raw)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}