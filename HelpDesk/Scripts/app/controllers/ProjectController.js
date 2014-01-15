var app = angular.module('helpdesk.controllers', []);

app.controller("ProjectController", ['$scope', 'Project', '$http', 'MsgBox', 'OBJ', 'pagingService', function ProjectController($scope, Project, $http, MsgBox, OBJ, pagingService) {

    var defaultObj = {
        Id: "",
        Name: "",
        Description: "",
        Leaders: [],
        Teams: [],
        IsActive: false
    };
    //var pagerId = "project_pager";
    //var objPager = pagingService.add(pagerId);

    $scope.projects = [];
    $scope.formTitle = "Add Project";
    var projectForm = $("#project_form");

    $scope.disable = true;

    function getProjects(callback) {
        Project.get(function (res) {
            $scope.projects = angular.copy(res.data);
            //objPager.recalibrate(res);
            if (callback)
                callback(res);
        });
    }

    function getTeams() {
        $http.get('/api/Team').success(function (res) {
            $scope.teams = res.data;
        });
    }
    
    $scope.teamChange = function (teams) {
        $scope.teamAgents = [];
        if (teams) {
            $.each(teams, function (i, team) {
                $.each(team.Members, function (x, member) {
                    $scope.teamAgents.push(member);
                });
            });
        }
    };

    $scope.leadSelected = function(leaders) {
        if (leaders) {
            if (leaders.length < 1) {
                return false;
            } else {
                return true;
            }
        }
        return false;
    };

    $scope.saveProject = function (newProject) {
        //retrieve the model from the client and extend the object with the defaults
        var theProject = OBJ.rectify(angular.copy(newProject), defaultObj);

        if (theProject["Id"] || theProject["Id"] !== "") {
            //this is an update
            Project.update(theProject, function (res) {
                afterSave(res);
            });
        } else {
            //we are creating a new cell
            var project = new Project(theProject);
            project.$save(function (res) {
                afterSave(res);
            });
        }
    };
    $scope.deleteObj = {};
    $scope.confirmDelete = function (deleteObj) {
        projectForm.modal("hide");
        $("#confirm_delete").modal("show");
        $scope.deleteObj = deleteObj;
    };
    $scope.deleteProject = function () {
        Project.delete({ ID: $scope.deleteObj.Id }, function (res) {
            var msg = "";
            if (res.success) {
                start();
                msg = res.message || "Project deleted";
                MsgBox.show(msg, "success");
                $("#confirm_delete").modal("hide");
                $scope.deleteObj = {};
            } else {
                msg = res.message || "Sorry errors were encountered";
                MsgBox.show(res.message, "error");
            }
        });
    };

    $scope.teamOptions = {
        'multiple': true,
        'simple_tags': true
    };


    $scope.showNewProject = function () {
        $scope.clear();
        $scope.formTitle = "Add Project";
    };

    $scope.editProject = function (project) {
        $scope.formTitle = "Edit Project";
        $scope.newProject = angular.copy(project);
        console.log($scope.newProject);
        projectForm.modal("show");
    };

    function afterSave(res, callback) {
        var msg = "";
        if (res.success) {
            //reload data into grid
            getProjects();
            msg = res.message || "Project Saved Successful.";
            MsgBox.show(msg, "success");
            $scope.clear();
            projectForm.modal("hide");

            //any other business
            if (callback)
                callback();
        } else {
            msg = res.message || "Sorry, errors were ecountered";
            MsgBox.show(msg, "error");
        }
    }

    $scope.clear = function () {
        $scope.newProject = {};
    };
    $scope.reload = function () {
        start();
    };

    function start() {
        getProjects();
        getTeams();
        $scope.newProject = {Leaders: []};
    }

    start();
}]);