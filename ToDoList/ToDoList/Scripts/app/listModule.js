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
    //add task in list in database
    ListService.addTask = function (task) {
        return $http({
            url: 'api/TaskItems',
            data: task,
            method: "POST"
        });
    }
    //remove task from list in database
    ListService.removeTask = function (id) {
        var apiUrl = 'api/TaskItems/' + id;
        return $http({
            url: apiUrl,
            method: "DELETE"
        });
    }

    return ListService;
}]);
app.controller("listCtrl", function ($scope, listService) {
    //view data
    $scope.data = {
        selectList: -1,
        newTask: {},
        showTaskModal: false
    };

    //receive all tasks and lists from server
    $scope.initialize = function(){
        listService.getTasks()
        .success(function (allTasks) {
            $scope.data.tasks = allTasks.$values;
        });

        listService.getLists()
        .success(function (allLists) {
            $scope.data.lists = allLists.$values;
        });
    }

    //add new task 
    $scope.addTask = function () {
        listService.addTask($scope.data.newTask)
        .then(function (response) {
            $scope.data.tasks.push(response.data);
            $scope.data.newTask = {};
            $scope.data.showTaskModal = false;
        }); 
    }

    //clear and hide task modal
    $scope.cancelTask = function () {
        $scope.data.newTask = {};
        $scope.data.showTaskModal = false;
    }

    //remove selected task
    $scope.removeTask = function (item) {
        listService.removeTask(item.Id)
        .then(function (ans) {
            var index = $scope.data.tasks.indexOf(item);
            $scope.data.tasks.splice(index, 1);
        });
    }

    $scope.initialize();
});