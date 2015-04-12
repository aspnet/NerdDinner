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
                controller: 'DinnersPopularController'
            })
            .when('/dinners/all', {
                title: 'Nerd Dinner - All Dinners',
                templateUrl: '/views/list.html',
                controller: 'DinnersListController'
            })
            .when('/dinners/my', {
                title: 'Nerd Dinner - My Dinners',
                templateUrl: '/views/my.html',
                controller: 'DinnersMyController'
            })
            .when('/dinners/details/:id', {
                title: 'Nerd Dinner - Dinner Details',
                templateUrl: '/views/detail.html',
                controller: 'DinnersDetailController'
            })
            .when('/dinners/add', {
                title: 'Nerd Dinner - Host Dinner',
                templateUrl: '/views/add.html',
                controller: 'DinnersAddController'
            })
            .when('/dinners/edit/:id', {
                title: 'Nerd Dinner - Edit Dinner',
                templateUrl: '/views/edit.html',
                controller: 'DinnersEditController'
            })
            .when('/dinners/delete/:id', {
                title: 'Nerd Dinner - Delete Dinner',
                templateUrl: '/views/delete.html',
                controller: 'DinnersDeleteController'
            })
            .when('/login', {
                title: 'Nerd Dinner - Log In',
                templateUrl: '/Account/Login',
                controller: 'LoginController'
            })
            .when('/register', {
                title: 'Nerd Dinner - Register',
                templateUrl: '/Account/Register',
                controller: 'RegisterController'
            })
            .otherwise({ redirectTo: '/' });

        $locationProvider.html5Mode(true);
    }
})();