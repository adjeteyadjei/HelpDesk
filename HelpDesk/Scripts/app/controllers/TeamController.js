angular.module('helpdesk').controller("TeamController", ['$scope', 'Team', '$http', 'MsgBox', 'OBJ', 'pagingService', function TeamController($scope, Team, $http, MsgBox, OBJ, pagingService) {

    var defaultObj = {
        TeamId: "",
        Name: "",
        Description: ""
    };
    var pagerId = "team_pager";
    var objPager = pagingService.add(pagerId);

    $scope.teams = [];
    $scope.formTitle = "Add Team";
    var teamForm = $("#team_form");


    function getTeams(callback) {
        Team.get(function(res) {
            $scope.teams = angular.copy(res.data);
            objPager.recalibrate(res);
            if (callback)
                callback(res);
        });
    }

    $scope.members = [];
    $scope.addMember = function (agent) {
        if (!agent) {
            MsgBox.notice("Please select agent.");
            return;
        }
        if (findMemberInList(agent).length === 0) {
            $scope.members.push(agent);
        } else {
            MsgBox.notice(agent.FullName + " is already added to list.");
        }
    };

    function findMemberInList(obj) {
        var result = $scope.members.filter(function (n) {
            return n.Id === obj.Id;
        });
        return result;
    }

    $scope.removeMember = function (index) {
        $scope.members.splice(index, 1);
    };

    $scope.saveTeam = function(newTeam) {
        //retrieve the model from the client and extend the object with the defaults
        var theTeam = OBJ.rectify(angular.copy(newTeam), defaultObj);

        if (theTeam["TeamId"] || theTeam["TeamId"] !== "") {
            //this is an update
            Team.update(theTeam, function(res) {
                afterSave(res);
            });
        } else {
            //we are creating a new cell
            var team = new Team(theTeam);
            team.$save(function(res) {
                afterSave(res);
            });
        }
    };
    $scope.deleteObj = {};
    $scope.confirmDelete = function(deleteObj) {
        teamForm.modal("hide");
        $("#confirm_delete").modal("show");
        $scope.deleteObj = deleteObj;
    };
    $scope.deleteTeam = function() {
        Team.delete({ ID: $scope.deleteObj.TeamId }, function(res) {
            var msg = "";
            if (res.success) {
                start();
                msg = res.message || "Team deleted";
                MsgBox.show(msg, "success");
                $("#confirm_delete").modal("hide");
                $scope.deleteObj = {};
            } else {
                msg = res.message || "Sorry errors were encountered";
                MsgBox.show(res.message, "error");
            }
        });
    };

    function getAgents() {
        $http.get("/api/account/users").success(function(res) {
            $scope.agents = res.data;
        });
    }

    $scope.membersOptions = {
        'multiple': true,
        initSelection: function (element, callback) {
            callback($(element).data('$ngModelController').$modelValue);
        }
    };

    $scope.showNewTeam = function() {
        $scope.clear();
        $scope.formTitle = "Add Team";
    };

    $scope.editTeam = function(team) {
        $scope.formTitle = "Edit Team";
        $scope.newTeam = angular.copy(team);
        $scope.members = $scope.newTeam.Members;
        teamForm.modal("show");
    };

    function afterSave(res, callback) {
        var msg = "";
        if (res.success) {
            //reload data into grid
            getTeams();
            msg = res.message || "Team Saved Successful.";
            MsgBox.show(msg, "success");
            $scope.clear();
            teamForm.modal("hide");

            //any other business
            if (callback)
                callback();
        } else {
            msg = res.message || "Sorry, errors were ecountered";
            MsgBox.show(msg, "error");
        }
    }

    $scope.clear = function() {
        $scope.newTeam = {};
    };
    $scope.reload = function() {
        start();
    };

    function start() {
        getTeams();
        getAgents();
    }

    start();
}]);