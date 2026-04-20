using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MediGrids.EntityFramework;
using MediGrids.Models;

namespace MediGrids.Controllers
{
    public class GestionarPacientesController : Controller
    {
        private Entities db = new Entities();

        //GestionarPacientes
        [HttpGet]
        public ActionResult GestionarPacientes()
        {
            if (Session["UserId"] == null || Session["Rol"] == null)
                return RedirectToAction("Login", "Home");

            int rol = (int)Session["Rol"];

            if (rol == 1 || rol == 2)
            {
                ViewBag.obtenerPacientes = db.Paciente
                    .Select(p => new PacienteViewModel
                    {
                        IdPaciente = p.id_paciente,
                        Nombre = p.nombre,
                        Apellidos = p.apellidos,
                        Email = p.email,
                        Activo = p.activo
                    })
                    .ToList();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        //Paciente/EditarPaciente/5
        [HttpGet]
        public ActionResult EditarPaciente(int id)
        {
            if (Session["UserId"] == null || Session["Rol"] == null)
                return RedirectToAction("Login", "Home");

            var paciente = db.Paciente.Find(id);

            if (paciente == null)
                return HttpNotFound();

            var model = new PacienteViewModel
            {
                IdPaciente = paciente.id_paciente,
                Nombre = paciente.nombre,
                Apellidos = paciente.apellidos,
                Email = paciente.email,
                Activo = paciente.activo
            };

            return View(model);
        }

        //Paciente/EditarPaciente
        [HttpPost]
        public ActionResult EditarPaciente(PacienteViewModel model)
        {
            if (Session["UserId"] == null || Session["Rol"] == null)
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                var paciente = db.Paciente.Find(model.IdPaciente);

                if (paciente != null)
                {
                    paciente.nombre = model.Nombre;
                    paciente.apellidos = model.Apellidos;
                    paciente.email = model.Email;

                    db.SaveChanges();
                }

                return RedirectToAction("GestionarPacientes");
            }

            return View(model);
        }

        // Paciente/ToggleActivoPaciente
        [HttpPost]
        public ActionResult ToggleActivoPaciente(int id)
        {
            if (Session["UserId"] == null || Session["Rol"] == null)
                return RedirectToAction("Login", "Home");

            var paciente = db.Paciente.Find(id);

            if (paciente != null)
            {
                paciente.activo = !paciente.activo;
                db.SaveChanges();
            }

            return RedirectToAction("GestionarPacientes");
        }
    }
}