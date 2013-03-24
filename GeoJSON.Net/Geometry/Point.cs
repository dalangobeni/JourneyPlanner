﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Point.cs" company="Jörg Battermann">
//   Copyright © Jörg Battermann 2011
// </copyright>
// <summary>
//   Defines the Point type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GeoJSON.Net.Geometry
{
    using System;
    using System.Collections.Generic;

    using GeoJSON.Net.Converters;

    using Newtonsoft.Json;

    /// <summary>
    /// In geography, a point refers to a Position on a map, expressed in latitude and longitude.
    /// </summary>
    /// <seealso cref="http://geojson.org/geojson-spec.html#point"/>
    public class Point : GeoJSONObject, IGeometryObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> class.
        /// </summary>
        /// <param name="coordinates">The Position.</param>
        public Point(GeographicPosition coordinates)
        {
            if (coordinates == null)
            {
                throw new ArgumentNullException("coordinates");
            }

            this.Coordinates = new List<GeographicPosition> { coordinates };
            this.Type = GeoJSONObjectType.Point;
        }

        /// <summary>
        /// Gets the Coordinate(s).
        /// </summary>
        /// <value>The Coordinates.</value>
        [JsonProperty(PropertyName = "coordinates", Required = Required.Always)]
        [JsonConverter(typeof(PositionConverter))]
        public List<GeographicPosition> Coordinates { get; private set; }
    }
}
