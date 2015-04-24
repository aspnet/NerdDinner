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
            .when('/dinners/all', {
                title: 'Nerd Dinner - All Dinners',
                templateUrl: '/views/list.html',
                controller: 'listController'
            })
            .when('/dinners/my', {
                title: 'Nerd Dinner - My Dinners',
                templateUrl: '/views/my.html',
                controller: 'myController'
            })
            .when('/dinners/detail/:id', {
                title: 'Nerd Dinner - Details',
                templateUrl: '/views/detail.html',
                controller: 'detailController'
            })
            .when('/dinners/edit/:id', {
                title: 'Nerd Dinner - Edit Dinner',
                templateUrl: '/views/edit.html',
                controller: 'editController'
            })
            .when('/dinners/delete/:id', {
                title: 'Nerd Dinner - Delete Dinner',
                templateUrl: '/views/delete.html',
                controller: 'deleteController'
            })
            .when('/account/login', {
                title: 'Nerd Dinner - Log In',
                templateUrl: '/account/login',
                controller: 'loginController'
            })
            .when('/account/register', {
                title: 'Nerd Dinner - Register',
                templateUrl: '/account/register',
                controller: 'registerController'
            })
            .otherwise({ redirectTo: '/' });

        $locationProvider.html5Mode({
            enabled: true,
            requireBase: false
        });
    }
})();