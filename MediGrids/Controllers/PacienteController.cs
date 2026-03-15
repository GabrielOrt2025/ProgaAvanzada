using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MediGrids.EntityFramework;
using MediGrids.Models;

namespace MediGrids.Controllers
{
    public class PacienteController : Controller
    {
        private Entities db = new Entities();


        // GET: Paciente/AgendarCitas
        [HttpGet]
        public ActionResult AgendarCitas()
        {
            // 1. Obtener todos los tipos de terapia disponibles
            var tiposTerapia = db.TipoTerapia
                .Select(t => new TipoTerapiaViewModel
                {
                    IdTerapia = t.id_terapia,
                    Nombre = t.nombre,
                    Descripcion = t.descripcion
                })
                .ToList();

            ViewBag.TiposTerapia = tiposTerapia;

            // 2. Obtener todas las categorías clínicas (sin referencias circulares)
            var categorias = db.CategoriaClinica
                .Select(c => new CategoriaClinicaViewModel
                { 
                    IdCategoria = c.id_categoria,
                    Nombre = c.nombre,
                    Descripcion = c.descripcion,
                    IdTerapia = c.id_terapia
                })
                .ToList();

            ViewBag.Categorias = categorias;

            // 3. Obtener terapeutas activos
            var terapeutasActivos = db.Terapeuta
                .Where(t => t.activo == true)
                .ToList();

            // 4. Para cada terapeuta, obtener sus especialidades y categorías
            var terapeutasViewModel = terapeutasActivos.Select(t => new TerapeutaViewModel
            {
                IdTerapeuta = t.id_terapeuta,
                Nombre = t.nombre,
                Apellidos = t.apellidos,
                Telefono = t.telefono,
                Email = t.email,
                Activo = t.activo,
                IdCategoria = t.id_categoria,

            }).ToList();

            ViewBag.Terapeutas = terapeutasViewModel;

            // Mantener la lista completa de categorías para el dropdown (no usado actualmente)
            ViewBag.CategoriasCompletas = db.CategoriaClinica.ToList();

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}