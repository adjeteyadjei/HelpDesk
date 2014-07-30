'use strict';
var helpdesk = angular.module('helpdesk',
    [
        "ngRoute",
        "ui.router",
        "ngCookies",
        "modelResources",
        "helpers",
		"pager_component",
		"filter_component",
		"pagingServices",
		"filterServices",
        "ui.select2"
    ]);

helpdesk.config(["$routeProvider", "$locationProvider", function ($routeProvider, $locationProvider) {
}]);

helpdesk.config(["$stateProvider", "$urlRouterProvider", function ($stateProvider, $urlRouterProvider) {
    $stateProvider
        .state("/", {
            url: "",
            templateUrl: "/home/dashboard",
            controller: "DashboardController"
        })
        .state("tickets", {
            url: "/ticket/index",
            templateUrl: "/ticket/index",
            controller: "TicketController",
        })
        .state("thread", {
            url: "/ticket/TicketThread/:id",
            templateUrl: "/ticket/TicketThread",
            controller: "TicketThreadController",
        })
        .state("reports", {
            url: "/report/index",
            templateUrl: "/report/index",
            controller: "ReportController",
        })
        .state("projects", {
            url: "/project/index",
            templateUrl: "/project/index",
            controller: "ProjectController",
        })
        .state("config", {
            url: "/configuration/index",
            templateUrl: "/configuration/index",
            controller: "LookUpController",
        })
        .state("teams", {
            url: "/administration/team",
            templateUrl: "/administration/team",
            controller: "TeamController",
        })
        .state("users", {
            url: "/administration/users",
            templateUrl: "/administration/users",
            controller: "UserController",
        });
}]);

helpdesk.run(["$http", "$cookies", function ($http, $cookies) {
    $http.defaults.headers.common['Authorization'] = 'Bearer ' + $cookies.accessToken;
}]);
