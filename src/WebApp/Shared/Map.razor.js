// TODO mu88: Refactor class into better functions

let _dotNetObjectReference;
let myMap;
let newLifePointIdCounter = 0;
let newLifePointsMap = new Map();

export function initializeMap(startLongitude, startLatitude, startZoom, dotNetObjectReference) {
    _dotNetObjectReference = dotNetObjectReference;
    myMap = L.map("mapid").setView([startLongitude, startLatitude], startZoom);
    L.tileLayer("https://a.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution: '&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors'
    }).addTo(myMap);

    myMap.on('dblclick', onMapDoubleClick);
}

export function addMarker(id, latitude, longitude) {
    let marker = L.marker([latitude, longitude]).addTo(myMap);
    marker.bindPopup("<life-point-detail id='" + id + "'></life-point-detail>", { minWidth: 500 });
    return marker;
}

export function closeNewLifePointPopup(id) {
    newLifePointsMap.get(id).remove();
    newLifePointsMap.delete(id);
}

function onMapDoubleClick(e) {
    let latitude = e.latlng.lat;
    let longitude = e.latlng.lng;
    let popup = L.popup({ minWidth: 500, closeButton: true, autoClose: false, closeOnEscapeKey: false, closeOnClick: false })
        .setLatLng([latitude, longitude])
        .setContent("<new-life-point latitude='" + latitude + "' longitude='" + longitude + "' id='" + ++newLifePointIdCounter + "'></new-life-point>")
        .on("remove", function () {
            _dotNetObjectReference.invokeMethodAsync("ReloadAndDrawAllLocationsAsync");
        })
        .openOn(myMap);
    newLifePointsMap.set(newLifePointIdCounter, popup);
}