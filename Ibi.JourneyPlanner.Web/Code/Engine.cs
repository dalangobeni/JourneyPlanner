namespace Ibi.JourneyPlanner.Web.Code
{
    using System.IO;
    using System.Web.Hosting;

    using OsmSharp.Osm.Core;
    using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
    using OsmSharp.Osm.Routing.Data.Processing;
    using OsmSharp.Osm.Routing.Interpreter;
    using OsmSharp.Routing.Core;
    using OsmSharp.Routing.Core.Graph.DynamicGraph.SimpleWeighed;
    using OsmSharp.Routing.Core.Graph.Memory;
    using OsmSharp.Routing.Core.Graph.Router.Dykstra;

    public static class Engine
    {
        private static IRouter<RouterPoint> _instance;

        public static IRouter<RouterPoint> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetEngine();
                }

                return _instance;
            }
        }

        public static void Initialise()
        {
            var instance = Instance;
        }

        private static IRouter<RouterPoint> GetEngine()
        {
            // keeps a memory-efficient version of the osm-tags.
            var tagsIndex = new OsmTagsIndex();

            // creates a routing interpreter. (used to translate osm-tags into a routable network)
            var interpreter = new OsmRoutingInterpreter();

            // create a routing datasource, keeps all processed osm routing data.
            var osmData = new MemoryRouterDataSource<SimpleWeighedEdge>(tagsIndex);

            // load data into this routing datasource.
            var fileSource = HostingEnvironment.MapPath("~/App_Data/Manchester.osm.pbf");
            Stream osmXmlData = new FileInfo(fileSource).OpenRead(); // for example moscow!
            using (osmXmlData)
            {
                var targetData = new SimpleWeighedDataGraphProcessingTarget(
                                osmData,
                                interpreter,
                                osmData.TagsIndex,
                                VehicleEnum.Car);

                // replace this with PBFdataProcessSource when having downloaded a PBF file.
                var dataProcessorSource = new
                  OsmSharp.Osm.Data.PBF.Raw.Processor.PBFDataProcessorSource(osmXmlData);

                // pre-process the data.
                var sorter = new DataProcessorFilterSort();
                sorter.RegisterSource(dataProcessorSource);
                targetData.RegisterSource(sorter);
                targetData.Pull();
            }

            // create the router object: there all routing functions are available.
            IRouter<RouterPoint> router = new Router<SimpleWeighedEdge>(
                osmData, interpreter, new DykstraRoutingLive(osmData.TagsIndex));

            return router;
        }
    }
}