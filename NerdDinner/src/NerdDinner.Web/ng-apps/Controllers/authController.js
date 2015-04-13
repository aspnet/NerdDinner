(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .controller('loginController', loginController)
        .controller('registerController', registerController)

    /* Login Controller  */
    loginController.$inject = ['$scope', '$routeParams', '$location', 'authService'];

    function loginController($scope, $routeParams, $location, authService) {
        $scope.loginForm = {
            userName: '',
            password: '',
            returnUrl: $routeParams.returnUrl,
            loginFailure: false
        };

        $scope.login = function () {
            var result = authService.login($scope.loginForm.userName, $scope.loginForm.password);
            result.then(function (result) {
                validateLogin($scope, $location, result, authService)
            });
        }

        $scope.externalLogin = function (provider) {
            var result = authService.externalLogin(provider, $location.path);
            result.then(function (result) {
                validateLogin($scope, $location, result, authService)
            });
        }
    }

    /* Register Controller  */
    registerController.$inject = ['$scope', '$routeParams', '$location', 'authService'];

    function registerController($scope, $routeParams, $location, authService) {
        $scope.registerForm = {
            userName: '',
            password: '',
            confirmPassword: '',
            registrationFailure: false,
            errorMessage: ''
        };

        $scope.register = function () {
            var result = authService.register($scope.registerForm.userName, $scope.registerForm.password, $scope.registerForm.confirmPassword);
            result.then(function (result) {
                if (authService.isUserLoggedIn()) {
                    if ($scope.registerForm.returnUrl !== undefined) {
                        $location.path($scope.registerForm.returnUrl);
                    } else {
                        $location.path('/');
                    }
                } else {
                    $scope.registerForm.registrationFailure = true;
                    $scope.registerForm.errorMessage = result.error;
                }
            });
        }
    }

    function validateLogin($scope, $location, result, authService) {
        if (authService.isUserLoggedIn()) {
            if ($scope.loginForm.returnUrl !== undefined) {
                $location.path($scope.loginForm.returnUrl);
            } else {
                $location.path('/');
            }
        } else {
            $scope.loginForm.loginFailure = true;
            $scope.loginForm.errorMessage = result.error;
        }
    }
})();