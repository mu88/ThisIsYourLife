/* global L */

let _markers = [];
let _markerClusterGroup;
let _leafletMap;

export function initialize(leafletMap) {
    _leafletMap = leafletMap;
    reset();
}

export function reset() {
    if (_markerClusterGroup) {
        _leafletMap.removeLayer(_markerClusterGroup);
    }

    _markers = [];
    _markerClusterGroup = L.markerClusterGroup();
    _leafletMap.addLayer(_markerClusterGroup);
}

export function createMarkerForExistingLifePoint(id, latitude, longitude) {
    let marker = L.marker([latitude, longitude]);
    marker._id = id;
    marker.bindPopup("<life-point-detail id='" + id + "'></life-point-detail>", {maxWidth: _calculateMaxWidth()});
    _markerClusterGroup.addLayer(marker);
    _markers.push(marker);
}

export function removeMarkerOfLifePoint(id) {
    // https://stackoverflow.com/questions/45931963/leaflet-remove-specific-marker
    let newMarkers = [];
    _markers.forEach(function (marker) {
        if (marker._id === id) {
            _markerClusterGroup.removeLayer(marker);
        } else {
            newMarkers.push(marker);
        }
    });
    _markers = newMarkers;
}

export function updatePopup(id) {
    _markers.forEach(function (marker) {
        if (marker._id === id) {
            let popup = marker.getPopup();
            if (!popup._map) {
                return;
            }

            // the following code is borrowed from the original Leaflet sources
            // /src/layer/DivOverlay.js (base class of popup), update().
            // Make sure that this._updateContent() doesn't get called, because otherwise there'll be an infinite loop
            // between updating the popup and recreating the Blazor component. 
            popup._updateLayout();
            popup._updatePosition();
            popup._adjustPan();
        }
    });
}

export function enableSpinner() {
    _leafletMap.spin(true);
}

export function disableSpinner() {
    _leafletMap.spin(false);
}

function _calculateMaxWidth() {
    return (window.devicePixelRatio > 1 ? 300 : 500);
}
