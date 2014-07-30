angular.module('helpdesk').controller("DashboardController", ['$scope', '$http', 'OBJ', '$location',
    function DashboardController($scope, $http, OBJ, $location) {

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
    };

    $scope.openTicket = function (id) {
        if (id) {
            $location.path("/ticket/TicketThread/" + id);
        }
    };

    function start() {
        getSummaries();
    }

    start();
}]);