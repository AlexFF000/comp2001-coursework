<?php
// Return the JSON-LD data to the user
require_once("../processDataset.php");
const DATAFILE = "../plymouth rights of way dataset.geojson";

header("Content-Type: application/json");
echo json_encode(getJSONLD(DATAFILE));
