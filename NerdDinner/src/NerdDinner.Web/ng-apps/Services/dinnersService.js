(function () {
    'use strict';

    angular
        .module('dinnersService', ['ngResource'])
        .factory('Dinner', Dinner)

    Dinner.$inject = ['$resource'];

    function Dinner($resource) {
        return {
            all: $resource('/api/dinners/:id', { Id: "@Id" }),
            popular: $resource('/api/dinners/popular')
        };
    }
})();