/* global L */

let _markers = [];
let _markerClusterGroup;

export function createMarkerForExistingLifePoint(leafletMap, id, latitude, longitude) {
    if (!_markerClusterGroup) {
        _markerClusterGroup = L.markerClusterGroup();
        leafletMap.addLayer(_markerClusterGroup);
    }

    let marker = L.marker([latitude, longitude]);
    marker._id = id;
    marker.bindPopup("<life-point-detail id='" + id + "'></life-point-detail>", {maxWidth: _calculateMaxWidth()});
    _markerClusterGroup.addLayer(marker);
    _markers.push(marker);
}

function _calculateMaxWidth() {
    return (window.devicePixelRatio > 1 ? 300 : 500);
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
            if (!popup._map)
                return;

            // the following code is borrowed from the original Leaflet sources
            // /src/layer/DivOverlay.js (base class of popup), update().
            // Make sure that this._updateContent() doesn't get called, because otherwise there'll be an infinite loop
            // between updating the popup and recreating the Blazor component. 
            popup._updateLayout();
            popup._updatePosition();
            popup._adjustPan();
        }
    })
}