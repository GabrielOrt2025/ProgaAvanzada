using System.Collections.Generic;

namespace MediGrids.Models
{
    public class TerapeutaEjercicioViewModel
    {

        public int id_ejercicio { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string objetivo { get; set; }
        public string dificultad { get; set; }
        public string video_url { get; set; }
        public int id_terapia { get; set; }
        public int id_categoria { get; set; }
        public int creado_por { get; set; }
    }
}