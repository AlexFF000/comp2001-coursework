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
<div class="container">
    <h1 class="text-center">Right of Way Routes</h1>
    <div id="map_area" class="col h-100 text-center map_area">
        <p>The lines on the map represent right of way routes around Plymouth, as specified in <a href="https://plymouth.thedata.place/dataset/public-rights-of-way">the dataset</a></p>
        <div class="row">
            <div id="map" class="col h-100 map"></div>
        </div>
    </div>
</div>

<script>
    // Setup map
    var map = L.map("map").setView([50.376389, -4.143841], 13);
    L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
        attribution: "mapbox",
        tileSize: 512,
        id: "mapbox/streets-v11",
        accessToken: "pk.eyJ1IjoiYWxyZWQiLCJhIjoiY2sydW1sM2EwMTY4bzNkbnZocmU2a3o3MiJ9.W5YwrLtEaSwtftKQravgTw",
        maxZoom: 14,
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
                L.polyline(coords, {color: "red"}).addTo(map);
            }
        }
    }
    http.open("GET", "../route", true);
    http.send();
</script>
</body>
</html>
