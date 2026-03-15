namespace MediGrids.Models
{
    /// <summary>
    /// ViewModel para categorías clínicas sin referencias circulares
    /// Usado en la serialización JSON para evitar problemas de referencia circular
    /// </summary>
    public class CategoriaClinicaViewModel
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdTerapia { get; set; }
    }
}
