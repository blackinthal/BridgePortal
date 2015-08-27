(function() {
    "use strict";

    var displayScore = function() {
        return function(input, nsScore) {
            input = input || '';
            if (nsScore) {
                return input >= 0 ? Math.abs(input) : '';
            } else {
                return input <= 0 ? Math.abs(input) : '';
            }
        }
    };

    angular.module('BridgePortal')
           .filter('displayscore', displayScore);
})();