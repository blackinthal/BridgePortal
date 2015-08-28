(function () {
    "use strict";

    var displayContract = function () {
        return function (input) {
            return input && input.level && input.level > 0 ? input.contract : '-';
        }
    };

    angular.module('BridgePortal')
           .filter('displaycontract', displayContract);
})();