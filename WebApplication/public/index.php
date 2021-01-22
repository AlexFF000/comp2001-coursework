<?php
// Store script name for use in navigation.php, as __FILE__ gives the included file not the parent one
$currentScript = "index";
include("navigation.php");
?>

<!doctype html>
<html lang="en">
<head>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/css/bootstrap.min.css" integrity="sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh" crossorigin="anonymous">
    <script src="https://code.jquery.com/jquery-3.4.1.slim.min.js" integrity="sha384-J6qa4849blE2+poT4WnyKhv5vZF5SrPo0iEjwBvKU7imGFAV0wwj1yYfoRSJoZ+n" crossorigin="anonymous"></script>
    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js" integrity="sha384-Q6E9RHvbIyZFJoft+2mJbHaEWldlvI9IOYy5n3zV9zzTtmI3UksdQRVvoxMfooAo" crossorigin="anonymous"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/js/bootstrap.min.js" integrity="sha384-wfSDF2E50Y2D1uUdj0O3uMBJnjuUD4Ih7YwaYd1iqfktj0Uod8GCExl3Og8ifwB6" crossorigin="anonymous"></script>
    <link rel="stylesheet" href="../assets/css/home.css" type="text/css"/>
    <title>Right of Way Routes</title>
</head>
<div>
<div class="container h-100 text-center">
    <h1>Right of Way Routes</h1>
    <div class="align-items-center vertical_centre">
    <div class="row">
        <div class="col">
    <p>This right of way route locator is a linked data web application for ramblers which presents right of way routes around Plymouth in both <a href="data.php">human</a> and <a href="../route">machine</a> readable formats</p>
        </div>
    </div>
    <div class="row">
        <div class="col">
        <p>This application uses the "Public Rights of Way- Plymouth 2016" dataset provided by Plymouth City Council on Plymouth Data Place:</p>
        <a href="https://plymouth.thedata.place/dataset/public-rights-of-way">https://plymouth.thedata.place/dataset/public-rights-of-way</a>
        </div>
    </div>
    <br>
    <div class="row">
    <div class="col">
        <p>The data is available in machine readable RDF form at: <a href="../route">/route</a></p>
    </div>
    </div>
    <div class="row">
         <div class="col"
        <p>To view the data in human readable form:</p>
    </div>
    </div>
    <div class="row">
        <div class="col">
        <button type="button" class="btn btn-primary" onclick="window.location.href='data.php'">View Routes</button>
        </div>
    </div>
</div>
</div>
</body>
</html>
