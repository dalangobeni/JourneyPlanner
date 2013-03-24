(function (window, $, L, routing, geolocation, formatting, undefined) {

    var map,
        getTransportModeFunction,
        routes = [];

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
    
    function handleContextClick(eventTarget) {
        var link = $(eventTarget);
        alert(link.data("action"));
        return false;
    }
    
    function addRoutingPoint(position, name, options) {
        routing.addPoint(position);

        var color = options.color || "orange";
        var icon = options.icon || "icon-flag";

        var iconOptions = L.AwesomeMarkers.icon({
            color: color,
            icon: icon
        });

        var marker = L.marker(position, {
            icon: iconOptions
        }).bindPopup("<h1>" + name + "</h1>")
            .addTo(map);
    }

    function centreOnMyLocation(position) {
        map.setView(position, 20);
    }

    function forEachLayerIcon(feature, layer, displayText, color) {
        if (feature.properties) {
            var props = feature.properties;
            var displayName = props.name || displayText;

            var content = "<h1 style='color: " + color + "'>" + displayName + "</h1>";

            var dataItems = [];
            for (var property in props) {
                if (property != "name") {
                    dataItems.push({ key: property, value: props[property] });
                }
            }

            var dataItemCount = dataItems.length;
            if (dataItemCount > 0) {
                var table = "<table>";

                for (var i = 0; i < dataItemCount; i++) {
                    var item = dataItems[i];
                    table += "<tr><th>" + item.key + "</th><td>" + item.value + "</td></tr>";
                }

                table += "</table>";
                content += table;
            }

            layer.bindPopup(content);

            layer.on('contextmenu', function() {
                var position = layer.getLatLng();
                addRoutingPoint(position);
            });
        }
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

    function createLayerData(name, amenityName, color, icon) {
        return {
            name: name,            
            amenity: amenityName,
            color: color,
            icon: icon
        };
    }

    function getAvailableLayers() {
        var layers = [];

        layers.push(createLayerData('TfGM Live Parking Information','live-parking', 'red'));
        layers.push(createLayerData('Car Parking Locations','parking', 'red'));
        layers.push(createLayerData('Bars', 'bar', 'blue','icon-beer'));
        layers.push(createLayerData('Fast Food', 'fast_food', 'green','icon-food'));
        layers.push(createLayerData('Restaurants', 'restaurant', 'cadetblue', 'icon-glass'));
        layers.push(createLayerData('Bus Stations', 'bus_station', 'orange'));

        return layers;
    }

    function getRequestUrl(amenity, perPage, pageNumber) {
        
        if (amenity === "live-parking") {
            return "/api/live/GetAllCarParks";
        }

        var url = "http://api.citysdk.waag.org/admr.uk.gr.manchester/nodes?geom&osm::amenity=" + amenity + "&per_page=" + perPage;
        if (pageNumber) {
            url += "&page=" + pageNumber;
        }

        return url;
    }
    
    function loadLayerData(displayText, layer, amenity, color, icon) {
        var url = getRequestUrl(amenity, 500);
        
        var request = $.ajax({
            type: "GET",
            url: url,
            dataType: "JSON"
        });

        request.success(function (response) {
            if (response && response.results) {
                for (var i = response.results.length - 1; i >= 0; i--) {
                    var thisItem = response.results[i];

                    var properties = thisItem.properties || {
                        "name": thisItem.name
                    };

                    var geojsonFeature = {
                        "type": "Feature",
                        "properties": properties,
                        "geometry": thisItem.geom
                    };

                    var myStyle = {
                        "color": "#ff7800",
                        "weight": 5,
                        "opacity": 1
                    };

                    var geojsonMarkerOptions = L.AwesomeMarkers.icon({
                        color: color,
                        icon: icon
                    });

                    L.geoJson(geojsonFeature, {
                        pointToLayer: function(feature, latlng) {
                            return L.marker(latlng, { icon: geojsonMarkerOptions });
                        },
                        style: myStyle,
                        onEachFeature: function(featureItem, layerItem) {
                            forEachLayerIcon(featureItem, layerItem, displayText, color);
                        }
                    }).addTo(layer);

                }

                //var nextPage = response.next_page;
                //if (nextPage && nextPage > 0) {
                //    loadPageBlock(numberOfSitesPerPage, nextPage);
                //}

                layer.extraOptions.isLoaded = true;
            }
        });
    }

    function loadLayers() {
        var layers = getAvailableLayers();
        var overlayMaps = {};

        var layerCount = layers.length;

        for (var i = 0; i < layerCount; i++) {
            var thisItem = layers[i];
            var layer = L.featureGroup([]);

            layer.extraOptions = {
                amenityName: thisItem.amenity,
                color: thisItem.color,
                icon: thisItem.icon,                
                isLoaded: false
            };

            overlayMaps[thisItem.name] = layer;
        }
        
        L.control.layers(null, overlayMaps).addTo(map);
        
        // Hook up events
        // This is horrible.
        $(".leaflet-control-layers-overlays input").change(function (e) {
            var target = $(e.target);
            if (target.is(":checked"))
            {
                var text = target.siblings("span").text();
                var trimmed = $.trim(text);

                var matchingLayer = overlayMaps[trimmed];
                if (matchingLayer.extraOptions) {
                    if (!matchingLayer.extraOptions.isLoaded) {
                        var amenity = matchingLayer.extraOptions.amenityName;
                        var icon = matchingLayer.extraOptions.icon;
                        var color = matchingLayer.extraOptions.color;
                        loadLayerData(text, matchingLayer, amenity, color, icon);
                    }
                }
            }
        });
    }    
	
    function drawRoutes(routesToDraw) {
        for (var i = routesToDraw.length - 1; i >= 0; i--) {
            var thisItem = routesToDraw[i];

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

            var path = L.geoJson(geojsonFeature, {
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
                var position = { lat: point.Latitude, lng: point.Longitude };
                addRoutingPoint(position, pointName);
            });  
            
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

        // loading the required mapping layers for user to select
        loadLayers();
        
        // Set up routing
        routing.init({
            routeCompleteHandler: drawRoutes
        });

        if (readyCallback) {
            readyCallback();
        }
    }

    var api = {
        init: init,
        getTransportModes: getTransportModes,
        handleContextClick: handleContextClick,
        addPoint: addRoutingPoint,
        centreOnMyLocation: centreOnMyLocation
    };

    window["mapping"] = api;

})(window, $, L, routing, geolocation, formatting);
