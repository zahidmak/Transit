using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZMBusService.Models
{
    public class RouteScheduleVM
    {
        public System.TimeSpan startTime { get; set; }
        public Nullable<int> offset { get; set; }
        public bool isWeekDay { get; set; }
    }
}