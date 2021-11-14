let _marker;

export function createMarkerForExistingLifePoint(id, latitude, longitude) {
    _marker = L.marker([latitude, longitude]);
    _marker.bindPopup("<life-point-detail id='" + id + "'></life-point-detail>", { minWidth: 500 });
    return _marker;
}

export function removeMarkerOfLifePoint() {
    console.warn(_marker);
    _marker.remove();
}