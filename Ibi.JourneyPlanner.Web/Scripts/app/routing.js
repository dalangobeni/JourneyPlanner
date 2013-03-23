(function (window, $, undefined) {

    var currentRoute = {};

    var isRouteInProgress = false;
    
    function buildRoute(buildRouteHandler, callback) {

        var points = {
            fromLatitude: currentRoute.start.lat,
            fromLongitude: currentRoute.start.lng,
            toLatitude: currentRoute.end.lat,
            toLongitude: currentRoute.end.lng,
        };

        var request = $.ajax({
            type: "POST",
            url: "/api/Routing/PointToPoint",
            data: points,
            dataType: "JSON"
        });

        request.success(function (response) {
            if (response && response.results) {
                buildRouteHandler(response.results);
            }
        });
        
        if (callback) {
            callback();
        }
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
    
    function startRoute(point) {
        if (isRouteInProgress) {
            resetRoute();
        }

        currentRoute.start = point;
        isRouteInProgress = true;
    }
    
    function endRoute(point, routeCompleteHandler) {
        if (isRouteInProgress) {
            currentRoute.end = point;
            buildRoute(routeCompleteHandler, function () {
                resetRoute();
            });
        }
    }
    
    function addPoint(point, routeCompleteHandler) {
        if (!isRouteInProgress) {
            startRoute(point);
        } else {
            endRoute(point, routeCompleteHandler);
        }
    }

    var api = {
        addPoint: addPoint,
        resolvePoint: resolvePoint
    };

    window.routing = api;

})(window, $);