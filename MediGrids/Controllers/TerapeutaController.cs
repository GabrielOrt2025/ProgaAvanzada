using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MediGrids.EntityFramework;
using MediGrids.Models;
using Microsoft.Ajax.Utilities;

namespace MediGrids.Controllers
{
    public class TerapeutaController : Controller
    {
        private Entities db = new Entities();

        // GET: Terapeuta/GestionarEjercicio
        [HttpGet]
        public ActionResult GestionarEjercicio()
        {
            // Se traen los valores puestos en la tabla de Ejercicio en la base de datos
            var obtenerEjercicios = db.Ejercicio.Select(t => new TerapeutaEjercicioViewModel
            {
                id_ejercicio = t.id_ejercicio,
                nombre = t.nombre,
                descripcion = t.descripcion,
                objetivo = t.objetivo,
                dificultad = t.dificultad,
                video_url = t.video_url,
                id_terapia = t.id_terapia,
                id_categoria = t.id_categoria,
                creado_por = t.creado_por
            })
            .ToList();

            ViewBag.obtenerEjercicios = obtenerEjercicios;

            return View();
        }

        [HttpPost]
        public ActionResult EliminarEjercicio(int id)
        {
            var ejercicio = db.Ejercicio.Find(id);

            // Si no es null se procede a eliminar el ejercicio
            if (ejercicio != null)
            {
                // Se puede eliminar el ejercicio usando Entity Framework con Remove
                db.Ejercicio.Remove(ejercicio);

                // Se guardan los cambios
                db.SaveChanges();
            }

            // Se redirecciona a la pagina donde se ven todos los ejercicios
            return RedirectToAction("GestionarEjercicio");
        }

        // Este metodo es para poder cargar los datos en el formulario, no para hacer el cambio como tal
        // Esta asignado al boton de editar que viene en los cards de cada ejercicio
        [HttpGet]
        // Se redirige al form de editar segun el ID del ejercicio
        public ActionResult EditarForm(int id)
        {
            var ejercicio = db.Ejercicio.Find(id);

            if (ejercicio == null)
            {
                return HttpNotFound();
            }

            // Estos son los datos que se muestran en el form para poder editarlos
            // Talvez se tengan que eliminar algunos
            var edit = new TerapeutaEjercicioViewModel
            {
                id_ejercicio = ejercicio.id_ejercicio,
                nombre = ejercicio.nombre,
                descripcion = ejercicio.descripcion,
                objetivo = ejercicio.objetivo,
                dificultad = ejercicio.dificultad,
                video_url = ejercicio.video_url,
                id_terapia = ejercicio.id_terapia,
                id_categoria = ejercicio.id_categoria,
                creado_por = ejercicio.creado_por
            };

            return View(edit);
        }

        [HttpPost]
        public ActionResult Editar(TerapeutaEjercicioViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ejercicios = db.Ejercicio.Where(e => e.id_ejercicio == model.id_ejercicio).FirstOrDefault();

                if (ejercicios != null)
                {
                    ejercicios.nombre = model.nombre;
                    ejercicios.descripcion = model.descripcion;
                    ejercicios.objetivo = model.objetivo;
                    ejercicios.dificultad = model.dificultad;
                    ejercicios.video_url = model.video_url;
                    ejercicios.id_terapia = model.id_terapia;
                    ejercicios.id_categoria = model.id_categoria;

                    db.SaveChanges();
                }

                return RedirectToAction("GestionarEjercicio");
            }

            return View(model);
        }
    }
}