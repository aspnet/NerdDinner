(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .controller('DinnersListController', DinnersListController)
        .controller('DinnersDetailController', DinnersDetailController)
        .controller('DinnersAddController', DinnersAddController)
        .controller('DinnersEditController', DinnersEditController)
        .controller('DinnersDeleteController', DinnersDeleteController)
        .controller('LoginController', LoginController)
        .controller('RegisterController', RegisterController)
        .controller('NavController', NavController);

    /* Nav Controller  */
    NavController.$inject = ['$scope', '$location', 'AuthService'];

    function NavController($scope, $location, AuthService) {
        $scope.isActive = function (viewLocation) {
            return viewLocation === $location.path();
        };
        $scope.$watch(AuthService.isLoggedIn, function (isLoggedIn) {
            $scope.isLoggedIn = isLoggedIn;
            $scope.currentUser = AuthService.currentUser();
        });
        $scope.logOff = function () {
            AuthService.logOff();
        };
    }

    /* Login Controller  */
    LoginController.$inject = ['$scope', '$routeParams', '$location', 'AuthService'];

    function LoginController($scope, $routeParams, $location, AuthService) {
        $scope.loginForm = {
            userName: '',
            password: '',
            returnUrl: $routeParams.returnUrl,
            loginFailure: false
        };

        $scope.login = function () {
            var result = AuthService.login($scope.loginForm.userName, $scope.loginForm.password);
            result.then(function (result) {
                if (result.success) {
                    $scope.isUserLoggedIn = true;
                    if ($scope.loginForm.returnUrl !== undefined) {
                        $location.path($scope.loginForm.returnUrl);
                    } else {
                        $location.path('/');
                    }
                } else {
                    $scope.isUserLoggedIn = false;
                    $scope.loginForm.loginFailure = true;
                }
            });
        }

        $scope.externalLogin = function (provider) {
            var result = AuthService.externalLogin(provider, 'http://localhost:5002/');
            result.then(function (result) {
                if (result.success) {
                    $scope.isUserLoggedIn = true;
                    if ($scope.loginForm.returnUrl !== undefined) {
                        $location.path($scope.loginForm.returnUrl);
                    } else {
                        $location.path('/');
                    }
                } else {
                    $scope.isUserLoggedIn = false;
                    $scope.loginForm.loginFailure = true;
                }
            });
        }
    }

    /* Register Controller  */
    RegisterController.$inject = ['$scope', '$routeParams', '$location', 'AuthService'];

    function RegisterController($scope, $routeParams, $location, AuthService) {
        $scope.registerForm = {
            userName: '',
            password: '',
            confirmPassword: '',
            registrationFailure: false
        };

        $scope.register = function () {
            var result = AuthService.register($scope.registerForm.userName, $scope.registerForm.password, $scope.registerForm.confirmPassword);
            result.then(function (result) {
                if (result.success) {
                    $scope.isUserLoggedIn = true;
                    $location.path('/');
                } else {
                    $scope.isUserLoggedIn = false;
                    $scope.registerForm.registrationFailure = true;
                }
            });
        }
    }

    /* Dinners List Controller  */
    DinnersListController.$inject = ['$scope', '$location', 'Dinner'];

    function DinnersListController($scope, $location, Dinner) {
        $scope.dinners = Dinner.query();
        $scope.myDinners = Dinner.query({'myDinners':'true'});
        $scope.selectDinner = function (dinnerId) {
            $location.path('/dinners/details/' + dinnerId);
        };
    }

    /* Dinners Detail Controller  */
    DinnersDetailController.$inject = ['$scope', '$routeParams', 'Dinner'];

    function DinnersDetailController($scope, $routeParams, Dinner) {
        $scope.dinner = Dinner.get({ id: $routeParams.id });
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