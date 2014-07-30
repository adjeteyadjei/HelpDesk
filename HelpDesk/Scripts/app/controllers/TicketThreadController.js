angular.module('helpdesk').controller("TicketThreadController", ['$scope', '$http', 'Ticket', 'MsgBox', 'OBJ', '$stateParams',
    function TicketThreadController($scope, $http, Ticket, MsgBox, OBJ, $stateParams) {

        var defaultObj = {
            Id: "",
            Subject: "",
            Description: "",
            AssignedTo: {},
            Project: {},
            Type: {},
            Priority: {}
        };

        //$scope.formTitle = "Add Ticket";
        //var ticketForm = $("#ticket_form");

        $scope.newTicket = {};

        $scope.postComment = function (comment) {
            if ($scope.newTicket.Id) {
                var commentObj = { 'TicketId': $scope.newTicket.Id, 'Comment': comment };
                $http.put('/api/ticket/comment', commentObj).success(function (res) {
                    afterSave(res);
                });
            } else {
                MsgBox.error("No ticket selected.");
            }

        };

        $scope.updateTicket = function (newTicket) {
            var theTicket = OBJ.rectify(angular.copy(newTicket), defaultObj);

            if (theTicket["Id"] || theTicket["Id"] !== "") {
                //this is an update
                Ticket.update(theTicket, function (res) {
                    afterSave(res);
                });
            }
        };

        $scope.forwardTicket = function (team) {
            $http.put('/api/ticket/forward', { Team: team, TicketId: newTicket.Id }).success(function (res) {
                afterSave(res);
            });
        };

        $scope.deleteObj = {};
        $scope.confirmDelete = function (deleteObj) {
            $("#confirm_delete").modal("show");
            $scope.deleteObj = deleteObj;
        };

        $scope.deleteTicket = function () {
            Ticket.delete({ ID: $scope.deleteObj.Id }, function (res) {
                var msg = "";
                if (res.success) {
                    start();
                    msg = res.message || "Ticket deleted";
                    MsgBox.show(msg, "success");
                    $("#confirm_delete").modal("hide");
                    $scope.deleteObj = {};
                } else {
                    msg = res.message || "Sorry errors were encountered";
                    MsgBox.show(msg, "error");
                }
            });
        };

        function getTeams() {
            $http.get('/api/Team').success(function (res) {
                $scope.teams = res.data;
            });
        }

        function getTypes() {
            $http.get('/api/Type').success(function (res) {
                $scope.types = res.data;
            });
        }

        function getAgents() {
            $http.get('/api/account/users').success(function (res) {
                $scope.agents = res.data;
            });
        }

        function getPriorities() {
            $http.get('/api/Priority').success(function (res) {
                $scope.priorities = res.data;
            });
        }

        $scope.solvedTicket = function (ticket) {
            $http.put('/api/ticket/resolve', { TicketId: ticket.Id }).success(function (res) {
                afterSave(res);
            });
        };

        $scope.closeTicket = function (ticket) {
            $http.put('/api/ticket/close', { TicketId: ticket.Id }).success(function (res) {
                afterSave(res);
            });
        };

        function afterSave(res) {
            if (res.success) {
                MsgBox.success(res.message);
                $scope.newTicket = res.data;
                $scope.comment = "";
            } else {
                MsgBox.success(res.message);
            }
        }

        function loadTicket() {
            $scope.loading = "fa-spin";
            if ($stateParams.id > -1) {
                var id = $stateParams.id;
                $http.get("/api/ticket/open?TicketId=" + id).success(function (res) {
                    if (res.success) {
                        $scope.newTicket = res.data;
                    } else {
                        MsgBox.notice(res.message);
                        $scope.newTicket = {};
                    }
                    $scope.loading = "";
                });
            };
        }

        function start() {
            getTeams();
            getTypes();
            getPriorities();
            getAgents();
            loadTicket();
        }

        start();
    }]);