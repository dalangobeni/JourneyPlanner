using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ibi.JourneyPlanner.Web.Models.LiveData
{
    public class MetroShuttle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Route { get; set; }
        public string Registration { get; set; }
        public string Reading { get; set; }
        public bool HasFix { get; set; }
        public bool IsParked { get; set; }
        public DateTime LastUpdated { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
