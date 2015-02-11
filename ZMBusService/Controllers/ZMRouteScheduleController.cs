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
    public class ZMRouteScheduleController : Controller
    {
        private BusServiceSQLContext db = new BusServiceSQLContext();

        /// <summary>
        /// Prepares the list of routeSchedules from busRouteCode
        /// </summary>
        /// <param name="busRouteCode">busRouteCode</param>
        /// <returns>View along with the list of routeSchedule corresponding to busRouteCode </returns>
        public ActionResult RouteStopSchedule(string busRouteCode)
        {
            if (busRouteCode != null)
           {
               //if no route schedule found corresponding to busRouteCode
               if (db.routeSchedules.Where(a=>a.busRouteCode==busRouteCode).ToList().Count <= 0)
               {
                   TempData["message"] = "No route schedule found for selected route stop.";
                   TempData["messageType"] = "danger";
                   return RedirectToAction("RouteSelector", "ZMBusStop");
               }
               else 
              {
                  //Storing busRouteCode and routename in viewBag to display on the view
                  ViewBag.busRouteCode = busRouteCode;
                  ViewBag.routeName = db.busRoutes.Find(busRouteCode).routeName;
                  int busStopNumber = int.Parse(Session["busStopId"].ToString());
                  var query = from rs in db.routeSchedules
                              join rss in db.routeStops
                              on rs.busRouteCode equals rss.busRouteCode
                              where (rss.busRouteCode == busRouteCode && rss.busStopNumber==busStopNumber)
                              select (new RouteScheduleVM
                              {
                                  startTime = rs.startTime,
                                  offset= rss.offsetMinutes,
                                  isWeekDay=rs.isWeekDay

                              });
                  return View(query.OrderBy(a=>a.startTime));
               }
               
           }
           //if no busRouteCode passed from URL
           else
           {
               TempData["message"] = "Please select/provide the correct route stop";
               TempData["messageType"] = "danger";
               return RedirectToAction("Index", "ZMBusStop");
           }
          
           
        }
        /// <summary>
        /// Passes list of all route schedule to the view
        /// </summary>
        /// <returns>View along with list of route shcedule</returns>
        public ActionResult Index()
        {
            var routeSchedules = db.routeSchedules.Include(r => r.busRoute);
            return View(routeSchedules.ToList());
        }

        /// <summary>
        /// Passes the complete route schedule record to the view
        /// </summary>
        /// <param name="id">id of requested route schedule</param>
        /// <returns>View along with route schedule model</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            return View(routeSchedule);
        }

        /// <summary>
        ///Renders view for creating route schedule
        /// </summary>
        /// <returns>Create route schedule view</returns>
        public ActionResult Create()
        {
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName");
            return View();
        }

        /// <summary>
        /// Creates route schedule record from posted data from create route schedule view
        /// </summary>
        /// <param name="category">route schedule model received from view</param>
        /// <returns>View along with same saved route schedule model</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "routeScheduleId,busRouteCode,startTime,isWeekDay,comments")] routeSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                db.routeSchedules.Add(routeSchedule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        /// <summary>
        /// Renders view for editing the route schedule record having specific id
        /// </summary>
        /// <param name="id">routeSchedule Id</param>
        /// <returns>View along with routeSchedule model</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        /// <summary>
        /// Saves the edited route schedule record
        /// </summary>
        /// <param name="category">routeSchedule model</param>
        /// <returns>View along in with routeSchedule model</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "routeScheduleId,busRouteCode,startTime,isWeekDay,comments")] routeSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(routeSchedule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        /// <summary>
        /// Renders view with detail route schedule to confirm deletion
        /// </summary>
        /// <param name="id">routeSchedule Id</param>
        /// <returns>routeSchedule model</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            return View(routeSchedule);
        }

        /// <summary>
        /// Delets the route schedule record
        /// </summary>
        /// <param name="id">routeSchedule Id</param>
        /// <returns>Redirects to home page</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            db.routeSchedules.Remove(routeSchedule);
            db.SaveChanges();
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
