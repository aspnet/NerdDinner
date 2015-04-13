(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .controller('LoginController', LoginController)
        .controller('RegisterController', RegisterController)

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
                validateLogin($scope, $location, result, AuthService)
            });
        }

        $scope.externalLogin = function (provider) {
            var result = AuthService.externalLogin(provider, $location.path);
            result.then(function (result) {
                validateLogin($scope, $location, result, AuthService)
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
            registrationFailure: false,
            errorMessage: ''
        };

        $scope.register = function () {
            var result = AuthService.register($scope.registerForm.userName, $scope.registerForm.password, $scope.registerForm.confirmPassword);
            result.then(function (result) {
                if (AuthService.isUserLoggedIn()) {
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

    function validateLogin($scope, $location, result, AuthService) {
        if (AuthService.isUserLoggedIn()) {
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