var app = angular.module('helpdesk.controllers', []);

app.controller("TicketController", ['$scope', '$http', 'Ticket', 'MsgBox', 'OBJ', function TicketController($scope, $http, Ticket, MsgBox, OBJ) {

    var defaultObj = {
        Id: "",
        Subject: "",
        Description: "",
        AssignedTo: {},
        Project: {},
        Type: {},
        Priority: {}
    };
    $scope.showCreate = false;
    $scope.grid_size = "col-sm-12";
    $scope.tickets = [];
    $scope.formTitle = "Add Ticket";
    var ticketForm = $("#ticket_form");

    function getTickets(callback) {
        Ticket.get(function (res) {
            $scope.tickets = angular.copy(res.data);
            if (callback)
                callback(res);
        });
    }

    $scope.showCreateTicket = function() {
        $scope.showCreate = true;
        $scope.grid_size = "col-sm-8";
    };

    $scope.closeTicketForm = function() {
        $scope.showCreate = false;
        $scope.grid_size = "col-sm-12";
    };

    $scope.saveComment = function (comment) {

    };

    $scope.projectChange = function () {
        $scope.tickets = [];
        
        $http.get('/api/ticket?ProjectId=' + $scope.selectedProject.Id).success(function (res) {
            $scope.tickets = res.data;
            if (res.success) {
                MsgBox.info(res.message);
            } else {
                MsgBox.notice(res.message);
            }
        });
    };

    $scope.saveTicket = function (newTicket) {
        //retrieve the model from the client and extend the object with the defaults
        newTicket.Project = $scope.selectedProject;
        var theTicket = OBJ.rectify(angular.copy(newTicket), defaultObj);

        if (theTicket["Id"] || theTicket["Id"] !== "") {
            //this is an update
            Ticket.update(theTicket, function (res) {
                afterSave(res);
            });
        } else {
            //we are creating a new cell
            var ticket = new Ticket(theTicket);
            ticket.$save(function (res) {
                afterSave(res);
            });
        }
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
    
    function getProjects() {
        $http.get('/api/project').success(function (res) {
            $scope.projects = res.data;
        });
    }

    function getPriorities() {
        $http.get('/api/Priority').success(function (res) {
            $scope.priorities = res.data;
        });
    }

    $scope.openTicket = function () {
        MsgBox.show("Sorry this functionality is under construction.", "notice");
    };

    function afterSave(res, callback) {
        var msg = "";
        if (res.success) {
            //reload data into grid
            getTickets();
            msg = res.message || "Ticket Saved Successful.";
            MsgBox.success(msg);
            $scope.clear();
            ticketForm.modal("hide");

            //any other business
            if (callback)
                callback();
        } else {
            msg = res.message || "Sorry, errors were ecountered";
            MsgBox.error(msg);
        }
    }

    function loadTicket() {
        var code = $("#ticket_code").val();
        if (code) {
            $http.get("/api/ticket/" + code).success(function (res) {
                $scope.newTicket = res.data;
            });
        }
    }
    

    $scope.clear = function () {
        $scope.newTicket = {};
    };

    $scope.reload = function () {
        start();
    };

    function start() {
        getProjects();
        getTickets();
        getTypes();
        getPriorities();
        getAgents();
        loadTicket();
    }

    start();
}]);
