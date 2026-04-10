using System;

namespace MediGrids.Models
{
    public class AgendaCitaViewModel
    {
        public int id_cita { get; set; }
        public DateTime fecha { get; set; }
        public TimeSpan hora_inicio { get; set; }
        public TimeSpan hora_fin { get; set; }
        public string estado { get; set; }
        public string motivo_cancelacion { get; set; }
        public int id_terapia { get; set; }
        public int id_categoria { get; set; }
        public int id_paciente { get; set; }
        public string nombre_paciente { get; set; }
        public string telefono_paciente { get; set; }
        public string email_paciente { get; set; }
    }
}