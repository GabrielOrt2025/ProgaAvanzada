namespace MediGrids.Models
{
    /// <summary>
    /// ViewModel para tipos de terapia
    /// Usado para mantener consistencia en los ViewModels
    /// </summary>
    public class TipoTerapiaViewModel
    {
        public int IdTerapia { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}
