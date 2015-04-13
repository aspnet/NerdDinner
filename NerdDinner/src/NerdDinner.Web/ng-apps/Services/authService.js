(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .service('authService', authService);

    authService.$inject = ['$http', '$q'];

    function authService($http, $q) {
        var currentUser;
        var isUserLoggedIn;

        this.register = function (userName, password, confirmPassword) {
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
        };

        this.login = function (userName, password) {
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
        };

        this.logOff = function () {
            isUserLoggedIn = false;
            currentUser = null;
            $http.post('/Account/LogOff');
        };

        this.externalLogin = function (provider, returnUrl) {
            var externalProviderUrl = '/Account/ExternalLogin?Provider=' + provider + '&ReturnUrl=' + returnUrl;
            window.location = externalProviderUrl;
        };

        this.isUserLoggedIn = function () { return isUserLoggedIn };
        this.currentUser = function () { return currentUser };
    }
})();