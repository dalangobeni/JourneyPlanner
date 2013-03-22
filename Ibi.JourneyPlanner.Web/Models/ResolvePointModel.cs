using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ibi.JourneyPlanner.Web.Models
{
    public class ResolvePointModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string TransportMode { get; set; }
    }
}