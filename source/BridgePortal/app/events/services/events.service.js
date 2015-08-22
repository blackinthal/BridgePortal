(function() {
    "use strict";

    var eventService = function($resource, urlBuilder) {
        return $resource(
            urlBuilder.build('api/events/:id'),
            {id: '@id'}
        );
    }

    eventService.$inject = ['$resource', 'UrlBuilder'];

    angular.module('BridgePortal')
           .factory('EventsService', eventService);
})();