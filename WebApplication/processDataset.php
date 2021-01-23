<?php
// Parses relevant data from geoJSON dataset file into a JSON-LD formatted PHP array

require_once("dependencies/proj4php-2.0.12/vendor/autoload.php");
use proj4php\Point;
use proj4php\Proj;
use proj4php\Proj4php;
const SCHEMA = "https://schema.org";

// Load projection objects as globals as they take some time to initialise
$proj = new Proj4php();
$osFormat = new Proj("EPSG:27700", $proj);
$wgsFormat = new Proj("EPSG:4326", $proj);


function getJSONLD($dataFilePath){
    // Function to be called from other PHP files to return the dataset as a JSON-LD PHP array
    // Override default error handler to allow warnings to be handled
    set_error_handler("handleExceptions");
    try {
        $data = toJSONLD(readData($dataFilePath));
        restore_error_handler();
        return $data;
    }
    catch (ErrorException $e){
        // If the data cannot be parsed, simply return an empty array
        restore_error_handler();
        return [];
    }
}

function readData($dataFilePath){
    // Read the data in from the file and parse into a PHP object
    $file = fopen($dataFilePath, "r");
    $rawData = fread($file, filesize($dataFilePath));
    return json_decode($rawData, true);
}

function toJSONLD($data){
    // Create a JSON-LD array from the dataset array
    $routes = [];
    foreach ($data["features"] as $datum){
        array_push($routes, toRoute($datum));
    }
    return ["@context"=>SCHEMA, "Place"=>$routes];
}

function toRoute($data){
    // Convert a datapoint in the dataset to a JSON-LD object representing a route
    return [
        "@type" => "Place",
        "geo" =>[
        "@type"=>"GeoShape",
        "line"=> convertCoordinatesArray($data["geometry"]["coordinates"])
        ]
    ];
}

function convertCoordinatesArray(array $coords){
    // Use Proj4PHP to convert coordinates from EPSG:27700 (Ordnance Survey) format to more common WGS84 format
    global $proj, $osFormat, $wgsFormat;
    $converted = [];
    foreach ($coords as $location){
        $osPoint = new Point($location[0], $location[1], $osFormat);
        $wgsPoint = $proj->transform($wgsFormat, $osPoint);
        $xyz = $wgsPoint->toArray();
        array_push($converted, [$xyz[1], $xyz[0]]);
    }
    return $converted;
}

function handleExceptions($severity, $message, $file, $line, array $context = []){  // Do not accept Throwable to maintain compatibility with PHP > 7, provide a default for $context because it is deprecated in PHP 7.2 so will not be passed
    // Upgrade warnings to exceptions so they can be properly caught and handled
    throw new ErrorException($message, 0, $severity, $file);
}