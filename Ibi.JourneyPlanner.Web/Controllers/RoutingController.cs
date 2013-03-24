// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingController.cs" company="IBI Group">
//   (c) Copyright IBI Group. Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// </copyright>
// <summary>
//   Defines the RoutingController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Ibi.JourneyPlanner.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using GeoJSON.Net;
    using GeoJSON.Net.Feature;
    using GeoJSON.Net.Geometry;

    using Ibi.JourneyPlanner.Web.Code;
    using Ibi.JourneyPlanner.Web.Extensions;
    using Ibi.JourneyPlanner.Web.Models;
    using Ibi.JourneyPlanner.Web.Models.Exceptions;

    using OsmSharp.Routing.Core;
    using OsmSharp.Tools.Math.Geo;

    public class RoutingController : ApiController
    {
        public Dictionary<string, string> GetTransportModes()
        {
            var values = new Dictionary<string, string>
                             {
                                 // { "Cycling", VehicleEnum.Bicycle.ToString() },
                                 { "Driving", VehicleEnum.Car.ToString() },
                                 { "Walking", VehicleEnum.Pedestrian.ToString() },
                                 // { "Bus", VehicleEnum.Bus.ToString() },
                             };

            return values;
        }

        [HttpPost]
        public LocationModel GetClosestPointTo(ResolvePointModel resolvePointModel)
        {
            var router = Engine.Instance;
            var transportMode = this.ResolveVehicleEnum(resolvePointModel.TransportMode);

            if (!router.SupportsTransportMode(transportMode))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Tranport mode not supported."),
                    ReasonPhrase = string.Format("Transport mode {0} is not supported.", transportMode.ToString())
                });
            }

            var point = router.GetNearestPointTo(transportMode, resolvePointModel.Latitude, resolvePointModel.Longitude);
            if (point == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Point cannot be mapped"),
                    ReasonPhrase = string.Format("There are no viable positions near this point.")
                });
            }

            return point;
        }

        [HttpPost]
        public ResultSet PointToPoint(PointToPointModel pointToPointModel)
        {
            var router = Engine.Instance;

            // Get transport mode
            var transportMode = this.ResolveVehicleEnum(pointToPointModel.TransportMode);

            if (!router.SupportsTransportMode(transportMode))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Tranport mode not supported."),
                    ReasonPhrase = string.Format("Transport mode {0} is not supported.", transportMode.ToString())
                });
            }

            // resolve both points; find the closest routable point.
            try
            {
                var route = router.CalculatePointToPoint(
                    transportMode, pointToPointModel.ToStartGeoCoordinate(), pointToPointModel.ToEndCoordinate());

                return route.Results;
            }
            catch (RoutingException e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Routing error"),
                    ReasonPhrase = e.Message
                });
            }
        }

        private VehicleEnum ResolveVehicleEnum(string transportMode, VehicleEnum defaultType = VehicleEnum.Car)
        {
            VehicleEnum mode;
            if (!Enum.TryParse(transportMode, true, out mode))
            {
                mode = defaultType;
            }

            return mode;
        }
    }
}
