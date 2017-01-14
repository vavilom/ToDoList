var app = angular.module("listModule", []);
app.factory('listService', ['$http', function ($http) {
    //send text on the server, and get encrypt/decrypt result
    var ListService = {};
    //work with tasks
    ListService.getTasks = function (postData) {
        return $http({
            url: 'api/TaskItems',
            method: "GET"
        });
    };
    //work with lists
    ListService.getLists = function (postData) {
        return $http({
            url: 'api/Lists',
            method: "GET"
        });
    };

    return ListService;
}]);
app.controller("listCtrl", function ($scope, listService) {
    //view data
    $scope.data = {
        selectList: -1
    };

    //receive all tasks and lists from server
    $scope.initialize = function (encState) {
        listService.getTasks()
        .success(function (allTasks) {
            $scope.data.tasks = allTasks.$values;
        });

        listService.getLists()
        .success(function (allLists) {
            $scope.data.lists = allLists.$values;
        });
    }

    $scope.initialize();
});