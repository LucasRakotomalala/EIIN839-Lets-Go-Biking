let map;

let path = null;

let defaultPosition = {
    latitude: 45.764043,
    longitude: 4.835659
};

let currentPosition;
let currentMarker;

let stations;

navigator.geolocation.watchPosition(
    (position) => {
        if (map !== undefined && currentMarker !== undefined) {
            map.removeLayer(currentMarker);
        }
        currentPosition = position.coords;
        currentMarker = L.marker([position.coords.latitude, position.coords.longitude], { title: "Position actuelle" })
        .bindPopup(`<b>Postion actuelle</b>`)
        .addTo(map);
    },
    () => {
        console.error("Can't watch the position");
    }
);

window.onload = () => {
    retrieveStations();
    if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition((position) => {
            map = L.map('map').setView([position.coords.latitude, position.coords.longitude], 14);
            constructMap(map);
        }, error => {
            if (error.code == error.PERMISSION_DENIED) {
                map = L.map('map').setView([defaultPosition.latitude, defaultPosition.longitude], 14); // Position de Lyon
                constructMap(map);
            }
        })
    } else {
        map = L.map('map').setView([defaultPosition.latitude, defaultPosition.longitude], 14); // Position de Lyon
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
    }
    caller.send();
}


const constructMap = (map) => {
    L.tileLayer('https://{s}.tile.openstreetmap.fr/osmfr/{z}/{x}/{y}.png', {
        attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        minZoom: 1,
        maxZoom: 20
    }).addTo(map);
    showMarkers();
}

const showMarkers = () => {
    let markersCluster = L.markerClusterGroup();

    stations.forEach(station => {
        markersCluster.addLayer(L
            .marker([station.position.latitude, station.position.longitude], { title: station.address })
            .bindPopup(`<b>` + station.address + `</b><br>` + `<a href='javascript:console.log(${station.position.latitude}, ${station.position.longitude});'>S'y rendre</a>`)
        );
    });

    map.addLayer(markersCluster);
}

const getPath = (latitudeStart, longitudeStart, latitudeEnd, longitudeEnd) => {
    if (path !== null) {
        map.removeLayer(path);
    }

    const startStationPosition = findNearestStartStation(latitudeStart, longitudeStart);
    const endStationPosition = findNearestEndStation(latitudeEnd, longitudeEnd);

    const targetUrl = "http://localhost:8080/api/path?startLat=" + latitudeStart + "&startLng=" + longitudeStart + "&endLat=" + latitudeEnd + "&endLng=" + longitudeEnd;
    const requestType = "GET";

    const caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    caller.setRequestHeader("Accept", "application/json");
    caller.onload = () => {
        const response = JSON.parse(caller.responseText);
        path = L.geoJSON(JSON.parse(response)).addTo(map);
    }
    caller.send();
}

const findNearestStartStation = (latitude, longitude) => {
    let position;

    const targetUrl = "http://localhost:8080/api/nearestStartStation?lat=" + latitude + "&lng=" + longitude;
    const requestType = "GET";

    const caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    caller.setRequestHeader("Accept", "application/json");
    caller.onload = () => {
        const station = JSON.parse(caller.responseText);
        position = station.position;
    }
    caller.send();

    return position;
}

const findNearestEndStation = (latitude, longitude) => {
    let position;

    const targetUrl = "http://localhost:8080/api/nearestEndStation?lat=" + latitude + "&lng=" + longitude;
    const requestType = "GET";

    const caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    caller.setRequestHeader("Accept", "application/json");
    caller.onload = () => {
        const station = JSON.parse(caller.responseText);
        position = station.position;
    }
    caller.send();

    return position;
}