using System; // Importa funcionalidades básicas del sistema
using System.Collections.Generic; // Permite usar listas y colecciones
using System.Data; // Funciona con datos y tablas
using System.Data.Entity; // Permite trabajar con bases de datos usando Entity Framework
using System.Linq; // Permite usar consultas sobre listas y datos
using System.Net; // Permite manejar respuestas HTTP como errores
using System.Web; // Funcionalidades web básicas
using System.Web.Mvc; // Permite usar controladores y vistas en MVC
using SistemaNomina; // Usa los modelos definidos en el proyecto

namespace SistemaNomina.Controllers
{
    public class SolicitudesVacacionesController : Controller
    {
        // Conexión bd
        private smartbuilding_rhEntities db = new smartbuilding_rhEntities();

        // Muestra la lista de todas las solicitudes de vacaciones
        public ActionResult Index()
        {


            // Trae las solicitudes junto con sus estados, usuarios y datos de vacaciones
            var solicitudesVacaciones = db.SolicitudesVacaciones.Include(s => s.Estados).Include(s => s.Usuarios).Include(s => s.Vacaciones);
            return View(solicitudesVacaciones.ToList());
        }

        public ActionResult SolicitudesPendientes()
        {
            // Filtrar solicitudes que están pendientes de aprobación
            var solicitudesPendientes = db.SolicitudesVacaciones
                .Include(s => s.Estados)
                .Include(s => s.Usuarios)
                .Include(s => s.Vacaciones)
                .Where(s => s.id_estado == 101);

            return View(solicitudesPendientes.ToList());
        }
        // Muestra el formulario para aprobar una solicitud
        public ActionResult Aprobar(int id)
        {
            var solicitud = db.SolicitudesVacaciones.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }

            // Cambiar el estado a "Aprobado"
            solicitud.id_estado = 102;
            solicitud.fecha_aprobacion = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("SolicitudesPendientes");
        }

        // Muestra el formulario para rechazar una solicitud
        public ActionResult Rechazar(int id)
        {
            var solicitud = db.SolicitudesVacaciones.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }

            // Cambiar el estado a "rechazado"
            solicitud.id_estado = 103;
            solicitud.fecha_aprobacion = DateTime.Now;
            db.SaveChanges();

            return RedirectToAction("SolicitudesPendientes");
        }


        // Muestra los detalles de una solicitud específica
        public ActionResult Details(int? id)
        {
            // Si no se manda un ID, da un error de solicitud incorrecta
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca la solicitud con ese ID
            SolicitudesVacaciones solicitudesVacaciones = db.SolicitudesVacaciones.Find(id);

            // Si no la encuentra, devuelve error 
            if (solicitudesVacaciones == null)
            {
                return HttpNotFound();
            }

            // Muestra los detalles de la solicitud
            return View(solicitudesVacaciones);
        }

        // Muestra el formulario para crear una nueva solicitud
        public ActionResult Create()
        {
            // Llenar los combos desplegables con los datos necesarios
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "nombre");
            ViewBag.aprobado_por = new SelectList(db.Usuarios, "id_usuario", "usuario");
            ViewBag.id_vacacion = new SelectList(db.Vacaciones, "id_vacacion", "periodo");
            return View();
        }

        // Procesa la información enviada al crear una solicitud
        [HttpPost]
        [ValidateAntiForgeryToken] // Seguridad
        public ActionResult Create([Bind(Include = "id_solicitud,id_vacacion,fecha_inicio,fecha_fin,fecha_solicitud,fecha_aprobacion,aprobado_por,comentario_solicitud,comentario_respuesta,id_estado,fecha_creacion,fecha_actualizacion")] SolicitudesVacaciones solicitudesVacaciones)
        {
            // Verifica si los datos del formulario son válidos
            if (ModelState.IsValid)
            {
                // Agrega la nueva solicitud a la bd
                db.SolicitudesVacaciones.Add(solicitudesVacaciones);
                db.SaveChanges(); // Guarda los cambios
                return RedirectToAction("Index"); // Redirige al listado
            }

            // Si hubo errores, recarga los combos desplegables
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "nombre", solicitudesVacaciones.id_estado);
            ViewBag.aprobado_por = new SelectList(db.Usuarios, "id_usuario", "usuario", solicitudesVacaciones.aprobado_por);
            ViewBag.id_vacacion = new SelectList(db.Vacaciones, "id_vacacion", "periodo", solicitudesVacaciones.id_vacacion);
            return View(solicitudesVacaciones);
        }

        // Muestra el formulario para editar una solicitud
        public ActionResult Edit(int? id)
        {
            // Si no se manda ID, devuelve error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca la solicitud
            SolicitudesVacaciones solicitudesVacaciones = db.SolicitudesVacaciones.Find(id);

            // Si no existe, muestra error
            if (solicitudesVacaciones == null)
            {
                return HttpNotFound();
            }

            // Llenar los combos desplegables con los datos actuales
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "nombre", solicitudesVacaciones.id_estado);
            ViewBag.aprobado_por = new SelectList(db.Usuarios, "id_usuario", "usuario", solicitudesVacaciones.aprobado_por);
            ViewBag.id_vacacion = new SelectList(db.Vacaciones, "id_vacacion", "periodo", solicitudesVacaciones.id_vacacion);
            return View(solicitudesVacaciones);
        }

        // Procesa la información al editar una solicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_solicitud,id_vacacion,fecha_inicio,fecha_fin,fecha_solicitud,fecha_aprobacion,aprobado_por,comentario_solicitud,comentario_respuesta,id_estado,fecha_creacion,fecha_actualizacion")] SolicitudesVacaciones solicitudesVacaciones)
        {
            // Verifica si los datos son válidos
            if (ModelState.IsValid)
            {
                // Marca la solicitud como modificada
                db.Entry(solicitudesVacaciones).State = EntityState.Modified;
                db.SaveChanges(); // Guarda los cambios
                return RedirectToAction("Index");
            }

            // Si hubo errores, recarga los combos
            ViewBag.id_estado = new SelectList(db.Estados, "id_estado", "nombre", solicitudesVacaciones.id_estado);
            ViewBag.aprobado_por = new SelectList(db.Usuarios, "id_usuario", "usuario", solicitudesVacaciones.aprobado_por);
            ViewBag.id_vacacion = new SelectList(db.Vacaciones, "id_vacacion", "periodo", solicitudesVacaciones.id_vacacion);
            return View(solicitudesVacaciones);
        }

        // Muestra la confirmación para eliminar una solicitud
        public ActionResult Delete(int? id)
        {
            // Si no hay ID, devuelve error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca la solicitud
            SolicitudesVacaciones solicitudesVacaciones = db.SolicitudesVacaciones.Find(id);

            // Si no existe, muestra error
            if (solicitudesVacaciones == null)
            {
                return HttpNotFound();
            }

            // Muestra la vista de confirmación
            return View(solicitudesVacaciones);
        }

        // Confirma y elimina una solicitud
        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Busca la solicitud
            SolicitudesVacaciones solicitudesVacaciones = db.SolicitudesVacaciones.Find(id);

            // La elimina de la bd
            db.SolicitudesVacaciones.Remove(solicitudesVacaciones);
            db.SaveChanges(); // Guarda los cambios
            return RedirectToAction("Index"); // Vuelve al listado
        }

        // Libera los recursos usados 
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
