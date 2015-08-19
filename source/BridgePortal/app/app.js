(function () {
    "use strict";
    var app = angular.module('BridgePortal', ['ui.router', 'ngAnimate', 'ui.bootstrap', 'ngResource']);

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
            })
            .state('events', {
                url: '/events?year&month',
                templateUrl: 'app/events/events.html',
                controller: 'EventsController as vm'
            });
    });
})();