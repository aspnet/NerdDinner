(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .service('rsvpService', rsvpService);

    rsvpService.$inject = ['$http', '$q'];

    function rsvpService($http, $q) {
        this.addRsvp = function (dinnerId) {
            var deferredObject = $q.defer();
            $http.post(
                '/api/rsvp?dinnerId=' + dinnerId
            ).
            success(function (data) {
                if (data) {
                    deferredObject.resolve({ success: true });
                } else {
                    deferredObject.resolve({ success: false });
                }
            }).
            error(function (err) {
                deferredObject.resolve({ error: err });
            });

            return deferredObject.promise;
        };

        this.cancelRsvp = function (dinnerId) {
            var deferredObject = $q.defer();
            $http.delete(
                '/api/rsvp?dinnerId=' + dinnerId
            ).
            success(function (data) {
                if (data) {
                    deferredObject.resolve({ success: true });
                } else {
                    deferredObject.resolve({ success: false });
                }
            }).
            error(function (err) {
                deferredObject.resolve({ error: err });
            });

            return deferredObject.promise;
        };
    }
})();