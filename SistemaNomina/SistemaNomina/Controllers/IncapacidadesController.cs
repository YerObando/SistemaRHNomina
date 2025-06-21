using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity; // Accede al bd
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc; 
using SistemaNomina; 

namespace SistemaNomina.Controllers
{
    public class IncapacidadesController : Controller
    {
        // Conexión bd
        private smartbuilding_rhEntities db = new smartbuilding_rhEntities();

        // Muestra la lista de incapacidades
        public ActionResult Index()
        {
            // Trae las incapacidades e info del empleado 
            var incapacidades = db.Incapacidades.Include(i => i.Empleados).Include(i => i.Estados).Include(i => i.TipoIncapacidades);
            return View(incapacidades.ToList()); // Envía la lista a la vista
        }

        // Muestra los detalles de una incapacidad específica
        public ActionResult Details(int? id)
        {
            if (id == null) // Si no se envió un id, devuelve error
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Incapacidades incapacidades = db.Incapacidades.Find(id); // Busca la incapacidad en la base
            if (incapacidades == null) // Si no la encuentra, muestra error
            {
                return HttpNotFound();
            }
            return View(incapacidades); // Muestra la vista con los datos encontrados
        }

        // Muestra el formulario para crear una nueva incapacidad
        public ActionResult Create()
        {
            // Llena la vista con los datos necesarios
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula");
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "nombre");
            ViewBag.id_tipo_incapacidad = new SelectList(db.TipoIncapacidades, "id_tipo", "nombre");
            return View(); // Muestra el formulario vacío
        }

        // Recibe los datos del formulario para crear una nueva incapacidad
        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques 
        public ActionResult Create([Bind(Include = "id_incapacidad,id_empleado,fecha_inicio,fecha_fin,numero_boleta,id_tipo_incapacidad,descripcion,id_estado,dias_incapacidad,fecha_registro")] Incapacidades incapacidades)
        {
            if (ModelState.IsValid) // Si los datos son válidos
            {
                db.Incapacidades.Add(incapacidades); // Agrega la nueva incapacidad
                db.SaveChanges(); // Guarda bd
                return RedirectToAction("Index"); // Vuelve al listado
            }

            // Si algo falla, recarga y vuelve a mostrar el formulario
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", incapacidades.id_empleado);
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "nombre", incapacidades.id_estado);
            ViewBag.id_tipo_incapacidad = new SelectList(db.TipoIncapacidades, "id_tipo", "nombre", incapacidades.id_tipo_incapacidad);
            return View(incapacidades);
        }

        // Muestra el formulario para editar una incapacidad
        public ActionResult Edit(int? id)
        {
            if (id == null) // Si no se envió un id, muestra error
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Incapacidades incapacidades = db.Incapacidades.Find(id); // Busca la incapacidad
            if (incapacidades == null)
            {
                return HttpNotFound();
            }

            // Llena con los datos actuales seleccionados
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", incapacidades.id_empleado);
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "nombre", incapacidades.id_estado);
            ViewBag.id_tipo_incapacidad = new SelectList(db.TipoIncapacidades, "id_tipo", "nombre", incapacidades.id_tipo_incapacidad);
            return View(incapacidades); // Muestra los datos para editar
        }

        // Recibe los datos modificados para guardar la edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_incapacidad,id_empleado,fecha_inicio,fecha_fin,numero_boleta,id_tipo_incapacidad,descripcion,id_estado,dias_incapacidad,fecha_registro")] Incapacidades incapacidades)
        {
            if (ModelState.IsValid)
            {
                db.Entry(incapacidades).State = EntityState.Modified; // Marca el registro como modificado
                db.SaveChanges(); // Guarda cambios en la base
                return RedirectToAction("Index"); // Vuelve a la lista
            }

            // Si hay errores, vuelve a mostrar el formulario
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", incapacidades.id_empleado);
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "nombre", incapacidades.id_estado);
            ViewBag.id_tipo_incapacidad = new SelectList(db.TipoIncapacidades, "id_tipo", "nombre", incapacidades.id_tipo_incapacidad);
            return View(incapacidades);
        }

        // Muestra la pantalla para confirmar la eliminación
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Incapacidades incapacidades = db.Incapacidades.Find(id);
            if (incapacidades == null)
            {
                return HttpNotFound();
            }
            return View(incapacidades); // Muestra los datos antes de borrar
        }

        // Borra la incapacidad después de confirmar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Incapacidades incapacidades = db.Incapacidades.Find(id); // Busca la incapacidad
            db.Incapacidades.Remove(incapacidades); // La elimina
            db.SaveChanges(); // Guarda los cambios
            return RedirectToAction("Index"); // Vuelve al listado
        }

        // Libera recursos
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); // Cierra la conexión a la bd
            }
            base.Dispose(disposing);
        }
    }
}
