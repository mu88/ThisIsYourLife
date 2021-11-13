// TODO mu88: Refactor class into better functions

let _dotNetObjectReference;
let myMap;
let newLifePointIdCounter = 0;
let newLifePointsMap = new Map();
let existingLifePointsMap = new Map();

export function initializeMap(startLongitude, startLatitude, startZoom, dotNetObjectReference) {
    _dotNetObjectReference = dotNetObjectReference;
    myMap = L.map("mapid").setView([startLongitude, startLatitude], startZoom);
    L.tileLayer("https://a.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution: '&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors'
    }).addTo(myMap);

    myMap.on('dblclick', onMapDoubleClick);
}

export function addExistingLifePointMarker(id, latitude, longitude) {
    let marker = L.marker([latitude, longitude]).addTo(myMap);
    marker.bindPopup("<life-point-detail id='" + id + "'></life-point-detail>", { minWidth: 500 });
    existingLifePointsMap.set(id, marker);
}

export function removeNewLifePointPopup(id) {
    let popupOfNewLifePoint = newLifePointsMap.get(id);
    if (popupOfNewLifePoint) {
        popupOfNewLifePoint.remove();
    }
    newLifePointsMap.delete(id);
}

export function removeExistingLifePointMarker(id) {
    let markerOfExistingLifePoint = existingLifePointsMap.get(id);
    if (markerOfExistingLifePoint) {
        markerOfExistingLifePoint.remove();
    }
    existingLifePointsMap.delete(id);
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