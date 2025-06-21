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
    public class NominasController : Controller
    {
        // Conexión bd
        private smartbuilding_rhEntities db = new smartbuilding_rhEntities();

        // Muestra la lista de todas las nóminas registradas
        public ActionResult Index()
        {
            var nomina = db.Nomina.Include(n => n.Empleados).Include(n => n.ISR1);
            return View(nomina.ToList());
        }

        // Muestra los detalles de una nómina en sí
        public ActionResult Details(int? id)
        {
            // Verifica si no se recibió un ID
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca la nómina con el ID indicado
            Nomina nomina = db.Nomina.Find(id);

            // Si no se encuentra, muestra error
            if (nomina == null)
            {
                return HttpNotFound();
            }

            // Muestra la información de la nómina
            return View(nomina);
        }

        // Muestra el formulario para crear una nueva nómina
        public ActionResult Create()
        {
            // Llena de empleados
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula");

            // Llena de tipos de ISR
            ViewBag.id_isr = new SelectList(db.ISR, "id_isr", "descripcion");

            return View();
        }

        // Procesa los datos del formulario para crear una nueva nómina
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_nomina,id_empleado,id_isr,mes,anio,salario_bruto,horas_extras,salario_dias_feriados,ccss,ivm,isr,credito_hijos,credito_conyuge,otros_descuentos,base_imponible,salario_neto,fecha_creacion,fecha_actualizacion")] Nomina nomina)
        {
            // Verifica si los datos son válidos
            if (ModelState.IsValid)
            {
                // Agrega la nueva nómina a la bd
                db.Nomina.Add(nomina);
                db.SaveChanges();

                // Redirige a la lista de nóminas
                return RedirectToAction("Index");
            }

            // Si hubo error, vuelve a llenar los combos y muestra el formulario
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", nomina.id_empleado);
            ViewBag.id_isr = new SelectList(db.ISR, "id_isr", "descripcion", nomina.id_isr);
            return View(nomina);
        }

        // Muestra el formulario para editar una nómina existente
        public ActionResult Edit(int? id)
        {
            // Verifica si no se recibió un ID
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca la nómina con el ID indicado
            Nomina nomina = db.Nomina.Find(id);

            // Si no se encuentra, muestra error
            if (nomina == null)
            {
                return HttpNotFound();
            }

            // Llena los combos con la información actual
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", nomina.id_empleado);
            ViewBag.id_isr = new SelectList(db.ISR, "id_isr", "descripcion", nomina.id_isr);
            return View(nomina);
        }

        // Procesa los datos del formulario para guardar los cambios en la nómina
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_nomina,id_empleado,id_isr,mes,anio,salario_bruto,horas_extras,salario_dias_feriados,ccss,ivm,isr,credito_hijos,credito_conyuge,otros_descuentos,base_imponible,salario_neto,fecha_creacion,fecha_actualizacion")] Nomina nomina)
        {
            // Verifica si los datos son válidos
            if (ModelState.IsValid)
            {
                // Marca la nómina como modificada
                db.Entry(nomina).State = EntityState.Modified;
                db.SaveChanges();

                // Redirige a la lista de nóminas
                return RedirectToAction("Index");
            }

            // Si hubo error, vuelve a llenar los combos y muestra el formulario
            ViewBag.id_empleado = new SelectList(db.Empleados, "id_empleado", "cedula", nomina.id_empleado);
            ViewBag.id_isr = new SelectList(db.ISR, "id_isr", "descripcion", nomina.id_isr);
            return View(nomina);
        }

        // Muestra la confirmación para eliminar una nómina
        public ActionResult Delete(int? id)
        {
            // Verifica si no se recibió un ID
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Busca la nómina con el ID indicado
            Nomina nomina = db.Nomina.Find(id);

            // Si no se encuentra, muestra error
            if (nomina == null)
            {
                return HttpNotFound();
            }

            // Muestra la confirmación para eliminar
            return View(nomina);
        }

        // Procesa la eliminación de la nómina
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Busca y elimina la nómina
            Nomina nomina = db.Nomina.Find(id);
            db.Nomina.Remove(nomina);
            db.SaveChanges();

            // Redirige a la lista de nóminas
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
