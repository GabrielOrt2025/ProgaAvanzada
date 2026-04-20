using System;

namespace MediGrids.Models
{
    public class MisEjerciciosAsignadosViewModel
    {
        public int IdPaciente { get; set; }
        public int IdEjercicio { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public string Estado { get; set; }

        public string NombreEjercicio { get; set; }
        public string Descripcion { get; set; }

        public int? IdCategoria { get; set; }
        public int? IdTerapia { get; set; }
    }
}