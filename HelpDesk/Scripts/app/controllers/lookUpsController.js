var app = angular.module('helpdesk.controllers', []);

app.controller("LookUpController", ['$scope', '$http', 'MsgBox', 'OBJ', function LookUpController($scope, $http, MsgBox, OBJ) {

    var _default = {
        Id: "",
        Name: "",
        Description: ""
    };

    $scope.roomtypes = [];
    $scope.formTitle = "Add ";
    var lookUpForm = $("#lookUp_form");

    $scope.lookUpModels = [
        { name: "Ticket Status", model: "Status" },
        { name: "Ticket Type", model: "Type" },
        { name: "Ticket Priority", model: "Priority" }
    ];

    function getLookUps() {
        if ($scope.lookUpModel) {
            $http.get('/api/' + $scope.lookUpModel.model).success(function(res) {
                $scope.lookUps = res.data;
            });
        }
    }

    $scope.selectModel = function() {
        getLookUps();
    };

    $scope.saveLookUp = function(newLookUp) {
        //retrieve the model from the client and extend the object with the defaults
        var newEntry = OBJ.rectify(angular.copy(newLookUp), _default);

        if (newEntry["Id"] || newEntry["Id"] !== "") {
            //this is an update
            $http.put('/api/' + $scope.lookUpModel.model, newEntry).success(function(res) {
                afterSave(res);
            });
        } else {
            //we are creating a new cell
            $http.post('/api/' + $scope.lookUpModel.model, newEntry).success(function(res) {
                afterSave(res);
            });
        }
    };

    $scope.deleteObj = {};
    $scope.confirmDelete = function(deleteObj) {
        lookUpForm.modal("hide");
        $("#confirm_delete").modal("show");
        $scope.deleteObj = deleteObj;
    };

    $scope.deleteLookUp = function() {
        $http.delete('/api/' + $scope.lookUpModel.model + "?id=" + $scope.deleteObj.Id).success(function(res) {
            if (res.success) {
                start();
                MsgBox.success(res.message);
                $("#confirm_delete").modal("hide");
                $scope.deleteObj = {};
            } else {
                MsgBox.error(res.message);
            }
        });
    };

    $scope.showNewLookUp = function() {
        $scope.clear();
        $scope.formTitle = "Add " + $scope.lookUpModel.name;
    };
    $scope.editLookUp = function(lookUp) {
        $scope.formTitle = "Edit " + $scope.lookUpModel.name;
        $scope.newLookUp = angular.copy(lookUp);
        lookUpForm.modal("show");
    };

    function afterSave(res, callback) {
        var msg = "";
        if (res.success) {
            //reload data into grid
            getLookUps();
            MsgBox.show(res.message, "success");
            $scope.clear();
            lookUpForm.modal("hide");
            //any other business
            if (callback)
                callback();
        } else {
            msg = res.message || "Sorry, errors were ecountered";
            MsgBox.show(res.message, "error");
        }
    }

    $scope.clear = function() {
        $scope.newLookUp = {};
    };

    function start() {
        getLookUps();
    }

    start();
}]);