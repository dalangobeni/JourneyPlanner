using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ibi.JourneyPlanner.Web.Extensions
{
    using Ibi.JourneyPlanner.Web.Models;

    using OsmSharp.Tools.Math.Geo;

    public static class PointToPointModelExtensions
    {
        public static GeoCoordinate ToStartGeoCoordinate(this PointToPointModel pointToPointModel)
        {
            return new GeoCoordinate(pointToPointModel.FromLatitude, pointToPointModel.FromLongitude);
        }

        public static GeoCoordinate ToEndCoordinate(this PointToPointModel pointToPointModel)
        {
            return new GeoCoordinate(pointToPointModel.ToLatitude, pointToPointModel.ToLongitude);
        }
    }
}