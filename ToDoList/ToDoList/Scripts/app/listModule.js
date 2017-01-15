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

    ListService.executeTask = function (idTask, change) {
        var apiUrl = 'api/TaskItems/ExecuteItem/' + idTask;
        return $http({
            url: apiUrl,
            data: change,
            method: "PUT"
        });
    }

    //add new list in database
    ListService.addList = function (list) {
        return $http({
            url: 'api/Lists',
            data: list,
            method: "POST"
        });
    }

    //remove list from database
    ListService.removeList = function (id) {
        var apiUrl = 'api/Lists/' + id;
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
        showTaskModal: false,
        showFinished: false,
        newTask: {},
        newList: {},
    };

    //receive all tasks and lists from server
    $scope.initialize = function(){
        listService.getTasks()
        .success(function (allTasks) {
            $scope.data.tasks = allTasks.$values;

            //initialize checkbox depend from date finish 
            angular.forEach($scope.data.tasks, function (task) {
                task.Check = $scope.getDateCheck(task.Finished);
            });
        });

        listService.getLists()
        .success(function (allLists) {
            $scope.data.lists = allLists.$values;
        });
    }

    //add new task 
    $scope.addTask = function (isValid) {
        if (isValid) {
            listService.addTask($scope.data.newTask)
            .then(function (response) {
                var task = response.data;
                task.Check = $scope.getDateCheck(task.Finished);
                $scope.data.tasks.push(response.data);
                $scope.data.newTask = {};
                $scope.data.showTaskModal = false;
            });
        }
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

    //add new list
    $scope.addList = function (isValid) {
        if (isValid) {
            listService.addList($scope.data.newList)
            .then(function (response) {
                $scope.data.lists.push(response.data);
                $scope.data.newList = {};
            });
        }
    }

    //remove selected list
    $scope.removeList = function (item) {
        listService.removeList(item.Id)
        .then(function (ans) {
            //remove child tasks
            $scope.data.tasks = $scope.data.tasks.filter(function (element, index) {
                return (element.ListId != item.Id);
            });

            var index = $scope.data.lists.indexOf(item);
            $scope.data.lists.splice(index, 1);

            //select menu list all items
            $scope.data.selectList = -1;
        });
    }

    $scope.checkDate = function (id) {
        var task = $scope.data.tasks.filter(function (element, index) {
            return (element.Id == id);
        });
        listService.executeTask(task[0].Id, task[0].Check).then(function (ans) {

        });
    }

    $scope.getDateCheck = function(date){
        if (date === "0001-01-01T00:00:00") {
            return false;
        }
        else {
            return true;
        }
    }

    //get from server and add all user items to view
    $scope.initialize();
});