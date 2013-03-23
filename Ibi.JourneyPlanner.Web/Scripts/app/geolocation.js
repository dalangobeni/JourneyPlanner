(function (window, $, undefined) {
    
    function geocode(location, callback) {
        if (callback) {
            callback("Lat/lng for location " + location);
        }
    }

    function reverseGeocode(point, callback) {
        if (callback) {
            callback("Resolved name for point " + point.latitude + "," + point.longitude);
        }
    }
    
    function getUserLocation(callback) {
        if (callback) {
            callback("User's location");
        }
    }

    var api = {
        geocode: geocode,
        reverseGeocode: reverseGeocode,
        getUserLocation: getUserLocation
    };

    window.geolocation = api;

})(window, $);