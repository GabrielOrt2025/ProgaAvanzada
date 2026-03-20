using System;

namespace MediGrids.Models
{
    /// <summary>
    /// ViewModel para asignaciones de ejercicios a pacientes sin referencias circulares
    /// Usado en la serialización JSON para la tabla de asignaciones
    /// </summary>
    public class AsignacionEjercicioViewModel
    {
        public int IdPaciente { get; set; }
        public int IdEjercicio { get; set; }
        public string NombrePaciente { get; set; }
        public string ApellidosPaciente { get; set; }
        public string NombreEjercicio { get; set; }
        public string DescripcionEjercicio { get; set; }
        public string ObjetivoEjercicio { get; set; }
        public string DificultadEjercicio { get; set; }
        public string NombreTerapia { get; set; }
        public string NombreCategoria { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public string Estado { get; set; }
    }
}
