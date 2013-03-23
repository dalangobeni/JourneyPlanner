(function (window, undefined) {
    
    function roundNumber(number, digits) {
        var multiple = Math.pow(10, digits);
        var rndedNum = Math.round(number * multiple) / multiple;
        return rndedNum;
    }
    
    function journeyDetails(output) {
        var content = [];
        for (var item in output) {
            content.push("<b>" + item + "</b><br>" + output[item]);
        }

        return content.join("<br><br>");
    }

    var api = {
        roundNumber: roundNumber,
        journeyDetails: journeyDetails
    };

    window["formatting"] = api;

})(window);