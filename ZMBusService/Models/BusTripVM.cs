using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZMBusService.Models
{
    public class BusTripVM
    {
        [Display(Name = "Date")]
        public System.DateTime tripDate { get; set; }
        [Display(Name = "Start Time")]
        public System.TimeSpan startTime { get; set; }
        [Display(Name = "Driver")]
        public string driverFullName { get; set; }
        [Display(Name = "Bus #")]
        public int busNumber { get; set; }
        [Display(Name = "Comments")]
        public string comments { get; set; }

    }
}