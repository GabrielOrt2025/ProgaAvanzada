using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MediGrids.EntityFramework;
using MediGrids.Models;

namespace MediGrids.Controllers
{
    public class AgendaController : Controller
    {
        private Entities db = new Entities();

        // GET: Agenda/GestionarAgendaTerapeuta
        [HttpGet]
        public ActionResult GestionarAgendaTerapeuta()
        {
            if (Session["UserId"] == null || Session["Rol"] == null)
                return RedirectToAction("Login", "Home");

            int rol = (int)Session["Rol"];

            if (rol == 2)
            {
                int idTerapeuta = (int)Session["TerapeutaId"];

                ViewBag.Citas = db.Cita
                    .Where(c => c.id_terapeuta == idTerapeuta)
                    .Select(c => new AgendaCitaViewModel
                    {
                        id_cita = c.id_cita,
                        fecha = c.fecha,
                        hora_inicio = c.hora_inicio,
                        hora_fin = c.hora_fin,
                        estado = c.estado,
                        motivo_cancelacion = c.motivo_cancelacion,
                        id_terapia = c.id_terapia,
                        id_categoria = c.id_categoria,
                        id_paciente = c.id_paciente,
                        nombre_paciente = c.Paciente.nombre + " " + c.Paciente.apellidos
                    })
                    .ToList();

                ViewBag.Bloqueos = db.BloqueHorario
                    .Where(b => b.id_terapeuta == idTerapeuta)
                    .Select(b => new AgendaBloqueoViewModel
                    {
                        id_bloque = b.id_bloque,
                        fecha = b.fecha,
                        hora_inicio = b.hora_inicio,
                        hora_fin = b.hora_fin,
                        motivo = b.motivo
                    })
                    .ToList();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        // GET: Agenda/DetalleCita/5
        [HttpGet]
        public ActionResult DetalleCita(int id)
        {
            if (Session["UserId"] == null || Session["Rol"] == null)
                return RedirectToAction("Login", "Home");

            int rol = (int)Session["Rol"];

            if (rol == 2)
            {
                int idTerapeuta = (int)Session["TerapeutaId"];

                var cita = db.Cita
                    .Where(c => c.id_cita == id && c.id_terapeuta == idTerapeuta)
                    .Select(c => new AgendaCitaViewModel
                    {
                        id_cita = c.id_cita,
                        fecha = c.fecha,
                        hora_inicio = c.hora_inicio,
                        hora_fin = c.hora_fin,
                        estado = c.estado,
                        motivo_cancelacion = c.motivo_cancelacion,
                        id_terapia = c.id_terapia,
                        id_categoria = c.id_categoria,
                        id_paciente = c.id_paciente,
                        nombre_paciente = c.Paciente.nombre + " " + c.Paciente.apellidos,
                        telefono_paciente = c.Paciente.telefono,
                        email_paciente = c.Paciente.email
                    })
                    .FirstOrDefault();

                if (cita == null)
                    return HttpNotFound();

                return View(cita);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: Agenda/CrearBloqueo
        [HttpGet]
        public ActionResult CrearBloqueo()
        {
            if (Session["UserId"] == null || Session["Rol"] == null)
                return RedirectToAction("Login", "Home");

            int rol = (int)Session["Rol"];

            if (rol == 2)
                return View(new AgendaBloqueoViewModel());
            else
                return RedirectToAction("Login", "Home");
        }

        // POST: Agenda/CrearBloqueo
        [HttpPost]
        public ActionResult CrearBloqueo(AgendaBloqueoViewModel model)
        {
            if (Session["UserId"] == null || Session["Rol"] == null)
                return RedirectToAction("Login", "Home");

            int rol = (int)Session["Rol"];

            if (rol == 2)
            {
                int idTerapeuta = (int)Session["TerapeutaId"];

                if (ModelState.IsValid)
                {
                    if (model.hora_fin <= model.hora_inicio)
                    {
                        ModelState.AddModelError("hora_fin", "La hora de fin debe ser mayor a la hora de inicio.");
                        return View(model);
                    }

                    bool tieneConflicto = db.Cita.Any(c =>
                        c.id_terapeuta == idTerapeuta &&
                        c.fecha == model.fecha &&
                        c.hora_inicio < model.hora_fin &&
                        c.hora_fin > model.hora_inicio &&
                        c.estado != "Cancelada");

                    if (tieneConflicto)
                    {
                        ModelState.AddModelError("", "Ya existe una cita agendada en ese horario.");
                        return View(model);
                    }

                    var bloqueo = new BloqueHorario
                    {
                        id_terapeuta = idTerapeuta,
                        fecha = model.fecha,
                        hora_inicio = model.hora_inicio,
                        hora_fin = model.hora_fin,
                        motivo = model.motivo
                    };

                    db.BloqueHorario.Add(bloqueo);
                    db.SaveChanges();

                    return RedirectToAction("GestionarAgendaTerapeuta");
                }

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // POST: Agenda/EliminarBloqueo
        [HttpPost]
        public ActionResult EliminarBloqueo(int id)
        {
            if (Session["UserId"] == null || Session["Rol"] == null)
                return RedirectToAction("Login", "Home");

            int rol = (int)Session["Rol"];

            if (rol == 2)
            {
                int idTerapeuta = (int)Session["TerapeutaId"];

                var bloqueo = db.BloqueHorario
                    .Where(b => b.id_bloque == id && b.id_terapeuta == idTerapeuta)
                    .FirstOrDefault();

                if (bloqueo != null)
                {
                    db.BloqueHorario.Remove(bloqueo);
                    db.SaveChanges();
                }

                return RedirectToAction("GestionarAgendaTerapeuta");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
    }
}