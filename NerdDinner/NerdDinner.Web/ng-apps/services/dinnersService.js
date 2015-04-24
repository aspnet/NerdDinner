(function () {
    'use strict';

    angular
        .module('dinnersService', ['ngResource'])
        .factory('dinner', dinner)

    dinner.$inject = ['$resource', '$http', '$q'];

    function dinner($resource, $http, $q) {
    }
})();