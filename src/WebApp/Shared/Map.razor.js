let _dotNetObjectReference;

export function initializeMap(startLongitude, startLatitude, startZoom, dotNetObjectReference) {
    _dotNetObjectReference = dotNetObjectReference;
    let myMap = L.map("mapid").setView([startLongitude, startLatitude], startZoom);
    L.tileLayer("https://a.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution: '&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors'
    }).addTo(myMap);

    myMap.on('dblclick', onMapDoubleClick);

    return myMap;
}

export function addMarker(myMap, id, longitude, latitude) {
    let marker = L.marker([longitude, latitude]).addTo(myMap);
    marker.bindPopup("<life-point-detail id='" + id + "'></life-point-detail>", { minWidth: 500 });
    return marker;
}

function onMapDoubleClick(e) {
    _dotNetObjectReference.invokeMethodAsync("CreateNewLifePointAsync", e.latlng.lng, e.latlng.lat)
        .then(data => {
            _dotNetObjectReference.invokeMethodAsync("ReloadAndDrawAllLocationsAsync");
        });
}