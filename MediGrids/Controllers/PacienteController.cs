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

        // GET: Paciente/ObtenerHorariosDisponibles?idTerapeuta=1&fecha=2026-04-01
        [HttpGet]
        public JsonResult ObtenerHorariosDisponibles(int idTerapeuta, string fecha)
        {
            try
            {
                DateTime fechaCita = DateTime.Parse(fecha);
                // Convertir dia de semana: DayOfWeek (0=Domingo..6=Sabado) coincide con dia_semana en BD
                int diaSemana = (int)fechaCita.DayOfWeek;

                // 1. Obtener horario laboral del terapeuta para ese dia de la semana
                var horariosLaborales = db.HorarioTerapeuta
                    .Where(h => h.id_terapeuta == idTerapeuta
                             && h.dia_semana == diaSemana
                             && h.activo == true)
                    .OrderBy(h => h.hora_inicio)
                    .ToList();

                if (!horariosLaborales.Any())
                {
                    return Json(new { success = true, horarios = new List<object>(), mensaje = "El terapeuta no tiene horario asignado para este dia" }, JsonRequestBehavior.AllowGet);
                }

                // 2. Generar slots de 1 hora basados en los bloques de horario laboral
                var todosLosSlots = new List<TimeSpan>();
                foreach (var bloque in horariosLaborales)
                {
                    var horaActual = bloque.hora_inicio;
                    while (horaActual < bloque.hora_fin)
                    {
                        todosLosSlots.Add(horaActual);
                        horaActual = horaActual.Add(TimeSpan.FromHours(1));
                    }
                }

                // 3. Obtener citas ya agendadas para ese terapeuta en esa fecha (no canceladas)
                var citasExistentes = db.Cita
                    .Where(c => c.id_terapeuta == idTerapeuta
                             && c.fecha == fechaCita
                             && c.estado != "Cancelada")
                    .Select(c => new { c.hora_inicio, c.hora_fin })
                    .ToList();

                // 4. Obtener bloqueos de horario para ese terapeuta en esa fecha
                var bloqueos = db.BloqueHorario
                    .Where(b => b.id_terapeuta == idTerapeuta
                             && b.fecha == fechaCita)
                    .Select(b => new { b.hora_inicio, b.hora_fin })
                    .ToList();

                // 5. Determinar disponibilidad de cada slot
                var resultado = todosLosSlots.Select(slot =>
                {
                    var slotFin = slot.Add(TimeSpan.FromHours(1));

                    // Verificar si hay cita que se solape con este slot
                    bool ocupadoPorCita = citasExistentes.Any(c =>
                        slot < c.hora_fin && slotFin > c.hora_inicio);

                    // Verificar si hay bloqueo que se solape con este slot
                    bool ocupadoPorBloqueo = bloqueos.Any(b =>
                        slot < b.hora_fin && slotFin > b.hora_inicio);

                    return new
                    {
                        hora = slot.Hours.ToString("00") + ":" + slot.Minutes.ToString("00"),
                        disponible = !ocupadoPorCita && !ocupadoPorBloqueo
                    };
                }).ToList();

                return Json(new { success = true, horarios = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error al obtener horarios: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Paciente/ConfirmarCita
        [HttpPost]
        public JsonResult ConfirmarCita(int idTerapeuta, string fecha, string horaInicio)
        {
            try
            {
                DateTime fechaCita = DateTime.Parse(fecha);
                TimeSpan hora = TimeSpan.Parse(horaInicio);
                TimeSpan horaFin = hora.Add(TimeSpan.FromHours(1));

                // Verificar que no exista ya un bloqueo para ese horario
                bool yaExiste = db.BloqueHorario.Any(b =>
                    b.id_terapeuta == idTerapeuta
                    && b.fecha == fechaCita
                    && b.hora_inicio == hora);

                if (yaExiste)
                {
                    return Json(new { success = false, mensaje = "Este horario ya se encuentra bloqueado" });
                }

                // Insertar en BloqueHorario
                var bloqueo = new BloqueHorario
                {
                    id_terapeuta = idTerapeuta,
                    fecha = fechaCita,
                    hora_inicio = hora,
                    hora_fin = horaFin,
                    motivo = "Confirmada"
                };

                db.BloqueHorario.Add(bloqueo);
                db.SaveChanges();

                return Json(new { success = true, mensaje = "Cita confirmada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error al confirmar la cita: " + ex.Message });
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