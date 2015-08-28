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