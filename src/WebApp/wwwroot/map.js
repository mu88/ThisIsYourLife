export function initializeMap(startLongitude, startLatitude, startZoom) {
    let myMap = L.map('mapid').setView([startLongitude, startLatitude], startZoom);
    L.tileLayer('https://a.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors'
    }).addTo(myMap);
    return myMap;
}

export function addMarker(myMap, id, longitude, latitude) {
    let marker = L.marker([longitude, latitude]).addTo(myMap);
    marker.bindPopup("Hello World!<br>Id='" + id + "'");
    return marker;
}