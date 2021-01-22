// Use Proj4Js to convert the EPSG:277000 / Ordnance Survey format coordinates given in the file into the common WGS84 format
// Define EPSG:27700 format using definition string from epsg.io
proj4.defs("EPSG:27700","+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 +x_0=400000 +y_0=-100000 +ellps=airy +towgs84=446.448,-125.157,542.06,0.15,0.247,0.842,-20.489 +units=m +no_defs");

function convertCoordinatesArray(coords){
    // Convert array of coordinates into WGS84
    var converted = [];
    for (let i = 0; i < coords.length; i++){
        let currentCoords = coords[i];
        converted.push(proj4("EPSG:27700", "WGS84", currentCoords));
    }
    return converted;
}