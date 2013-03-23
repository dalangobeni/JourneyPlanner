(function (window, $, L, routing, geolocation, formatting, undefined) {

    var map, getTransportModeFunction, contextMenu;

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
    
    //function spawnContextMenu(layer) {
    //    if (!contextMenu) {
    //        contextMenu = $("<div class='contextmenu' />");

    //        var links = [];

    //        var routeLink = $("<a href='#'>Start Here</a>");
    //        routeLink.click(function(e) {
    //            e.preventDefault();
    //            alert("click");
    //        });
    //        links.push(routeLink);

    //        // Create menu HTML
    //        var list = $("<ul>");
    //        for (var i = 0; i < links.length; i++) {
    //            var li = $("<li>");
    //            li.append(links[i]);
    //            list.append(li);
    //        }
            
    //        contextMenu.append(list);
    //        $("body article").append(contextMenu);
    //    }

    //    // Get location of clicked marker
    //    var locationOfMarker = e.latLng;
    //}
    
    function handleContextClick(eventTarget) {
        var link = $(eventTarget);
        alert(link.data("action"));
        return false;
    }
    
    function addRoutingPoint(position) {
        routing.addPoint(position, drawRoutes);
        
        var options = L.AwesomeMarkers.icon({
            color: "green",
            icon: "icon-flag"
        });

        var marker = L.marker(position, { icon: options }).addTo(map);
    }

    function forEachLayerIcon(feature, layer, color) {
        if (feature.properties) {
            var props = feature.properties;
            if (props.name) {
                var content = "<h1 style='color: " + color + "'>" + props.name + "</h1>";

                //var links = [];
                //links.push({ text: "Start Route", action: "startRoute" });
                //links.push({ text: "Clear Route", action: "clearRoute" });

                //content += "<ul class='contextmenu'>";
                //for (var i = 0; i < links.length; i++) {
                //    var link = links[i];
                //    content += "<li><a onclick='return mapping.handleContextClick(this)' data-action='" + link.action + "' class='context' href='#'>" + link.text + "</a></li>";
                //}
                //content += "</ul>";

                layer.bindPopup(content);

                layer.on('contextmenu', function () {
                    var position = layer.getLatLng();
                    addRoutingPoint(position);
                });
            }
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

        layers.push(createLayerData('Car Parks','parking', 'red'));
        layers.push(createLayerData('Bars', 'bar', 'blue','icon-beer'));
        layers.push(createLayerData('Fast Food', 'fast_food', 'green','icon-food'));
        layers.push(createLayerData('Restaurants', 'restaurant', 'cadetblue'));
        layers.push(createLayerData('Bus Stations', 'bus_station', 'orange'));

        return layers;
    }

    function getRequestUrl(amenity, perPage, pageNumber) {
        var url = "http://api.citysdk.waag.org/admr.uk.gr.manchester/nodes?geom&osm::amenity=" + amenity + "&per_page=" + perPage;
        if (pageNumber) {
            url += "&page=" + pageNumber;
        }

        return url;
    }
    
    function loadLayerData(layer, amenity, color, icon) {
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

                    if (thisItem.name) {

                        var geojsonFeature = {
                            "type": "Feature",
                            "properties": {
                                "name": thisItem.name
                            },
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
                            pointToLayer: function (feature, latlng) {
                                return L.marker(latlng, { icon: geojsonMarkerOptions });
                            },
                            style: myStyle,
                            onEachFeature: function (featureItem, layerItem) {
                                forEachLayerIcon(featureItem, layerItem, color);
                            }
                        }).addTo(layer);
                    }
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
                if (!matchingLayer.extraOptions.isLoaded) {
                    var amenity = matchingLayer.extraOptions.amenityName;
                    var icon = matchingLayer.extraOptions.icon;
                    var color = matchingLayer.extraOptions.color;
                    loadLayerData(matchingLayer, amenity, color, icon);
                }
            }
        });
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
                // TODO: Something with the nameal
            });
            
            var position = { lat: point.Latitude, lng: point.Longitude };
            addRoutingPoint(position);
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

        if (readyCallback) {
            readyCallback();
        }
    }

    var api = {
        init: init,
        getTransportModes: getTransportModes,
        handleContextClick: handleContextClick
    };

    window["mapping"] = api;

})(window, $, L, routing, geolocation, formatting);
