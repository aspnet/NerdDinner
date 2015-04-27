(function () {
    'use strict';

    angular
        .module('nerdDinner')
        .service('mapService', mapService);

    mapService.$inject = ['$http', '$location', '$q'];

    function mapService($http, $location, $q) {

        var map = null;
        var infobox = null;
        var infoboxLayer = new Microsoft.Maps.EntityCollection();
        var pinLayer = new Microsoft.Maps.EntityCollection();
        var bingMapsKey = 'Al1IumsJbHmAUYWKYXq33XIxwbJCkRTRZcVGCO3wJD2J3-ICC0lWUp2Adu_z_qtt';

        this.loadDefaultMap = function () {
            map = new Microsoft.Maps.Map(document.getElementById('dinnerMap'), {
                credentials: bingMapsKey,
                mapTypeId: Microsoft.Maps.MapTypeId.road,
                disableBirdseye: true,
                showMapTypeSelector: false
            });
            map.setView({ zoom: 1 });
        };

        this.findAddress = function (address, setPin) {
            var url = "http://dev.virtualearth.net/REST/v1/Locations?query=" + encodeURI(address) + "&jsonp=JSON_CALLBACK&key=" + bingMapsKey + "";
            $http.jsonp(url)
                .success(function (result) {
                    if (result &&
                            result.resourceSets &&
                            result.resourceSets.length > 0 &&
                            result.resourceSets[0].resources &&
                            result.resourceSets[0].resources.length > 0) {
                        if (setPin) {
                            var bbox = result.resourceSets[0].resources[0].bbox;
                            var viewBoundaries = Microsoft.Maps.LocationRect.fromLocations(new Microsoft.Maps.Location(bbox[0], bbox[1]), new Microsoft.Maps.Location(bbox[2], bbox[3]));
                            var location = new Microsoft.Maps.Location(result.resourceSets[0].resources[0].point.coordinates[0], result.resourceSets[0].resources[0].point.coordinates[1]);
                            var pin = new Microsoft.Maps.Pushpin(location);
                            map.entities.clear();
                            map.entities.push(pin);
                            var bestview = Microsoft.Maps.LocationRect.fromLocations(location);
                            //map.getRootElement().style.cursor = "move";
                            map.setView({ bounds: bestview });
                            map.setView({ zoom: 10 });
                        }
                        else {
                            var bbox = result.resourceSets[0].resources[0].bbox;
                            zoomMapToLocation(bbox[0], bbox[1]);
                        }
                    }
                })
                .error(function (data, status, error, thing) {
                    console.log(data);
                });
        };

        this.loadMap = function (dinners, zoomlevel) {
            if (map != null && map.entities) {
                // map.entities.clear();
                infobox = null;
                infoboxLayer = new Microsoft.Maps.EntityCollection();
            }
            map = new Microsoft.Maps.Map(document.getElementById('dinnerMap'), {
                credentials: bingMapsKey,
                mapTypeId: Microsoft.Maps.MapTypeId.road,
                disableBirdseye: true,
                showMapTypeSelector: false
            });

            dinners.$promise.then(function (result) {
                dinners = result;
                if (dinners.length) {
                    loadDinnersOnMap(dinners, zoomlevel);
                }
                else {
                    loadDetailsMap(dinners);
                }
            });
        };

        this.showInfoBoxPin = function (dinner) {
            var location = new Microsoft.Maps.Location(dinner.latitude, dinner.longitude);
            var pin = new Microsoft.Maps.Pushpin(location);
            //pin details
            pin.Id = dinner.dinnerId;
            pin.Title = dinner.title;
            pin.Date = dinner.eventDate;
            pin.Description = dinner.description;
            pin.Address = dinner.address;
            pin.rsvps = dinner.rsvps.length;
            var e = new Object();
            e.target = pin;
            displayInfobox(e);
        };

        this.hideInfoBoxPin = function () {
            startInfoboxTimer();
        }

        function zoomMapToLocation(latitude, longitude) {
            map.setView({ center: new Microsoft.Maps.Location(latitude, longitude), zoom: 10 });
        };

        function loadDetailsMap(dinner) {
            map.entities.clear();
            map = null;
            infobox = null;
            infoboxLayer = new Microsoft.Maps.EntityCollection();
            map = new Microsoft.Maps.Map(document.getElementById('dinnerMap'), {
                credentials: bingMapsKey,
                mapTypeId: Microsoft.Maps.MapTypeId.road
            });

            var locs = [];
            var location = new Microsoft.Maps.Location(dinner.latitude, dinner.longitude);
            locs.push(location);

            var pin = new Microsoft.Maps.Pushpin(location, { icon: '../../images/poi_usergenerated.gif', width: 50, height: 50 });
            //pin details
            pin.Id = dinner.dinnerId;
            pin.Title = dinner.title;
            pin.Date = dinner.eventDate;
            pin.Address = dinner.address;
            pin.Description = dinner.description;
            pinLayer.push(pin);
            Microsoft.Maps.Events.addHandler(pin, 'mouseover', pinMouseOver);
            Microsoft.Maps.Events.addHandler(pin, 'mouseout', pinMouseOut);

            map.entities.push(pinLayer);
            map.entities.push(infoboxLayer);

            var bestview = Microsoft.Maps.LocationRect.fromLocations(locs);
            map.setView({ bounds: bestview });
            map.setView({ zoom: 10 });
        };

        function showInfobox() {
            infobox.setOptions({ visible: true });
        };

        function hideInfobox() {
            if (infobox != null) {
                infobox.setOptions({ visible: false });
            }
        };

        function pinInfoboxMouseLeave(e) {
            hideInfobox();
        };

        function startInfoboxTimer() {
            if (infobox != null && infobox.pinTimer != null) {
                clearTimeout(infobox.pinTimer);
            }
            if (infobox != null) {
                infobox.pinTimer = setTimeout(timerTriggered, 300);
            }
        };

        function stopInfoboxTimer() {
            if (infobox != null && infobox.pinTimer != null) {
                clearTimeout(infobox.pinTimer);
            }
        };

        function pinInfoboxMouseEnter(e) {
            stopInfoboxTimer();
        };

        function pinMouseOver(e) {
            displayInfobox(e);
        };

        function pinMouseOut() {
            startInfoboxTimer();
        };

        function timerTriggered() {
            hideInfobox();
        };

        function displayInfobox(e) {
            hideInfobox();
            stopInfoboxTimer();
            var pin = e.target;
            if (pin != null) {
                var currentDinnerId = pin.Id;
                var currentDinnerTitle = pin.Title;
                var date = new Date(pin.Date);
                var dateString = date.toDateString();
                var location = pin.getLocation();
                var options = {
                    id: pin.Id, zIndex: 999, visible: true, showPointer: true, showCloseButton: true, title: currentDinnerTitle, description: popupDescription(pin.Description, dateString, pin.rsvps), titleClickHandler: showDetail, offset: new Microsoft.Maps.Point(-12, 40)
                };
                if (infobox != null) {
                    map.entities.remove(infobox);
                    map.entities.remove(infoboxLayer);
                    if (Microsoft.Maps.Events.hasHandler(infobox, 'mouseleave')) {
                        Microsoft.Maps.Events.removeHandler(infobox.mouseLeaveHandler);
                    }
                    if (Microsoft.Maps.Events.hasHandler(infobox, 'mouseenter')) {
                        Microsoft.Maps.Events.removeHandler(infobox.mouseEnterHandler);
                    }
                    infobox = null;
                }
                infobox = new Microsoft.Maps.Infobox(location, options);
                infobox.mouseLeaveHandler = Microsoft.Maps.Events.addHandler(infobox, 'mouseleave', pinInfoboxMouseLeave);
                infobox.mouseEnterHandler = Microsoft.Maps.Events.addHandler(infobox, 'mouseenter', pinInfoboxMouseEnter);
                infoboxLayer.push(infobox);
                map.entities.push(infoboxLayer);
            }
        };

        function popupDescription(description, dateString, rsvps) {
            var mapDescription = '<strong>' + dateString + '</strong><p>' + description + '</p>';
            if (rsvps) {
                mapDescription += '<p>' + rsvps + ' RSVPs</p>';
            }

            return mapDescription;
        }

        function showDetail() {
            window.location('/#dinners/detail/8');
        }

        function loadDinnersOnMap(dinners, zoomlevel) {
            var locs = [];
            for (var i = 0; i < dinners.length; i++) {
                var location = new Microsoft.Maps.Location(dinners[i].latitude, dinners[i].longitude);
                locs.push(location);

                var pin = new Microsoft.Maps.Pushpin(location, { icon: 'images/poi_usergenerated.gif', width: 50, height: 50 });
                //pin details
                pin.Id = dinners[i].dinnerId;
                pin.Title = dinners[i].title;
                pin.Date = dinners[i].eventDate;
                pin.Address = dinners[i].address;
                pin.Description = dinners[i].description;
                pin.rsvps = dinners[i].rsvps.length;
                pinLayer.push(pin);
                Microsoft.Maps.Events.addHandler(pin, 'mouseover', pinMouseOver);
                Microsoft.Maps.Events.addHandler(pin, 'mouseout', pinMouseOut);
            }
            map.entities.push(pinLayer);
            map.entities.push(infoboxLayer);

            var bestview = Microsoft.Maps.LocationRect.fromLocations(locs);
            //map.getRootElement().style.cursor = "move";
            map.setView({ bounds: bestview });
        };
    }
})();
