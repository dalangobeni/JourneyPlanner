namespace Ibi.JourneyPlanner.Web.Models
{
    using System.Collections.Generic;

    using Geo.IO.GeoJson;

    using Ibi.JourneyPlanner.Web.Code.JsonConverters;

    using Newtonsoft.Json;

    [JsonConverter(typeof(ResultSetJsonConverter))]
    public class ResultSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultSet"/> class.
        /// </summary>
        /// <param name="results">The results.</param>
        public ResultSet(params Feature[] results)
        {
            this.Results = results;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultSet"/> class.
        /// </summary>
        /// <param name="results">The results.</param>
        public ResultSet(IEnumerable<Feature> results)
        {
            this.Results = results;
        }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        [JsonProperty(PropertyName = "results", Required = Required.Always)]
        public IEnumerable<Feature> Results { get; private set; }
    }
}
