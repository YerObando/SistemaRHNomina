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
    public class EstadoCivilController : Controller
    {
        // Conexión bd
        private smartbuilding_rhEntities db = new smartbuilding_rhEntities();

        // Mostrar la lista estados civiles
        // Método GET: EstadoCivil
        public ActionResult Index()
        {
            // Obtener todos los registros de EstadoCivil y enviarlos a la vista
            return View(db.EstadoCivil.ToList());
        }

        // Muestra los detalles de un estado civil específico
        // Método GET: EstadoCivil
        public ActionResult Details(int? id)
        {
            // Verificar que el id no sea nulo, si es nulo devolver error 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Buscar el estado civil por id en la bd
            EstadoCivil estadoCivil = db.EstadoCivil.Find(id);
            // Si no existe, devolver error 
            if (estadoCivil == null)
            {
                return HttpNotFound();
            }
            // Enviar el estado civil a la vista para mostrar detalles
            return View(estadoCivil);
        }

        // Muestra el formulario para crear un nuevo estado civil
        // Método GET: EstadoCivil
        public ActionResult Create()
        {
            return View();
        }

        // Recibe los datos del formulario y crear el estado civil en la bd
        [HttpPost]
        [ValidateAntiForgeryToken] //Contra ataque
        public ActionResult Create([Bind(Include = "id_estado_civil,nombre")] EstadoCivil estadoCivil)
        {
            // Verificar que los datos enviados cumplen con las reglas 
            if (ModelState.IsValid)
            {
                // Agregar el nuevo estado civil 
                db.EstadoCivil.Add(estadoCivil);
                // Guardar los cambios en la bd
                db.SaveChanges();
                // Redirigir a la lista de estados civiles
                return RedirectToAction("Index");
            }
            // Si los datos no son válidos, regresar al formulario con los datos ingresados
            return View(estadoCivil);
        }

        // Muestra el formulario para editar un estado civil existente
        public ActionResult Edit(int? id)
        {
            // Validar que el id no sea nulo
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Buscar el estado civil por id
            EstadoCivil estadoCivil = db.EstadoCivil.Find(id);
            // Si no se encuentra, devolver error 
            if (estadoCivil == null)
            {
                return HttpNotFound();
            }
            // Enviar el estado civil a la vista para editar
            return View(estadoCivil);
        }

        // Recibe los datos editados y guardar los cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_estado_civil,nombre")] EstadoCivil estadoCivil)
        {
            // Validar que los datos son correctos
            if (ModelState.IsValid)
            {
                // Marcar como modificado para que se actualice en la bd
                db.Entry(estadoCivil).State = EntityState.Modified;
                // Guardar los cambios
                db.SaveChanges();
                // Volver a la lista de estados civiles
                return RedirectToAction("Index");
            }
            // Si hay errores, volver al formulario con los datos ingresados
            return View(estadoCivil);
        }

        // Muestra confirmación para eliminar un estado civil
        public ActionResult Delete(int? id)
        {
            // Validar que el id no sea nulo
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Buscar el estado civil por id
            EstadoCivil estadoCivil = db.EstadoCivil.Find(id);
            // Si no existe, devolver error
            if (estadoCivil == null)
            {
                return HttpNotFound();
            }
            // Mostrar la vista de confirmación para eliminar
            return View(estadoCivil);
        }

        // Eliminar estado civil después de la confirmación
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Buscar el estado civil por id
            EstadoCivil estadoCivil = db.EstadoCivil.Find(id);
            // Eliminar el registro de bd
            db.EstadoCivil.Remove(estadoCivil);
            // Guardar los cambios
            db.SaveChanges();
            // Volver a la lista de estados civiles
            return RedirectToAction("Index");
        }

        // Libera recursos usados 
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
