using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

        // =============================================
        // Gestión de Ejercicios (CRUD)
        // =============================================

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

        // =============================================
        // Asignación de Ejercicios a Pacientes
        // =============================================

        // GET: Terapeuta/AsignacionEjercicios
        [HttpGet]
        public ActionResult AsignacionEjercicios(int? idPaciente, int? idTerapia, int? idCategoria)
        {
            // Pacientes activos
            var pacientes = db.Paciente
                .Where(p => p.activo == true)
                .Select(p => new PacienteViewModel
                {
                    IdPaciente = p.id_paciente,
                    Nombre = p.nombre,
                    Apellidos = p.apellidos,
                    Email = p.email,
                    Activo = p.activo
                })
                .ToList();
            ViewBag.Pacientes = pacientes;

            // Tipos de terapia
            var tiposTerapia = db.TipoTerapia
                .Select(t => new TipoTerapiaViewModel
                {
                    IdTerapia = t.id_terapia,
                    Nombre = t.nombre,
                    Descripcion = t.descripcion
                })
                .ToList();
            ViewBag.TiposTerapia = tiposTerapia;

            // Categorías clínicas
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

            // Preservar filtros seleccionados
            ViewBag.IdPaciente = idPaciente;
            ViewBag.IdTerapia = idTerapia;
            ViewBag.IdCategoria = idCategoria;

            if (idPaciente.HasValue)
            {
                // Nombre del paciente seleccionado
                var pacienteSeleccionado = pacientes.FirstOrDefault(p => p.IdPaciente == idPaciente.Value);
                ViewBag.PacienteNombre = pacienteSeleccionado != null
                    ? pacienteSeleccionado.Nombre + " " + pacienteSeleccionado.Apellidos
                    : "";

                // Asignaciones actuales del paciente
                var asignaciones = db.PacienteEjercicio
                    .Where(pe => pe.id_paciente == idPaciente.Value)
                    .Select(pe => new AsignacionEjercicioViewModel
                    {
                        IdPaciente = pe.id_paciente,
                        IdEjercicio = pe.id_ejercicio,
                        NombreEjercicio = pe.Ejercicio.nombre,
                        DescripcionEjercicio = pe.Ejercicio.descripcion,
                        ObjetivoEjercicio = pe.Ejercicio.objetivo,
                        DificultadEjercicio = pe.Ejercicio.dificultad,
                        NombreTerapia = pe.Ejercicio.TipoTerapia.nombre,
                        NombreCategoria = pe.Ejercicio.CategoriaClinica.nombre,
                        FechaAsignacion = pe.fecha_asignacion,
                        Estado = pe.estado
                    })
                    .ToList();
                ViewBag.Asignaciones = asignaciones;

                // IDs de ejercicios ya asignados
                ViewBag.IdsAsignados = asignaciones.Select(a => a.IdEjercicio).ToList();

                // Ejercicios disponibles (solo si hay filtros activos)
                if (idTerapia.HasValue || idCategoria.HasValue)
                {
                    var ejercicios = db.Ejercicio.AsQueryable();

                    if (idTerapia.HasValue)
                        ejercicios = ejercicios.Where(e => e.id_terapia == idTerapia.Value);

                    if (idCategoria.HasValue)
                        ejercicios = ejercicios.Where(e => e.id_categoria == idCategoria.Value);

                    ViewBag.EjerciciosDisponibles = ejercicios
                        .Select(e => new EjercicioViewModel
                        {
                            IdEjercicio = e.id_ejercicio,
                            Nombre = e.nombre,
                            Descripcion = e.descripcion,
                            Objetivo = e.objetivo,
                            Dificultad = e.dificultad,
                            VideoUrl = e.video_url,
                            IdTerapia = e.id_terapia,
                            IdCategoria = e.id_categoria,
                            NombreCategoria = e.CategoriaClinica.nombre
                        })
                        .ToList();
                }
            }

            // Mensajes de retroalimentación
            if (TempData["Mensaje"] != null)
            {
                ViewBag.Mensaje = TempData["Mensaje"];
                ViewBag.TipoMensaje = TempData["TipoMensaje"];
            }

            return View();
        }

        // POST: Terapeuta/AsignarEjercicios (RF-08.3)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarEjercicios(int idPaciente, int[] idsEjercicios, int? idTerapia, int? idCategoria)
        {
            if (idsEjercicios == null || idsEjercicios.Length == 0)
            {
                TempData["Mensaje"] = "Debe seleccionar al menos un ejercicio.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("AsignacionEjercicios", new { idPaciente, idTerapia, idCategoria });
            }

            var asignados = 0;
            var yaExistentes = 0;
            var nombresEjercicios = new List<string>();

            foreach (var idEjercicio in idsEjercicios)
            {
                var existente = db.PacienteEjercicio
                    .FirstOrDefault(pe => pe.id_paciente == idPaciente && pe.id_ejercicio == idEjercicio);

                if (existente != null)
                {
                    yaExistentes++;
                    continue;
                }

                var asignacion = new PacienteEjercicio
                {
                    id_paciente = idPaciente,
                    id_ejercicio = idEjercicio,
                    fecha_asignacion = DateTime.Now,
                    estado = "Pendiente"
                };

                db.PacienteEjercicio.Add(asignacion);
                asignados++;

                var ejercicio = db.Ejercicio.Find(idEjercicio);
                if (ejercicio != null)
                    nombresEjercicios.Add(ejercicio.nombre);
            }

            db.SaveChanges();

            // RF-08.4: Notificación por email al paciente
            if (asignados > 0)
            {
                var paciente = db.Paciente.Find(idPaciente);
                if (paciente != null && !string.IsNullOrEmpty(paciente.email))
                {
                    EnviarNotificacionEmail(
                        paciente.email,
                        paciente.nombre + " " + paciente.apellidos,
                        nombresEjercicios);
                }
            }

            var mensaje = asignados + " ejercicio(s) asignado(s) correctamente.";
            if (yaExistentes > 0)
                mensaje += " " + yaExistentes + " ya estaban asignados.";

            TempData["Mensaje"] = mensaje;
            TempData["TipoMensaje"] = "success";
            return RedirectToAction("AsignacionEjercicios", new { idPaciente, idTerapia, idCategoria });
        }

        // POST: Terapeuta/ActualizarEstado (RF-08.6)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstado(int idPaciente, int idEjercicio, string estado, int? idTerapia, int? idCategoria)
        {
            var asignacion = db.PacienteEjercicio
                .FirstOrDefault(pe => pe.id_paciente == idPaciente && pe.id_ejercicio == idEjercicio);

            if (asignacion != null)
            {
                asignacion.estado = estado;
                db.SaveChanges();
                TempData["Mensaje"] = "Estado actualizado a \"" + estado + "\".";
                TempData["TipoMensaje"] = "success";
            }
            else
            {
                TempData["Mensaje"] = "La asignación no fue encontrada.";
                TempData["TipoMensaje"] = "error";
            }

            return RedirectToAction("AsignacionEjercicios", new { idPaciente, idTerapia, idCategoria });
        }

        // POST: Terapeuta/EliminarAsignacion (RF-08.6)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarAsignacion(int idPaciente, int idEjercicio, int? idTerapia, int? idCategoria)
        {
            var asignacion = db.PacienteEjercicio
                .FirstOrDefault(pe => pe.id_paciente == idPaciente && pe.id_ejercicio == idEjercicio);

            if (asignacion != null)
            {
                db.PacienteEjercicio.Remove(asignacion);
                db.SaveChanges();
                TempData["Mensaje"] = "Asignación eliminada correctamente.";
                TempData["TipoMensaje"] = "success";
            }
            else
            {
                TempData["Mensaje"] = "La asignación no fue encontrada.";
                TempData["TipoMensaje"] = "error";
            }

            return RedirectToAction("AsignacionEjercicios", new { idPaciente, idTerapia, idCategoria });
        }

        /// <summary>
        /// RF-08.4: Envía notificación por email al paciente cuando se le asignan ejercicios.
        /// Requiere configurar SmtpHost, SmtpPort, SmtpUser y SmtpPass en appSettings del Web.config.
        /// </summary>
        private void EnviarNotificacionEmail(string emailPaciente, string nombrePaciente, List<string> ejercicios)
        {
            try
            {
                var smtpHost = System.Configuration.ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
                var smtpUser = System.Configuration.ConfigurationManager.AppSettings["SmtpUser"] ?? "";
                var smtpPass = System.Configuration.ConfigurationManager.AppSettings["SmtpPass"] ?? "";

                if (string.IsNullOrEmpty(smtpUser)) return;

                var listaHtml = string.Join("",
                    ejercicios.Select(e => "<li>" + HttpUtility.HtmlEncode(e) + "</li>"));

                var body = "<html><body style='font-family:Arial,sans-serif;'>"
                    + "<div style='max-width:600px;margin:0 auto;'>"
                    + "<div style='background:#0c7a3e;padding:20px;border-radius:8px 8px 0 0;text-align:center;'>"
                    + "<h1 style='color:white;margin:0;'>MediGrids</h1></div>"
                    + "<div style='background:#f8f9fa;padding:30px;border-radius:0 0 8px 8px;'>"
                    + "<h2 style='color:#333;'>Hola, " + HttpUtility.HtmlEncode(nombrePaciente) + "</h2>"
                    + "<p>Se le han asignado nuevos ejercicios terapéuticos:</p>"
                    + "<ul>" + listaHtml + "</ul>"
                    + "<p>Por favor ingrese a la plataforma para ver los detalles de cada ejercicio.</p>"
                    + "<p style='color:#666;font-size:12px;margin-top:30px;'>Este es un mensaje automático, no responder.</p>"
                    + "</div></div></body></html>";

                var message = new MailMessage
                {
                    From = new MailAddress(smtpUser, "MediGrids - Centro Terapéutico"),
                    Subject = "Nuevos ejercicios asignados - MediGrids",
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(emailPaciente));

                using (var smtp = new SmtpClient(smtpHost, smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtp.EnableSsl = true;
                    smtp.Send(message);
                }
            }
            catch
            {
                // No interrumpir el flujo si falla el envío del email
            }
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