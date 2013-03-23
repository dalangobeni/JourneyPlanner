(function (window, $, L, formatting, undefined) {

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

    function routePoint(points) {

        var request = $.ajax({
            type: "POST",
            url: "/api/Routing/PointToPoint",
            data: points,
            dataType: "JSON"
        });

        request.success(function (response) {
            if (response && response.results) {
                for (var i = response.results.length - 1; i >= 0; i--) {
                    var thisItem = response.results[i];

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
        });
    }

    function resolvePoint(point, callback) {
        var selectedMode = getTransportModeFunction();
        var model = {
            latitude: point.latitude,
            longitude: point.longitude,
            transportMode: selectedMode
        };

        var request = $.ajax({
            type: "POST",
            url: "/api/Routing/GetClosestPointTo",
            data: model,
            dataType: "JSON"
        });

        request.success(function (response) {
            if (callback) {
                callback(response);
            }
        });
    }
    
    var routingPoints = [];

    function onMapClick(e) {
        var clickedPosition = e.latlng;

        resolvePoint({ latitude: clickedPosition.lat, longitude: clickedPosition.lng }, function (point) {

            var position = { lat: point.Latitude, lng: point.Longitude };
            L.marker(position).addTo(map);

            if (routingPoints.length == 0) {
                routingPoints.push(position);
            } else {
                routingPoints.push(position);

                var from = routingPoints[0];
                var to = routingPoints[1];

                routePoint({
                    fromLatitude: from.lat,
                    fromLongitude: from.lng,
                    toLatitude: to.lat,
                    toLongitude: to.lng,
                });

                routingPoints = [];
            }
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

})(window, $, L, formatting);