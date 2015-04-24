(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .controller('homeController', homeController);

    /* Home Controller  */
    homeController.$inject = ['$scope', '$location'];

    function homeController($scope, $location) {
    }
})();