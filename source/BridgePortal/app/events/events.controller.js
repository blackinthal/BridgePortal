(function() {
    "use strict";

    var eventsController = function(eventService) {
        var vm = this;

        vm.events = [];

        vm.eventsLoaded = false;
        vm.loadEvents = function () {
            vm.eventsLoaded = false;
            vm.events = eventService.query({},function() {
                vm.eventsLoaded = true;
            });
        };

        vm.loadEvents();

        return vm;
    };
    eventsController.$inject = ['EventsService'];

    angular.module('BridgePortal')
        .controller('EventsController', eventsController);

})();