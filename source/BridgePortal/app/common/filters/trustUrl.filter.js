(function() {
    "use strict";

    var trustUrlFilter = function($sce) {
        return function(val) {
            return $sce.trustAsResourceUrl(val);
        };
    };

    trustUrlFilter.$inject = ['$sce'];

    angular.module('BridgePortal')
        .filter('trustAsResourceUrl', trustUrlFilter);
})();