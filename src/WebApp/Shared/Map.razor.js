// TODO mu88: Refactor class into better functions

let _dotNetMapReference;
let _leafletMap;

export function initializeMap(startLongitude, startLatitude, startZoom, dotNetMapReference) {
    _dotNetMapReference = dotNetMapReference;
    _leafletMap = L.map("mapid").setView([startLongitude, startLatitude], startZoom);
    L.tileLayer("https://a.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution: '&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors'
    }).addTo(_leafletMap);

    _leafletMap.on('dblclick', onMapDoubleClick);
    
    return _leafletMap;
}

function onMapDoubleClick(e) {
    // TODO mu88: Handle 'Abort/Close'
    let latitude = e.latlng.lat;
    let longitude = e.latlng.lng;
    _dotNetMapReference.invokeMethodAsync('OpenPopupForNewLifePoint', latitude, longitude);
}