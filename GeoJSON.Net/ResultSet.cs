namespace GeoJSON.Net
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class ResultSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultSet"/> class.
        /// </summary>
        /// <param name="results">The results.</param>
        public ResultSet(params GeoJSONObject[] results)
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
        public IEnumerable<GeoJSONObject> Results { get; set; }
    }
}
