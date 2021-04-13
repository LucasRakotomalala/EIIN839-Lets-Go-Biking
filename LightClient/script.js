let map;
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
                map = L.map('map').setView([45.764043, 4.835659], 14); // Position de Lyon
                constructMap(map);
            }
        })
    } else {
        map = L.map('map').setView([45.764043, 4.835659], 14); // Position de Lyon
        constructMap(map);
    }
}

const retrieveStations = () => {
    const targetUrl = "https://api.jcdecaux.com/vls/v2/stations?apiKey=ff987c28b1313700e2c97651cec164bd6cb4ed76";
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
            .bindPopup(`<b>` + station.address + `</b><br>` + `<a href='javascript:goTo(${station.position.latitude}, ${station.position.longitude});'>S'y rendre </a>`)
        );
    });

    map.addLayer(markersCluster);
}

String.prototype.capitalize = () => {
    return this.charAt(0).toUpperCase() + this.slice(1);
}

const goTo = (latitude, longitude) => {
    console.log(latitude, longitude);
}