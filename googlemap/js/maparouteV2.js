$(document).ready(function(){
	// Initalize map
	initialize();
	// Handle enter button click event for start address input text field
	$("#gmap_start_address").keypress(function(event){
	    if(event.keyCode == 13){
	    	event.preventDefault();
	    	goStartAddr();
	    }
	});
	// Handle enter button click event for name your route
	$("#bwflRouteName").keypress(function(event){
	    if(event.keyCode == 13){
	    	event.preventDefault();
	    	var routeName = $("#bwflRouteName").val();
	    	if(routeName == "" || routeName == "Name of your route"){
	    		alert("Please enter a route name");
	    		$('#bwflRouteName').focus();
	    		return false;
	    	}
	    	// Check routeName length
	    	if(routeName.length > 22){
	    		alert('Route name can not be greater than 22 characters long');
	    		$('#bwflRouteName').focus();
	    		return false;
	    	}
	    	addHiddenInputs();
	    	$('#bwflSaveRouteForm').trigger('submit');
	    }
	    
	    // Handle delet Route Button
	});
	/**
	 * Handle onclick, onblur and onfocus event for gmap_start_address input text field
	 * Handle onclick, onblur and onfocus event for bwflRouteName input text field
	 */
	$('#gmap_start_address').click(function(){
		$chkVal = $(this).val();
		if($chkVal == 'Starting address, city, zip code'){
			$(this).val('');
		}
	});
	$('#gmap_start_address').focus(function(){
		$chkVal = $(this).val();
		if($chkVal == 'Starting address, city, zip code'){
			$(this).val('');
		}
	});
	$('#gmap_start_address').blur(function(){
		$chkVal = $(this).val();
		if($chkVal == "") {
			$(this).val('Starting address, city, zip code');
		}
	});
	$('#bwflRouteName').click(function(){
		$chkVal = $(this).val();
		if($chkVal == 'Name of your route'){
			$(this).val('');
		}
	});
	$('#bwflRouteName').focus(function(){
		$chkVal = $(this).val();
		if($chkVal == 'Name of your route'){
			$(this).val('');
		}
	});
	$('#bwflRouteName').blur(function(){
		$chkVal = $(this).val();
		if($chkVal == "") {
			$(this).val('Name of your route');
		}
	});
});
// Delete Route click handler
$('a[name="bwflDelRoute"]').click(function(e){
	if(confirm("Are you sure you wish to delete? This action is not reversible.")){
		return true;
	} else {
		e.preventDefault();
		return false;
	}
});
// Initiate variables
var tmpCoordinates = [];
var markersArray = [];
var arrowMarkersArray = [];
var totalDist = [];
// Configurations
var alertDist = false;
var onlyOneDelete = false;

/**
 * Handle initialization of google map
 * @params
 */
function initialize() {
	$('#gmapDeleteMkrBtn').attr('disabled','disabled');
	$('#gmap_start_address').val('Starting address, city, zip code');
	$('#bwflRouteName').val('Name of your route');
	var gmapZoom = 4;
	geocoder = new google.maps.Geocoder();
	// Set default myLatLng to be used if google.loader.ClientLocation fails
	var myLatlng = new google.maps.LatLng(40.8802950337396, -102.21679674999996);
	// If ClientLocation was filled in by the loader, use that info instead
	if (google.loader.ClientLocation) {
		gmapZoom = 13;
		myLatlng = new google.maps.LatLng( google.loader.ClientLocation.latitude, google.loader.ClientLocation.longitude);
	}
	var myOptions = {
		zoom: gmapZoom,
		center: myLatlng,
		mapTypeId: google.maps.MapTypeId.ROADMAP
	}
	map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
	var polyOptions = {
		strokeColor: '#5F5CFA',
		strokeOpacity: 0.5,
		strokeWeight: 3
	}
	poly = new google.maps.Polyline(polyOptions);
	poly.setMap(map);
	// Add a listener for the click event
	google.maps.event.addListener(map, 'click', addLatLng);
}

/**
 * Handle GO button click to process start address
 * @params
 */
function goStartAddr(){
	var startAddr = $('#gmap_start_address').val();
	if(startAddr != '' && startAddr != 'Starting address, city, zip code'){
		placeStartMkr();
	} else {
		alert('Please enter start address.');
		$('#gmap_start_address').focus();
	}
}
    
/**
 * Handle marker placement from human address start point.
 * Originally this function placed a starting marker
 * Starting marker code is commented out and preserved for
 * ease of use in the future.
 * @params
 */
function placeStartMkr(){
	// clear out any existing markers
	clearOverlays(markersArray);
	// Grab address
	var humanAddr = $('#gmap_start_address').val();
	geocoder.geocode( { 'address': humanAddr}, function(results, status) {
	if (status == google.maps.GeocoderStatus.OK) {
			map.setCenter(results[0].geometry.location);
			map.setZoom(16);
			// Add coordinates to coordinates array
			/*var location = results[0].geometry.location;
			tmpCoordinates.push(location);
			var path = poly.getPath();
			// Because path is an MVCArray, we can simply append a new coordinate
			// and it will automatically appear
			path.push(location);
			var marker = new google.maps.Marker({
				map: map,
				position: results[0].geometry.location,
				title: '#' + path.getLength(),
			});
			markersArray.push(marker);*/
		} else {
			alert("Geocode was not successful for the following reason: " + status);
		}
	});
}
    
/**
 * Handles click events on a map, and adds a new point to the Polyline.
 * Adds new latLng object to tmpCoordinates array
 * Removes oldest latLng object in tmpCoordinates array to store new value
 * Calculate distance between two latLng objects
 * @param {MouseEvent} mouseEvent
 */
function addLatLng(event) {
	$('#gmapDeleteMkrBtn').removeAttr('disabled');
	// Add coordinates to coordinates array
	tmpCoordinates.push(event.latLng);
	if(tmpCoordinates.length == 1){
		arrowMarkersArray.push(event.latLng);
	}
	
	if(tmpCoordinates.length > 1){
		setArrowHeadMarker(event.latLng);
	}
	
	var path = poly.getPath();
	// Because path is an MVCArray, we can simply append a new coordinate
	// and it will automatically appear
	path.push(event.latLng);
	// Add a new marker at the new plotted point on the polyline.
	var marker = new google.maps.Marker({
		position: event.latLng,
		title: '#' + path.getLength(),
		map: map
	});
	markersArray.push(marker);
	// Calculate distance between two points and store results
	if (tmpCoordinates.length > 1) {
		calculateMkrDist(tmpCoordinates);
	}
} 
/**
 * Handles loading of saved route, adds a new point to the Polyline.
 * Adds new latLng object to tmpCoordinates array
 * Removes oldest latLng object in tmpCoordinates array to store new value
 * Calculate distance between two latLng objects
 * @param {array} points
 */
function routeLatLng(points,name) {
	$('#gmapDeleteMkrBtn').removeAttr('disabled');
	var pointsLength = points.length;
	// create new latlngbounds object to handle auto scale zoom of points on map
	var latlngbounds = new google.maps.LatLngBounds();
	for(i=0;i<pointsLength;++i){
		var latLngNum = points[i].toString();
		var latLngArray = latLngNum.split(",");
		var latNum = latLngArray[0];
		var lngNum = latLngArray[1];
		var latLngPair = new google.maps.LatLng(latNum,lngNum);
		latlngbounds.extend(latLngPair);
		tmpCoordinates.push(latLngPair);
		if(tmpCoordinates.length == 1){
			arrowMarkersArray.push(latLngPair);
			map.setCenter(latLngPair);
			map.setZoom(15);
		}
		if(tmpCoordinates.length > 1){
			setArrowHeadMarker(latLngPair);
		}
		var path = poly.getPath();
		// Because path is an MVCArray, we can simply append a new coordinate
		// and it will automatically appear
		path.push(latLngPair);
		// Add a new marker at the new plotted point on the polyline.
		var marker = new google.maps.Marker({
			position: latLngPair,
			title: '#' + path.getLength(),
			map: map
		});
		markersArray.push(marker);
		// Calculate distance between two points and store results
		if (tmpCoordinates.length > 1) {
			calculateMkrDist(tmpCoordinates);
		}
	}
	map.fitBounds(latlngbounds);
	$('#bwflRouteName').val(name);
} 

/**
 * Handles calculating distance between two markers on the map
 * @param array latLngArray {google.maps.LatLng objects}
 */
function calculateMkrDist(latLngArray,removeLast){
	var totalAggregate = 0;
	if(removeLast != undefined){
		totalDist.pop();
		tmpCoordinates.pop();
		// Otherwise remove last value in totalDist
		for(var j = 0;j < totalDist.length; ++j){
			totalAggregate = totalAggregate + totalDist[j];
		}
		if (alertDist) alert(totalAggregate.toFixed(2) + " Miles");
		return false;
	}
	// Calculate distance in miles using latLng coordinates
	// Pass in optional radius parameter of 3963.19 (earth's radius) to return results in miles.
	if(latLngArray.length > 2){
		var pOne = latLngArray.length - 2;
		var pTwo = latLngArray.length - 1;
	} else {
		var pOne = 0;
		var pTwo = 1;
	}
	var d = google.maps.geometry.spherical.computeDistanceBetween(latLngArray[pOne], latLngArray[pTwo],3963.19);
	// Add distance to totalDistance storage variable
	// Add new distance to total distance array
	totalDist.push(d);
	for(var k = 0;k < totalDist.length; ++k){
		totalAggregate = totalAggregate + totalDist[k];
	}
	if (alertDist) alert(totalAggregate.toFixed(2) + " Miles");
	return false;
}

function startFieldChk(fieldName){
	switch(fieldName){
		case 'address':
			var chkVal = $('#gmap_start_address').val();
			if(chkVal == "Starting address, city, zip code"){
				$('#gmap_start_address').val('');
				return false;
			}
			if(chkVal == ""){
				$('#gmap_start_address').val('Starting address, city, zip code');
				return false;
			}
			
		break;
		case 'routeName':
			var chkVal = $('#bwflRouteName').val();
			if(chkVal == "Name of your route"){
				$('#bwflRouteName').val('');
				return false;
			}
			if(chkVal == ""){
				$('#bwflRouteName').val('Name of your route');
				return false;
			}
		break;
	}
	return false;
}

/**
* Handle Clear Markers
* @params array markers
*/
function clearOverlays(markers) {
	// Clear existing poly object from map
	poly.setMap(null);
	// Create a new poly object to clear out old values
	var polyOptions = {
		strokeColor: '#5F5CFA',
		strokeOpacity: 0.5,
		strokeWeight: 3
	}
	poly = new google.maps.Polyline(polyOptions);
	poly.setMap(map);
	if (markersArray) {
		for (var j = 0; j < markersArray.length; j++ ) {
	      markersArray[j].setMap(null);
	    }
	}
}

/**
* Handle delete last marker
* @params
*/
function deleteMarker(){
	// Passing in 1 to calculateMkrDist removes last value.
	calculateMkrDist(tmpCoordinates,1);
	var lastMkr = markersArray.length - 1;
	var lastArrowMkr = arrowMarkersArray.length - 1;
	var lastPath = poly.getPath();
	var lastPathPos = lastPath.length - 1;
	lastPath.removeAt(lastPathPos);
	if (onlyOneDelete) $('#gmapDeleteMkrBtn').attr('disabled','disabled');
	markersArray[lastMkr].setMap(null);
	if(arrowMarkersArray.length > 1){
		arrowMarkersArray[lastArrowMkr].setMap(null);
	}
	if(arrowMarkersArray.length == 1){
		$('#gmapDeleteMkrBtn').attr('disabled','disabled');
	}
	markersArray.pop();
	arrowMarkersArray.pop();
	return false;
}

/**
* Handle reset entire map
* @params
*/
function resetMap(){
	// Reset all variables and arrays
	tmpCoordinates = [];
	markersArray = [];
	totalDist = [];
	// Call initalize
	initialize();
}

/**
 * Arrowhead code based on code provided by Mike Williams
 * http://econym.org.uk/gmap/arrows.htm
 * Improved and transformed to v3 by http://www.wolfpil.de/v3/arrow-heads.html
 * Improved and transformed to v3 by Mike Aguilar
*/

/**
* Handle create the arrow head at the end of the polyline
* @params latLng points
*/
function arrowHead(points) {
	var from=points[points.length-2];
	var to=points[points.length-1];
 
	var heading = google.maps.geometry.spherical.computeHeading(to, from).toFixed();
	if(heading < 0){
		var num = new Number(360);
		var addNum = new Number(heading);
		heading = addNum + num;
	}
	
	// == obtain the bearing between the last two points
	/* var p1=points[points.length-2];
 	var p2=points[points.length-1];
 	var dir = bearing(p1,p2);*/
	
	// == round it to a multiple of 3 and cast out 120s
	var dir = Math.round(heading/3) * 3;
	while (dir >= 120) {dir -= 120;}
	// == use the corresponding triangle marker 
	var returnImage = "http://www.google.com/intl/en_ALL/mapfiles/dir_"+dir+".png";
	//alert(returnImage);
  	return returnImage;
}

/**
* Helper function to replace google map api v2 latRadians method that no longer exists
* @params LatLng from, LatLng to
*/
function latRadians(latitude)
{
	return (Math.PI * latitude.lat()) / 180;
}

/**
* Helper function to replace google map api v2 lngRadians method that no longer exists
* @params LatLng from, LatLng to
*/
function lngRadians (longitude)
{
  return (Math.PI * longitude.lng()) / 180;
}
//extend Number object with methods for converting degrees/radians

Number.prototype.toRad = function() {  // convert degrees to radians
  return this * Math.PI / 180;
}

Number.prototype.toDeg = function() {  // convert radians to degrees (signed)
  return this * 180 / Math.PI;
}

Number.prototype.toBrng = function() {  // convert radians to degrees (as bearing: 0...360)
  return (this.toDeg()+360) % 360;
}
/**
* Returns the bearing in degrees between two points.
* North = 0, East = 90, South = 180, West = 270.
* @params LatLng from, LatLng to
*/
var degreesPerRadian = 180.0 / Math.PI;
function bearing( from, to ) {
  // See T. Vincenty, Survey Review, 23, No 176, p 88-93,1975.
  // Convert to radians.
  var lat1 = latRadians(from);
  var lon1 = lngRadians(from);
  var lat2 = latRadians(to);
  var lon2 = lngRadians(to);

  // Compute the angle.
  var angle = - Math.atan2( Math.sin( lon1 - lon2 ) * Math.cos( lat2 ), Math.cos( lat1 ) * Math.sin( lat2 ) - Math.sin( lat1 ) * Math.cos( lat2 ) * Math.cos( lon1 - lon2 ) );
  if ( angle < 0.0 )
 angle  += Math.PI * 2.0;
 
  // And convert result to degrees.
  angle = angle * degreesPerRadian;
  angle = angle.toFixed(1);

  return angle;
}

/**
* Place arrowhead marker on map
* @params LatLng location
*/
function setArrowHeadMarker(location){
	var image = arrowHead(tmpCoordinates);
	var position = location;
	var arrowMarker = new google.maps.Marker({
	    position: position,
	    map: map,
	    icon: image
	});
	arrowMarkersArray.push(arrowMarker);
}

/**
 * Handle #btnSaveRoute btn click event
 */
$("#btnSaveRoute").click(function(e){
	e.preventDefault();
	var routeName = $("#bwflRouteName").val();
	if(routeName == "" || routeName == "Name of your route"){
		alert("Please enter a route name");
		$('#bwflRouteName').focus();
		return false;
	}
	// Check routeName length
	if(routeName.length > 22){
		alert('Route name can not be greater than 22 characters long');
		$('#bwflRouteName').focus();
		return false;
	}
	addHiddenInputs();
	$('#bwflSaveRouteForm').trigger('submit');
});
/**
 * Add hidden input to store coordinates and mileage
 */
function addHiddenInputs(){
	var totalDistance = 0;
	for(var i = 0;i < totalDist.length; ++i){
		totalDistance = totalDistance + totalDist[i];
	}
	var totalDistance = totalDistance.toFixed(2);
	$('<input>').attr({
			type: 'hidden',
			id: 'miles',
			name: 'bwflRouteMiles',
			value: totalDistance
	}).appendTo('#bwflSaveRouteForm');
	var length = tmpCoordinates.length;
	for(var i = 0; i < length; i++){
		$('<input>').attr({
			type: 'hidden',
			id: 'lat[]',
			name: 'lat[]',
			value: tmpCoordinates[i].lat()
		}).appendTo('#bwflSaveRouteForm');
		$('<input>').attr({
			type: 'hidden',
			id: 'lng[]',
			name: 'lng[]',
			value: tmpCoordinates[i].lng()
		}).appendTo('#bwflSaveRouteForm');
	}
};
