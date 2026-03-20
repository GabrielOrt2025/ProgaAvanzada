using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MediGrids.EntityFramework;
using MediGrids.Models;

namespace MediGrids.Controllers
{
    /// <summary>
    /// Gestion de Terapeutas - RF-09
    /// Usa Entity Framework (sin procedimientos almacenados)
    /// </summary>
    public class GestionTerapeutaController : Controller
    {
        private Entities db = new Entities();

        // GET: GestionTerapeuta/Index
        [HttpGet]
        public ActionResult Index(int? id_categoria)
        {
            // RF-09.5: Visualizacion por especialidad (id_categoria)
            var query = db.Terapeuta.AsQueryable();
            if (id_categoria.HasValue)
                query = query.Where(t => t.id_categoria == id_categoria.Value);

            var lista = query
                .Select(t => new GestionTerapeutaViewModel
                {
                    IdTerapeuta = t.id_terapeuta,
                    IdUsuario = t.id_usuario,
                    Nombre = t.nombre,
                    Apellidos = t.apellidos,
                    Telefono = t.telefono,
                    Email = t.email,
                    Activo = t.activo ?? true,
                    IdCategoria = t.id_categoria,
                    NombreCategoria = t.CategoriaClinica != null ? t.CategoriaClinica.nombre : null,
                    NombreTipoTerapia = t.CategoriaClinica != null && t.CategoriaClinica.TipoTerapia != null
                        ? t.CategoriaClinica.TipoTerapia.nombre : null,
                    Username = t.Usuario != null ? t.Usuario.username : null
                })
                .OrderBy(t => t.Apellidos)
                .ThenBy(t => t.Nombre)
                .ToList();

            ViewBag.Categorias = db.CategoriaClinica.OrderBy(c => c.nombre).ToList();
            ViewBag.IdCategoriaFiltro = id_categoria;

            return View(lista);
        }

        // GET: GestionTerapeuta/Registro
        [HttpGet]
        public ActionResult Registro()
        {
            ViewBag.Categorias = db.CategoriaClinica.OrderBy(c => c.nombre).ToList();
            return View(new RegistroTerapeutaViewModel());
        }

        // POST: GestionTerapeuta/Crear - RF-09.1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(RegistroTerapeutaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = db.CategoriaClinica.OrderBy(c => c.nombre).ToList();
                return View("Registro", model);
            }

            if (db.Usuario.Any(u => u.username == model.UsernameRegistro))
            {
                ModelState.AddModelError("UsernameRegistro", "El usuario ya existe");
                ViewBag.Categorias = db.CategoriaClinica.OrderBy(c => c.nombre).ToList();
                return View("Registro", model);
            }

            try
            {
                var usuario = new Usuario
                {
                    username = model.UsernameRegistro,
                    password_hash = model.Password,
                    id_rol = 2,
                    activo = true
                };
                db.Usuario.Add(usuario);
                db.SaveChanges();

                var terapeuta = new Terapeuta
                {
                    id_usuario = usuario.id_usuario,
                    nombre = model.Nombre,
                    apellidos = model.Apellidos,
                    telefono = model.Telefono,
                    email = model.Email,
                    activo = true,
                    id_categoria = model.IdCategoria
                };
                db.Terapeuta.Add(terapeuta);
                db.SaveChanges();

                TempData["Mensaje"] = "Terapeuta registrado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al registrar: " + ex.Message);
                ViewBag.Categorias = db.CategoriaClinica.OrderBy(c => c.nombre).ToList();
                return View("Registro", model);
            }
        }

        // GET: GestionTerapeuta/Editar/5 - RF-09.3
        [HttpGet]
        public ActionResult Editar(int id)
        {
            var terapeuta = db.Terapeuta
                .Include("CategoriaClinica.TipoTerapia")
                .Include("Usuario")
                .FirstOrDefault(t => t.id_terapeuta == id);

            if (terapeuta == null)
                return HttpNotFound();

            var resultado = new GestionTerapeutaViewModel
            {
                IdTerapeuta = terapeuta.id_terapeuta,
                IdUsuario = terapeuta.id_usuario,
                Nombre = terapeuta.nombre,
                Apellidos = terapeuta.apellidos,
                Telefono = terapeuta.telefono,
                Email = terapeuta.email,
                Activo = terapeuta.activo ?? true,
                IdCategoria = terapeuta.id_categoria,
                NombreCategoria = terapeuta.CategoriaClinica?.nombre,
                NombreTipoTerapia = terapeuta.CategoriaClinica?.TipoTerapia?.nombre,
                Username = terapeuta.Usuario?.username
            };

            ViewBag.Categorias = db.CategoriaClinica.OrderBy(c => c.nombre).ToList();
            return View(resultado);
        }

        // POST: GestionTerapeuta/GuardarEdicion - RF-09.3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarEdicion(GestionTerapeutaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = db.CategoriaClinica.OrderBy(c => c.nombre).ToList();
                return View("Editar", model);
            }

            try
            {
                var terapeuta = db.Terapeuta.Find(model.IdTerapeuta);
                if (terapeuta == null)
                    return HttpNotFound();

                terapeuta.nombre = model.Nombre;
                terapeuta.apellidos = model.Apellidos;
                terapeuta.telefono = model.Telefono;
                terapeuta.email = model.Email;
                terapeuta.id_categoria = model.IdCategoria;

                db.SaveChanges();

                TempData["Mensaje"] = "Datos actualizados correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                ViewBag.Categorias = db.CategoriaClinica.OrderBy(c => c.nombre).ToList();
                return View("Editar", model);
            }
        }

        // POST: GestionTerapeuta/CambiarEstado - RF-09.4
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstado(int id)
        {
            var terapeuta = db.Terapeuta.Find(id);
            if (terapeuta == null)
                return HttpNotFound();

            terapeuta.activo = !(terapeuta.activo ?? true);
            db.SaveChanges();

            TempData["Mensaje"] = terapeuta.activo.Value ? "Terapeuta activado." : "Terapeuta desactivado.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
