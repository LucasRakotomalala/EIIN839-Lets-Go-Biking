let map = L.map('map');
let pathLayer = L.layerGroup();

let start;
let end;

let defaultPosition = {
    latitude: 45.764043,
    longitude: 4.835659
}; // Position de Lyon

let currentMarker;

let stations;
let startStationPosition;
let endStationPosition;

navigator.geolocation.watchPosition(
    (position) => {
        if (map && currentMarker) {
            map.removeLayer(currentMarker);
        }
        currentMarker = L.marker([position.coords.latitude, position.coords.longitude], { title: "Position actuelle" })
        .bindPopup(`<b>Postion actuelle</b>`)
        .addTo(map);
    },
    () => {
        console.warn("Can't watch the position");
    }
);

window.onload = () => {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register('serviceWorker.js');
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

const retrieveStations = () => {
    const targetUrl = "http://localhost:8080/api/stations";
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

    L.tileLayer('https://{s}.tile.openstreetmap.fr/osmfr/{z}/{x}/{y}.png', {
        attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        minZoom: 1,
        maxZoom: 17
    }).addTo(map);
    L.control.scale().addTo(map);

    map.addLayer(pathLayer);

    map.on('click', () => { // TODO: Add button to remove path
        start = null;
        startStationPosition = null;

        end = null;
        endStationPosition = null;

        pathLayer.clearLayers();
    });

    L.Control.geocoder({
        position: 'topright',
        collapsed: false,
        placeholder: 'Adresse de départ',
    }).on('markgeocode', (event) => {
        start = {
            latitude: event.geocode.center.lat,
            longitude: event.geocode.center.lng
        };
        findNearestStartStation(start.latitude, start.longitude);
      })
      .addTo(map);

      L.Control.geocoder({
        position: 'topright',
        collapsed: false,
        placeholder: 'Adresse d\'arrivée',
    }).on('markgeocode', (event) => {
        end = {
            latitude: event.geocode.center.lat,
            longitude: event.geocode.center.lng
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
            .bindPopup(`<b>` + station.address + `</b><br>` + `<a href='javascript:console.log(${station.position.latitude}, ${station.position.longitude});'>S'y rendre</a>`)
        );
    });

    map.addLayer(markersCluster);
}

const getPath = () => {
    const positions = [ start, startStationPosition, endStationPosition, end ];
    const jsonPositions = JSON.stringify(positions);
    const data = "{\"positions\": " + jsonPositions + "}";

    const targetUrl = "http://localhost:8080/api/path";
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
    const targetUrl = "http://localhost:8080/api/nearestStartStation?lat=" + latitude + "&lng=" + longitude;
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
    const targetUrl = "http://localhost:8080/api/nearestEndStation?lat=" + latitude + "&lng=" + longitude;
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