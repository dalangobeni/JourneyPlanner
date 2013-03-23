(function (window, undefined) {
    
    function roundNumber(number, digits) {
        var multiple = Math.pow(10, digits);
        var rndedNum = Math.round(number * multiple) / multiple;
        return rndedNum;
    }

    var api = {
        roundNumber: roundNumber
    };

    window["formatting"] = api;

})(window);