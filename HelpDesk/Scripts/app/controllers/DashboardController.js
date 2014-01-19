/// <reference path="../../jquery-ui-1.8.24.js" />
var app = angular.module('helpdesk.controllers', []);

app.controller("DashboardController", ['$scope', '$http', function DashboardController($scope, $http) {

    $scope.activeClass = "";
    $scope.activities = [];

    function ticketStatistics() {
        $scope.ticketStats = {
            New: [],
            Open: [],
            Pending: [],
            Solved: []
        };
    }

    function recentActivities() {
        $scope.activities = [
            /*{ Agent: "Adjetey", Action: "added a note to the ticket", TicketCode: "#241B", CreatedAt: "2 minutes ago" },
            { Agent: "Edwin", Action: "Solved your ticket", TicketCode: "#102W", CreatedAt: "12 minutes ago" },
            { Agent: "Samuel", Action: "closed ticket", TicketCode: "#313S", CreatedAt: "1 hour ago" },
            { Agent: "Ebo", Action: "fowarded your ticket at Axon", TicketCode: "#510N", CreatedAt: "10 seconds ago" }*/
        ];
    }

    $scope.showtickets = function(status) {
        console.log(status);
        $scope.activeClass = status;
    };

    function start() {
        recentActivities();
        ticketStatistics();
    }

    start();
}]);