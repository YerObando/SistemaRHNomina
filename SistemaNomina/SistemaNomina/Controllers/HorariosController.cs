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
    public class HorariosController : Controller
    {
        // Conexión bd
        private smartbuilding_rhEntities db = new smartbuilding_rhEntities();

        // Muestra la lista de horarios
        public ActionResult Index()
        {
            return View(db.Horarios.ToList());
        }

        // Muestra los detalles de un horario específico
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                // Si no se pasa el id, da error 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Busca el horario por id
            Horarios horarios = db.Horarios.Find(id);
            if (horarios == null)
            {
                // Si no se encuentra, da error 
                return HttpNotFound();
            }
            // Muestra los detalles
            return View(horarios);
        }

        // Muestra el formulario para crear un nuevo horario
        public ActionResult Create()
        {
            return View();
        }

        // Procesa el formulario para crear un nuevo horario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_horario,nombre,hora_entrada,hora_salida,fecha_creacion,fecha_actualizacion")] Horarios horarios)
        {
            if (ModelState.IsValid)
            {
                // Agrega el nuevo horario a la bd
                db.Horarios.Add(horarios);
                db.SaveChanges();
                // Redirige a la lista
                return RedirectToAction("Index");
            }

            // Si hay errores, vuelve a mostrar el formulario
            return View(horarios);
        }

        // Muestra el formulario para editar un horario existente
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Busca el horario a editar
            Horarios horarios = db.Horarios.Find(id);
            if (horarios == null)
            {
                return HttpNotFound();
            }
            return View(horarios);
        }

        // Procesa el formulario para guardar cambios en el horario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_horario,nombre,hora_entrada,hora_salida,fecha_creacion,fecha_actualizacion")] Horarios horarios)
        {
            if (ModelState.IsValid)
            {
                // Marca el objeto como modificado
                db.Entry(horarios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(horarios);
        }

        // Muestra el formulario para confirmar eliminación de un horario
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Busca el horario a eliminar
            Horarios horarios = db.Horarios.Find(id);
            if (horarios == null)
            {
                return HttpNotFound();
            }
            return View(horarios);
        }

        // Elimina el horario después de confirmar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Busca y elimina el horario
            Horarios horarios = db.Horarios.Find(id);
            db.Horarios.Remove(horarios);
            db.SaveChanges();
            return RedirectToAction("Index");
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
