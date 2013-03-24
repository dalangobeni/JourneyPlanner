(function (window, $, undefined) {

    var currentRoute = {},
        isRouteInProgress = false,
        routeCompleteHandler,
        routeErrorHandler;
    
    function buildRoute(buildRouteHandler, callback) {

        var points = {
            fromLatitude: currentRoute.start.point.lat,
            fromLongitude: currentRoute.start.point.lng,
            toLatitude: currentRoute.end.point.lat,
            toLongitude: currentRoute.end.point.lng,
        };

        var request = $.ajax({
            type: "POST",
            url: "/api/Routing/PointToPoint",
            data: points,
            dataType: "JSON"
        });

        request.success(function (response) {
            if (response && response.results) {
                buildRouteHandler(
                    response.results,
                    currentRoute.start,
                    currentRoute.end);
            }

            if (callback) {
                callback();
            }
        });

        request.fail(function(response) {
            if (routeErrorHandler) {
                routeErrorHandler(currentRoute, response);
            }
            
            if (callback) {
                callback();
            }
        });
    }
    
    function resolvePoint(selectedMode, point, callback) {
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
    
    function resetRoute() {
        currentRoute = {};
        isRouteInProgress = false;
    }
    
    function startRoute(point, name, marker) {
        if (isRouteInProgress) {
            resetRoute();
        }

        currentRoute.start = {
            point: point,
            marker: marker,
            name: name
        };
        
        isRouteInProgress = true;
    }
    
    function endRoute(point, name, marker) {
        if (isRouteInProgress) {
            currentRoute.end = {
                point: point,
                marker: marker,
                name: name
            };
            
            buildRoute(routeCompleteHandler, function () {
                resetRoute();
            });
        }
    }
    
    function addPoint(point, name, marker) {
        if (!isRouteInProgress) {
            startRoute(point, name, marker);
        } else {
            endRoute(point, name, marker);
        }
    }
    
    function init(options) {
        routeCompleteHandler = options.routeCompleteHandler;
        routeErrorHandler = options.routeErrorHandler;
    }

    var api = {
        init: init,
        addPoint: addPoint,
        resolvePoint: resolvePoint
    };

    window.routing = api;

})(window, $);