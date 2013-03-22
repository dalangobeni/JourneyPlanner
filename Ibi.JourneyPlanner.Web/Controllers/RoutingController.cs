using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Ibi.JourneyPlanner.Web.Controllers
{
    using Ibi.JourneyPlanner.Web.Code;

    using OsmSharp.Routing.Core;
    using OsmSharp.Tools.Math.Geo;
    using OsmSharp.Tools.Xml.Kml.v2_0;

    public class RoutingController : ApiController
    {
        public object Get()
        {
            var router = Engine.Instance;

            // resolve both points; find the closest routable road.
            RouterPoint point1 = router.Resolve(VehicleEnum.Car, new GeoCoordinate(53.4866, -2.2447));
            RouterPoint point2 = router.Resolve(VehicleEnum.Car, new GeoCoordinate(53.4732, -2.2540));

            // calculate route.
            var route = router.Calculate(VehicleEnum.Car, point1, point2);

            //var coordinates = route.Entries
            //    .Select(x => new GeographicPosition(x.Latitude, x.Longitude))
            //    .ToList();

            //var lineString = new LineString(new List<IPosition>(coordinates));

            //var feature = new Feature(
            //    lineString,
            //    new Dictionary<string, object>
            //        {
            //            { "name", "Test route result." }
            //        });

            //return new GeoResult(feature);

            return null;
        }
    }
}
