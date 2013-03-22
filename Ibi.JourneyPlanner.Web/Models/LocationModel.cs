using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ibi.JourneyPlanner.Web.Models
{
    public class LocationModel
    {
        public LocationModel(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}