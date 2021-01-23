<?php
// Return the JSON-LD data to the user
require_once("../processDataset.php");
const DATAFILE = "../assets/dataset/plymouth rights of way dataset.geojson";

$data = json_encode(getJSONLD(DATAFILE));
header("Content-Type: application/json; charset=utf8");
echo $data;
