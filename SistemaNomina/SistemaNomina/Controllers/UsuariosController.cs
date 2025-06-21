using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SistemaNomina;
using SistemaNomina.Filters;
using SistemaNomina.Helpers;

namespace SistemaNomina.Controllers
{
    [RoleAuthorize("Admin", "RRHH", "Supervisor", "Empleado", "IT")]
    public class UsuariosController : Controller
    {
        private smartbuilding_rhEntities db = new smartbuilding_rhEntities();

        // GET: Usuarios/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(string usuario, string contrasena)
        {
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
            {
                ViewBag.Error = "Debe ingresar el usuario y la contraseña.";
                return View();
            }

            var usuarioDB = db.Usuarios
                .Include(u => u.Roles)
                .Include(u => u.Empleados)
                .FirstOrDefault(u => u.usuario != null && u.usuario.ToLower() == usuario.ToLower());

            if (usuarioDB != null)
            {
                if (usuarioDB.contrasena == contrasena)
                {
                    // Guardar información en sesión
                    Session["Usuario"] = usuarioDB.usuario;
                    Session["RolUsuario"] = usuarioDB.Roles?.nombre ?? "Sin Rol";
                    Session["UserId"] = usuarioDB.id_usuario;

                    FormsAuthentication.SetAuthCookie(usuarioDB.usuario, false);

                    if (usuarioDB.primer_ingreso == true)
                    {
                        return RedirectToAction("CambiarContrasena", new { id = usuarioDB.id_usuario });
                    }

                    switch (usuarioDB.Roles?.nombre?.ToLower())
                    {
                        case "admin":
                            return RedirectToAction("Index", "Home");
                        case "rrhh":
                            return RedirectToAction("Index", "Home");
                        case "supervisor":
                            return RedirectToAction("Index", "Home");
                        case "empleado":
                            return RedirectToAction("Index", "Home");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = "Contraseña incorrecta.";
                }
            }
            else
            {
                ViewBag.Error = "El usuario no existe.";
            }

            return View();
        }

        // GET: Usuarios
        public ActionResult Index()
        {
            // Cargar relaciones necesarias para evitar null references
            var usuarios = db.Usuarios
                .Include(u => u.Empleados)
                .Include(u => u.Roles)
                .ToList();

            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var currentUserId = (int?)Session["UserId"];
            var currentUserRole = Session["RolUsuario"] as string;

            // Validar acceso
            if (currentUserRole != "Admin" && id != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            Usuarios usuario = db.Usuarios
                .Include(u => u.Empleados)
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.id_usuario == id);

            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            var roles = db.Roles.ToList();
            var currentUserRole = Session["RolUsuario"] as string;

            if (currentUserRole != "Admin")
            {
                roles = roles.Where(r => !r.nombre.Equals("Admin", StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula");
            ViewBag.id_rol = new SelectList(roles, "id_rol", "nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_usuario, id_empleado,usuario,contrasena,id_rol,primer_ingreso,fecha_creacion,fecha_actualizacion")] Usuarios usuarios)
        {
            if (ModelState.IsValid)
            {
                var currentUserRole = Session["RolUsuario"] as string;

                if (currentUserRole != "Admin")
                {
                    var rol = db.Roles.Find(usuarios.id_rol);
                    if (rol != null && rol.nombre.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("id_rol", "No tiene permisos para asignar este rol");
                        ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", usuarios.id_empleado);
                        ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "nombre", usuarios.id_rol);
                        return View(usuarios);
                    }
                }

                usuarios.fecha_creacion = DateTime.Now;
                usuarios.fecha_actualizacion = DateTime.Now;
                usuarios.primer_ingreso = true;

                db.Usuarios.Add(usuarios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", usuarios.id_empleado);
            ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "nombre", usuarios.id_rol);
            return View(usuarios);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuarios usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            var currentUserRole = Session["RolUsuario"] as string;
            var roles = db.Roles.ToList();

            if (currentUserRole != "Admin")
            {
                roles = roles.Where(r => !r.nombre.Equals("Admin", StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", usuario.id_empleado);
            ViewBag.id_rol = new SelectList(roles, "id_rol", "nombre", usuario.id_rol);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_usuario,id_empleado,usuario,contrasena,id_rol,primer_ingreso,fecha_creacion,fecha_actualizacion")] Usuarios usuarios)
        {
            if (ModelState.IsValid)
            {
                var currentUserRole = Session["RolUsuario"] as string;

                if (currentUserRole != "Admin")
                {
                    var rol = db.Roles.Find(usuarios.id_rol);
                    if (rol != null && rol.nombre.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("id_rol", "No tiene permisos para asignar este rol");
                        ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", usuarios.id_empleado);
                        ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "nombre", usuarios.id_rol);
                        return View(usuarios);
                    }
                }

                usuarios.fecha_actualizacion = DateTime.Now;
                db.Entry(usuarios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", usuarios.id_empleado);
            ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "nombre", usuarios.id_rol);
            return View(usuarios);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuarios usuario = db.Usuarios
                .Include(u => u.Empleados)
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.id_usuario == id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuarios usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }

        [Authorize]
        public ActionResult CambiarContrasena(int id)
        {
            var currentUserId = (int?)Session["UserId"];
            var currentUserRole = Session["RolUsuario"] as string;

            if (id != currentUserId && currentUserRole != "Admin" && currentUserRole != "IT")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            ViewBag.id_usuario = usuario.id_usuario;
            ViewBag.usuario = usuario.usuario;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CambiarContrasena(int id, string nuevaContrasena, string confirmarContrasena)
        {
            var currentUserId = (int?)Session["UserId"];
            var currentUserRole = Session["RolUsuario"] as string;

            if (id != currentUserId && currentUserRole != "Admin" && currentUserRole != "IT")
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (nuevaContrasena != confirmarContrasena)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                ViewBag.id_usuario = id;
                return View();
            }

            if (nuevaContrasena.Length < 6)
            {
                ViewBag.Error = "La contraseña debe tener al menos 6 caracteres.";
                ViewBag.id_usuario = id;
                return View();
            }

            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            usuario.contrasena = nuevaContrasena;
            usuario.primer_ingreso = false;
            usuario.fecha_actualizacion = DateTime.Now;

            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();

            if (id == currentUserId)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}