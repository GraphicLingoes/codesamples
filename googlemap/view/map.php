<div class="map" role="main">
<h1 class="entry-title">&raquo; Map a Route</h1>
<?php echo Bwfl_Flash::html(); ?>
<p class="intro">This planning tool lets you map out your route and save it for future use. Once you’ve saved your route, and calculated your distance, go back to the <a href="/">Dashboard</a> page to log your miles.</p>
<div class="saved-routes">
  <!-- <h2>My Saved Routes:</h2> -->
<table caption="My Saved Routes:" summary="This is a summary of my tabular data." id="mySavedRoutes">
  <caption>My Saved Routes:</caption>
  <thead id="savedHead">
    <tr>
      <th id="itemcolumn" scope="col"><span>Route Name</span></th>
      <th id="col1" scope="col"><span>Miles</span></th>
      <th id="col2" scope="col"><span>Delete</span></th>
    </tr>
  </thead>
  <tfoot>
    <tr>
    <td colspan="3"></td>
    </tr>
  </tfoot>
  <tbody>
  <?php if(sizeof($user_routes)):?>
    <?php $i=1;?>
    <?php foreach($user_routes as $route):?>
    <?php $routeName = preg_replace("/-/"," ", $route->name);?>
      <tr id="myRouteRow<?php echo $route->id;?>">
        <th id="row<?php echo $i; ?>" class="bwflLoadRoute" scope="row"><a id="loadRoute" href="/map/edit_route/<?php echo $route->id; ?>"><?php echo stripslashes(strip_tags($routeName)); ?></a></th>
        <td headers="itemcolumn col1"><span><?php echo $route->miles; ?></span></td>
        <td headers="itemcolumn col2"><a id="bwflDelRoute" name="bwflDelRoute" href="/map/delete_route/<?php echo $route->id; ?>" title="<?php echo stripslashes(strip_tags($routeName)); ?>"></a></td>
      </tr>
      <?php $i++; ?>
      <?php endforeach; ?>
    <?php endif; ?>
  </tbody>
</table> 
<input type="hidden" value="<?php echo Bwfl_Helper_Bwfl::ajax_helper_url().'/ajax_helper/load_route'; ?>" id="loadRouteEndPoint" name="loadRouteEndPoint" />
<input type="hidden" value="<?php echo Bwfl_Helper_Bwfl::ajax_helper_url().'/ajax_helper/del_route'; ?>" id="delRouteEndPoint" name="delRouteEndPoint" />
</div>

<div class="route-box">
  	<div id="map_start_address">
  		<span>1. Find Starting Point<b></b></span>
  		<form action="" method="post" id="form_start_address">
  			<a href="#" id="gmap_start_btn" class="gmapBtnClass text-btn-small right" onclick="goStartAddr();" /><i id="svg_mag"></i></a>
        <div class="inputfield alt left">
          <input type="text" id="gmap_start_address" />
    			<!-- <input type="button" id="gmap_start_btn" value="" class="gmapBtnClass text-btn-small" onclick="goStartAddr();" /> -->
        </div>
        
  		</form>
  	</div>
  	<div class="add-markers"><span>2. Add Markers<b></b></span><p class="left">Click the map to plot your route.</p class="left"></div>
  	<div class="map-box"> 
      <div id="map_actions">
    		<input type="button" id="gmapDeleteMkrBtn" value="Delete Last Marker" class="gmapBtnClass" onclick="deleteMarker();" />
    		&nbsp;
    		<input type="button" id="gmapResetMapbtn" value="Clear Map" class="gmapBtnClass" onclick="resetMap();" />
    	</div>
     	<div id="map_canvas"></div>
    </div>
    <div class="save-route">
      <span>3. Save Your Route<b></b></span>
      <form action="/map/save_route/" method="post" id="bwflSaveRouteForm">
        <div class="inputfield alt left">
        	<input type="text" value="Name of your route" id="bwflRouteName" name="bwflRouteName" />
        </div>  
        	<input type="submit" value="Save" class="text-btn-small" id="btnSaveRoute" name="btnSaveRoute" />
        	<input type="hidden" value="<?php echo Bwfl_Helper_Bwfl::ajax_helper_url().'/ajax_helper/save_route';?>" id="saveEndPoint" name="saveEndPoint" />
   	</form>
    </div>
</div>
<p class="left privacy">Privacy statement: Your map locations are confidential and will not be shared.</p>

    <script type="text/javascript" src="/media/js/svg_icons.js"></script>

    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?&sensor=false&libraries=geometry"></script>
    <script type="text/javascript" src="http://www.google.com/jsapi"></script>
    <script type="text/javascript" src="/media/js/maparouteV2.js"></script>
    <?php if(isset($json_coordinates)): ?>
    	<script type="text/javascript">
    	$(document).ready(function(){
        	var loadRouteCords = <?php echo $json_coordinates; ?>;
        	var loadRouteName = "<?php echo stripslashes(strip_tags($route_name)); ?>";
			routeLatLng(loadRouteCords,loadRouteName);
		});
    	</script>



    <?php endif; ?>
    

