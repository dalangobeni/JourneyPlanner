using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ibi.JourneyPlanner.Web.Models.LiveData
{
    public class CarPark
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public DateTime LastUpdated { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string SCN { get; set; }
        public int Capacity { get; set; }
        public int SpacesNow { get; set; }
        public int PredictedSpaces60Mins { get; set; }
        public int PredictedSpaces30Mins { get; set; }
    }
}