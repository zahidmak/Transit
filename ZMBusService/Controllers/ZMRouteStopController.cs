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
    public class ZMRouteStopController : Controller
    {
        private BusServiceSQLContext db = new BusServiceSQLContext();

        /// <summary>
        /// Passes the list of routestops
        /// </summary>
        /// <param name="routeName">routeName of request routeStops</param>
        /// <param name="routeCode">routeCode of request routeStops</param>
        /// <returns>View along with list of routeStops</returns>
        public ActionResult Index(string routeName, int routeCode = 0)
        {
            if (routeCode != 0 && routeName != null)
            {
                Session["routeCode"] = routeCode;
                Session["routeName"] = routeName;
                string tempRouteCode = Session["routeCode"].ToString();
                var routeStops = db.routeStops.Where(a => a.busRouteCode == tempRouteCode).Include(r => r.busRoute).Include(r => r.busStop).OrderBy(a=>a.offsetMinutes);
                return View(routeStops.ToList());
            }
            else
            {
                if (Session["routeCode"] != null)
                {
                    string tempRouteCode = Session["routeCode"].ToString();
                    var routeStops = db.routeStops.Where(a => a.busRouteCode == tempRouteCode).Include(r => r.busRoute).Include(r => r.busStop).OrderBy(a=>a.offsetMinutes); ;
                    return View(routeStops.ToList());
                }
                else
                {
                    TempData["message"] = "Please select the Show Route corresponding to bus";
                    TempData["messageType"] = "danger";

                    return RedirectToAction("Index", "ZMBusRoute");
                }
            }


        }

        /// <summary>
        /// Passes the complete routeStop model
        /// </summary>
        /// <param name="id">id of the request routeStop</param>
        /// <returns>View along with routeStop model</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routestop = db.routeStops.Find(id);
            if (routestop == null)
            {
                return HttpNotFound();
            }
            return View(routestop);
        }

        /// <summary>
        /// Renders create routeStop page
        /// </summary>
        /// <returns>Create routestop view</returns>
        public ActionResult Create()
        {
           // ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName");
            if (Session["routeCode"] != null)
            {
                ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location");
                return View();
            }
            else
            {
                TempData["message"] = "Please select the Show Route corresponding to bus";
                TempData["messageType"] = "danger";

                return RedirectToAction("Index", "ZMBusRoute");
            }
           
        }

        /// <summary>
        /// Saves the post back response from the user for create a new route stop
        /// </summary>
        /// <param name="routestop">routeStop model</param>
        /// <returns>View along with routeStop model</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="routeStopId,busRouteCode,busStopNumber,offsetMinutes")] routeStop routestop)
        {
            if (ModelState.IsValid)
            {
                routestop.busRouteCode = Session["routeCode"].ToString();
                db.routeStops.Add(routestop);
                db.SaveChanges();
                TempData["message"] = "Record created successfully";
                TempData["messageType"] = "success";
                return RedirectToAction("Index");
            }

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routestop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routestop.busStopNumber);
            return View(routestop);
        }

        /// <summary>
        /// Renders edit routeStop page
        /// </summary>
        /// <param name="id">id of the request routestop</param>
        /// <returns>View along with route stop model</returns>
        public ActionResult Edit(int? id)
        {
           
            if (Session["routeCode"] != null)
            {
                routeStop routestop = db.routeStops.Find(id);
                ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routestop.busRouteCode);
                ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routestop.busStopNumber);
                return View(routestop);
            }
            else
            {
                TempData["message"] = "Please select the Show Route corresponding to bus";
                TempData["messageType"] = "danger";

                return RedirectToAction("Index", "ZMBusRoute");
            }
            
          
           
        }

        /// <summary>
        /// Saves the post back response from the user for edit route stop
        /// </summary>
        /// <param name="routestop">routeStop model</param>
        /// <returns>View along with routeStop model</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="routeStopId,busRouteCode,busStopNumber,offsetMinutes")] routeStop routestop)
        {
            if (ModelState.IsValid)
            {
                routestop.busRouteCode = Session["routeCode"].ToString();
                db.Entry(routestop).State = EntityState.Modified;
                db.SaveChanges();
                TempData["message"] = "Record updated successfully";
                TempData["messageType"] = "success";
                return RedirectToAction("Index");
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routestop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routestop.busStopNumber);
            return View(routestop);
        }

        /// <summary>
        /// Renders delete routeStop page
        /// </summary>
        /// <param name="id">id of the requested routestop for delete</param>
        /// <returns>View along with routeStop model</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            } 
            routeStop routestop = db.routeStops.Find(id);
            if (routestop == null)
            {
                return HttpNotFound();
            }
            return View(routestop);
        }

        /// <summary>
        /// Deletes the routeStop upon confirmation from the user
        /// </summary>
        /// <param name="id">id of the requested routeStop for delete</param>
        /// <returns>Redirects to Index after delete</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            routeStop routestop = db.routeStops.Find(id);
            db.routeStops.Remove(routestop);
            db.SaveChanges();
            TempData["message"] = "Record deleted successfully";
            TempData["messageType"] = "success";
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
