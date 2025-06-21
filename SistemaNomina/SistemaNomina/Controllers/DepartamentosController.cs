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
    public class DepartamentosController : Controller
    {
        // Conexión bd
        private smartbuilding_rhEntities db = new smartbuilding_rhEntities();

        // Muestra la lista de departamentos
        public ActionResult Index()
        {
            // Muestra todos los departamentos
            return View(db.Departamentos.ToList());
        }

        // Muestra los detalles de un departamento específico
        public ActionResult Details(int? id)
        {
            // Si no se recibe un ID, muestra error
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Busca el departamento con ese ID
            var departamentos = db.Departamentos.Find(id);

            // Si no existe ese departamento, muestra error
            if (departamentos == null)
                return HttpNotFound();

            // Muestra la información del departamento
            return View(departamentos);
        }

        // Muestra el formulario para crear un nuevo departamento
        public ActionResult Create()
        {
            return View();
        }

        // Guarda el nuevo departamento enviado desde el formulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_departamento,nombre,fecha_creacion,fecha_actualizacion")] Departamentos departamentos)
        {
            // Verifica que los datos estén correctos
            if (ModelState.IsValid)
            {
                // Agrega el nuevo departamento a la base de datos
                db.Departamentos.Add(departamentos);
                db.SaveChanges(); // Guarda los cambios
                return RedirectToAction("Index"); // Vuelve a la lista
            }

            // Si algo está mal, muestra de nuevo el formulario con los datos
            return View(departamentos);
        }

        // Muestra el formulario para editar un departamento existente
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var departamentos = db.Departamentos.Find(id);

            if (departamentos == null)
                return HttpNotFound();

            return View(departamentos);
        }

        // Guarda los cambios realizados en un departamento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_departamento,nombre,fecha_creacion,fecha_actualizacion")] Departamentos departamentos)
        {
            if (ModelState.IsValid)
            {
                // Marca el departamento como modificado para guardar cambios
                db.Entry(departamentos).State = EntityState.Modified;
                db.SaveChanges(); // Guarda en la base de datos
                return RedirectToAction("Index");
            }
            // Si hay errores, vuelve a mostrar el formulario
            return View(departamentos);
        }

        // Muestra la confirmación para eliminar un departamento
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var departamentos = db.Departamentos.Find(id);

            if (departamentos == null)
                return HttpNotFound();

            return View(departamentos);
        }

        // Elimina un departamento después de confirmación
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var departamentos = db.Departamentos.Find(id);

            // Quita el departamento de la base de datos
            db.Departamentos.Remove(departamentos);
            db.SaveChanges(); // Guarda los cambios
            return RedirectToAction("Index");
        }

        // Limpia los recursos cuando el controlador ya no se usa
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
