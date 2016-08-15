(function() {
    "use strict";
    // ReSharper disable once InconsistentNaming
    var EventsController = function ($stateParams, importEventService, logger) {
        var vm = this;

        var month = $stateParams.month || new Date().getMonth() + 1;
        var year = $stateParams.year || new Date().getFullYear();

        var importSuccess = function() {
            logger.success('Event imported succesfully');
            vm.loadEvents();
        };

        var importError = function(err) {
            logger.error(err);
        };

        vm.events = [];

        vm.dateOptions = {
            minViewMode: "months",
            maxViewMode: "months",
            todayBtn: false,
            clearBtn: false,
            datepickerMode: "'month'",
            minMode: 'month'
        };

        vm.eventsLoaded = false;
        vm.format = 'MMMM yyyy';
        vm.minDate = new Date(2015, 1, 1);

        vm.selectedDate = new Date(year, month - 1, 1);

        vm.status = {
            opened: false
        };

        vm.open = function() {
            vm.status.opened = true;
        };

        vm.loadEvents = function () {
            vm.eventsLoaded = false;
            vm.events = importEventService.query({
                month: vm.selectedDate.getMonth() + 1,
                year: vm.selectedDate.getFullYear()
            }, function() {
                vm.eventsLoaded = true;
            }, function() {
                logger.error({});
            });
        };

        vm.importEvent = function(event) {
            event.$save({}, importSuccess, importError);
        };

        vm.goToImportedEvents = function() {
        };

        vm.loadEvents();

        return vm;
    };
    EventsController.$inject = ['$stateParams', 'ImportEventService', 'logger'];

    angular.module('BridgePortal')
           .controller('ImportEventsController', EventsController);

})();