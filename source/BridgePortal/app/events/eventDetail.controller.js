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