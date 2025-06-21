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
    public class VacacionesController : Controller
    {
        // Conexión bd
        private smartbuilding_rhEntities db = new smartbuilding_rhEntities();

        // Pagina prin
        public ActionResult Index()
        {
            // Trae las vacaciones y también trae la información del empleado relacionada 
            var vacaciones = db.Vacaciones.Include(v => v.Empleados);
            // Convertir la lista a un formato que la vista pueda mostrar
            return View(vacaciones.ToList());
        }

        // Detalles de una vacación específica
        public ActionResult Details(int? id)
        {
            // Validar que se envió un id, si es null da error 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Busca la vacación con ese id
            Vacaciones vacaciones = db.Vacaciones.Find(id);
            // Si no la encuentra, error
            if (vacaciones == null)
            {
                return HttpNotFound();
            }
            // Muestra la vista con los detalles de esa vacación
            return View(vacaciones);
        }

        // Nueva vacaciones
        public ActionResult Create()
        {
            // Crea una lista desplegable con los empleados para seleccionar en el formulario
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula");
            // Muestra la vista con el formulario vacío
            return View();
        }

        // CRear vacaciones
        [HttpPost]
        [ValidateAntiForgeryToken]  // Seguridad 
        public ActionResult Create([Bind(Include = "id_vacacion,id_empleado,periodo,dias_disponibles,dias_disfrutados,fecha_creacion,fecha_actualizacion")] Vacaciones vacaciones)
        {
            // Verifica que los datos recibidos sean válidos
            if (ModelState.IsValid)
            {
                // Agrega la nueva vacación a la base de datos
                db.Vacaciones.Add(vacaciones);
                // Guarda los cambios en la base de datos
                db.SaveChanges();
                // Redirige a la lista de vacaciones
                return RedirectToAction("Index");
            }

            // Si hay un error, volver a mostrar el formulario con la lista de empleados cargada
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", vacaciones.id_empleado);
            return View(vacaciones);
        }

        // Edición vacaciones
        public ActionResult Edit(int? id)
        {
            // Verifica que se envió el id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Busca la vacación a editar
            Vacaciones vacaciones = db.Vacaciones.Find(id);
            // Si no se encuentra, error
            if (vacaciones == null)
            {
                return HttpNotFound();
            }
            // Lista de empleados para el formulario
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", vacaciones.id_empleado);
            // Muestra el formulario con los datos cargados
            return View(vacaciones);
        }

        // Act vacaciones 
        [HttpPost]
        [ValidateAntiForgeryToken]  
        public ActionResult Edit([Bind(Include = "id_vacacion,id_empleado,periodo,dias_disponibles,dias_disfrutados,fecha_creacion,fecha_actualizacion")] Vacaciones vacaciones)
        {
            // Verifica que los datos sean válidos
            if (ModelState.IsValid)
            {
                // Indica que este objeto fue modificado
                db.Entry(vacaciones).State = EntityState.Modified;
                // Guarda los cambios en la base de datos
                db.SaveChanges();
                // Regresa a la lista de vacaciones
                return RedirectToAction("Index");
            }
            // Si hay error, mostrar el formulario con la lista de empleados y datos actuales
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", vacaciones.id_empleado);
            return View(vacaciones);
        }

        // Confirmación eliminación vacaciones
        public ActionResult Delete(int? id)
        {
            // Verifica que se envió el id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Busca la vacación a eliminar
            Vacaciones vacaciones = db.Vacaciones.Find(id);
            // Si no se encuentra, mostrar error 
            if (vacaciones == null)
            {
                return HttpNotFound();
            }
            // Muestra la vista de confirmación de borrado
            return View(vacaciones);
        }

        // Conf eliminación
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]  
        public ActionResult DeleteConfirmed(int id)
        {
            // Busca la vacación por id
            Vacaciones vacaciones = db.Vacaciones.Find(id);
            // Elimina de la base de datos
            db.Vacaciones.Remove(vacaciones);
            // Guarda los cambios
            db.SaveChanges();
            // Regresa a la lista de vacaciones
            return RedirectToAction("Index");
        }

    // Libera recursos
    protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Cerrar la conexión con la base de datos
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}


