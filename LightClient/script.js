let map = L.map('map');
let pathLayer = L.layerGroup();

let start = null;
let end = null;

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
        if (map !== undefined && currentMarker !== undefined) {
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
    //const targetUrl = "http://localhost:8080/api/stations";
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
    L.control.scale().addTo(map);

    map.addLayer(pathLayer);

    showStationMarkers();
    showPath(); //TODO: Remove

    map.on('click', () => { // TODO: Add button to remove path
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
        findNearestStartStation(end.lat, end.lng);
      })
      .addTo(map);

    L.Control.B
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
        if (endStationPosition !== null) {
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
        if (startStationPosition !== null) {
            getPath(startStationPosition.latitude, startStationPosition.longitude, endStationPosition.latitude, endStationPosition.longitude);
        }
    }
    caller.send();
}

const showPath = () => {
    const geo0 = { "type": "FeatureCollection", "features": [{ "bbox": [4.838262, 45.753617, 4.838624, 45.75388], "type": "Feature", "properties": { "segments": [{ "distance": 65.1, "duration": 39.0, "steps": [{ "distance": 24.0, "duration": 14.4, "type": 11, "instruction": "Head northeast", "name": "-", "way_points": [0, 5] }, { "distance": 19.9, "duration": 11.9, "type": 0, "instruction": "Turn left", "name": "-", "way_points": [5, 8] }, { "distance": 21.2, "duration": 12.7, "type": 0, "instruction": "Turn left", "name": "-", "way_points": [8, 9] }, { "distance": 0.0, "duration": 0.0, "type": 10, "instruction": "Arrive at your destination, on the left", "name": "-", "way_points": [9, 9] }] }], "summary": { "distance": 65.1, "duration": 39.0 }, "way_points": [0, 9] }, "geometry": { "coordinates": [[4.838476, 45.753617], [4.838526, 45.753683], [4.838553, 45.753699], [4.838582, 45.753733], [4.838608, 45.753764], [4.838624, 45.753804], [4.838528, 45.753836], [4.838451, 45.753861], [4.838393, 45.75388], [4.838262, 45.753713]], "type": "LineString" } }], "bbox": [4.838262, 45.753617, 4.838624, 45.75388], "metadata": { "attribution": "openrouteservice.org | OpenStreetMap contributors", "service": "routing", "timestamp": 1618333145487, "query": { "coordinates": [[4.8385087, 45.7536047], [4.838283, 45.753705]], "profile": "cycling-regular", "format": "json" }, "engine": { "version": "6.4.1", "build_date": "2021-04-12T07:11:51Z", "graph_date": "1970-01-01T00:00:00Z" } } }
    const geo1 = { "type": "FeatureCollection", "features": [{ "bbox": [4.838262, 45.753713, 4.853627, 45.759941], "type": "Feature", "properties": { "segments": [{ "distance": 1758.3, "duration": 375.4, "steps": [{ "distance": 21.2, "duration": 12.7, "type": 11, "instruction": "Head northeast", "name": "-", "way_points": [0, 1] }, { "distance": 5.0, "duration": 3.0, "type": 1, "instruction": "Turn right", "name": "-", "way_points": [1, 2] }, { "distance": 285.1, "duration": 57.0, "type": 0, "instruction": "Turn left", "name": "-", "way_points": [2, 9] }, { "distance": 32.6, "duration": 6.5, "type": 1, "instruction": "Turn right", "name": "-", "way_points": [9, 12] }, { "distance": 99.1, "duration": 19.8, "type": 13, "instruction": "Keep right", "name": "-", "way_points": [12, 16] }, { "distance": 214.2, "duration": 42.8, "type": 0, "instruction": "Turn left", "name": "-", "way_points": [16, 27] }, { "distance": 875.3, "duration": 175.1, "type": 1, "instruction": "Turn right onto Rue de la Part-Dieu", "name": "Rue de la Part-Dieu", "way_points": [27, 57] }, { "distance": 74.2, "duration": 14.8, "type": 1, "instruction": "Turn right onto Rue Garibaldi", "name": "Rue Garibaldi", "way_points": [57, 60] }, { "distance": 19.5, "duration": 3.9, "type": 0, "instruction": "Turn left", "name": "-", "way_points": [60, 62] }, { "distance": 11.5, "duration": 2.3, "type": 1, "instruction": "Turn right onto Rue Garibaldi", "name": "Rue Garibaldi", "way_points": [62, 64] }, { "distance": 87.4, "duration": 17.5, "type": 0, "instruction": "Turn left onto Rue du Docteur Bouchut", "name": "Rue du Docteur Bouchut", "way_points": [64, 68] }, { "distance": 33.3, "duration": 20.0, "type": 1, "instruction": "Turn right", "name": "-", "way_points": [68, 70] }, { "distance": 0.0, "duration": 0.0, "type": 10, "instruction": "Arrive at your destination, on the right", "name": "-", "way_points": [70, 70] }] }], "summary": { "distance": 1758.3, "duration": 375.4 }, "way_points": [0, 70] }, "geometry": { "coordinates": [[4.838262, 45.753713], [4.838393, 45.75388], [4.838451, 45.753861], [4.839003, 45.754555], [4.839229, 45.75484], [4.839421, 45.75508], [4.839628, 45.755364], [4.839729, 45.755501], [4.840115, 45.75605], [4.840085, 45.756132], [4.840149, 45.7562], [4.840214, 45.756299], [4.840275, 45.756393], [4.840362, 45.756552], [4.840473, 45.75679], [4.840562, 45.757182], [4.840652, 45.75722], [4.840678, 45.75736], [4.840692, 45.757437], [4.840712, 45.757652], [4.840737, 45.757926], [4.840751, 45.758054], [4.840788, 45.7584], [4.840796, 45.758477], [4.840845, 45.758942], [4.840854, 45.759023], [4.840856, 45.759045], [4.840866, 45.75914], [4.840924, 45.759143], [4.840948, 45.759144], [4.841071, 45.759153], [4.841099, 45.759155], [4.842399, 45.759248], [4.842472, 45.759253], [4.842633, 45.759263], [4.843057, 45.759292], [4.843146, 45.759298], [4.843204, 45.759302], [4.843724, 45.759339], [4.843807, 45.759345], [4.843881, 45.75935], [4.844551, 45.7594], [4.84465, 45.759407], [4.844779, 45.759417], [4.845668, 45.759486], [4.845823, 45.759498], [4.84678, 45.759566], [4.847732, 45.759634], [4.847826, 45.759641], [4.847955, 45.759649], [4.8484, 45.75968], [4.848961, 45.75972], [4.849042, 45.759726], [4.84911, 45.759731], [4.849789, 45.759783], [4.850762, 45.759856], [4.851958, 45.759933], [4.85209, 45.759941], [4.852098, 45.759903], [4.852208, 45.75932], [4.852213, 45.75928], [4.852321, 45.75929], [4.852462, 45.759303], [4.852463, 45.759234], [4.852464, 45.759199], [4.852542, 45.759181], [4.852618, 45.759188], [4.852671, 45.759192], [4.853577, 45.759274], [4.853584, 45.75923], [4.853627, 45.758976]], "type": "LineString" } }], "bbox": [4.838262, 45.753713, 4.853627, 45.759941], "metadata": { "attribution": "openrouteservice.org | OpenStreetMap contributors", "service": "routing", "timestamp": 1618333145574, "query": { "coordinates": [[4.838283, 45.753705], [4.853599, 45.758974]], "profile": "cycling-regular", "format": "json" }, "engine": { "version": "6.4.1", "build_date": "2021-04-12T07:11:51Z", "graph_date": "1970-01-01T00:00:00Z" } } }
    const geo2 = { "type": "FeatureCollection", "features": [{ "bbox": [4.853577, 45.758453, 4.854582, 45.759339], "type": "Feature", "properties": { "segments": [{ "distance": 199.5, "duration": 53.2, "steps": [{ "distance": 33.3, "duration": 20.0, "type": 11, "instruction": "Head north", "name": "-", "way_points": [0, 2] }, { "distance": 53.0, "duration": 10.6, "type": 1, "instruction": "Turn right onto Rue du Docteur Bouchut", "name": "Rue du Docteur Bouchut", "way_points": [2, 5] }, { "distance": 41.7, "duration": 8.3, "type": 1, "instruction": "Turn right onto Rue du Lac", "name": "Rue du Lac", "way_points": [5, 8] }, { "distance": 48.4, "duration": 9.7, "type": 0, "instruction": "Turn left", "name": "-", "way_points": [8, 11] }, { "distance": 23.1, "duration": 4.6, "type": 1, "instruction": "Turn right", "name": "-", "way_points": [11, 12] }, { "distance": 0.0, "duration": 0.0, "type": 10, "instruction": "Arrive at your destination, on the left", "name": "-", "way_points": [12, 12] }] }], "summary": { "distance": 199.5, "duration": 53.2 }, "way_points": [0, 12] }, "geometry": { "coordinates": [[4.853627, 45.758976], [4.853584, 45.75923], [4.853577, 45.759274], [4.854143, 45.75933], [4.854152, 45.759331], [4.854254, 45.759339], [4.854268, 45.759281], [4.854281, 45.759221], [4.854334, 45.758968], [4.854441, 45.758976], [4.854497, 45.758654], [4.854548, 45.758659], [4.854582, 45.758453]], "type": "LineString" } }], "bbox": [4.853577, 45.758453, 4.854582, 45.759339], "metadata": { "attribution": "openrouteservice.org | OpenStreetMap contributors", "service": "routing", "timestamp": 1618333145650, "query": { "coordinates": [[4.853599, 45.758974], [4.8547063, 45.7584625]], "profile": "cycling-regular", "format": "json" }, "engine": { "version": "6.4.1", "build_date": "2021-04-12T07:11:51Z", "graph_date": "1970-01-01T00:00:00Z" } } }
    pathLayer.addLayer(L.geoJSON(geo0));
    pathLayer.addLayer(L.geoJSON(geo1));
    pathLayer.addLayer(L.geoJSON(geo2));
}

