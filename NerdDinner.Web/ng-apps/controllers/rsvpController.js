
(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .directive('rsvpSection', rsvpSection)

    /* RSVP Controller  */
    rsvpController.$inject = ['$scope', '$routeParams', '$location', 'rsvpService'];

    function rsvpController($scope, $routeParams, $location, rsvpService) {
        $scope.showMessage = false;
        $scope.addRsvp = function (dinnerId) {
            if ($scope.isUserAuthenticated == 'False') {
                $location.path('/account/login');
            }

            var result = rsvpService.addRsvp(dinnerId);
            result.then(function (result) {
                if (result) {
                    $scope.message = 'Thanks - we\'ll see you there!';
                    $scope.showMessage = true;
                    $scope.isUserRegistered = true;
                } else {
                    $scope.showMessage = false;
                }
            });
        }

        $scope.cancelRsvp = function (dinnerId) {
            if ($scope.isUserAuthenticated == 'False') {
                $location.path('/account/login');
            }

            var result = rsvpService.cancelRsvp(dinnerId);
            result.then(function (result) {
                if (result) {
                    $scope.message = 'Sorry you can\'t make it!';
                    $scope.showMessage = true;
                    $scope.isUserRegistered = false;
                } else {
                    $scope.showMessage = false;
                }
            });
        }
    }

    function rsvpSection() {
        return {
            restrict: 'E',
            templateUrl: "/views/rsvp.html",
            controller: rsvpController
        }
    }
})();