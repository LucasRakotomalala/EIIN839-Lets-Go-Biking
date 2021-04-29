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

let map = L.map("map", { worldCopyJump: true });
let pathLayer = L.layerGroup();

let start;
let end;

let currentMarker;
let currentPosition;

let stations;

let startStationPosition;
let endStationPosition;

window.onload = () => {
    if ("serviceWorker" in navigator) {
        navigator.serviceWorker.register("serviceWorker.js").then(
            () => {
                //console.log("Registration to the serviceWorker successful");
            },
            () => {
                console.error("Failed to register the serviceWorker");
            }
        );
    }

    map = map.setView([defaultPosition.latitude, defaultPosition.longitude], 14);
    constructMap(map);
}

const retrieveStations = () => {
    const targetUrl = API + "stations";
    const requestType = "GET";

    const caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    caller.setRequestHeader("Accept", "application/json");
    caller.onload = () => {
        stations = JSON.parse(caller.responseText);
        showStationsMarker();
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

    L.easyButton("<img src=\"resources/my_location_icon.png\" alt=\"My Location\">", (button, map) => {
        map.locate({
            setView: true,
            maxZoom: 16,
            watch: true,
            enableHighAccuracy: true
        })
        .on("locationfound", (event) => {
            if (map && currentMarker) {
                currentPosition = null;
                map.removeLayer(currentMarker);
            }
            currentPosition = event.latlng;
            currentMarker = L.marker(event.latlng, { title: "Position actuelle" })
                .bindPopup("<b>Postion actuelle</b>")
                .addTo(map);
        });
    }, "Me localiser").addTo(map);

    L.easyButton("<img src=\"resources/remove_path_icon.png\" alt=\"Remove Paths\">", () => {
        start = null;
        end = null;

        startStationPosition = null;
        endStationPosition = null;

        pathLayer.clearLayers();

        const inputs = document.getElementsByTagName("input");
        for (let i = 0; i < inputs.length; i++) {
            if (inputs[i].type === "text") {
                inputs[i].value = "";
            }
        }
    }, "Supprimer les chemins cherchés").addTo(map);
}

const showStationsMarker = () => {
    let markersCluster = L.markerClusterGroup();

    stations.forEach(station => {
        markersCluster.addLayer(L
            .marker([station.position.latitude, station.position.longitude], { title: station.name })
            .bindPopup(`<b>` + station.name + `</b><br>` + `<a href="javascript:goToStation(${station.position.latitude}, ${station.position.longitude});">S'y rendre</a>
<a href="javascript:retrieveStationInformations('${station.contract_name}', ${station.number});" style="float: right;">Plus d'infos</a>`)
        );
    });

    map.addLayer(markersCluster);
}

const getPath = () => {
    const positions = [start, startStationPosition, endStationPosition, end];
    const data = "{\"positions\": " + JSON.stringify(positions) + "}";

    const targetUrl = API + "path";
    const requestType = "POST";

    const caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    caller.setRequestHeader("Content-Type", "application/json");
    caller.onreadystatechange = () => {
        if (caller.readyState === XMLHttpRequest.DONE && caller.status === 200) {
            pathLayer.addLayer(L.geoJSON(JSON.parse(caller.responseText)));
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
            console.warn("No station found");
        }
        if (startStationPosition && endStationPosition) {
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
        if (startStationPosition && endStationPosition) {
            getPath();
        }
    }
    caller.send();
}

const goToStation = (latitude, longitude) => {
    if (currentPosition) {
        const positions = [{latitude: currentPosition.lat, longitude: currentPosition.lng}, {latitude, longitude}];
        const data = "{\"positions\": " + JSON.stringify(positions) + "}";

        const targetUrl = API + "goToStation";
        const requestType = "POST";

        const caller = new XMLHttpRequest();
        caller.open(requestType, targetUrl, true);
        caller.setRequestHeader("Content-Type", "application/json");
        caller.onreadystatechange = () => {
            if (caller.readyState === XMLHttpRequest.DONE && caller.status === 200) {
                pathLayer.addLayer(L.geoJSON(JSON.parse(caller.responseText)));
            }
        }
        caller.send(data);
    }
    else {
        map.locate({
            setView: true,
            maxZoom: 16,
            watch: true,
            enableHighAccuracy: true
        })
        .once("locationfound", (event) => {
            if (map && currentMarker) {
                currentPosition = null;
                map.removeLayer(currentMarker);
            }
            currentPosition = event.latlng;
            currentMarker = L.marker(event.latlng, { title: "Position actuelle" })
                .bindPopup("<b>Postion actuelle</b>")
                .addTo(map);
            goToStation(latitude, longitude);
        });
    }
}

const retrieveStationInformations = (city, stationNumber) => {
    const targetUrl = API + "station?city=" + city + "&number=" + stationNumber;
    const requestType = "GET";

    const caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    caller.setRequestHeader("Accept", "application/json");
    caller.onload = () => {
        if (caller.responseText) {
            const station = JSON.parse(caller.responseText);
            document.getElementById("offcanvasLabel").innerHTML = station.name + " à " + station.contract_name;
            document.getElementById("offcanvasBody").innerHTML = "Station : " + ((station.status === "OPEN") ? "Ouverte" : "Fermée") + "<br>Adresse : " + station.address + "<br>Nombre de vélos disponibles : " + station.available_bikes + "<br>Nombre de place disponibles : " + station.available_bike_stands;
            document.getElementById("buttonoffcanvas").click();
        }
        else {
            console.warn("No information found");
            document.getElementById("offcanvasLabel").innerHTML = "No information found";
            document.getElementById("buttonoffcanvas").click();
        }
    }
    caller.send();
}
