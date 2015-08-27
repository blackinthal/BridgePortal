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
                resolve: {
                    dealsResource: 'DealsService',
                    deal: function (dealsResource, $stateParams) {
                        return dealsResource.get({ id: $stateParams.id }).$promise;
                    }
                },
                url: '/deal/{id}',
                templateUrl: 'app/deals/viewDeal.html',
                controller: 'DealDetailController as vm'
            })
            .state('importEvents', {
                url: '/importEvents?year&month',
                templateUrl: 'app/events/importEvents.html',
                controller: 'ImportEventsController as vm'
            })
            .state('events', {
                abstract: true,
                url: '/events',
                template: '<ui-view/>',
            })
             .state('events.all', {
                 url: '/all',
                 templateUrl: 'app/events/events.html',
                 controller: 'EventsController as vm'
             })
            .state('events.detail', {
                resolve: {
                    eventsResource: 'EventsService',
                    event: function (eventsResource, $stateParams) {
                        return eventsResource.get({ id: $stateParams.id }).$promise;
                    }
                },
                url: '/:id',
                templateUrl: 'app/events/eventDetail.html',
                controller: 'EventDetailController as vm'
            })
            ;
    });
})();