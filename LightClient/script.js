let map = L.map('map');
let pathLayer = L.layerGroup();

let start;
let end;

let defaultPosition = {
    latitude: 45.764043,
    longitude: 4.835659
};

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
    retrieveStations();
    if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition((position) => {
            map = map.setView([position.coords.latitude, position.coords.longitude], 14);
            constructMap(map);
        }, error => {
            if (error.code == error.PERMISSION_DENIED) {
                map = map.setView([defaultPosition.latitude, defaultPosition.longitude], 14); // Position de Lyon
                constructMap(map);
            }
        })
    } else {
        map = map.setView([defaultPosition.latitude, defaultPosition.longitude], 14); // Position de Lyon
        constructMap(map);
    }
}

const retrieveStations = () => {
    const targetUrl = "http://localhost:8080/api/stations";
    //const targetUrl = "https://api.jcdecaux.com/vls/v2/stations?apiKey=ff987c28b1313700e2c97651cec164bd6cb4ed76";
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
    L.control.scale().addTo(map);

    map.addLayer(pathLayer);

    showStationMarkers();

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
        start = event.geocode.center;
        findNearestStartStation(start.lat, start.lng);
      })
      .addTo(map);

      L.Control.geocoder({
        position: 'topright',
        collapsed: false,
        placeholder: 'Adresse d\'arrivée',
    }).on('markgeocode', (event) => {
        end = event.geocode.center;
        findNearestEndStation(end.lat, end.lng);
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

const getPath = (latitudeStart, longitudeStart, latitudeEnd, longitudeEnd) => {
    const targetUrl = "http://localhost:8080/api/path?startLat=" + latitudeStart + "&startLng=" + longitudeStart + "&endLat=" + latitudeEnd + "&endLng=" + longitudeEnd;
    const requestType = "GET";

    const caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    caller.setRequestHeader("Accept", "application/json");
    caller.onload = () => {
        const response = JSON.parse(caller.responseText);
        pathLayer.addLayer(L.geoJSON(JSON.parse(response)));
    }
    caller.send();
}

const findNearestStartStation = (latitude, longitude) => {
    const targetUrl = "http://localhost:8080/api/nearestStartStation?lat=" + latitude + "&lng=" + longitude;
    const requestType = "GET";

    const caller = new XMLHttpRequest();
    caller.open(requestType, targetUrl, true);
    caller.setRequestHeader("Accept", "application/json");
    caller.onload = () => {
        const station = JSON.parse(caller.responseText);
        startStationPosition = station.position;
        getPath(latitude, longitude, startStationPosition.latitude, startStationPosition.longitude);
        if (endStationPosition) {
            getPath(startStationPosition.latitude, startStationPosition.longitude, endStationPosition.latitude, endStationPosition.longitude);
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
        const station = JSON.parse(caller.responseText);
        endStationPosition = station.position;
        getPath(endStationPosition.latitude, endStationPosition.longitude, latitude, longitude);
        if (startStationPosition) {
            getPath(startStationPosition.latitude, startStationPosition.longitude, endStationPosition.latitude, endStationPosition.longitude);
        }
    }
    caller.send();
}

