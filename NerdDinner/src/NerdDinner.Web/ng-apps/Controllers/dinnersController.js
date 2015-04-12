(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .controller('DinnersPopularController', DinnersPopularController)
        .controller('DinnersMyController', DinnersMyController)
        .controller('DinnersListController', DinnersListController)
        .controller('DinnersDetailController', DinnersDetailController)
        .controller('DinnersAddController', DinnersAddController)
        .controller('DinnersEditController', DinnersEditController)
        .controller('DinnersDeleteController', DinnersDeleteController)

    /* Dinners Popular Controller  */
    DinnersPopularController.$inject = ['$scope', '$location', 'Dinner'];

    function DinnersPopularController($scope, $location, Dinner) {
        $scope.dinners = Dinner.popular.query();
        $scope.selectDinner = function (dinnerId) {
            $location.path('/dinners/details/' + dinnerId);
        };
    }

    /* Dinners My Controller  */
    DinnersMyController.$inject = ['$scope', '$location', 'Dinner'];

    function DinnersMyController($scope, $location, Dinner) {
        $scope.dinners = Dinner.all.query({ 'myDinners': 'true' });
        $scope.selectDinner = function (dinnerId) {
            $location.path('/dinners/details/' + dinnerId);
        };
    }

    /* Dinners List Controller  */
    DinnersListController.$inject = ['$scope', '$location', 'Dinner'];

    function DinnersListController($scope, $location, Dinner) {
        $scope.dinners = Dinner.all.query();
        $scope.selectDinner = function (dinnerId) {
            $location.path('/dinners/details/' + dinnerId);
        };
    }

    /* Dinners Detail Controller  */
    DinnersDetailController.$inject = ['$scope', '$routeParams', 'Dinner'];

    function DinnersDetailController($scope, $routeParams, Dinner) {
        $scope.dinner = Dinner.all.get({ id: $routeParams.id });
    }

    /* Dinners Create Controller */
    DinnersAddController.$inject = ['$scope', '$location', 'Dinner'];

    function DinnersAddController($scope, $location, Dinner) {
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
    DinnersEditController.$inject = ['$scope', '$routeParams', '$location', 'Dinner'];

    function DinnersEditController($scope, $routeParams, $location, Dinner) {
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
    DinnersDeleteController.$inject = ['$scope', '$routeParams', '$location', 'Dinner'];

    function DinnersDeleteController($scope, $routeParams, $location, Dinner) {
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