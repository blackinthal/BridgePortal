///#source 1 1 /common/services/urlBuilder.service.js
(function() {

    angular.module('BridgePortal')
        .factory('UrlBuilder', function() {
            return {
                build: function(url) {
                    return 'http://localhost/Bridge.WebAPI/' + url;
                }    
            }
        });
})();
///#source 1 1 /app/events/services/ImportEvents.service.js
(function() {
    "use strict";

    var importEvent = function($resource, urlBuilder) {
        return $resource(
            urlBuilder.build('api/events/:year/:month/:day'),
            {year: '@year', month:'@month', day:'@day'},
            {}
        );
    };

    importEvent.$inject = ['$resource', 'UrlBuilder'];

    angular
        .module('BridgePortal')
        .factory('ImportEventService', importEvent);

})();
///#source 1 1 /app/events/EventsController.js
(function() {
    "use strict";
    // ReSharper disable once InconsistentNaming
    var EventsController = function ($stateParams, importEventService) {
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

        vm.open = function() {
            vm.status.opened = true;
        };

        vm.loadEvents = function() {
            vm.events = importEventService.query({
                month: vm.selectedDate.getMonth() + 1,
                year: vm.selectedDate.getFullYear()
            });
        };

        vm.importEvent = function(event) {
            event.$save();
        }

        vm.loadEvents();

        return vm;
    };
    EventsController.$inject = ['$stateParams', 'ImportEventService'];

    angular.module('BridgePortal')
           .controller('EventsController', EventsController);

})();
