using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MediGrids.EntityFramework;
using MediGrids.Models;

namespace MediGrids.Controllers
{
    public class RolesController : Controller
    {
        private Entities db = new Entities();

        //Roles/GestionarRoles
        [HttpGet]
        public ActionResult GestionarRoles()
        {
            if (Session["UserId"] == null || Session["Rol"] == null)
                return RedirectToAction("Login", "Home");

            // Solo el administrador puede ver esto
            int rol = (int)Session["Rol"];
            if (rol != 1)
                return RedirectToAction("Login", "Home");

            ViewBag.obtenerUsuarios = db.Usuario
                .Select(u => new RolesViewModel
                {
                    id_usuario = u.id_usuario,
                    correo = u.correo,
                    id_rol = u.id_rol,
                    nombre_rol = u.Rol.nombre,
                    activo = u.activo
                })
                .ToList();

            return View();
        }

        //Roles/CambiarRol
        [HttpPost]
        public ActionResult CambiarRol(int id, int nuevoRol)
        {
            if (Session["UserId"] == null || Session["Rol"] == null)
                return RedirectToAction("Login", "Home");

            if ((int)Session["Rol"] != 1)
                return RedirectToAction("Login", "Home");

            var usuario = db.Usuario.Find(id);

            if (usuario != null)
            {
                usuario.id_rol = nuevoRol;
                db.SaveChanges();
            }

            return RedirectToAction("GestionarRoles");
        }

        //Roles/ToggleActivo
        [HttpPost]
        public ActionResult ToggleActivo(int id)
        {
            if (Session["UserId"] == null || Session["Rol"] == null)
                return RedirectToAction("Login", "Home");

            if ((int)Session["Rol"] != 1)
                return RedirectToAction("Login", "Home");

            var usuario = db.Usuario.Find(id);

            if (usuario != null)
            {
                usuario.activo = !usuario.activo;
                db.SaveChanges();
            }

            return RedirectToAction("GestionarRoles");
        }
    }
}