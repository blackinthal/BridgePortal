(function() {
    "use strict";

    var importEvent = function($resource, urlBuilder) {
        return $resource(
            urlBuilder.build('api/importevents/:year/:month/:day'),
            {year: '@year', month:'@month', day:'@day'},
            {}
        );
    };

    importEvent.$inject = ['$resource', 'UrlBuilder'];

    angular
        .module('BridgePortal')
        .factory('ImportEventService', importEvent);

})();