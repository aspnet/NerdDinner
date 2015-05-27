!function() {
    "use strict";
    function config($routeProvider, $locationProvider) {
        $routeProvider.when("/", {
            title: "Nerd Dinner",
            templateUrl: "/views/home.html",
            controller: "homeController"
        }).when("/dinners/all", {
            title: "Nerd Dinner - All Dinners",
            templateUrl: "/views/list.html",
            controller: "listController"
        }).when("/dinners/my", {
            title: "Nerd Dinner - My Dinners",
            templateUrl: "/views/my.html",
            controller: "myController",
            resolve: {
                isUserAuthenticated: "authService"
            }
        }).when("/dinners/add", {
            title: "Nerd Dinner - Host Dinner",
            templateUrl: "/views/add.html",
            controller: "addController",
            resolve: {
                isUserAuthenticated: "authService"
            }
        }).when("/dinners/detail/:id", {
            title: "Nerd Dinner - Details",
            templateUrl: "/views/detail.html",
            controller: "detailController",
            resolve: {
                isUserAuthenticated: "authService"
            }
        }).when("/dinners/edit/:id", {
            title: "Nerd Dinner - Edit Dinner",
            templateUrl: "/views/edit.html",
            controller: "editController",
            resolve: {
                isUserAuthenticated: "authService"
            }
        }).when("/dinners/delete/:id", {
            title: "Nerd Dinner - Delete Dinner",
            templateUrl: "/views/delete.html",
            controller: "deleteController",
            resolve: {
                isUserAuthenticated: "authService"
            }
        }).when("/account/login", {
            title: "Nerd Dinner - Log In",
            templateUrl: "/account/login",
            controller: "loginController"
        }).when("/account/register", {
            title: "Nerd Dinner - Register",
            templateUrl: "/account/register",
            controller: "registerController"
        }).when("/about", {
            title: "Nerd Dinner - About",
            templateUrl: "/views/about.html"
        }).otherwise({
            redirectTo: "/"
        }), $locationProvider.html5Mode({
            enabled: !0,
            requireBase: !1
        });
    }
    config.$inject = [ "$routeProvider", "$locationProvider" ], angular.module("nerdDinner", [ "ngRoute", "ui.bootstrap", "dinnersService" ]).config(config);
}(), function() {
    "use strict";
    function homeController($scope, $location, dinner, mapService) {
        $scope.dinners = dinner.popular.query(), $scope.selectDinner = function(dinnerId) {
            $location.path("/dinners/detail/" + dinnerId).search({
                nocache: new Date().getTime()
            });
        }, $scope.loadMap = function() {
            mapService.loadMap($scope.dinners, 4);
        }, $scope.showLocationPin = function(dinner) {
            mapService.showInfoBoxPin(dinner);
        }, $scope.hideInfoBoxPin = function(dinner) {
            mapService.hideInfoBoxPin();
        }, $scope.showLocation = function(searchText) {
            mapService.findAddress(searchText, !1);
        }, $scope.home = function() {
            $location.path("/");
        };
    }
    function listController($scope, $location, dinner) {
        dinner.count().then(function(result) {
            $scope.bigTotalItems = result.success;
        }), $scope.maxSize = 3, $scope.bigCurrentPage = 1, $scope.itemPerPage = 12, $scope.dinners = dinner.all.query({
            pageIndex: $scope.bigCurrentPage,
            pageSize: $scope.itemPerPage
        }), $scope.selectDinner = function(dinnerId) {
            $location.path("/dinners/detail/" + dinnerId);
        }, $scope.pageChanged = function() {
            $scope.dinners = dinner.all.query({
                pageIndex: $scope.bigCurrentPage,
                pageSize: $scope.itemPerPage
            });
        };
    }
    function myController($scope, $location, dinner, isUserAuthenticated) {
        "False" == isUserAuthenticated.success && $location.path("/account/login"), $scope.dinners = dinner.my.query(), 
        $scope.selectDinner = function(dinnerId) {
            $location.path("/dinners/detail/" + dinnerId);
        };
    }
    function detailController($scope, $routeParams, $location, dinner, mapService, isUserAuthenticated) {
        $scope.dinner = dinner.all.get({
            id: $routeParams.id
        }), "False" == isUserAuthenticated.success && ($scope.isUserAuthenticated = isUserAuthenticated.success), 
        dinner.isUserHost($routeParams.id).then(function(result) {
            $scope.isUserHost = result.success;
        }), dinner.isUserRegistered($routeParams.id).then(function(result) {
            $scope.isUserRegistered = result.success;
        }), $scope.editDinner = function(dinnerId) {
            $location.path("/dinners/edit/" + dinnerId);
        }, $scope.deleteDinner = function(dinnerId) {
            $location.path("/dinners/delete/" + dinnerId);
        }, $scope.loadMap = function() {
            mapService.loadMap($scope.dinner);
        };
    }
    function addController($http, $scope, $location, dinner, mapService, isUserAuthenticated) {
        $scope.dinner = {
            title: "",
            description: "",
            eventDate: "",
            address: "",
            contactPhone: ""
        }, "False" == isUserAuthenticated.success && $location.path("/account/login"), $scope.loadDefaultMap = function() {
            mapService.loadDefaultMap();
        }, $scope.changeAddress = function(address) {
            mapService.findAddress(address, !0);
        }, $scope.add = function() {
            var result = dinner.addDinner($scope.dinner);
            result.then(function(result) {
                result.success ? result.data.dinnerId ? ($location.path("/dinners/detail/" + result.data.dinnerId), 
                $scope.error = !1) : $scope.error = !0 : ($scope.error = !0, $scope.errorMessage = result.error);
            });
        }, $scope.cancel = function() {
            $location.path("/");
        };
    }
    function editController($scope, $routeParams, $location, dinner, mapService, isUserAuthenticated) {
        "False" == isUserAuthenticated.success && $location.path("/account/login"), $scope.dinner = dinner.all.get({
            id: $routeParams.id
        }), $scope.$watch(eventDate, function(newValue) {
            $scope.dinner.eventDate = newValue;
        }), $scope.$watch("dinner.eventDate", function(newValue) {
            $scope.eventDate = new Date($scope.dinner.eventDate);
        }), $scope.loadMap = function() {
            mapService.loadMap($scope.dinner);
        }, $scope.changeAddress = function(address) {
            mapService.findAddress(address, !0);
        }, $scope.edit = function() {
            var result = dinner.editDinner($routeParams.id, $scope.dinner);
            result.then(function(result) {
                result.success ? $location.path("/dinners/detail/" + $routeParams.id) : ($scope.error = !0, 
                $scope.errorMessage = result.error);
            });
        }, $scope.cancel = function() {
            $location.path("/dinners/detail/" + $routeParams.id);
        };
    }
    function deleteController($scope, $routeParams, $location, dinner, isUserAuthenticated) {
        "False" == isUserAuthenticated.success && $location.path("/account/login"), $scope.dinner = dinner.all.get({
            id: $routeParams.id
        }), $scope["delete"] = function() {
            var result = dinner.deleteDinner($routeParams.id);
            result.then(function(result) {
                result.success && $location.path("/dinners/my");
            });
        }, $scope.cancel = function() {
            $location.path("/dinners/detail/" + $routeParams.id);
        };
    }
    angular.module("nerdDinner").controller("homeController", homeController).controller("listController", listController).controller("myController", myController).controller("detailController", detailController).controller("addController", addController).controller("editController", editController).controller("deleteController", deleteController), 
    homeController.$inject = [ "$scope", "$location", "dinner", "mapService" ], listController.$inject = [ "$scope", "$location", "dinner" ], 
    myController.$inject = [ "$scope", "$location", "dinner", "isUserAuthenticated" ], 
    detailController.$inject = [ "$scope", "$routeParams", "$location", "dinner", "mapService", "isUserAuthenticated" ], 
    addController.$inject = [ "$http", "$scope", "$location", "dinner", "mapService", "isUserAuthenticated" ], 
    editController.$inject = [ "$scope", "$routeParams", "$location", "dinner", "mapService", "isUserAuthenticated" ], 
    deleteController.$inject = [ "$scope", "$routeParams", "$location", "dinner", "isUserAuthenticated" ];
}(), function() {
    "use strict";
    function loginController($scope, $location) {}
    angular.module("nerdDinner").controller("loginController", loginController), loginController.$inject = [ "$scope", "$location" ];
}(), function() {
    "use strict";
    function registerController($scope, $location) {}
    angular.module("nerdDinner").controller("registerController", registerController), 
    registerController.$inject = [ "$scope", "$location" ];
}(), function() {
    "use strict";
    function rsvpController($scope, $routeParams, $location, rsvpService) {
        $scope.showMessage = !1, $scope.addRsvp = function(dinnerId) {
            "False" == $scope.isUserAuthenticated && $location.path("/account/login");
            var result = rsvpService.addRsvp(dinnerId);
            result.then(function(result) {
                result ? ($scope.message = "Thanks - we'll see you there!", $scope.showMessage = !0, 
                $scope.isUserRegistered = !0) : $scope.showMessage = !1;
            });
        }, $scope.cancelRsvp = function(dinnerId) {
            "False" == $scope.isUserAuthenticated && $location.path("/account/login");
            var result = rsvpService.cancelRsvp(dinnerId);
            result.then(function(result) {
                result ? ($scope.message = "Sorry you can't make it!", $scope.showMessage = !0, 
                $scope.isUserRegistered = !1) : $scope.showMessage = !1;
            });
        };
    }
    function rsvpSection() {
        return {
            restrict: "E",
            templateUrl: "/views/rsvp.html",
            controller: rsvpController
        };
    }
    angular.module("nerdDinner").directive("rsvpSection", rsvpSection), rsvpController.$inject = [ "$scope", "$routeParams", "$location", "rsvpService" ];
}(), function() {
    "use strict";
    function authService($http, $q) {
        var deferredObject = $q.defer();
        return $http.get("/account/isUserAuthenticated").success(function(data) {
            data && deferredObject.resolve({
                success: data
            });
        }).error(function() {
            deferredObject.resolve({
                success: !1
            });
        }), deferredObject.promise;
    }
    angular.module("nerdDinner").service("authService", authService), authService.$inject = [ "$http", "$q" ];
}(), function() {
    "use strict";
    function dinner($resource, $http, $q) {
        return {
            all: $resource("/api/dinners/:id", {
                id: "@_id"
            }),
            popular: $resource("/api/dinners/popular"),
            my: $resource("/api/dinners/my"),
            count: function() {
                return dinnerHelper($http, $q, "/api/dinners/count");
            },
            isUserHost: function(dinnerId) {
                return dinnerHelper($http, $q, "/api/dinners/isUserHost?id=" + dinnerId);
            },
            isUserRegistered: function(dinnerId) {
                return dinnerHelper($http, $q, "/api/dinners/isUserRegistered?id=" + dinnerId);
            },
            addDinner: function(dinner) {
                var deferredObject = $q.defer();
                return $http.post("/api/dinners", dinner).success(function(data) {
                    data ? deferredObject.resolve({
                        success: !0,
                        data: data
                    }) : deferredObject.resolve({
                        success: !1
                    });
                }).error(function(err) {
                    deferredObject.resolve({
                        error: err
                    });
                }), deferredObject.promise;
            },
            editDinner: function(dinnerId, dinner) {
                var deferredObject = $q.defer();
                return $http.put("/api/dinners/" + dinnerId, dinner).success(function(data) {
                    data ? deferredObject.resolve({
                        success: !0
                    }) : deferredObject.resolve({
                        success: !1
                    });
                }).error(function(err) {
                    deferredObject.resolve({
                        error: err
                    });
                }), deferredObject.promise;
            },
            deleteDinner: function(dinnerId) {
                var deferredObject = $q.defer();
                return $http["delete"]("/api/dinners/" + dinnerId).success(function(data) {
                    deferredObject.resolve({
                        success: !0
                    });
                }).error(function() {
                    deferredObject.resolve({
                        success: !1
                    });
                }), deferredObject.promise;
            }
        };
    }
    function dinnerHelper($http, $q, url) {
        var deferredObject = $q.defer();
        return $http.get(url).success(function(data) {
            data && deferredObject.resolve({
                success: data
            });
        }).error(function() {
            deferredObject.resolve({
                success: !1
            });
        }), deferredObject.promise;
    }
    angular.module("dinnersService", [ "ngResource" ]).factory("dinner", dinner), dinner.$inject = [ "$resource", "$http", "$q" ];
}(), function() {
    "use strict";
    function mapService($http, $location, $q) {
        function zoomMapToLocation(latitude, longitude) {
            map.setView({
                center: new Microsoft.Maps.Location(latitude, longitude),
                zoom: 10
            });
        }
        function loadDetailsMap(dinner) {
            map.entities.clear(), map = null, infobox = null, infoboxLayer = new Microsoft.Maps.EntityCollection(), 
            map = new Microsoft.Maps.Map(document.getElementById("dinnerMap"), {
                credentials: bingMapsKey,
                mapTypeId: Microsoft.Maps.MapTypeId.road
            });
            var locs = [], location = new Microsoft.Maps.Location(dinner.latitude, dinner.longitude);
            locs.push(location);
            var pin = new Microsoft.Maps.Pushpin(location, {
                icon: "../../images/poi_usergenerated.gif",
                width: 50,
                height: 50
            });
            pin.Id = dinner.dinnerId, pin.Title = dinner.title, pin.Date = dinner.eventDate, 
            pin.Address = dinner.address, pin.Description = dinner.description, pinLayer.push(pin), 
            Microsoft.Maps.Events.addHandler(pin, "mouseover", pinMouseOver), Microsoft.Maps.Events.addHandler(pin, "mouseout", pinMouseOut), 
            map.entities.push(pinLayer), map.entities.push(infoboxLayer);
            var bestview = Microsoft.Maps.LocationRect.fromLocations(locs);
            map.setView({
                bounds: bestview
            }), map.setView({
                zoom: 10
            });
        }
        function hideInfobox() {
            null != infobox && infobox.setOptions({
                visible: !1
            });
        }
        function pinInfoboxMouseLeave(e) {
            hideInfobox();
        }
        function startInfoboxTimer() {
            null != infobox && null != infobox.pinTimer && clearTimeout(infobox.pinTimer), null != infobox && (infobox.pinTimer = setTimeout(timerTriggered, 300));
        }
        function stopInfoboxTimer() {
            null != infobox && null != infobox.pinTimer && clearTimeout(infobox.pinTimer);
        }
        function pinInfoboxMouseEnter(e) {
            stopInfoboxTimer();
        }
        function pinMouseOver(e) {
            displayInfobox(e);
        }
        function pinMouseOut() {
            startInfoboxTimer();
        }
        function timerTriggered() {
            hideInfobox();
        }
        function displayInfobox(e) {
            hideInfobox(), stopInfoboxTimer();
            var pin = e.target;
            if (null != pin) {
                var currentDinnerTitle = (pin.Id, pin.Title), date = new Date(pin.Date), dateString = date.toDateString(), location = pin.getLocation(), options = {
                    id: pin.Id,
                    zIndex: 999,
                    visible: !0,
                    showPointer: !0,
                    showCloseButton: !0,
                    title: currentDinnerTitle,
                    description: popupDescription(pin.Description, dateString, pin.rsvps),
                    titleClickHandler: showDetail,
                    offset: new Microsoft.Maps.Point(-12, 40)
                };
                null != infobox && (map.entities.remove(infobox), map.entities.remove(infoboxLayer), 
                Microsoft.Maps.Events.hasHandler(infobox, "mouseleave") && Microsoft.Maps.Events.removeHandler(infobox.mouseLeaveHandler), 
                Microsoft.Maps.Events.hasHandler(infobox, "mouseenter") && Microsoft.Maps.Events.removeHandler(infobox.mouseEnterHandler), 
                infobox = null), infobox = new Microsoft.Maps.Infobox(location, options), infobox.mouseLeaveHandler = Microsoft.Maps.Events.addHandler(infobox, "mouseleave", pinInfoboxMouseLeave), 
                infobox.mouseEnterHandler = Microsoft.Maps.Events.addHandler(infobox, "mouseenter", pinInfoboxMouseEnter), 
                infoboxLayer.push(infobox), map.entities.push(infoboxLayer);
            }
        }
        function popupDescription(description, dateString, rsvps) {
            var mapDescription = "<strong>" + dateString + "</strong><p>" + description + "</p>";
            return rsvps && (mapDescription += "<p>" + rsvps + " RSVPs</p>"), mapDescription;
        }
        function showDetail() {
            window.location("/#dinners/detail/8");
        }
        function loadDinnersOnMap(dinners, zoomlevel) {
            for (var locs = [], i = 0; i < dinners.length; i++) {
                var location = new Microsoft.Maps.Location(dinners[i].latitude, dinners[i].longitude);
                locs.push(location);
                var pin = new Microsoft.Maps.Pushpin(location, {
                    icon: "images/poi_usergenerated.gif",
                    width: 50,
                    height: 50
                });
                pin.Id = dinners[i].dinnerId, pin.Title = dinners[i].title, pin.Date = dinners[i].eventDate, 
                pin.Address = dinners[i].address, pin.Description = dinners[i].description, pin.rsvps = dinners[i].rsvps.length, 
                pinLayer.push(pin), Microsoft.Maps.Events.addHandler(pin, "mouseover", pinMouseOver), 
                Microsoft.Maps.Events.addHandler(pin, "mouseout", pinMouseOut);
            }
            map.entities.push(pinLayer), map.entities.push(infoboxLayer);
            var bestview = Microsoft.Maps.LocationRect.fromLocations(locs);
            map.setView({
                bounds: bestview
            });
        }
        var map = null, infobox = null, infoboxLayer = new Microsoft.Maps.EntityCollection(), pinLayer = new Microsoft.Maps.EntityCollection(), bingMapsKey = "Al1IumsJbHmAUYWKYXq33XIxwbJCkRTRZcVGCO3wJD2J3-ICC0lWUp2Adu_z_qtt";
        this.loadDefaultMap = function() {
            map = new Microsoft.Maps.Map(document.getElementById("dinnerMap"), {
                credentials: bingMapsKey,
                mapTypeId: Microsoft.Maps.MapTypeId.road,
                disableBirdseye: !0,
                showMapTypeSelector: !1
            }), map.setView({
                zoom: 1
            });
        }, this.findAddress = function(address, setPin) {
            var url = "http://dev.virtualearth.net/REST/v1/Locations?query=" + encodeURI(address) + "&jsonp=JSON_CALLBACK&key=" + bingMapsKey;
            $http.jsonp(url).success(function(result) {
                if (result && result.resourceSets && result.resourceSets.length > 0 && result.resourceSets[0].resources && result.resourceSets[0].resources.length > 0) if (setPin) {
                    var bbox = result.resourceSets[0].resources[0].bbox, location = (Microsoft.Maps.LocationRect.fromLocations(new Microsoft.Maps.Location(bbox[0], bbox[1]), new Microsoft.Maps.Location(bbox[2], bbox[3])), 
                    new Microsoft.Maps.Location(result.resourceSets[0].resources[0].point.coordinates[0], result.resourceSets[0].resources[0].point.coordinates[1])), pin = new Microsoft.Maps.Pushpin(location);
                    map.entities.clear(), map.entities.push(pin);
                    var bestview = Microsoft.Maps.LocationRect.fromLocations(location);
                    map.setView({
                        bounds: bestview
                    }), map.setView({
                        zoom: 10
                    });
                } else {
                    var bbox = result.resourceSets[0].resources[0].bbox;
                    zoomMapToLocation(bbox[0], bbox[1]);
                }
            }).error(function(data, status, error, thing) {
                console.log(data);
            });
        }, this.loadMap = function(dinners, zoomlevel) {
            null != map && map.entities && (infobox = null, infoboxLayer = new Microsoft.Maps.EntityCollection()), 
            map = new Microsoft.Maps.Map(document.getElementById("dinnerMap"), {
                credentials: bingMapsKey,
                mapTypeId: Microsoft.Maps.MapTypeId.road,
                disableBirdseye: !0,
                showMapTypeSelector: !1
            }), dinners.$promise.then(function(result) {
                dinners = result, dinners.length ? loadDinnersOnMap(dinners, zoomlevel) : loadDetailsMap(dinners);
            });
        }, this.showInfoBoxPin = function(dinner) {
            var location = new Microsoft.Maps.Location(dinner.latitude, dinner.longitude), pin = new Microsoft.Maps.Pushpin(location);
            pin.Id = dinner.dinnerId, pin.Title = dinner.title, pin.Date = dinner.eventDate, 
            pin.Description = dinner.description, pin.Address = dinner.address, pin.rsvps = dinner.rsvps.length;
            var e = new Object();
            e.target = pin, displayInfobox(e);
        }, this.hideInfoBoxPin = function() {
            startInfoboxTimer();
        };
    }
    angular.module("nerdDinner").service("mapService", mapService), mapService.$inject = [ "$http", "$location", "$q" ];
}(), function() {
    "use strict";
    function rsvpService($http, $q) {
        this.addRsvp = function(dinnerId) {
            var deferredObject = $q.defer();
            return $http.post("/api/rsvp?dinnerId=" + dinnerId).success(function(data) {
                data ? deferredObject.resolve({
                    success: !0
                }) : deferredObject.resolve({
                    success: !1
                });
            }).error(function(err) {
                deferredObject.resolve({
                    error: err
                });
            }), deferredObject.promise;
        }, this.cancelRsvp = function(dinnerId) {
            var deferredObject = $q.defer();
            return $http["delete"]("/api/rsvp?dinnerId=" + dinnerId).success(function(data) {
                data ? deferredObject.resolve({
                    success: !0
                }) : deferredObject.resolve({
                    success: !1
                });
            }).error(function(err) {
                deferredObject.resolve({
                    error: err
                });
            }), deferredObject.promise;
        };
    }
    angular.module("nerdDinner").service("rsvpService", rsvpService), rsvpService.$inject = [ "$http", "$q" ];
}();