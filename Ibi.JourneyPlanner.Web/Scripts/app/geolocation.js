(function (window, google, undefined) {


    // Gets the first formatted address from a list of geocoded results.
    function getFirstGeocodedAddress(results, callback) {
        //console.log("Getting geocoded address");
        var address = results[0].formatted_address;
        if (address) {
            //console.log("Getting geocoded address success");
            callback(address);
        }
        else {
            // _log("Getting geocoded address failure");
            callback("Getting geocoded address failure");
        }
    }
    
    function geocode(location, callback) {
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'address': location, 'region': 'GB' }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                getFirstGeocodedAddress(results, callback);                
            }
        });
    }

    function reverseGeocode(point, callback) {
        if (callback) {
            callback("Resolved name for point " + point.latitude + "," + point.longitude);
        }

        var geocoder = new google.maps.Geocoder();

        // console.log("Reverse geocoding available");
        var latLng = new google.maps.LatLng(point.latitude, point.longitude);
        geocoder.geocode({ 'latLng': latLng }, function(results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                getFirstGeocodedAddress(results, callback);
            } else {
                //console.log("Reverse geocoding failure");
                callback(status);
            }
        });
    }
    
    function getUserLocation(callback) {
        if (callback) {
            callback("User's location");
        }
            // HTML5 geolocation
            // Check to see if this browser supports geolocation.
            if (navigator.geolocation) {

                // This is the location marker that we will be using
                // on the map. Let's store a reference to it here so
                // that it can be updated in several places.
                var locationMarker = null;

                // Get the location of the user's browser using the
                // native geolocation service. When we invoke this method
                // only the first callback is requied. The second
                // callback - the error handler - and the third
                // argument - our configuration options - are optional.
                navigator.geolocation.getCurrentPosition(
                    function (position) {

                        // Check to see if there is already a location.
                        // There is a bug in FireFox where this gets
                        // invoked more than once with a cahced result.
                        if (locationMarker) {
                            return;
                        }

                    },
                    function (error) {
                        console.log("Something went wrong: ", error);
                    },
                    {
                        timeout: (5 * 1000),
                        maximumAge: (1000 * 60 * 15),
                        enableHighAccuracy: true
                    }
                );
            }
        
    }

    var api = {
        geocode: geocode,
        reverseGeocode: reverseGeocode,
        getUserLocation: getUserLocation
    };

    window.geolocation = api;

})(window, google);