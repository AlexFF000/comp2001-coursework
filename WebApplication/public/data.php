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
    <h1 class="text-center" role="heading">Right of Way Routes</h1>
    <div id="map_area" class="col h-100 text-center map_area" role="main">
        <p>The lines on the map represent right of way routes around Plymouth, as specified in <a href="https://plymouth.thedata.place/dataset/public-rights-of-way">the dataset</a></p>
        <a href="#" onclick="openColourPicker();">Change line colour</a>
        <div class="row">
            <div id="map" class="col h-100 map"></div>
        </div>
    </div>
</div>

<!-- Colour picker dialog-->
<div class="page_cover" id="page_cover">
    <div class="colour_dialog center-text" role="dialog">
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
<script src="../assets/js/data.js"></script>
</body>
</html>
