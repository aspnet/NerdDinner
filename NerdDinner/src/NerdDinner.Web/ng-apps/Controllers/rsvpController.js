(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .directive('rsvpSection', rsvpSection)
        .controller('rsvpController', rsvpController)

    /* RSVP Controller  */
    rsvpController.$inject =  ['$http', '$q'];

    function rsvpController(http, q) {
    }

    rsvpSection.$inject = ['$http'];

    function rsvpSection($http) {
        return {
            restrict: 'E',
            templateUrl: "views/rsvp.html",
            controller: function () {
                this.showMessage = false;
                this.message = '';
                /* Add Rsvp for the selected dinner */
                this.addRsvp = function () {
                    var self = this;
                    $http({
                        url: '/api/RSVP/',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                        method: "POST",
                        data: $.param({ dinnerId: 8, userName: "User1" })
                    })
                    .then(function (response) {
                        // if the status is 200/success then show the welcome message.
                        debugger;
                        if (response.status == 200) {
                            self.message = "Thanks - we'll see you there!";
                            self.showMessage = true;
                        }
                    },
                    function (response) {
                        // failed
                    }
                );
                };

                /* Remove Rsvp for the selected dinner */
                this.removeRsvp = function () {
                    alert('cancel RSVP');
                    var self = this;
                    $http({
                        url: '/api/RSVP/',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                        method: "DELETE",
                        data: $.param({ dinnerId: 8, userName: "User1" })
                    })
                    .then(function (response) {
                        self.message = "Sorry you can't make it!";
                        self.showMessage = true;
                    },
                    function (response) {
                        // failed
                    }
                    );
                };
            },

            controllerAs: 'rsvp'
        }
    }

})();
