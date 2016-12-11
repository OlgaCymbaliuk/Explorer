app.controller('ShowAllCtrl', function ($scope, DirectoryManager) {

    $scope.goToChild = function (value) {
        return DirectoryManager.GoToChild(patch($scope.CurrentPath), patch(value))
    }

    $scope.goToParent = function (value) {

        return DirectoryManager.GoToParent(patch(value));
    }

    $scope.InvokeAction = function (action, value) {
        if (angular.isFunction(action)) {
            $scope.HttpInProgress = true;
            action(value).then(function (response) {
                    fill(response.data)
                $scope.HttpInProgress = false;
            }, function (err) { alert(err.status) });
        }
    }
    function fill(object) {
        $scope.CurrentPath = object.CurrentPath;
        $scope.Items = object.Items;
        $scope.SmallFiles = object.SmallFiles;
        $scope.MediumFiles = object.MediumFiles;
        $scope.LargeFiles = object.LargeFiles;
        $scope.IsRoot = object.IsRoot;
        $scope.Errors = object.ErrorsList;
    }

    function patch(value) {
        return value.replace(/(\\|\:)/g, "|");
    }
    $scope.HttpInProgress = false;
    $scope.InvokeAction($scope.goToParent, "");
});

app.factory('DirectoryManager', function ($http) {
    var fac = {};
    fac.GoToChild = function (path, value) {
        //alert('/api/App/GetId/' + value);
        return $http.get('/api/App/' + path + '/' + value, { cache: true });
    }

    fac.GoToParent = function (currentPath) {

        return $http.get('/api/App/' + currentPath, { cache: true });
    }

    return fac;
});

