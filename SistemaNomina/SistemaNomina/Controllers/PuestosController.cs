using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SistemaNomina;

namespace SistemaNomina.Controllers
{
    public class PuestosController : Controller
    {
        // Conexión bd
        private smartbuilding_rhEntities db = new smartbuilding_rhEntities();

        // Muestra todos los puestos
        public ActionResult Index()
        {
            // Incluye información de departamentos y horarios asociados a cada puesto
            var puestos = db.Puestos.Include(p => p.Departamentos).Include(p => p.Horarios);
            return View(puestos.ToList());
        }

        // Muestra los detalles de un puesto en sío
        public ActionResult Details(int? id)
        {
            if (id == null) // Si no se envía un ID, se retorna error
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca el puesto por ID
            Puestos puestos = db.Puestos.Find(id);

            if (puestos == null) // Si no existe, muestra error
            {
                return HttpNotFound();
            }

            return View(puestos); // Muestra la vista con los datos
        }

        // Muestra el formulario para crear un nuevo puesto
        public ActionResult Create()
        {
            // Llenar listas desplegables para seleccionar departamento y horario
            ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre");
            ViewBag.id_horario = new SelectList(db.Horarios, "id_horario", "nombre");
            return View();
        }

        // Procesa el formulario de creación
        [HttpPost]
        [ValidateAntiForgeryToken] // Seguridad 
        public ActionResult Create([Bind(Include = "nombre_puesto,id_departamento,salario_base,descripcion,horas_jornada,es_jefe,id_horario")] Puestos puestos)
        {
            try
            {
                // Asigna fecha de creación y actualización automática
                puestos.fecha_creacion = DateTime.Now;
                puestos.fecha_actualizacion = DateTime.Now;

                // Valida que el departamento exista en la base de datos
                if (!db.Departamentos.Any(d => d.id_departamento == puestos.id_departamento))
                {
                    ModelState.AddModelError("id_departamento", "El departamento seleccionado no existe");
                }

                // Valida que el horario exista si se seleccionó uno
                if (puestos.id_horario.HasValue && !db.Horarios.Any(h => h.id_horario == puestos.id_horario.Value))
                {
                    ModelState.AddModelError("id_horario", "El horario seleccionado no existe");
                }

                // Si todo es válido, guarda el puesto en la base de datos
                if (ModelState.IsValid)
                {
                    db.Puestos.Add(puestos);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DbEntityValidationException ex) // Captura errores de validación
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError(validationError.PropertyName,
                            $"Error en {validationError.PropertyName}: {validationError.ErrorMessage}");
                    }
                }
            }
            catch (DbUpdateException dbEx) // Captura errores al guardar en la bd
            {
                HandleDbUpdateException(dbEx);
            }
            catch (Exception ex) // Errores inesperados
            {
                ModelState.AddModelError("", $"Error inesperado: {ex.Message}");
                LogExceptionDetails(ex);
            }

            // Si algo falla, se vuelve a cargar el formulario con los valores
            ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre", puestos.id_departamento);
            ViewBag.id_horario = new SelectList(db.Horarios, "id_horario", "nombre", puestos.id_horario);
            return View(puestos);
        }

        // Muestra el formulario para editar un puesto existente
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca el puesto por ID
            Puestos puestos = db.Puestos.Find(id);
            if (puestos == null)
            {
                return HttpNotFound();
            }

            // Carga listas para editar departamento y horario
            ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre", puestos.id_departamento);
            ViewBag.id_horario = new SelectList(db.Horarios, "id_horario", "nombre", puestos.id_horario);
            return View(puestos);
        }

        // Procesa el formulario de edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_puesto,nombre_puesto,id_departamento,salario_base,descripcion,horas_jornada,es_jefe,id_horario,fecha_creacion,fecha_actualizacion")] Puestos puestos)
        {
            try
            {
                // Actualiza la fecha de modificación
                puestos.fecha_actualizacion = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Entry(puestos).State = EntityState.Modified; // Marca como modificado
                    db.SaveChanges(); // Guarda cambios
                    return RedirectToAction("Index");
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError(validationError.PropertyName,
                            $"Error en {validationError.PropertyName}: {validationError.ErrorMessage}");
                    }
                }
            }
            catch (DbUpdateException dbEx)
            {
                HandleDbUpdateException(dbEx);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error inesperado: {ex.Message}");
                LogExceptionDetails(ex);
            }

            // Si hay errores, se vuelve a cargar el formulario
            ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre", puestos.id_departamento);
            ViewBag.id_horario = new SelectList(db.Horarios, "id_horario", "nombre", puestos.id_horario);
            return View(puestos);
        }

        // Muestra la confirmación para eliminar un puesto
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Puestos puestos = db.Puestos.Find(id);
            if (puestos == null)
            {
                return HttpNotFound();
            }

            return View(puestos); // Muestra los datos del puesto a eliminar
        }

        // Procesa la eliminación del puesto
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Puestos puestos = db.Puestos.Find(id);
                db.Puestos.Remove(puestos); // Elimina el puesto
                db.SaveChanges(); // Guarda cambios
                return RedirectToAction("Index");
            }
            catch (DbUpdateException dbEx) // Error al eliminar
            {
                HandleDbUpdateException(dbEx);
                return View("Delete", db.Puestos.Find(id));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar: {ex.Message}");
                LogExceptionDetails(ex);
                return View("Delete", db.Puestos.Find(id));
            }
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

        #region Métodos auxiliares para manejo de errores

        // Método para mostrar mensajes de error específicos según el tipo de error en la base de datos
        private void HandleDbUpdateException(DbUpdateException dbEx)
        {
            var innerException = dbEx.InnerException?.InnerException ?? dbEx.InnerException;

            if (innerException != null)
            {
                if (innerException.Message.Contains("FK_"))
                {
                    ModelState.AddModelError("", "No se puede realizar esta acción porque hay registros relacionados.");
                }
                else if (innerException.Message.Contains("IX_") || innerException.Message.Contains("UNIQUE"))
                {
                    ModelState.AddModelError("", "Ya existe un registro con estos valores (violación de restricción única).");
                }
                else
                {
                    ModelState.AddModelError("", $"Error de base de datos: {innerException.Message}");
                }
            }
            else
            {
                ModelState.AddModelError("", $"Error de base de datos: {dbEx.Message}");
            }
        }

        // Método para registrar detalles del error 
        private void LogExceptionDetails(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }

        #endregion
    }
}
