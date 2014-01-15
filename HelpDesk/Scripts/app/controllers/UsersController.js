var app = angular.module('helpdesk.controllers', []);

app.controller("UserController", ['$scope', '$http', 'MsgBox', 'OBJ', 'pagingService', function UserController($scope, $http, MsgBox, OBJ, pagingService) {

    var _default = {
        Id: "",
        UserName: "",
        FullName: "",
        PhoneNumber: "",
        Email: "",
        IsAdmin: false,
    };
    var pagerId = "user_pager";
    var objPager = pagingService.add(pagerId);

    $scope.users = [];
    $scope.formTitle = "Add User";
    var userForm = $("#user_form");


    function getUsers() {
        $http.get('/api/account/users').success(function (res) {
            $scope.users = res.data;
        });
    }

    function getTeams() {
        $http.get('/api/team').success(function(res) {
            $scope.teams = res.data;
        });
    }

    $scope.saveUser = function(newUser) {
        //retrieve the model from the client and extend the object with the defaults
        var theUser = OBJ.rectify(angular.copy(newUser), _default);

        if (theUser["Id"] || theUser["Id"] !== "") {
            $http.put("/api/account/update", theUser).success(function(res) {
                afterSave(res);
            });
        } else {
            $http.post("/api/account/signup", theUser).success(function (res) {
                afterSave(res);
            });
        }
    };
    $scope.deleteObj = {};
    $scope.confirmDelete = function(deleteObj) {
        userForm.modal("hide");
        $("#confirm_delete").modal("show");
        $scope.deleteObj = deleteObj;
    };
    $scope.deleteUser = function() {
        $http.delete("/api/account/delete", $scope.deleteObj).success(function(res) {
            var msg = "";
            if (res.success) {
                start();
                msg = res.message || "User deleted";
                MsgBox.show(msg, "success");
                $("#confirm_delete").modal("hide");
                $scope.deleteObj = {};
            } else {
                msg = res.message || "Sorry errors were encountered";
                MsgBox.show(msg);
            }
        });
    };
    $scope.showNewUser = function() {
        $scope.clear();
        $scope.formTitle = "Add User";
    };

    $scope.editUser = function(user) {
        $scope.formTitle = "Edit User";
        $scope.newUser = angular.copy(user);
        //console.log($scope.newUser.UserType);
        //$scope.newUser.UserType = "Standard";
        userForm.modal("show");
    };

    function afterSave(res, callback) {
        var msg = "";
        if (res.success) {
            //reload data into grid
            getUsers();
            msg = res.message || "User Saved Successful.";
            MsgBox.show(msg, "success");
            $scope.clear();
            userForm.modal("hide");

            //any other business
            if (callback)
                callback();
        } else {
            msg = res.message || "Sorry, errors were ecountered";
            MsgBox.show(res.message, "error");
        }
    }

    $scope.clear = function() {
        $scope.newUser = {};
    };
    $scope.reload = function() {
        start();
    };

    function start() {
        getUsers();
        getTeams();
    }

    start();
}]);