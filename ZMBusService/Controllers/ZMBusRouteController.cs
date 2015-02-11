using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZMBusService.Models;

namespace ZMBusService.Controllers
{
    public class ZMBusRouteController : Controller
    {
        private BusServiceSQLContext db = new BusServiceSQLContext();

        /// <summary>
        /// Passes list of all bus routes to the view
        /// </summary>
        /// <returns>View along with list of bus routes</returns>
        public ActionResult Index()
        {
            return View(db.busRoutes.ToList());
        }

        /// <summary>
        /// Passes the complete bus route record to the view
        /// </summary>
        /// <param name="id">id of requested bus route</param>
        /// <returns>View along with bus route model</returns>
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        /// <summary>
        ///Renders view for creating bus route
        /// </summary>
        /// <returns>Create bus route view</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates bus route record from posted data from create bus route view
        /// </summary>
        /// <param name="category">bus route model received from view</param>
        /// <returns>View along with same saved bus route model</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "busRouteCode,routeName")] busRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                db.busRoutes.Add(busRoute);
                db.SaveChanges();
                TempData["message"] = "Record created successfully";
                TempData["messageType"] = "success";
                return RedirectToAction("Index");
            }

            return View(busRoute);
        }

        /// <summary>
        /// Renders view for editing the busroute record having specific id
        /// </summary>
        /// <param name="id">busRoute Id</param>
        /// <returns>View along with bus route model</returns>
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        /// <summary>
        /// Saves the edited bus route record
        /// </summary>
        /// <param name="category">BusRoute model</param>
        /// <returns>View along in with busroute model</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "busRouteCode,routeName")] busRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(busRoute).State = EntityState.Modified;
                db.SaveChanges();
                TempData["message"] = "Record updated successfully";
                TempData["messageType"] = "success";
                return RedirectToAction("Index");
            }
            return View(busRoute);
        }

        /// <summary>
        /// Renders view with detail bus route to confirm deletion
        /// </summary>
        /// <param name="id">busroute Id</param>
        /// <returns>BusRoute model</returns>
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        /// <summary>
        /// Delets the bus route record
        /// </summary>
        /// <param name="id">busroute Id</param>
        /// <returns>Redirects to home page</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                busRoute busRoute = db.busRoutes.Find(id);
                db.busRoutes.Remove(busRoute);
                db.SaveChanges();
                TempData["message"] = "Record deleted successfully";
                TempData["messageType"] = "success";

            }
            catch(Exception ex)
            {
              TempData["message"]= ex.GetBaseException().Message.ToString();
              TempData["messageType"] = "danger";
            }
           
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Disposes the db context to free up the memory and closes connection with database
        /// </summary>
        /// <param name="disposing"></param>
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
