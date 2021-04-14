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
    retrieveStations();
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

    showStationMarkers();

    //const geo1 = { "type": "FeatureCollection", "features": [{ "bbox": [4.837026, 45.756591, 4.846338, 45.769489], "type": "Feature", "properties": { "segments": [{ "distance": 141.3, "duration": 28.3, "steps": [{ "distance": 81.3, "duration": 16.3, "type": 11, "instruction": "Démarrez en direction du Nord-Est sur Rue Saint-Jacques", "name": "Rue Saint-Jacques", "way_points": [0, 4] }, { "distance": 60.1, "duration": 12.0, "type": 0, "instruction": "Tournez à gauche sur Avenue Maréchal de Saxe", "name": "Avenue Maréchal de Saxe", "way_points": [4, 7] }, { "distance": 0.0, "duration": 0.0, "type": 10, "instruction": "Arrivé à Avenue Maréchal de Saxe, destination sur la gauche", "name": "-", "way_points": [7, 7] }] }, { "distance": 1835.6, "duration": 386.0, "steps": [{ "distance": 662.2, "duration": 132.4, "type": 11, "instruction": "Démarrez en direction du Nord sur Avenue Maréchal de Saxe", "name": "Avenue Maréchal de Saxe", "way_points": [7, 32] }, { "distance": 8.3, "duration": 5.0, "type": 0, "instruction": "Tournez à gauche", "name": "-", "way_points": [32, 33] }, { "distance": 9.5, "duration": 5.7, "type": 1, "instruction": "Tournez à droite", "name": "-", "way_points": [33, 35] }, { "distance": 9.9, "duration": 5.9, "type": 1, "instruction": "Tournez à droite", "name": "-", "way_points": [35, 37] }, { "distance": 133.3, "duration": 26.7, "type": 0, "instruction": "Tournez à gauche", "name": "-", "way_points": [37, 43] }, { "distance": 7.3, "duration": 4.4, "type": 1, "instruction": "Tournez à droite", "name": "-", "way_points": [43, 44] }, { "distance": 4.6, "duration": 2.8, "type": 0, "instruction": "Tournez à gauche", "name": "-", "way_points": [44, 45] }, { "distance": 453.2, "duration": 90.6, "type": 1, "instruction": "Tournez à droite sur Rue Molière", "name": "Rue Molière", "way_points": [45, 59] }, { "distance": 5.2, "duration": 3.1, "type": 0, "instruction": "Tournez à gauche", "name": "-", "way_points": [59, 61] }, { "distance": 2.4, "duration": 1.4, "type": 13, "instruction": "Tournez à droite", "name": "-", "way_points": [61, 62] }, { "distance": 73.1, "duration": 14.6, "type": 0, "instruction": "Tournez à gauche sur Place du Maréchal Lyautey", "name": "Place du Maréchal Lyautey", "way_points": [62, 65] }, { "distance": 111.4, "duration": 22.3, "type": 1, "instruction": "Tournez à droite", "name": "-", "way_points": [65, 70] }, { "distance": 217.2, "duration": 43.4, "type": 0, "instruction": "Tournez à gauche sur Pont Morand", "name": "Pont Morand", "way_points": [70, 77] }, { "distance": 25.5, "duration": 5.1, "type": 1, "instruction": "Tournez à droite sur Quai André Lassagne", "name": "Quai André Lassagne", "way_points": [77, 78] }, { "distance": 9.0, "duration": 1.8, "type": 0, "instruction": "Tournez à gauche sur Quai André Lassagne", "name": "Quai André Lassagne", "way_points": [78, 79] }, { "distance": 25.4, "duration": 5.1, "type": 13, "instruction": "Tournez à droite sur Quai André Lassagne", "name": "Quai André Lassagne", "way_points": [79, 82] }, { "distance": 78.2, "duration": 15.6, "type": 6, "instruction": "Continuez tout droit sur Place Tolozan", "name": "Place Tolozan", "way_points": [82, 88] }, { "distance": 0.0, "duration": 0.0, "type": 10, "instruction": "Arrivé à Place Tolozan, destination sur la gauche", "name": "-", "way_points": [88, 88] }] }, { "distance": 165.5, "duration": 56.4, "steps": [{ "distance": 67.2, "duration": 13.4, "type": 11, "instruction": "Démarrez en direction du Sud sur Place Tolozan", "name": "Place Tolozan", "way_points": [88, 93] }, { "distance": 44.8, "duration": 26.9, "type": 1, "instruction": "Tournez à droite", "name": "-", "way_points": [93, 98] }, { "distance": 53.5, "duration": 16.0, "type": 1, "instruction": "Tournez à droite sur Place Louis Pradel", "name": "Place Louis Pradel", "way_points": [98, 102] }, { "distance": 0.0, "duration": 0.0, "type": 10, "instruction": "Arrivé à Place Louis Pradel, destination sur la droite", "name": "-", "way_points": [102, 102] }] }], "summary": { "distance": 2142.4, "duration": 470.7 }, "way_points": [0, 7, 88, 102] }, "geometry": { "coordinates": [[4.845556, 45.756591], [4.846054, 45.756963], [4.846124, 45.757005], [4.84624, 45.757031], [4.846338, 45.757055], [4.846253, 45.757459], [4.846243, 45.757509], [4.846226, 45.757589], [4.846117, 45.758108], [4.846097, 45.758204], [4.846078, 45.758291], [4.845999, 45.758669], [4.845984, 45.758737], [4.845894, 45.759165], [4.845847, 45.759386], [4.845834, 45.759448], [4.845823, 45.759498], [4.845817, 45.759527], [4.845777, 45.759717], [4.845696, 45.760098], [4.845687, 45.76014], [4.845682, 45.760165], [4.845669, 45.760226], [4.845659, 45.760272], [4.845527, 45.7609], [4.845397, 45.761509], [4.845375, 45.761617], [4.845352, 45.761726], [4.845259, 45.762164], [4.845158, 45.762642], [4.845149, 45.762685], [4.845045, 45.763175], [4.844977, 45.763481], [4.844871, 45.763478], [4.844843, 45.763516], [4.844785, 45.763531], [4.844784, 45.763568], [4.844781, 45.76362], [4.844397, 45.763619], [4.843971, 45.763615], [4.843867, 45.763614], [4.843778, 45.763614], [4.843143, 45.76361], [4.843064, 45.763602], [4.843051, 45.763666], [4.842991, 45.763661], [4.842892, 45.764203], [4.842778, 45.764756], [4.842771, 45.764789], [4.842765, 45.764819], [4.842757, 45.764865], [4.842621, 45.765606], [4.842587, 45.765794], [4.842578, 45.765841], [4.842568, 45.765892], [4.842442, 45.76653], [4.84243, 45.766587], [4.84242, 45.766639], [4.842216, 45.7677], [4.842216, 45.7677], [4.84217, 45.767695], [4.842153, 45.767704], [4.842148, 45.767725], [4.841346, 45.767656], [4.841271, 45.767647], [4.841213, 45.767644], [4.841191, 45.767764], [4.84106, 45.768499], [4.841054, 45.768534], [4.841045, 45.768585], [4.841036, 45.768637], [4.840963, 45.768637], [4.84088, 45.768636], [4.840787, 45.768636], [4.838419, 45.768613], [4.838391, 45.768613], [4.838362, 45.768613], [4.838237, 45.768612], [4.838226, 45.768841], [4.83814, 45.768896], [4.837992, 45.76893], [4.837912, 45.768968], [4.837845, 45.76899], [4.837709, 45.769018], [4.837626, 45.769028], [4.837616, 45.76903], [4.837531, 45.769083], [4.837525, 45.769102], [4.837435, 45.769489], [4.837525, 45.769102], [4.837531, 45.769083], [4.837616, 45.76903], [4.837626, 45.769028], [4.837709, 45.769018], [4.837713, 45.768961], [4.837733, 45.768905], [4.837739, 45.768708], [4.837703, 45.768672], [4.837707, 45.768625], [4.837428, 45.768604], [4.837357, 45.768594], [4.837118, 45.768568], [4.837026, 45.768551]], "type": "LineString" } }], "bbox": [4.837026, 45.756591, 4.846338, 45.769489], "metadata": { "attribution": "openrouteservice.org | OpenStreetMap contributors", "service": "routing", "timestamp": 1618398488746, "query": { "coordinates": [[4.8456121, 45.7565545], [4.846117, 45.757578], [4.837495, 45.769508], [4.8370114, 45.7685914]], "profile": "cycling-regular", "preference": "shortest", "format": "geojson", "units": "m", "language": "fr", "instructions": true }, "engine": { "version": "6.4.1", "build_date": "2021-04-12T07:11:51Z", "graph_date": "1970-01-01T00:00:00Z" } } }
    //L.geoJSON(geo1).addTo(map)
    //const geo2 = { "type": "FeatureCollection", "features": [{ "bbox": [4.837026, 45.756591, 4.846338, 45.769489], "type": "Feature", "properties": { "segments": [{ "distance": 165.5, "duration": 56.4, "steps": [{ "distance": 53.5, "duration": 16.0, "type": 11, "instruction": "Démarrez en direction de l'Est sur Place Louis Pradel", "name": "Place Louis Pradel", "way_points": [0, 4] }, { "distance": 44.8, "duration": 26.9, "type": 0, "instruction": "Tournez à gauche", "name": "-", "way_points": [4, 9] }, { "distance": 67.2, "duration": 13.4, "type": 0, "instruction": "Tournez à gauche sur Place Tolozan", "name": "Place Tolozan", "way_points": [9, 14] }, { "distance": 0.0, "duration": 0.0, "type": 10, "instruction": "Arrivé à Place Tolozan, destination sur la droite", "name": "-", "way_points": [14, 14] }] }, { "distance": 1834.4, "duration": 422.2, "steps": [{ "distance": 78.2, "duration": 15.6, "type": 11, "instruction": "Démarrez en direction du Sud sur Place Tolozan", "name": "Place Tolozan", "way_points": [14, 20] }, { "distance": 37.2, "duration": 7.4, "type": 1, "instruction": "Tournez à droite sur Quai André Lassagne", "name": "Quai André Lassagne", "way_points": [20, 23] }, { "distance": 244.8, "duration": 48.9, "type": 0, "instruction": "Tournez à gauche sur Quai André Lassagne", "name": "Quai André Lassagne", "way_points": [23, 31] }, { "distance": 28.4, "duration": 17.0, "type": 1, "instruction": "Tournez à droite", "name": "-", "way_points": [31, 34] }, { "distance": 82.9, "duration": 49.7, "type": 12, "instruction": "Tournez à gauche", "name": "-", "way_points": [34, 36] }, { "distance": 3.3, "duration": 2.0, "type": 0, "instruction": "Tournez à gauche", "name": "-", "way_points": [36, 37] }, { "distance": 33.6, "duration": 6.7, "type": 1, "instruction": "Tournez à droite", "name": "-", "way_points": [37, 38] }, { "distance": 82.0, "duration": 17.2, "type": 12, "instruction": "Tournez à gauche sur Sortie Parking", "name": "Sortie Parking", "way_points": [38, 41] }, { "distance": 6.3, "duration": 1.3, "type": 0, "instruction": "Tournez à gauche", "name": "-", "way_points": [41, 42] }, { "distance": 8.2, "duration": 1.6, "type": 1, "instruction": "Tournez à droite", "name": "-", "way_points": [42, 44] }, { "distance": 70.5, "duration": 14.1, "type": 0, "instruction": "Tournez à gauche sur Rue Cuvier", "name": "Rue Cuvier", "way_points": [44, 46] }, { "distance": 336.4, "duration": 67.3, "type": 1, "instruction": "Tournez à droite sur Rue Molière", "name": "Rue Molière", "way_points": [46, 58] }, { "distance": 138.6, "duration": 27.7, "type": 0, "instruction": "Tournez à gauche sur Cours Lafayette", "name": "Cours Lafayette", "way_points": [58, 64] }, { "distance": 4.1, "duration": 2.5, "type": 1, "instruction": "Tournez à droite", "name": "-", "way_points": [64, 65] }, { "distance": 9.5, "duration": 5.7, "type": 0, "instruction": "Tournez à gauche", "name": "-", "way_points": [65, 67] }, { "distance": 8.3, "duration": 5.0, "type": 0, "instruction": "Tournez à gauche", "name": "-", "way_points": [67, 68] }, { "distance": 662.2, "duration": 132.4, "type": 1, "instruction": "Tournez à droite sur Avenue Maréchal de Saxe", "name": "Avenue Maréchal de Saxe", "way_points": [68, 93] }, { "distance": 0.0, "duration": 0.0, "type": 10, "instruction": "Arrivé à Avenue Maréchal de Saxe, destination sur la droite", "name": "-", "way_points": [93, 93] }] }, { "distance": 141.3, "duration": 28.3, "steps": [{ "distance": 60.1, "duration": 12.0, "type": 11, "instruction": "Démarrez en direction du Sud sur Avenue Maréchal de Saxe", "name": "Avenue Maréchal de Saxe", "way_points": [93, 96] }, { "distance": 24.7, "duration": 4.9, "type": 1, "instruction": "Tournez à droite sur Rue Saint-Jacques", "name": "Rue Saint-Jacques", "way_points": [96, 99] }, { "distance": 56.6, "duration": 11.3, "type": 12, "instruction": "Tournez à gauche sur Rue Saint-Jacques", "name": "Rue Saint-Jacques", "way_points": [99, 100] }, { "distance": 0.0, "duration": 0.0, "type": 10, "instruction": "Arrivé à Rue Saint-Jacques, destination sur la gauche", "name": "-", "way_points": [100, 100] }] }], "summary": { "distance": 2141.2, "duration": 506.9 }, "way_points": [0, 14, 93, 100] }, "geometry": { "coordinates": [[4.837026, 45.768551], [4.837118, 45.768568], [4.837357, 45.768594], [4.837428, 45.768604], [4.837707, 45.768625], [4.837703, 45.768672], [4.837739, 45.768708], [4.837733, 45.768905], [4.837713, 45.768961], [4.837709, 45.769018], [4.837626, 45.769028], [4.837616, 45.76903], [4.837531, 45.769083], [4.837525, 45.769102], [4.837435, 45.769489], [4.837525, 45.769102], [4.837531, 45.769083], [4.837616, 45.76903], [4.837626, 45.769028], [4.837709, 45.769018], [4.837845, 45.76899], [4.837837, 45.768785], [4.837846, 45.768678], [4.837848, 45.768656], [4.837936, 45.768638], [4.838128, 45.768557], [4.838239, 45.768556], [4.838365, 45.768558], [4.838394, 45.768559], [4.838439, 45.76856], [4.840787, 45.768583], [4.840967, 45.768583], [4.840969, 45.768569], [4.84098, 45.768529], [4.841016, 45.76833], [4.841073, 45.768259], [4.841176, 45.767599], [4.841218, 45.767601], [4.841261, 45.7673], [4.841318, 45.767185], [4.841345, 45.76702], [4.841422, 45.766574], [4.841503, 45.766574], [4.841521, 45.766543], [4.84153, 45.766503], [4.842345, 45.766579], [4.84243, 45.766587], [4.842442, 45.76653], [4.842568, 45.765892], [4.842578, 45.765841], [4.842587, 45.765794], [4.842621, 45.765606], [4.842757, 45.764865], [4.842765, 45.764819], [4.842771, 45.764789], [4.842778, 45.764756], [4.842892, 45.764203], [4.842991, 45.763661], [4.843002, 45.763588], [4.84307, 45.763574], [4.843137, 45.763563], [4.843783, 45.763563], [4.843876, 45.763563], [4.843972, 45.763565], [4.844784, 45.763568], [4.844785, 45.763531], [4.844843, 45.763516], [4.844871, 45.763478], [4.844977, 45.763481], [4.845045, 45.763175], [4.845149, 45.762685], [4.845158, 45.762642], [4.845259, 45.762164], [4.845352, 45.761726], [4.845375, 45.761617], [4.845397, 45.761509], [4.845527, 45.7609], [4.845659, 45.760272], [4.845669, 45.760226], [4.845682, 45.760165], [4.845687, 45.76014], [4.845696, 45.760098], [4.845777, 45.759717], [4.845817, 45.759527], [4.845823, 45.759498], [4.845834, 45.759448], [4.845847, 45.759386], [4.845894, 45.759165], [4.845984, 45.758737], [4.845999, 45.758669], [4.846078, 45.758291], [4.846097, 45.758204], [4.846117, 45.758108], [4.846226, 45.757589], [4.846243, 45.757509], [4.846253, 45.757459], [4.846338, 45.757055], [4.84624, 45.757031], [4.846124, 45.757005], [4.846054, 45.756963], [4.845556, 45.756591]], "type": "LineString" } }], "bbox": [4.837026, 45.756591, 4.846338, 45.769489], "metadata": { "attribution": "openrouteservice.org | OpenStreetMap contributors", "service": "routing", "timestamp": 1618398789446, "query": { "coordinates": [[4.8370114, 45.7685914], [4.837495, 45.769508], [4.846117, 45.757578], [4.8456121, 45.7565545]], "profile": "cycling-regular", "preference": "shortest", "format": "geojson", "units": "m", "language": "fr", "instructions": true }, "engine": { "version": "6.4.1", "build_date": "2021-04-12T07:11:51Z", "graph_date": "1970-01-01T00:00:00Z" } } }
    //L.geoJSON(geo2).addTo(map)
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

