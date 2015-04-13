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
                '/api/rsvp/' + dinnerId
            ).
            success(function (data) {
                if (data.success) {

                } else {

                }

                deferredObject.resolve({ error: data });
            }).
            error(function (err) {
                deferredObject.resolve({ error: err });
            });

            return deferredObject.promise;
        };
    }
})();