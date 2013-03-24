(function(window, $, undefined) {

    var routes = [],
        onRoutesChanged;
    
    function init(options) {
        onRoutesChanged = options.onRoutesChanged;
    }

    function addRoute(routeData) {
        routes.push(routeData);
        
        if (onRoutesChanged) {
            onRoutesChanged(routes);
        }
    }
    
    function clearRoutes() {
        routes = [];

        if (onRoutesChanged) {
            onRoutesChanged(routes);
        }
    }

    var api = {
        init: init,
        add: addRoute,
        clear: clearRoutes
    };

    window.routeManager = api;

}(window, $));