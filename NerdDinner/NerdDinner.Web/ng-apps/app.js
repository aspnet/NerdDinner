(function () {
    'use strict';

    config.$inject = ['$routeProvider', '$locationProvider'];

    angular.module('nerdDinner', [
        'ngRoute', 'ui.bootstrap', 'dinnersService'
    ]).config(config);

    function config($routeProvider, $locationProvider) {
        $routeProvider
            .when('/', {
                title: 'Nerd Dinner',
                templateUrl: '/views/home.html',
                controller: 'homeController'
            })
            .when('/Account/Login', {
                title: 'Nerd Dinner - Log In',
                templateUrl: '/Account/Login',
                controller: 'loginController'
            })
            .when('/Account/Register', {
                title: 'Nerd Dinner - Register',
                templateUrl: '/Account/Register',
                controller: 'registerController'
            })
            .otherwise({ redirectTo: '/' });

        $locationProvider.html5Mode({
            enabled: true,
            requireBase: false
        });
    }
})();