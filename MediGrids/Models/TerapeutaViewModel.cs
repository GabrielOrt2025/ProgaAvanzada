using System.Collections.Generic;

namespace MediGrids.Models
{
    /// <summary>
    /// ViewModel para mostrar información de terapeutas en la vista de agendar citas
    /// Basado estrictamente en la entidad Terapeuta de Entity Framework
    /// Incluye especialidades y categorías calculadas sin referencias circulares
    /// </summary>
    public class TerapeutaViewModel
    {
        // Propiedades basadas en la entidad Terapeuta de EF
        public int IdTerapeuta { get; set; }
        public int? IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool? Activo { get; set; }
        public int? IdCategoria { get; set; }
        
        // Propiedades adicionales calculadas para la vista
        /// <summary>
        /// Lista de IDs de tipos de terapia en los que el terapeuta tiene experiencia
        /// Calculado basándose en los ejercicios creados por el terapeuta
        /// </summary>
        public List<int> Especialidades { get; set; }
        
        /// <summary>
        /// Lista de IDs de categorías clínicas en las que el terapeuta trabaja
        /// Calculado basándose en los ejercicios creados por el terapeuta
        /// </summary>
        public List<int> Categorias { get; set; }

        public TerapeutaViewModel()
        {
            Especialidades = new List<int>();
            Categorias = new List<int>();
        }
    }
}
