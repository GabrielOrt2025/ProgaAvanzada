namespace MediGrids.Models
{
    /// <summary>
    /// ViewModel para ejercicios sin referencias circulares
    /// Usado en la vista de asignación de ejercicios para mostrar ejercicios disponibles
    /// </summary>
    public class EjercicioViewModel
    {
        public int IdEjercicio { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Objetivo { get; set; }
        public string Dificultad { get; set; }
        public string VideoUrl { get; set; }
        public int IdTerapia { get; set; }
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
    }
}
