let _markers = []
let _markerClusterGroup;

export function createMarkerForExistingLifePoint(leafletMap, id, latitude, longitude) {
    if (!_markerClusterGroup) {
        _markerClusterGroup = L.markerClusterGroup();
        leafletMap.addLayer(_markerClusterGroup);
    }

    let marker = L.marker([latitude, longitude]);
    marker._id = id;
    marker.bindPopup("<life-point-detail id='" + id + "'></life-point-detail>", {minWidth: 500});
    _markerClusterGroup.addLayer(marker);
    _markers.push(marker);
}

export function removeMarkerOfLifePoint(id) {
    // https://stackoverflow.com/questions/45931963/leaflet-remove-specific-marker
    let new_markers = [];
    _markers.forEach(function (marker) {
        if (marker._id === id) {
            _markerClusterGroup.removeLayer(marker);
        } else new_markers.push(marker);
    })
    _markers = new_markers;
}