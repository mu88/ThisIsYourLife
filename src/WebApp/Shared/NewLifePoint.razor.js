let _popup;
let _dotNetMapReference;

export function createPopupForNewLifePoint(dotNetMapReference, latitude, longitude) {
    _dotNetMapReference = dotNetMapReference;
    return _popup = L.popup({ minWidth: 500, closeButton: true, autoClose: false, closeOnEscapeKey: false, closeOnClick: false })
        .setLatLng([latitude, longitude])
        .setContent("<new-life-point latitude='" + latitude + "' longitude='" + longitude + "'></new-life-point>")
}

export function removePopupForNewLifePoint() {
    _popup.remove();
}

export function addMarkerForCreatedLifePoint(id, latitude, longitude) {
    _dotNetMapReference.invokeMethodAsync("AddMarkerAsync", id, latitude, longitude);
}