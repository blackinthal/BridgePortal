(function () {
    "use strict";
    var eventDetailController = function(event) {
        var vm = this;
        vm.event = event;

        return vm;
    }

    eventDetailController.$inject = ['event'];
    angular.module('BridgePortal')
        .controller('EventDetailController', eventDetailController);
})();