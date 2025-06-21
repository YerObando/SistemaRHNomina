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
    public class AguinaldoController : Controller
    {
        //Conexión con la bd
        private smartbuilding_rhEntities db = new smartbuilding_rhEntities();

        // Muestra una lista con todos los registros de aguinaldos
        public ActionResult Index()
        {
            var aguinaldo = db.Aguinaldo.Include(a => a.Empleados); // Se incluye la información del empleado
            return View(aguinaldo.ToList()); // Se envía la lista a la vista para mostrarla
        }

        // Muestra los detalles de un aguinaldo específico, según su id
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                // Si no se recibe el id, se devuelve un error
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aguinaldo aguinaldo = db.Aguinaldo.Find(id); // Busca el aguinaldo en la bd
            if (aguinaldo == null)
            {
                // Si no se encuentra, muestra un mensaje de error
                return HttpNotFound();
            }
            return View(aguinaldo); // Si se encuentra, muestra la información en la vista
        }

        // Muestra el formulario para crear un nuevo aguinaldo
        public ActionResult Create()
        {
            // Se cargan los empleados para que el usuario seleccione a cuál corresponde el aguinaldo
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula");
            return View();
        }

        // Recibe los datos ingresados por el usuario al crear un nuevo aguinaldo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_aguinaldo,id_empleado,monto_total,meses_laborados,anio,fecha_creacion,fecha_actualizacion")] Aguinaldo aguinaldo)
        {
            if (ModelState.IsValid) // Revisa que los datos estén bien
            {
                db.Aguinaldo.Add(aguinaldo); // Se guarda el nuevo aguinaldo en la bd
                db.SaveChanges(); // Se aplican los cambios
                return RedirectToAction("Index"); // Se vuelve a la lista de aguinaldos
            }

            // Si hay errores, vuelve a mostrar el formulario
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", aguinaldo.id_empleado);
            return View(aguinaldo);
        }

        // Muestra el formulario para editar un aguinaldo existente
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                // Si no se recibe el id, devuelve un error
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aguinaldo aguinaldo = db.Aguinaldo.Find(id); // Se busca el aguinaldo por su id
            if (aguinaldo == null)
            {
                // Si no se encuentra, muestra un error
                return HttpNotFound();
            }
            // Se cargan los empleados para el formulario
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", aguinaldo.id_empleado);
            return View(aguinaldo);
        }

        // Recibe los datos modificados para actualizar un aguinaldo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_aguinaldo,id_empleado,monto_total,meses_laborados,anio,fecha_creacion,fecha_actualizacion")] Aguinaldo aguinaldo)
        {
            if (ModelState.IsValid) // Verifica que los datos sean correctos
            {
                db.Entry(aguinaldo).State = EntityState.Modified; // Indica que el registro fue modificado
                db.SaveChanges(); // Se guardan los cambios en la bd
                return RedirectToAction("Index"); // Se vuelve a la lista de aguinaldos
            }
            // Si hay errores, recarga el formulario
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", aguinaldo.id_empleado);
            return View(aguinaldo);
        }

        // Muestra la vista para confirmar la eliminación de un aguinaldo
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                // Si no se recibe el id, devuelve un error
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aguinaldo aguinaldo = db.Aguinaldo.Find(id); // Busca el aguinaldo
            if (aguinaldo == null)
            {
                // Si no se encuentra, muestra un error
                return HttpNotFound();
            }
            return View(aguinaldo); // Muestra la vista de confirmación
        }

        // Elimina un aguinaldo después de la confirmación
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Aguinaldo aguinaldo = db.Aguinaldo.Find(id); // Busca el aguinaldo por id
            db.Aguinaldo.Remove(aguinaldo); // Elimina el registro
            db.SaveChanges(); // Guarda los cambios
            return RedirectToAction("Index"); // Regresa a la lista
        }

        // Libera los recursos que usó la bd cuando ya no se necesita más
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); // Cierra la conexión con la bd
            }
            base.Dispose(disposing);
        }
    }
}
