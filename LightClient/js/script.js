"use strict";

const API = Object.freeze("http://localhost:8080/api/");

const defaultPosition = Object.freeze({
    latitude: 45.764043,
    longitude: 4.835659
}); // Position de Lyon

const greenIcon = Object.freeze(new L.Icon({
    iconUrl: "resources/marker-icon-2x-green.png",
    shadowUrl: "resources/marker-shadow.png",
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowSize: [41, 41]
}));

const redIcon = Object.freeze(new L.Icon({
    iconUrl: "resources/marker-icon-2x-red.png",
    shadowUrl: "resources/marker-shadow.png",
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowSize: [41, 41]
}));

let map = L.map("map");
let pathLayer = L.layerGroup();

let start;
let end;

let currentMarker;

let stations;
let startStationPosition;
let endStationPosition;

window.onload = () => {
    if ("serviceWorker" in navigator) {
        navigator.serviceWorker.register("serviceWorker.js");
    }

    if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition((position) => {
            map = map.setView([position.coords.latitude, position.coords.longitude], 14);
            constructMap(map);
        }, error => {
            if (error.code == error.PERMISSION_DENIED) {
                map = map.setView([defaultPosition.latitude, defaultPosition.longitude], 14);
                constructMap(map);
            }
        })
    } else {
        map = map.setView([defaultPosition.latitude, defaultPosition.longitude], 14);
        constructMap(map);
    }
}

navigator.geolocation.watchPosition(
    (position) => {
        if (map && currentMarker) {
            map.removeLayer(currentMarker);
        }
        currentMarker = L.marker([position.coords.latitude, position.coords.longitude], { title: "Position actuelle" })
            .bindPopup("<b>Postion actuelle</b>")
            .addTo(map);
    },
    () => {
        console.warn("Can't watch the position");
    }
);

const retrieveStations = () => {
    const targetUrl = API + "stations";
    const requestType = "GET";

    const caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    caller.setRequestHeader("Accept", "application/json");
    caller.onload = () => {
        stations = JSON.parse(caller.responseText);
        showStationMarkers();
    }
    caller.send();
}


const constructMap = (map) => {
    retrieveStations();

    L.tileLayer("https://{s}.tile.openstreetmap.fr/osmfr/{z}/{x}/{y}.png", {
        attribution: "Map data &copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors",
        minZoom: 1,
        maxZoom: 17
    }).addTo(map);

    L.control.scale().addTo(map);

    map.addLayer(pathLayer);

    map.on("click", () => { // TODO: Add button to remove path
        start = null;
        startStationPosition = null;

        end = null;
        endStationPosition = null;

        pathLayer.clearLayers();
    });

    L.Control.geocoder({
        position: "topright",
        collapsed: false,
        placeholder: "Adresse de départ",
        defaultMarkGeocode: false
    }).on("markgeocode", (event) => {
        const center = event.geocode.center;
        pathLayer.addLayer(L
            .marker(center,{
                icon: greenIcon,
                title: event.geocode.name})
            .bindPopup(`<b>Départ</b><br>` + event.geocode.html)
        );
        map.fitBounds(event.geocode.bbox);
        start = {
            latitude: center.lat,
            longitude: center.lng
        };
        findNearestStartStation(start.latitude, start.longitude);
      })
      .addTo(map);

      L.Control.geocoder({
        position: "topright",
        collapsed: false,
        placeholder: "Adresse d'arrivée",
        defaultMarkGeocode: false
    }).on("markgeocode", (event) => {
        const center = event.geocode.center;
        pathLayer.addLayer(L
            .marker(center, {
                icon: redIcon,
                title: event.geocode.name
            })
            .bindPopup(`<b>Arrivée</b><br>` + event.geocode.html)
        );
        map.fitBounds(event.geocode.bbox);
        end = {
            latitude: center.lat,
            longitude: center.lng
        };
        findNearestEndStation(end.latitude, end.longitude);
      })
      .addTo(map);
}

const showStationMarkers = () => {
    let markersCluster = L.markerClusterGroup();

    stations.forEach(station => {
        markersCluster.addLayer(L
            .marker([station.position.latitude, station.position.longitude], { title: station.address })
            .bindPopup(`<b>` + station.address + `</b><br>` + `<a href="javascript:console.log(${station.position.latitude}, ${station.position.longitude});">S'y rendre</a>`)
        );
    });

    map.addLayer(markersCluster);
}

const getPath = () => {
    const positions = [ start, startStationPosition, endStationPosition, end ];
    const data = "{\"positions\": " + JSON.stringify(positions) + "}";

    const targetUrl = API + "path";
    const requestType = "POST";

    const caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    caller.setRequestHeader("Content-Type", "application/json");
    caller.onreadystatechange = () => {
        if (caller.readyState === XMLHttpRequest.DONE && caller.status === 200) {
            const response = JSON.parse(caller.responseText);
            pathLayer.addLayer(L.geoJSON(JSON.parse(response)));
        }
    }

    caller.send(data);
}

const findNearestStartStation = (latitude, longitude) => {
    const targetUrl = API + "nearestStartStation?lat=" + latitude + "&lng=" + longitude;
    const requestType = "GET";

    const caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    caller.setRequestHeader("Accept", "application/json");
    caller.onload = () => {
        if (caller.responseText) {
            const station = JSON.parse(caller.responseText);
            startStationPosition = station.position;
        }
        else {
            console.warn("No stations found");
        }
        if (endStationPosition) {
            getPath();
        }
    }
    caller.send();
}

const findNearestEndStation = (latitude, longitude) => {
    const targetUrl = API + "nearestEndStation?lat=" + latitude + "&lng=" + longitude;
    const requestType = "GET";

    const caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    caller.setRequestHeader("Accept", "application/json");
    caller.onload = () => {
        if (caller.responseText) {
            const station = JSON.parse(caller.responseText);
            endStationPosition = station.position;
        }
        else {
            console.warn("No stations found");
        }
        if (startStationPosition) {
            getPath();
        }
    }
    caller.send();
}