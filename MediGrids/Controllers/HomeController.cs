using MediGrids.EntityFramework;
using Microsoft.Ajax.Utilities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace MediGrids.Controllers
{
    public class HomeController : Controller
    {
        private Entities db = new Entities();

        #region Vistas Públicas

        [HttpGet]
        public ActionResult Index()
        {
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

        #endregion

        #region Autenticación

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

            // Dependiendo del rol, el usuario verá un panel distinto
            if (user.id_rol == 2)
            {
                var terapeuta = db.Terapeuta.FirstOrDefault(t => t.email == user.correo);
                if (terapeuta != null)
                    Session["TerapeutaId"] = terapeuta.id_terapeuta;

                return RedirectToAction("PanelTerapeuta");
            }
            else if (user.id_rol == 1) // Admin
            {
                return RedirectToAction("PanelInterno");
            }
            else if (user.id_rol == 3) // Paciente
            {
                return RedirectToAction("PanelPaciente");
            }
            else
            {
                ViewBag.Error = "El usuario no tiene un rol válido.";
                return View();
            }
        }

        [HttpPost]
        public ActionResult Register(string nombre, string apellidos, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellidos) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.Error = "Todos los campos son obligatorios.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            var existeUsuario = db.Usuario.FirstOrDefault(u => u.correo == email);
            if (existeUsuario != null)
            {
                ViewBag.Error = "Ya existe una cuenta registrada con ese correo.";
                return View();
            }

            var nuevoUsuario = new Usuario
            {
                correo = email,
                password_hash = password,
                id_rol = 3,
                activo = true
            };

            db.Usuario.Add(nuevoUsuario);
            db.SaveChanges();

            var nuevoPaciente = new Paciente
            {
                id_usuario = nuevoUsuario.id_usuario,
                nombre = nombre,
                apellidos = apellidos,
                telefono = null,
                email = email,
                activo = true
            };

            db.Paciente.Add(nuevoPaciente);
            db.SaveChanges();

            TempData["MensajeExito"] = "Cuenta creada correctamente. Ahora puedes iniciar sesión.";
            return RedirectToAction("Login", "Home");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Terapeutas()
        {
            var terapeutas = db.Terapeuta
                .Where(t => t.activo == true)
                .Select(t => new MediGrids.Models.TerapeutaPublicoViewModel
                {
                    IdTerapeuta = t.id_terapeuta,
                    NombreCompleto = t.nombre + " " + t.apellidos,
                    Especialidad = t.CategoriaClinica.nombre + " - " + t.CategoriaClinica.TipoTerapia.nombre,
                    Email = t.email,
                    Telefono = t.telefono
                })
                .ToList();

            return View(terapeutas);
        }


        [HttpGet]
        public ActionResult Blog()
        {
            return View();
        }

        // 1. Mostrar la página


        [HttpGet]
        public ActionResult SobreNosotros()
        {
            return View();
        }


        // 2. Recibir el formulario y enviar correo

        [HttpPost]
        public ActionResult EnviarMensaje(string nombre, string correo, string asunto, string mensaje)
        {
            try
            {
                var fromAddress = new MailAddress("medigridsnotificaciones@gmail.com", "MediGrids");
                var toAddress = new MailAddress("medigridsnotificaciones@gmail.com");

                const string fromPassword = "bvlsewtmfmelyyqi";
                
                string subject = "Nuevo mensaje de contacto: " + asunto;

                string body = $@"
Nombre: {nombre}
Correo: {correo}

Mensaje:
{mensaje}
";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }

                TempData["MensajeExito"] = "Tu mensaje fue enviado correctamente. Te estaremos contactando pronto.";
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al enviar el mensaje";
            }

            return RedirectToAction("SobreNosotros");
        }

        #endregion

        #region Paneles Internos

        [HttpGet]
        public ActionResult PanelInterno()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if ((int)Session["Rol"] != 1) // Solo Admin
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [HttpGet]
        public ActionResult PanelTerapeuta()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int rol = (int)Session["Rol"];

            if (rol != 1 && rol != 2) // Admin o Terapeuta
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        #endregion

        #region Biblioteca de Ejercicios - Consulta

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
            ViewBag.CategoriasCompletas = db.CategoriaClinica
                .Select(c => new
                {
                    id_categoria = c.id_categoria,
                    nombre = c.nombre,
                    id_terapia = c.id_terapia
                })
                .ToList();

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

        #endregion

        #region Panel Paciente 

        [HttpGet]
        public ActionResult PanelPaciente()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if ((int)Session["Rol"] != 3) // Solo Paciente
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }


        #endregion

        #region Métodos Privados de Seguridad

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

        #endregion
    }
}