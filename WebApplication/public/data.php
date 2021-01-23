<?php
// Store script name for use in navigation.php, as __FILE__ gives the included file not the parent one
$currentScript = "data";
include("navigation.php");
?>
<!doctype html>
<html lang="en">
<head>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/css/bootstrap.min.css" integrity="sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh" crossorigin="anonymous">
    <script src="https://code.jquery.com/jquery-3.4.1.slim.min.js" integrity="sha384-J6qa4849blE2+poT4WnyKhv5vZF5SrPo0iEjwBvKU7imGFAV0wwj1yYfoRSJoZ+n" crossorigin="anonymous"></script>
    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js" integrity="sha384-Q6E9RHvbIyZFJoft+2mJbHaEWldlvI9IOYy5n3zV9zzTtmI3UksdQRVvoxMfooAo" crossorigin="anonymous"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/js/bootstrap.min.js" integrity="sha384-wfSDF2E50Y2D1uUdj0O3uMBJnjuUD4Ih7YwaYd1iqfktj0Uod8GCExl3Og8ifwB6" crossorigin="anonymous"></script>
    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.7.1/dist/leaflet.css"
          integrity="sha512-xodZBNTC5n17Xt2atTPuE1HxjVMSvLVW9ocqUKLsCC5CXdbqCmblAshOMAS6/keqq/sMZMZ19scR4PsZChSR7A=="
          crossorigin=""/>
    <script src="https://unpkg.com/leaflet@1.7.1/dist/leaflet.js"
            integrity="sha512-XQoYMqMTK8LvdxXYG3nZ448hOEQiglfqkJs1NOQV44cWnUrBc8PkAOcXy20w0vlaXaVUearIOBhiXZ5V3ynxwA=="
            crossorigin=""></script>
    <link rel="stylesheet" href="../assets/css/data.css" type="text/css"/>

    <title>Right of Way Routes</title>
</head>
<body>
<!-- Main Content -->
<div class="container">
    <h1 class="text-center">Right of Way Routes</h1>
    <div id="map_area" class="col h-100 text-center map_area">
        <p>The lines on the map represent right of way routes around Plymouth, as specified in <a href="https://plymouth.thedata.place/dataset/public-rights-of-way">the dataset</a></p>
        <a href="#" onclick="openColourPicker();">Change line colour</a>
        <div class="row">
            <div id="map" class="col h-100 map"></div>
        </div>
    </div>
</div>

<!-- Colour picker dialog-->
<div class="page_cover" id="page_cover">
    <div class="colour_dialog center-text">
        <div class="row center-text">
            <div class="col justify-self-center">
                <label for="colour_button" class="colour_button_label">Change line colour:</label>
            </div>
        </div>
        <div class="row align-items-center">
            <div class="col">
                <input id="colour_button" type="color" class="colour_button"/>
            </div>
        </div>
        <div class = "row">
            <div class="col">
                <button type="button" class="colour_dialog_cancel" onclick="closeColourPicker();">Cancel</button>
            </div>
            <div class="col">
                <button type="button" class="colour_dialog_ok" onclick="chooseColour();">Ok</button>
            </div>
        </div>
    </div>
</div>

</div>
<script>
    var lineColour = "#ff0000";
    var lines = [];
    // Setup map
    var map = L.map("map").setView([50.376389, -4.143841], 13);
    L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
        attribution: "Map data &copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors, Imagery © <a href=\"https://www.mapbox.com/\">Mapbox</a>",
        tileSize: 512,
        id: "mapbox/streets-v11",
        accessToken: "pk.eyJ1IjoiYWxyZWQiLCJhIjoiY2sydW1sM2EwMTY4bzNkbnZocmU2a3o3MiJ9.W5YwrLtEaSwtftKQravgTw",
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
</script>
</body>
</html>
