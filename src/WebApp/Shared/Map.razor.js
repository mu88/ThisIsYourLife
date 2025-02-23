/* global L */

let _dotNetMapReference;
let _leafletMap;

export function initializeMap(startLongitude, startLatitude, startZoom, dotNetMapReference) {
  _dotNetMapReference = dotNetMapReference;
  _leafletMap = L.map("mapid").setView([startLongitude, startLatitude], startZoom);
  L.tileLayer("https://a.tile.openstreetmap.org/{z}/{x}/{y}.png", {
    attribution: "&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors"
  }).addTo(_leafletMap);

  _createFilterLifePointsCommand().addTo(_leafletMap);

  _leafletMap.on("dblclick", _onMapDoubleClick);

  return _leafletMap;
}

function _onMapDoubleClick(e) {
  let latitude = e.latlng.lat;
  let longitude = e.latlng.lng;
  _dotNetMapReference.invokeMethodAsync("OpenPopupForNewLifePointAsync", latitude, longitude);
}

function _createFilterLifePointsCommand() {
  let command = L.control({position: "topright"});
  command.onAdd = function () {
    let div = L.DomUtil.create("div", "command");
    div.innerHTML = "<filter-life-points/>";
    L.DomUtil.addClass(div, "filter-div");
    return div;
  };

  return command;
}
