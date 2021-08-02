export function initializeMap(startLongitude, startLatitude, startZoom) {
    var myMap = L.map('mapid').setView([startLongitude, startLatitude], startZoom);
    L.tileLayer('https://a.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors'
    }).addTo(myMap);
    return myMap;
}

export function addMarker(myMap, longitude, latitude) {
    L.marker([longitude, latitude]).addTo(myMap);
}