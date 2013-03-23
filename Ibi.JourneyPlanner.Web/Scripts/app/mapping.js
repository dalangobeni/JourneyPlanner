(function (window, $, L, routing, geolocation, formatting, undefined) {

    var map, getTransportModeFunction;

    function onEachFeature(feature, layer) {
        if (feature.properties) {

            var props = feature.properties;

            if (props.distance && props.journeytime) {
                var distance = formatting.roundNumber(props.distance / 1000, 2);
                var journeyTime = feature.properties.journeytime;
                var speed = props.distance / journeyTime;

                var output = {
                    "Distance": distance + " Km",
                    "Journey Time": journeyTime + " s",
                    "Speed": speed + " m/s"
                };

                var formatted = formatting.journeyDetails(output);
                layer.bindPopup(formatted);
            }
            else if (props.name) {
                layer.bindPopup(feature.properties.name);
            }
        }
    }

    function getRequestUrl(perPage, pageNumber) {
        // var url = "http://api.citysdk.waag.org/admr.uk.gr.manchester/nodes?geom&osm::amenity=parking&per_page=" + perPage;
        var url = "http://api.citysdk.waag.org/admr.uk.gr.manchester/ptstops?geom&osm::amenity=parking&per_page=" + perPage;
        if (pageNumber) {
            url += "&page=" + pageNumber;
        }

        return url;
    }
    
    function getTransportModes(callback) {

        if (callback) {
            var request = $.ajax({
                type: "GET",
                url: "/api/Routing/GetTransportModes",
                dataType: "JSON"
            });

            request.success(function(modes) {
                callback(modes);
            });
        }
    }

    function drawRoutes(routes) {
        for (var i = routes.length - 1; i >= 0; i--) {
            var thisItem = routes[i];

            var geojsonFeature = {
                "type": "Feature",
                "properties": thisItem.properties,
                "geometry": thisItem.geom
            };

            var myStyle = {
                "color": "#ff7800",
                "weight": 5,
                "opacity": 1
            };

            L.geoJson(geojsonFeature, {
                style: myStyle,
                onEachFeature: onEachFeature
            }).addTo(map);
        }
    }

    function resolvePoint(point, callback) {
        var selectedMode = getTransportModeFunction();
        routing.resolvePoint(selectedMode, point, callback);
    }

    function onMapClick(e) {
        var clickedPosition = e.latlng;

        resolvePoint({ latitude: clickedPosition.lat, longitude: clickedPosition.lng }, function (point) {

            geolocation.reverseGeocode({ latitude: point.Latitude, longitude: point.Longitude }, function (pointName) {
                alert(pointName);
            });
            
            var position = { lat: point.Latitude, lng: point.Longitude };
            L.marker(position).addTo(map);

            routing.addPoint(position, drawRoutes);
        });
    }

    function init(options, readyCallback) {
        var div = options.container;
        var lat = options.latitude;
        var lng = options.longitude;
        var zoom = options.zoom;        

        map = L.map(div).setView([lat, lng], zoom);
        getTransportModeFunction = options.getTransportMode;

        // add an OpenStreetMap tile layer
        L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors. Built by Mark & Kev.'
        }).addTo(map);

        map.on('contextmenu', onMapClick);
        
        if (readyCallback) {
            readyCallback();
        }
    }

    var api = {
        init: init,
        getTransportModes: getTransportModes
    };

    window["mapping"] = api;

})(window, $, L, routing, geolocation, formatting);