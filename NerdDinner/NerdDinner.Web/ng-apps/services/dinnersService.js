(function () {
    'use strict';

    angular
        .module('dinnersService', ['ngResource'])
        .factory('dinner', dinner)

    dinner.$inject = ['$resource', '$http', '$q'];

    function dinner($resource, $http, $q) {
        return {
            all: $resource('/api/dinners/:id', { id: "@_id" }),

            popular: $resource('/api/dinners/popular'),

            my: $resource('/api/dinners/my'),

            count: function () {
                return dinnerHelper($http, $q, '/api/dinners/count')
            },

            isUserHost: function (dinnerId) {
                return dinnerHelper($http, $q, '/api/dinners/isUserHost?id=' + dinnerId)
            },

            isUserRegistered: function (dinnerId) {
                return dinnerHelper($http, $q, '/api/dinners/isUserRegistered?id=' + dinnerId)
            },

            editDinner: function (dinnerId, dinner) {
                var deferredObject = $q.defer();
                $http.put(
                    '/api/dinners/' + dinnerId, dinner
                ).
                success(function (data) {
                    if (data) {
                        deferredObject.resolve({ success: true });
                    } else {
                        deferredObject.resolve({ success: false });
                    }
                }).
                error(function () {
                    deferredObject.resolve({ success: false });
                });

                return deferredObject.promise;
            },

            deleteDinner: function (dinnerId) {
                var deferredObject = $q.defer();
                $http.delete(
                    '/api/dinners/' + dinnerId
                ).
                success(function (data) {
                    if (data) {
                        deferredObject.resolve({ success: true });
                    } else {
                        deferredObject.resolve({ success: false });
                    }
                }).
                error(function () {
                    deferredObject.resolve({ success: false });
                });

                return deferredObject.promise;
            },
        };
    }

    function dinnerHelper($http, $q, url) {
        var deferredObject = $q.defer();
        $http.get(url).
        success(function (data) {
            if (data) {
                deferredObject.resolve({ success: data });
            } else {
                deferredObject.resolve({ success: data });
            }
        }).
        error(function () {
            deferredObject.resolve({ success: false });
        });

        return deferredObject.promise;
    }
})();