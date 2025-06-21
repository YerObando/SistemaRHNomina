using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SistemaNomina;

namespace SistemaNomina.Controllers
{
    public class HorasExtrasController : Controller
    {
        // Conexión bd
        private smartbuilding_rhEntities db = new smartbuilding_rhEntities();

        // Mostrar la lista de horas extras 
        public ActionResult Index()
        {
            // Se incluyen datos relacionados de otras tablas 
            var horasExtras = db.HorasExtras.Include(h => h.Empleados).Include(h => h.Estados).Include(h => h.Usuarios).Include(h => h.TiposHoraExtra);
            return View(horasExtras.ToList()); // Se envía la lista a la vista
        }

        // Mostrar detalles de un registro específico
        public ActionResult Details(int? id)
        {
            if (id == null) // Si no se pasa ningún ID
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Se devuelve error
            }
            HorasExtras horasExtras = db.HorasExtras.Find(id); // Busca el registro por ID
            if (horasExtras == null) // Si no se encuentra el registro
            {
                return HttpNotFound(); // Se devuelve error 
            }
            return View(horasExtras); // Se envía el registro a la vista
        }

        // Mostrar el formulario para crear una nueva hora extra
        public ActionResult Create()
        {
            // Se preparan las listas desplegables para los datos relacionados
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula");
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "nombre");
            ViewBag.aprobado_por = new SelectList(db.Usuarios, "id_usuario", "usuario");
            ViewBag.id_tipo = new SelectList(db.TiposHoraExtra, "id_tipo", "nombre");
            return View(); // Se muestra el formulario
        }

        // Guardar una nueva hora extra 
        [HttpPost]
        [ValidateAntiForgeryToken] // Previene ataques de seguridad
        public ActionResult Create([Bind(Include = "id_hora_extra,id_empleado,id_tipo,fecha,hora_inicio,hora_fin,horas,valor_hora,recargo,total,motivo,id_estado,aprobado_por,fecha_aprobacion,fecha_creacion,fecha_actualizacion")] HorasExtras horasExtras)
        {
            if (ModelState.IsValid) // Verifica que los datos estén correctos
            {
                db.HorasExtras.Add(horasExtras); // Agrega el nuevo registro
                db.SaveChanges(); // Guarda en la base de datos
                return RedirectToAction("Index"); // Vuelve a la lista principal
            }

            // Si hubo errores, se vuelven a llenar las listas desplegables
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", horasExtras.id_empleado);
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "nombre", horasExtras.id_estado);
            ViewBag.aprobado_por = new SelectList(db.Usuarios, "id_usuario", "usuario", horasExtras.aprobado_por);
            ViewBag.id_tipo = new SelectList(db.TiposHoraExtra, "id_tipo", "nombre", horasExtras.id_tipo);
            return View(horasExtras); // Se vuelve a mostrar el formulario con errores
        }

        // Mostrar el formulario para editar un registro
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HorasExtras horasExtras = db.HorasExtras.Find(id);
            if (horasExtras == null)
            {
                return HttpNotFound();
            }

            // Cargar listas desplegables con valores actuales
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", horasExtras.id_empleado);
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "nombre", horasExtras.id_estado);
            ViewBag.aprobado_por = new SelectList(db.Usuarios, "id_usuario", "usuario", horasExtras.aprobado_por);
            ViewBag.id_tipo = new SelectList(db.TiposHoraExtra, "id_tipo", "nombre", horasExtras.id_tipo);
            return View(horasExtras);
        }

        // Guardar los cambios después de editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_hora_extra,id_empleado,id_tipo,fecha,hora_inicio,hora_fin,horas,valor_hora,recargo,total,motivo,id_estado,aprobado_por,fecha_aprobacion,fecha_creacion,fecha_actualizacion")] HorasExtras horasExtras)
        {
            if (ModelState.IsValid)
            {
                db.Entry(horasExtras).State = EntityState.Modified; // Marca el registro como modificado
                db.SaveChanges(); // Guarda los cambios
                return RedirectToAction("Index"); // Vuelve a la lista
            }

            // Si hubo errores, se vuelve a llenar las listas desplegables
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", horasExtras.id_empleado);
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "nombre", horasExtras.id_estado);
            ViewBag.aprobado_por = new SelectList(db.Usuarios, "id_usuario", "usuario", horasExtras.aprobado_por);
            ViewBag.id_tipo = new SelectList(db.TiposHoraExtra, "id_tipo", "nombre", horasExtras.id_tipo);
            return View(horasExtras);
        }

        // Mostrar el formulario para confirmar la eliminación de un registro
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HorasExtras horasExtras = db.HorasExtras.Find(id);
            if (horasExtras == null)
            {
                return HttpNotFound();
            }
            return View(horasExtras); // Muestra los datos antes de eliminar
        }

        // Eliminar definitivamente el registro
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HorasExtras horasExtras = db.HorasExtras.Find(id);
            db.HorasExtras.Remove(horasExtras); // Elimina el registro
            db.SaveChanges(); // Guarda los cambios
            return RedirectToAction("Index"); // Vuelve a la lista
        }

        // Libera los recursos 
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
