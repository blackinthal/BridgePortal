(function () {
    "use strict";
    var app = angular.module('BridgePortal', ['ui.router']);

    app.config(function($stateProvider, $urlRouterProvider) {

        $urlRouterProvider.otherwise("/");

        $stateProvider
            .state('home', {
                url: '/',
                templateUrl: 'landing.html'
            })
            .state('viewDeal', {
                url: '/viewDeal',
                templateUrl: 'app/deals/viewDeal.html'
            });
    });
})();