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
    public class EstadosController : Controller
    {
        // Conexión bd
        private smartbuilding_rhEntities db = new smartbuilding_rhEntities();

        // Muestra la lista de todos los estados
        public ActionResult Index()
        {
            return View(db.Estados.ToList());
        }

        // Muestra los detalles de un estado específico
        public ActionResult Details(int? id)
        {
            // Si no se envió un id, devuelve error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca el estado con ese id
            Estados estados = db.Estados.Find(id);

            // Si no lo encuentra, muestra error 
            if (estados == null)
            {
                return HttpNotFound();
            }

            // Muestra la vista con los detalles del estado
            return View(estados);
        }

        // Muestra el formulario para crear un nuevo estado
        public ActionResult Create()
        {
            // Lista de opciones para el campo módulo
            ViewBag.Modulos = new List<string> { "Permisos", "Vacaciones", "Asistencias" };
            return View();
        }

        // Guarda el nuevo estado en la bd
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_estado,nombre,modulo")] Estados estados)
        {
            // Verifica que los datos estén correctos
            if (ModelState.IsValid)
            {
                // Agrega el estado y guarda cambios
                db.Estados.Add(estados);
                db.SaveChanges();
                return RedirectToAction("Index"); // Redirige a la lista de estados
            }

            // Si hay errores, vuelve a mostrar el formulario
            ViewBag.Modulos = new List<string> { "Permisos", "Vacaciones", "Asistencias" };
            return View(estados);
        }

        // Muestra el formulario para editar un estado existente
        public ActionResult Edit(int? id)
        {
            // Verifica si se envió un id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca el estado con ese id
            Estados estados = db.Estados.Find(id);

            // Si no lo encuentra, muestra error 
            if (estados == null)
            {
                return HttpNotFound();
            }

            // Lista de opciones para el campo módulo
            ViewBag.Modulos = new List<string> { "Permisos", "Vacaciones", "Asistencias" };
            return View(estados);
        }

        // Guarda los cambios después de editar un estado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_estado,nombre,modulo")] Estados estados)
        {
            // Verifica que los datos estén correctos
            if (ModelState.IsValid)
            {
                // Marca el estado como modificado y guarda cambios
                db.Entry(estados).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Si hay errores, vuelve a mostrar el formulario
            ViewBag.Modulos = new List<string> { "Permisos", "Vacaciones", "Asistencias" };
            return View(estados);
        }

        // Muestra la vista para confirmar la eliminación de un estado
        public ActionResult Delete(int? id)
        {
            // Verifica si se envió un id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca el estado con ese id
            Estados estados = db.Estados.Find(id);

            // Si no lo encuentra, muestra error 
            if (estados == null)
            {
                return HttpNotFound();
            }

            // Muestra la vista de confirmación
            return View(estados);
        }

        // Elimina el estado después de confirmar
        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Busca y elimina el estado
            Estados estados = db.Estados.Find(id);
            db.Estados.Remove(estados);
            db.SaveChanges();
            return RedirectToAction("Index"); // Redirige a la lista después de eliminar
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
