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
                templateUrl: '/views/popular.html',
                controller: 'dinnersPopularController'
            })
            .when('/dinners/all', {
                title: 'Nerd Dinner - All Dinners',
                templateUrl: '/views/list.html',
                controller: 'dinnersListController'
            })
            .when('/dinners/my', {
                title: 'Nerd Dinner - My Dinners',
                templateUrl: '/views/my.html',
                controller: 'dinnersMyController'
            })
            .when('/dinners/details/:id', {
                title: 'Nerd Dinner - Dinner Details',
                templateUrl: '/views/detail.html',
                controller: 'dinnersDetailController'
            })
            .when('/dinners/add', {
                title: 'Nerd Dinner - Host Dinner',
                templateUrl: '/views/add.html',
                controller: 'dinnersAddController'
            })
            .when('/dinners/edit/:id', {
                title: 'Nerd Dinner - Edit Dinner',
                templateUrl: '/views/edit.html',
                controller: 'dinnersEditController'
            })
            .when('/dinners/delete/:id', {
                title: 'Nerd Dinner - Delete Dinner',
                templateUrl: '/views/delete.html',
                controller: 'dinnersDeleteController'
            })
            .when('/login', {
                title: 'Nerd Dinner - Log In',
                templateUrl: '/Account/Login',
                controller: 'loginController'
            })
            .when('/register', {
                title: 'Nerd Dinner - Register',
                templateUrl: '/Account/Register',
                controller: 'registerController'
            })
            .otherwise({ redirectTo: '/' });

        $locationProvider.html5Mode(true);
    }
})();