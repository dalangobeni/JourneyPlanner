namespace Ibi.JourneyPlanner.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    using Geo;
    using Geo.Geometries;
    using Geo.IO.GeoJson;

    using Ibi.JourneyPlanner.Web.Models;
    using Ibi.JourneyPlanner.Web.Models.LiveData;
    using Newtonsoft.Json;
    using Configuration = System.Configuration.Configuration;

    public class LiveDataController : ApiController
    {
        private readonly Uri baseAddress = new Uri("http://opendata.tfgm.com/api/");

        // You need to add a Web.AppSettings.Secure.config file with these values
        private readonly string AppKey = ConfigurationManager.AppSettings["TfGMAppKey"];
        private readonly string DevKey = ConfigurationManager.AppSettings["TfGMDevKey"];

        public async Task<ResultSet> GetAllCarParks()
        {
            try
            {
                const string CacheName = "GetAllCarParks";
                var cachedResult = HttpContext.Current.Cache[CacheName] as ResultSet;

                if (cachedResult != null)
                {
                    return cachedResult;
                }

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("DevKey", DevKey);
                client.DefaultRequestHeaders.Add("AppKey", AppKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
                client.BaseAddress = this.baseAddress;

                var pageIndex = 0;
                var pageSize = 10;
                var endOfList = false;

                var items = new List<Feature>();

                while (!endOfList)
                {
                    var query = string.Format("Carparks/?pageIndex={0}&pageSize={1}", pageIndex, pageSize);
                    var response = client.GetAsync(query).Result;

                    // If it fails - outputs the request string followed by the Http response
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        var result = String.Format("{0}{1} \n{2}", baseAddress, query, response);
                        throw new HttpException(result);
                    }

                    // If it gets an OK status Outputs the json/xml recieved
                    var json = await response.Content.ReadAsStringAsync();
                    var data = (IEnumerable<CarPark>)JsonConvert.DeserializeObject(json, typeof(IEnumerable<CarPark>));
                    items.AddRange(this.GetFeaturesFromCarParks(data));

                    endOfList = !data.Any();
                    pageIndex++;
                }

                var resultSet = new ResultSet(items);

                HttpContext.Current.Cache.Insert(CacheName, resultSet, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
                return resultSet;
            }
            catch (Exception ex)
            {
                throw new HttpException("Error");
            }
        }

        private IEnumerable<Feature> GetFeaturesFromCarParks(IEnumerable<CarPark> carParks)
        {
            return carParks.Select(this.GetFeatureFromCarPark).ToList();
        }  

        private Feature GetFeatureFromCarPark(CarPark carPark)
        {
            // Reversed on purpose; result from API is wrong
            var coordinates = new Coordinate(carPark.Longitude, carPark.Latitude);

            var lineString = new Point(coordinates);

            var feature = new Feature(
                lineString,
                new Dictionary<string, object>
                    {
                        { "name", carPark.Name },
                        { "Spaces", carPark.SpacesNow },
                        { "Last Updated", carPark.LastUpdated.ToString("dd/MM/yy HH:mm") },
                        { "Predicted Spaces in 30 mins", carPark.PredictedSpaces30Mins },
                        { "Predicted Spaces in 60 mins", carPark.PredictedSpaces60Mins },
                    });

            feature.Id = carPark.Id.ToString();

            return feature;
        }

        public async Task<CarPark> GetCarParkById(int id)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("DevKey", DevKey);
                client.DefaultRequestHeaders.Add("AppKey", AppKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
                client.BaseAddress = this.baseAddress;

                var query = string.Format("Carparks/{0}", id);
                var response = client.GetAsync(query).Result;

                var items = new List<Feature>();

                // If it fails - outputs the request string followed by the Http response
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var result = String.Format("{0}{1} \n{2}", baseAddress, query, response);
                    throw new HttpException(result);
                }

                // If it gets an OK status Outputs the json/xml recieved
                var json = await response.Content.ReadAsStringAsync();
                var data = (IEnumerable<CarPark>)JsonConvert.DeserializeObject(json, typeof(IEnumerable<CarPark>));
                return data.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new HttpException("Error");
            }
        }

        public async Task<ResultSet> GetAllBuses()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("DevKey", DevKey);
                client.DefaultRequestHeaders.Add("AppKey", AppKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
                client.BaseAddress = this.baseAddress;

                var route = 1;

                var items = new List<Feature>();

                while (route <= 3)
                {
                    var query = string.Format("routes/met{0}/buses", route);
                    var response = client.GetAsync(query).Result;

                    // If it fails - outputs the request string followed by the Http response
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        var result = String.Format("{0}{1} \n{2}", baseAddress, query, response);
                        throw new HttpException(result);
                    }

                    // If it gets an OK status Outputs the json/xml recieved
                    var json = await response.Content.ReadAsStringAsync();
                    var data = (IEnumerable<MetroShuttle>)JsonConvert.DeserializeObject(json, typeof(IEnumerable<MetroShuttle>));
                    items.AddRange(this.GetFeaturesFromMetroShuttle(data));

                    route++;
                }

                var resultSet = new ResultSet(items);
                return resultSet;
            }
            catch (Exception ex)
            {
                throw new HttpException("Error");
            }
        }

        private IEnumerable<Feature> GetFeaturesFromMetroShuttle(IEnumerable<MetroShuttle> metroShuttles)
        {
            return metroShuttles.Select(this.GetFeatureFromMetroShuttle).ToList();
        }

        private Feature GetFeatureFromMetroShuttle(MetroShuttle metroShuttle)
        {
            // Reversed on purpose; original API is wrong
            var coordinates = new Coordinate(metroShuttle.Latitude, metroShuttle.Longitude);

            var lineString = new Point(coordinates);

            var feature = new Feature(
                lineString,
                new Dictionary<string, object>
                    {
                        { "name", "Metro Shuttle " + metroShuttle.Id },
                        { "Route", metroShuttle.Route },
                        { "Registration", metroShuttle.Registration },
                        { "Last Updated", metroShuttle.LastUpdated.ToString("dd/MM/yy HH:mm") },
                        { "IsParked", metroShuttle.IsParked },
                    });

            feature.Id = metroShuttle.Id.ToString();

            return feature;
        }
    }
}

