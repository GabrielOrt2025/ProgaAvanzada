using System;

namespace MediGrids.Models
{
    public class MiCitaViewModel
    {
        public int IdCita { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Estado { get; set; }
        public string NombreTerapeuta { get; set; }
        public string NombreTerapia { get; set; }
        public string NombreCategoria { get; set; }

        public string FechaFormateada
        {
            get { return Fecha.ToString("dd/MM/yyyy"); }
        }

        public string HoraFormateada
        {
            get { return HoraInicio.Hours.ToString("00") + ":" + HoraInicio.Minutes.ToString("00") + " - " + HoraFin.Hours.ToString("00") + ":" + HoraFin.Minutes.ToString("00"); }
        }

        public bool PuedeCancelar
        {
            get { return Estado == "Programada" || Estado == "Confirmada"; }
        }
    }
}
