(function() {
    "use strict";

    var dealDetailController = function(deal) {
        var vm = this;

        vm.deal = deal;
        vm.currentUrl = deal.handViewerInput;

        return vm;
    }

    dealDetailController.$inject = ['deal'];
    angular.module('BridgePortal')
        .controller('DealDetailController', dealDetailController);
})();