(function() {
    "use strict";
    // ReSharper disable once InconsistentNaming
    var EventsController = function ($stateParams) {
        var vm = this;

        vm.events = [];
        vm.month = $stateParams.month || new Date().getMonth() + 1;
        vm.year = $stateParams.year || new Date().getFullYear();

        vm.dateOptions = {
            minViewMode: "months",
            maxViewMode: "months",
            todayBtn: false,
            clearBtn: false,
            datepickerMode: "'month'",
            minMode: 'month'
        }

        vm.format = 'MMMM yyyy';
        vm.minDate = new Date(2015, 1, 1);
        vm.maxDate = new Date(2015, 12, 31);
        vm.selectedDate = new Date(vm.year, vm.month - 1, 1);

        vm.status = {
            opened: false
        };

        vm.open = function($event) {
            vm.status.opened = true;
        };

        vm.loadEvents = function() {
            console.log(vm.selectedDate);
        };

        vm.loadEvents();

        return vm;
    };
    EventsController.$inject = ['$stateParams'];

    angular.module('BridgePortal')
           .controller('EventsController', EventsController);

})();