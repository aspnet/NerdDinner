(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .controller('HomeController', HomeController);

    /* Home Controller  */
    HomeController.$inject = ['$rootScope', '$scope', '$location', 'authService'];

    function HomeController($rootScope, $scope, $location, authService) {
        $scope.isActive = function (viewLocation) {
            return viewLocation === $location.path();
        };

        $scope.$watch(authService.isUserLoggedIn, function (isUserLoggedIn) {
            $scope.isUserLoggedIn = isUserLoggedIn;
        });

        $scope.$watch(authService.currentUser, function (currentUser) {
            $scope.currentUser = currentUser;
        });

        $scope.logOff = function () {
            authService.logOff();
        };

        $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
            $rootScope.title = current.$$route.title;
        });
    }
})();