(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .controller('HomeController', HomeController);

    /* Home Controller  */
    HomeController.$inject = ['$rootScope', '$scope', '$location', 'AuthService'];

    function HomeController($rootScope, $scope, $location, AuthService) {
        $scope.isActive = function (viewLocation) {
            return viewLocation === $location.path();
        };
        $scope.$watch(AuthService.isLoggedIn, function (isLoggedIn) {
            $scope.isLoggedIn = isLoggedIn;
            $scope.currentUser = AuthService.currentUser();
        });
        $scope.logOff = function () {
            AuthService.logOff();
        };

        $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
            $rootScope.title = current.$$route.title;
        });
    }
})();