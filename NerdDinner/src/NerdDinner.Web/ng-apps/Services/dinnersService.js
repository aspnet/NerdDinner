(function () {
    'use strict';

    angular
        .module('dinnersService', ['ngResource'])
        .factory('Dinner', Dinner);

    Dinner.$inject = ['$resource'];

    function Dinner($resource) {
        return $resource('/api/dinners/:id');
    }
})();