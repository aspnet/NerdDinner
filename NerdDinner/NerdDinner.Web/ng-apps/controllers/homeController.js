(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .controller('homeController', homeController)
        .controller('listController', listController)
        .controller('myController', myController)
        .controller('detailController', detailController)
        .controller('editController', editController)
        .controller('deleteController', deleteController);

    /* Home Controller  */
    homeController.$inject = ['$scope', '$location', 'dinner', 'mapService'];

    function homeController($scope, $location, dinner, mapService) {
        $scope.dinners = dinner.popular.query();

        $scope.selectDinner = function (dinnerId) {
            $location.path('/dinners/detail/' + dinnerId);
        };

        $scope.loadMap = function () {
            mapService.loadMap($scope.dinners, 4);
        };

        $scope.showLocationPin = function (dinner) {
            mapService.showInfoBoxPin(dinner);
        };

        $scope.showLocation = function (searchText) {
            mapService.findAddress(searchText, false);
        }
    }

    /* Dinners List Controller  */
    listController.$inject = ['$scope', '$location', 'dinner'];

    function listController($scope, $location, dinner) {
        dinner.count().then(function (result) {
            $scope.bigTotalItems = result.success;
        });

        $scope.maxSize = 3;
        $scope.bigCurrentPage = 1;
        $scope.itemPerPage = 12;

        $scope.dinners = dinner.all.query({ pageIndex: $scope.bigCurrentPage, pageSize: $scope.itemPerPage });

        $scope.selectDinner = function (dinnerId) {
            $location.path('/dinners/detail/' + dinnerId);
        };

        $scope.pageChanged = function () {
            $scope.dinners = dinner.all.query({ pageIndex: $scope.bigCurrentPage, pageSize: $scope.itemPerPage });
        };
    }

    /* Dinners My Controller  */
    myController.$inject = ['$scope', '$location', 'dinner'];

    function myController($scope, $location, dinner) {
        $scope.dinners = dinner.my.query();
        $scope.selectDinner = function (dinnerId) {
            $location.path('/dinners/detail/' + dinnerId);
        };
    }

    /* Dinners Detail Controller  */
    detailController.$inject = ['$scope', '$routeParams', '$location', 'dinner', 'mapService'];

    function detailController($scope, $routeParams, $location, dinner, mapService) {
        $scope.dinner = dinner.all.get({ id: $routeParams.id });

        var hostResult = dinner.isUserHost($routeParams.id);
        hostResult.then(function (result) {
            $scope.isUserHost = result.success;
        });

        var registeredResult = dinner.isUserRegistered($routeParams.id);
        registeredResult.then(function (result) {
            $scope.isUserRegistered = result.success;
        });

        $scope.editDinner = function (dinnerId) {
            $location.path('/dinners/edit/' + dinnerId);
        };

        $scope.deleteDinner = function (dinnerId) {
            $location.path('/dinners/delete/' + dinnerId);
        };

        $scope.loadMap = function () {
            mapService.loadMap($scope.dinner);
        }
    }

    /* Dinners Edit Controller  */
    editController.$inject = ['$scope', '$routeParams', '$location', 'dinner', 'mapService'];

    function editController($scope, $routeParams, $location, dinner, mapService) {

        $scope.dinner = dinner.all.get({ id: $routeParams.id });

        $scope.$watch(eventDate, function (newValue) {
            $scope.dinner.eventDate = newValue;
        });

        $scope.$watch('dinner.eventDate', function (newValue) {
            $scope.eventDate = new Date($scope.dinner.eventDate);
        });

        $scope.loadMap = function () {
            mapService.loadMap($scope.dinner);
        }

        $scope.changeAddress = function (address) {
            mapService.findAddress(address, true);
        }

        $scope.edit = function () {
            var result = dinner.editDinner($routeParams.id, $scope.dinner);
            result.then(function (result) {
                if (result.success) {
                    $location.path('/dinners/detail/' + $routeParams.id);
                }
            });
        }

        $scope.cancel = function () {
            $location.path('/dinners/detail/' + $routeParams.id);
        }
    }

    /* Dinners Delete Controller */
    deleteController.$inject = ['$scope', '$routeParams', '$location', 'dinner'];

    function deleteController($scope, $routeParams, $location, dinner) {
        $scope.dinner = dinner.all.get({ id: $routeParams.id });

        $scope.delete = function () {
            var result = dinner.deleteDinner($routeParams.id);
            result.then(function (result) {
                if (result.success) {
                    $location.path('/dinners/my');
                }
            });
        };

        $scope.cancel = function () {
            $location.path('/dinners/detail/' + $routeParams.id);
        };
    }

})();