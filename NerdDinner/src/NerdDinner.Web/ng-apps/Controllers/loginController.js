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
                validateLogin($scope, $location, result)
            });
        }

        $scope.externalLogin = function (provider) {
            var result = AuthService.externalLogin(provider, 'http://localhost:5002/');
            result.then(function (result) {
                validateLogin($scope, $location, result)
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
                    if ($scope.registerForm.returnUrl !== undefined) {
                        $location.path($scope.registerForm.returnUrl);
                    } else {
                        $location.path('/');
                    }
                } else {
                    $scope.isUserLoggedIn = false;
                    $scope.registerForm.registrationFailure = true;
                }
            });
        }

        function validateLogin($scope, $location, result) {
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
        }
    }
})();