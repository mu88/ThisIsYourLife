/* global L */

let _popup;
let _dotNetMapReference;

export function createPopupForNewLifePoint(dotNetMapReference, leafletMap, latitude, longitude) {
  _dotNetMapReference = dotNetMapReference;
  _popup = L.popup({minWidth: calculateTooltipWidth(), closeOnEscapeKey: false, closeOnClick: false})
    .setLatLng([latitude, longitude])
    .setContent("<new-life-point latitude='" + latitude + "' longitude='" + longitude + "'></new-life-point>");
  _popup.openOn(leafletMap);
}

export function removePopupForNewLifePoint() {
  _popup.remove();
}

export function addMarkerForCreatedLifePoint(id, latitude, longitude) {
  _dotNetMapReference.invokeMethodAsync("AddMarkerAsync", id, latitude, longitude);
}

export function updatePopup() {
  if (!_popup._map) {
    return;
  }

  // the following code is borrowed from the original Leaflet sources
  // /src/layer/DivOverlay.js (base class of popup), update().
  // Make sure that this._updateContent() doesn't get called, because otherwise there'll be an infinite loop
  // between updating the popup and recreating the Blazor component. 
  _popup._updateLayout();
  _popup._updatePosition();
  _popup._adjustPan();
}
