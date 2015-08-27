(function() {
    "use strict";

    var dealsResource = function($resource, urlBuilder) {
        return $resource(
            urlBuilder.build('api/deals/:id'),
            { id: '@id' }
        );
    };

    dealsResource.$inject = ['$resource', 'UrlBuilder'];

    angular.module('BridgePortal')
          .factory('DealsService', dealsResource);
})();