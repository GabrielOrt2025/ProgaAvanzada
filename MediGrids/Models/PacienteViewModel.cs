namespace MediGrids.Models
{
    /// <summary>
    /// ViewModel para pacientes sin referencias circulares
    /// Usado en la vista de asignación de ejercicios para el dropdown de pacientes
    /// </summary>
    public class PacienteViewModel
    {
        public int IdPaciente { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public bool? Activo { get; set; }
    }
}
