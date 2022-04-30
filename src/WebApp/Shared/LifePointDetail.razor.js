let _leafletMap;
let _markers = []

export function createMarkerForExistingLifePoint(leafletMap, id, latitude, longitude) {
    _leafletMap = leafletMap;
    let marker = L.marker([latitude, longitude]);
    marker._id = id;
    marker.bindPopup("<life-point-detail id='" + id + "'></life-point-detail>", {minWidth: 500});
    marker.addTo(leafletMap);
    _markers.push(marker);
}

export function removeMarkerOfLifePoint(id) {
    // https://stackoverflow.com/questions/45931963/leaflet-remove-specific-marker
    let new_markers = [];
    _markers.forEach(function (marker) {
        if (marker._id === id) _leafletMap.removeLayer(marker);
        else new_markers.push(marker);
    })
    _markers = new_markers;
}