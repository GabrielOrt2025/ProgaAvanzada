using System;

namespace MediGrids.Models
{
    public class RolesViewModel
    {
        public int id_usuario { get; set; }
        public string correo { get; set; }
        public int id_rol { get; set; }
        public string nombre_rol { get; set; }
        public bool? activo { get; set; }
    }
}