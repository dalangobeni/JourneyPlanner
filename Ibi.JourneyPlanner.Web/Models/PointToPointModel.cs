using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ibi.JourneyPlanner.Web.Models
{
    public class PointToPointModel
    {
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }

        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }
    }
}