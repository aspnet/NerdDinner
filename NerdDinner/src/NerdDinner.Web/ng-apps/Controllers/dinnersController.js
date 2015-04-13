(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .controller('dinnersPopularController', dinnersPopularController)
        .controller('dinnersMyController', dinnersMyController)
        .controller('dinnersListController', dinnersListController)
        .controller('dinnersDetailController', dinnersDetailController)
        .controller('dinnersAddController', dinnersAddController)
        .controller('dinnersEditController', dinnersEditController)
        .controller('dinnersDeleteController', dinnersDeleteController)

    /* Dinners Popular Controller  */
    dinnersPopularController.$inject = ['$scope', '$location', 'Dinner'];

    function dinnersPopularController($scope, $location, Dinner) {
        $scope.dinners = Dinner.popular.query();
        $scope.selectDinner = function (dinnerId) {
            $location.path('/dinners/details/' + dinnerId);
        };
    }

    /* Dinners My Controller  */
    dinnersMyController.$inject = ['$scope', '$location', 'Dinner'];

    function dinnersMyController($scope, $location, Dinner) {
        $scope.dinners = Dinner.all.query({ 'myDinners': 'true' });
        $scope.selectDinner = function (dinnerId) {
            $location.path('/dinners/details/' + dinnerId);
        };
    }

    /* Dinners List Controller  */
    dinnersListController.$inject = ['$scope', '$location', 'Dinner'];

    function dinnersListController($scope, $location, Dinner) {
        $scope.dinners = Dinner.all.query();
        $scope.selectDinner = function (dinnerId) {
            $location.path('/dinners/details/' + dinnerId);
        };
    }

    /* Dinners Detail Controller  */
    dinnersDetailController.$inject = ['$scope', '$routeParams', 'Dinner'];

    function dinnersDetailController($scope, $routeParams, Dinner) {
        $scope.dinner = Dinner.all.get({ id: $routeParams.id });
    }

    /* Dinners Create Controller */
    dinnersAddController.$inject = ['$scope', '$location', 'Dinner'];

    function dinnersAddController($scope, $location, Dinner) {
        $scope.dinner = new Dinner();
        $scope.add = function () {
            $scope.dinner.$save(
                // success
                function () {
                    $location.path('/');
                },
                // error
                function (error) {
                    _showValidationErrors($scope, error);
                }
            );
        };
    }

    /* Dinners Edit Controller */
    dinnersEditController.$inject = ['$scope', '$routeParams', '$location', 'Dinner'];

    function dinnersEditController($scope, $routeParams, $location, Dinner) {
        $scope.dinner = Dinner.get({ id: $routeParams.id });
        $scope.edit = function () {
            $scope.dinner.$save(
                // success
                function () {
                    $location.path('/');
                },
                // error
                function (error) {
                    _showValidationErrors($scope, error);
                }
            );
        };
    }

    /* Dinners Delete Controller  */
    dinnersDeleteController.$inject = ['$scope', '$routeParams', '$location', 'Dinner'];

    function dinnersDeleteController($scope, $routeParams, $location, Dinner) {
        $scope.dinner = Dinner.get({ id: $routeParams.id });
        $scope.remove = function () {
            $scope.dinner.$remove({ id: $scope.dinner.DinnerId }, function () {
                $location.path('/');
            });
        };
    }

    /* Utility Functions */
    function _showValidationErrors($scope, error) {
        $scope.validationErrors = [];
        if (error.data && angular.isObject(error.data)) {
            for (var key in error.data) {
                $scope.validationErrors.push(error.data[key][0]);
            }
        } else {
            $scope.validationErrors.push('Could not add dinner.');
        };
    }
})();