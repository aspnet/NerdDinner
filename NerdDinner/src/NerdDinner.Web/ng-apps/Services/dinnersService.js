(function () {
    'use strict';

    angular
        .module('dinnersService', ['ngResource'])
        .factory('Dinner', Dinner)
        .factory('AuthService', AuthService);

    Dinner.$inject = ['$resource'];
    AuthService.$inject = ['$http', '$q'];

    function Dinner($resource) {
        return {
            all: $resource('/api/dinners/:id', { Id: "@Id" }),
            popular: $resource('/api/dinners/popular')
        };
    }

    function AuthService($http, $q) {
        var currentUser;
        var isUserLoggedIn;

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
                    if (data.success) {
                        isUserLoggedIn = true;
                        currentUser = data;
                    } else {
                        isUserLoggedIn = false;
                        currentUser = null;
                    }

                    deferredObject.resolve({ error: data });
                }).
                error(function (err) {
                    isUserLoggedIn = false;
                    currentUser = null;
                    deferredObject.resolve({ error: err });
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
                        isUserLoggedIn = true;
                        currentUser = data;
                        deferredObject.resolve({ success: true });
                    } else {
                        isUserLoggedIn = false;
                        currentUser = null;
                        deferredObject.resolve({ success: false });
                    }
                }).
                error(function (err) {
                    isUserLoggedIn = false;
                    currentUser = null;
                    deferredObject.resolve({ error: err });
                });

                return deferredObject.promise;
            },

            logOff: function () {
                isUserLoggedIn = false;
                currentUser = null;
                $http.post('/Account/LogOff');
            },

            externalLogin: function (provider, returnUrl) {
                var externalProviderUrl = '/Account/ExternalLogin?Provider=' + provider + '&ReturnUrl=' + returnUrl;
                window.location = externalProviderUrl;
            },

            isUserLoggedIn: function () { return isUserLoggedIn },
            currentUser: function () { return currentUser },
        };
    }
})();