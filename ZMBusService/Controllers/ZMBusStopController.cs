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
    public class ZMBusStopController : Controller
    {
        private BusServiceSQLContext db = new BusServiceSQLContext();

        /// <summary>
        /// Passes list of all bus stops to the view
        /// </summary>
        /// <returns>View along with list of bus stops</returns>
        public ActionResult Index()
        {
            return View(db.busStops.ToList().OrderBy(a => a.location));
        }

        /// <summary>
        /// Passes the complete bus stop record to the view
        /// </summary>
        /// <param name="id">id of requested bus stop</param>
        /// <returns>View along with bus stop model</returns>
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        /// <summary>
        ///Renders view for creating bus stop
        /// </summary>
        /// <returns>Create bus stop view</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates bus stop record from posted data from create bus stop view
        /// </summary>
        /// <param name="category">bus stop model received from view</param>
        /// <returns>View along with same saved bus stop model</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "busStopNumber,location,locationHash,goingDowntown")] busStop busStop)
        {
            if (ModelState.IsValid)
            {
                db.busStops.Add(busStop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(busStop);
        }

        /// <summary>
        /// Renders view for editing the bus stop record having specific id
        /// </summary>
        /// <param name="id">busStop Id</param>
        /// <returns>View along with bus stop model</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        /// <summary>
        /// Saves the edited bus stop record
        /// </summary>
        /// <param name="category">BusStop model</param>
        /// <returns>View along in with busStop model</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "busStopNumber,location,locationHash,goingDowntown")] busStop busStop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(busStop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(busStop);
        }

        /// <summary>
        /// Renders view with detail bus stop to confirm deletion
        /// </summary>
        /// <param name="id">busStop Id</param>
        /// <returns>BusStop model</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        /// <summary>
        /// Delets the bus stop record
        /// </summary>
        /// <param name="id">busStop Id</param>
        /// <returns>Redirects to home page</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            busStop busStop = db.busStops.Find(id);
            db.busStops.Remove(busStop);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// redirects or view the options to display the route schedule by redirecting to the RouteStopSchedule method of ZMRouteSchedule controller.
        /// </summary>
        /// <param name="location">location of the bus stop</param>
        /// <param name="busStopNumber">busStopNumber</param>
        /// <returns></returns>
        public ActionResult RouteSelector(string location,int busStopNumber = 0 )
        {

            //Checking if the busStopNumber exists
            busStop busStop = db.busStops.Find(busStopNumber);
            if (busStop != null)
            {
                //query joins routestopsm busroutes and busstops table on busStop number to find the busRouteCode for particular bus stop.
                var query = from rs in db.routeStops
                             join br in db.busRoutes
                                  on rs.busRouteCode equals br.busRouteCode
                             join bs in db.busStops
                                  on rs.busStopNumber equals bs.busStopNumber
                             where (rs.busStopNumber == busStopNumber)
                             select new { 
                                          routeStopId = rs.routeStopId,
                                          busStopNumber = (Int32)rs.busStopNumber,
                                          Location = bs.location,
                                          busRouteCode = rs.busRouteCode,
                                          routeName = br.routeName 
                             };
                //if there are no routes found at the busStop 
                if (query.ToList().Count <= 0)
                {
                    TempData["message"] = "No route stops found for " + busStopNumber.ToString() + " bus stop of " + location.ToString();
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index");
                }
                //if bus routes found at the busStop
                else
                {
                    //storing the busStopId and location to reuse later in the application
                    Session["busStopId"] = busStopNumber.ToString();
                    Session["location"] = location.ToString();
                    //if only one route stop is found the directly redirect to route schedule
                    if(query.ToList().Count==1)
                    {
                        return RedirectToAction("RouteStopSchedule", "ZMRouteSchedule", new { busRouteCode = query.ToList()[0].busRouteCode});
                    }
                    // else make them select one before redirecting to route schedule
                    else
                    {
                        //preparing dropdown list in case of multiple routes at the busStop
                        List< SelectListItem> selectListItem= new List<SelectListItem>();
                        foreach(var a in query.ToList())
                        {
                            selectListItem.Add(new SelectListItem{Text=a.routeName, Value= a.busRouteCode.ToString()});
                   
                        }
                        ViewBag.busRouteCode = selectListItem;
                        return View();
                    }
                   
                 
                }
            }
            //if busStopNumber doesnot exists in the database. Meaning user is passing wrong busStopNumber directly from the URL
            else
            {
                TempData["message"] = "Please select/provide the correct bus stop number";
                TempData["messageType"] = "danger";
                return RedirectToAction("Index");
            }



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
