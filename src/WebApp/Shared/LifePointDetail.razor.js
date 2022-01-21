let _marker;

export function createMarkerForExistingLifePoint(leafletMap, id, latitude, longitude) {
    _marker = L.marker([latitude, longitude]);
    _marker.bindPopup("<life-point-detail id='" + id + "'></life-point-detail>", { minWidth: 500 });
    _marker.addTo(leafletMap);
}

export function removeMarkerOfLifePoint() {
    _marker.remove();
}