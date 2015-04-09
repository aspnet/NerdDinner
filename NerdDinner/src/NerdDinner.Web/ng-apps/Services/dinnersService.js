(function () {
    'use strict';

    angular
        .module('dinnersService', ['ngResource'])
        .factory('Dinner', Dinner)
        .factory('AuthService', AuthService);

    Dinner.$inject = ['$resource'];
    AuthService.$inject = ['$http', '$q'];

    function Dinner($resource) {
        return $resource('/api/dinners/:id', { Id: "@Id" });
    }

    function AuthService($http, $q) {
        var currentUser;
        var isLoggedIn;

        return {
            register: function (userName, password, confirmPassword) {
                var deferredObject = $q.defer();
                var registerModel = {
                    UserName: userName,
                    password: password,
                    confirmPassword: confirmPassword
                };

                $http.post(
                    '/Account/Register', registerModel
                ).
                success(function (data) {
                    if (data) {
                        isLoggedIn = true;
                        deferredObject.resolve({ success: true });
                    } else {
                        isLoggedIn = false;
                        deferredObject.resolve({ success: false });
                    }
                }).
                error(function () {
                    isLoggedIn = false;
                    deferredObject.resolve({ success: false });
                });

                return deferredObject.promise;
            },

            login: function (userName, password) {
                var deferredObject = $q.defer();
                $http.post(
                    '/Account/Login', {
                        UserName: userName,
                        Password: password
                    }
                ).
                success(function (data) {
                    if (data) {
                        isLoggedIn = true;
                        deferredObject.resolve({ success: true });
                    } else {
                        isLoggedIn = false;
                        deferredObject.resolve({ success: false });
                    }
                }).
                error(function () {
                    isLoggedIn = false;
                    deferredObject.resolve({ success: false });
                });

                return deferredObject.promise;
            },

            logOff: function () {
                isLoggedIn = false;
                currentUser = null;
                $http.post('/Account/LogOff');
            },

            externalLogin: function (provider, returnUrl) {
                var externalProviderUrl = '/Account/ExternalLogin?Provider=' + provider + '&ReturnUrl=http%3A%2F%2Flocalhost%3A5002%2F'
                window.location = externalProviderUrl;
            },

            isLoggedIn: function () { return isLoggedIn },
            currentUser: function () { return currentUser },
        };
    }
})();