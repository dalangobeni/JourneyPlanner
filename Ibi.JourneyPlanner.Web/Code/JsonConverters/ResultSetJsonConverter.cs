using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ibi.JourneyPlanner.Web.Code.JsonConverters
{
    using Ibi.JourneyPlanner.Web.Models;

    using Newtonsoft.Json;

    public class ResultSetJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ResultSet)
            {
                var resultSet = value as ResultSet;

                writer.WriteStartObject();
                writer.WritePropertyName("results");
                writer.WriteStartArray();

                var featureArray = resultSet.Results.Select(x => x.ToGeoJson()).ToArray();
                writer.WriteRaw(string.Join(", ", featureArray));

                writer.WriteEndArray();
                writer.WriteEndObject();

                return;
            }

            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        ///   <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool CanConvert(Type objectType)
        {
            return objectType is ResultSet;
        }
    }
}