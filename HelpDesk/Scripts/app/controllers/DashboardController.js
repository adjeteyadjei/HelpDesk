/// <reference path="../../jquery-ui-1.8.24.js" />
var app = angular.module('helpdesk.controllers', []);

app.controller("DashboardController", ['$scope', '$http', "OBJ", function DashboardController($scope, $http, OBJ) {

    $scope.activeClass = "";
    $scope.activities = [];

    function ticketStatistics(data) {
        $scope.ticketStats = {
            New: [],
            Open: [],
            Pending: [],
            Solved: []
        };

        $scope.ticketStats = OBJ.rectify(data, $scope.ticketStats);
    }

    function getSummaries() {
        $http.get("/api/dashboard/summaries").success(function (res) {
            //console.log(res.data);
            ticketStatistics(res.data.TicketStats);
            recentActivities(res.data.Activities);
        });
    }

    function recentActivities(data) {
        $scope.activities = data;
    }

    $scope.showtickets = function(status) {
        $scope.activeClass = status;
        switch (status) {
            case 'New':
                $scope.tickets = $scope.ticketStats.New;
                break;
            case "Open":
                $scope.tickets = $scope.ticketStats.Open;
                break;
            case "Pending":
                $scope.tickets = $scope.ticketStats.Pending;
                break;
            default:
                $scope.tickets = $scope.ticketStats.Solved;
        }
        console.log(status);
        console.log($scope.tickets);

    };

    function start() {
        getSummaries();
    }

    start();
}]);