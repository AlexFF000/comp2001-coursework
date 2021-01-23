// Javascript for public/data.php
const mapboxToken = "pk.eyJ1IjoiYWxyZWQiLCJhIjoiY2sydW1sM2EwMTY4bzNkbnZocmU2a3o3MiJ9.W5YwrLtEaSwtftKQravgTw";
var lineColour = "#ff0000";
var lines = [];
// Setup map
var map = L.map("map").setView([50.376389, -4.143841], 13);
L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
    attribution: "Map data &copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors, Imagery © <a href=\"https://www.mapbox.com/\">Mapbox</a>",
    tileSize: 512,
    id: "mapbox/streets-v11",
    accessToken: mapboxToken,
    maxZoom: 18,
    minZoom: 12,
    zoomOffset: -1
}).addTo(map);

// Get data from /route
let http = new XMLHttpRequest();
http.onreadystatechange = function (){
    if (this.readyState == 4 && this.status == 200){
        // Display routes as polylines on the map
        let routes = JSON.parse(this.responseText)["Route"];
        for (let i = 0; i < routes.length; i++){
            let coords = routes[i]["geo"]["line"];
            lines.push(L.polyline(coords, {color: lineColour}).addTo(map));
        }
    }
}
http.open("GET", "../route", true);
http.send();

function changeLineColour(newColour){
    lineColour = newColour;
    for (let i = 0; i < lines.length; i++){
        lines[i].setStyle({color: newColour});
    }
}

function openColourPicker(){
    // Display colour picker dialog
    document.getElementById("page_cover").style.display = "block";
    // Set current colour in colour picker to the current line colour
    document.getElementById("colour_button").value = lineColour;
}

function closeColourPicker(){
    // Hide colour picker dialog
    document.getElementById("page_cover").style.display = "none";
}

function chooseColour(){
    let colour = document.getElementById("colour_button").value;
    changeLineColour(colour);
    closeColourPicker();
}