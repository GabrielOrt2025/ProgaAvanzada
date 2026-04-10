using System;


namespace MediGrids.Models
{
    public class AgendaBloqueoViewModel
    {
        public int id_bloque { get; set; }
        public DateTime fecha { get; set; }
        public TimeSpan hora_inicio { get; set; }
        public TimeSpan hora_fin { get; set; }
        public string motivo { get; set; }
    }
}
