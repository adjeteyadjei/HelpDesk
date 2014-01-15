'use strict';
angular.module("helpdesk.controllers", []);
var helpdesk = angular.module('helpdesk',
    [
        "helpdesk.controllers",
        "ngRoute",
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

helpdesk.run(["$http", "$cookies", function ($http, $cookies) {
    $http.defaults.headers.common['Authorization'] = 'Bearer ' + $cookies.accessToken;
}]);
