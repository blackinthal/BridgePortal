///#source 1 1 /app/common/filters/trustUrl.filter.js
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
///#source 1 1 /app/common/filters/scoreDisplay.filter.js
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
///#source 1 1 /app/common/filters/contractDisplay.filter.js
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
///#source 1 1 /app/common/services/urlBuilder.service.js
(function() {
    angular.module('BridgePortal')
        .factory('UrlBuilder', function() {
            return {
                build: function(url) {
                    return 'http://localhost:2128/' + url;
                }    
            }
        });
})();
///#source 1 1 /app/common/services/logger.service.js
(function () {
    angular.module('BridgePortal')
        .factory('logger', function () {
            return {
                success: function (message) {
                    toastr.success(message);
                },
                error: function (err) {
                    toastr.error((err && err.data && err.data.exceptionMessage) ? err.data.exceptionMessage : 'An error has occurred');
                }
            }
        });
})();
///#source 1 1 /app/events/services/importEvents.service.js
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
///#source 1 1 /app/events/services/events.service.js
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
///#source 1 1 /app/deals/services/deals.service.js
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
///#source 1 1 /app/events/importEvents.controller.js
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
///#source 1 1 /app/events/events.controller.js
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
///#source 1 1 /app/events/eventDetail.controller.js
(function () {
    "use strict";
    var eventDetailController = function(event) {
        var vm = this;
        var noOfDeals = event.deals.length;

        vm.event = event;
        vm.selectedDeal = vm.event.deals[0];

        vm.selectDeal = function(deal) {
            vm.selectedDeal = deal;
        }

        vm.goToPreviousDeal = function() {
            var index = vm.selectedDeal.index - 2;
            vm.selectedDeal = vm.event.deals[index >= 0 ? index : 0];
        }

        vm.goToNextDeal = function() {
            var index = vm.selectedDeal.index;
            vm.selectedDeal = vm.event.deals[index > noOfDeals - 1 ? noOfDeals - 1 : index];
        }

        vm.goToFirstDeal = function () {
            vm.selectedDeal = vm.event.deals[0];
        }

        vm.goToLastDeal = function () {
            vm.selectedDeal = vm.event.deals[noOfDeals - 1];
        }

        return vm;
    }

    eventDetailController.$inject = ['event'];
    angular.module('BridgePortal')
        .controller('EventDetailController', eventDetailController);
})();
///#source 1 1 /app/deals/dealDetail.controller.js
(function() {
    "use strict";

    var dealDetailController = function(deal) {
        var vm = this;

        vm.deal = deal;
        vm.currentUrl = deal.handViewerInput;

        vm.sContracts = deal.makeableContracts.splice(0, 4);
        vm.hContracts = deal.makeableContracts.splice(0, 4);
        vm.dContracts = deal.makeableContracts.splice(0, 4);
        vm.cContracts = deal.makeableContracts.splice(0, 4);
        vm.ntContracts = deal.makeableContracts.splice(0, 4);

        vm.loadContract = function(url) {
            vm.currentUrl = url;
        }

        return vm;
    }

    dealDetailController.$inject = ['deal'];
    angular.module('BridgePortal')
        .controller('DealDetailController', dealDetailController);
})();
