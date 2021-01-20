<?php
// Parses relevant data from geoJSON dataset file into a JSON-LD formatted PHP array
const SCHEMA = "https://schema.org";

function getJSONLD($dataFilePath){
    // Function to be called from other PHP files to return the dataset as a JSON-LD PHP array
    return toJSONLD(readData($dataFilePath));
}

function readData($dataFilePath){
    // Read the data in from the file and parse into a PHP object
    $file = fopen($dataFilePath, "r");
    $rawData = fread($file, filesize(FILEPATH));
    return json_decode($rawData, true);
}

function toJSONLD($data){
    // Create a JSON-LD array from the dataset array
    $routes = [];
    foreach ($data["features"] as $datum){
        array_push($routes, toRoute($datum));
    }
    return ["@context"=>SCHEMA, "Route"=>$routes];
}

function toRoute($data){
    // Convert a datapoint in the dataset to a JSON-LD object representing a route
    return [
        "@type" => "Place",
        "geo" =>[
        "@type"=>"GeoShape",
        "line"=> $data["geometry"]["coordinates"]
        ]
    ];
}