using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZMBusService.Models;
using System.Data.Entity;

namespace ZMBusService.Controllers
{
    public class ZMTripController : Controller
    {
        private BusServiceSQLContext db = new BusServiceSQLContext();

        /// <summary>
        /// Passes the list of trips for the selected bus route
        /// </summary>
        /// <param name="routeCode">bus route of the requested trips</param>
        /// <param name="routeName">route name of the requested trips</param>
        /// <returns>View along with list of trips</returns>
        public ActionResult Index(string routeCode, string routeName)
        {

            if (routeCode == null || routeName == null)
            {
                if (Session["routeCode"] == null && Session["routeName"] == null)
                {
                    TempData["message"] = "Please select the monitored trip link for the route";
                    TempData["messageType"] = "danger";
                    return RedirectToAction("Index", "ZMBusRoute");
                }

            }
            else
            {
                Session["routeCode"] = routeCode;
                Session["routeName"] = routeName;
            }
            string sRouteCode = Session["routeCode"].ToString();
            var query = from tr in db.trips
                        join dr in db.drivers
                        on tr.driverId equals dr.driverId
                        join bs in db.buses
                        on tr.busId equals bs.busId
                        join rs in db.routeSchedules
                        on tr.routeScheduleId equals rs.routeScheduleId
                        where (rs.busRouteCode == sRouteCode)
                        orderby tr.tripDate descending, rs.startTime
                        select (new BusTripVM { tripDate = tr.tripDate, startTime = rs.startTime, driverFullName = dr.fullName, busNumber = bs.busNumber, comments = tr.comments });
            return View(query);


        }
        /// <summary>
        ///Renders view for creating trip for the selected bus route
        /// </summary>
        /// <returns>Create trip view</returns>
        public ActionResult Create()
        {
            if (Session["routeCode"] == null && Session["routeName"] == null)//checking if user is following the correct navigation path to create the trip. There chances that user might come directly on create page using link
            {
                TempData["message"] = "Please select the monitored trip link for the route and then click on create link";
                TempData["messageType"] = "danger";
                return RedirectToAction("Index", "ZMBusRoute");
            }
            string sRouteCode = Session["routeCode"].ToString();
            ViewBag.routeSchedule = new SelectList(db.routeSchedules.OrderBy(a => a.startTime).Where(a => a.busRouteCode == sRouteCode), "routeScheduleId", "startTime");
            ViewBag.driver = new SelectList(db.drivers.OrderBy(a => a.fullName), "driverId", "fullName");
            ViewBag.bus = db.buses.OrderBy(a => a.busNumber).Where(a => a.status == "available");
            return View();
        }

        /// <summary>
        /// Creates trip record from posted data from trip view
        /// </summary>
        /// <param name="trip">create model received from view</param>
        /// <returns>View along with same saved trip model</returns>
        [HttpPost]
        public ActionResult Create([Bind(Include = "tripId,routeScheduleId,tripDate,driverId,busId, comments")] trip trip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.trips.Add(trip);
                    db.SaveChanges();
                    TempData["message"] = "Bus trip created succesfully";
                    TempData["messageType"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.GetBaseException().ToString()); //getting the base exception and adding to model state error
                }

            }
            string sRouteCode = Session["routeCode"].ToString();
            ViewBag.routeSchedule = new SelectList(db.routeSchedules.OrderBy(a => a.startTime).Where(a => a.busRouteCode == sRouteCode), "routeScheduleId", "startTime");//dropdown for start times
            ViewBag.driver = new SelectList(db.drivers, "driverId", "fullName"); //dropdown for drivers
            ViewBag.bus = db.buses.OrderBy(a => a.busNumber).Where(a => a.status == "available"); //list for creating radio buttons bus number
            return View(trip);

        }
    }
}