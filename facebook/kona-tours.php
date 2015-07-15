<?php header('Content-type: text/html; charset=utf-8'); ?>
<?php
  /* STEP 1: LOAD RECORDS - Copy this PHP code block near the TOP of your page */
  require_once "/lib/viewer_functions.php";
  // include fbGalleryClass.php
  require_once "/includes/fbGalleryClass.php";
  
  list($kona_tour_descriptionsRecords, $kona_tour_descriptionsMetaData) = getRecords(array(
    'tableName'   => 'kona_tour_descriptions',
  ));
  
  $sizeOfKonaTourDescriptions = count($kona_tour_descriptionsRecords);
  
  list($kona_landing_pageRecords, $kona_landing_pageMetaData) = getRecords(array(
    'tableName'   => 'kona_landing_page',
    'where'       => whereRecordNumberInUrl(1),
    'limit'       => '1',
  ));
  
  $kona_landing_pageRecord = @$kona_landing_pageRecords[0]; // get first record

  // show error message if no matching record is found
  if (!$kona_landing_pageRecord) {
    header("HTTP/1.0 404 Not Found");
    print "Record not found!";
    exit;
  }
  	// Start Breadcrumb code
	$sqlQuery = str_replace(':', ',', $kona_landing_pageRecord['breadcrumb_identity']);// replace ":" with ","
	$removeEnd = substr_replace($sqlQuery ,"",-1); // remove "," from end of string
	$SQL = substr($removeEnd, 1); // remove "," from begining of string
    list($breadcrumbsRecords, $breadcrumbsMetaData) = getRecords(array(
    'tableName'   => 'breadcrumbs',
   	'where' => 'num IN(' . $SQL . ')',
  ));
  
  //declare h1 variable
  $headLineText = $kona_landing_pageRecord['headline_text'];

  include("/includes/viewerCode.php");
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
<title><?php echo $kona_landing_pageRecord['title'] ?></title>
<meta name="description" content="<?php echo $kona_landing_pageRecord['seo_meta_description'] ?>" />
<meta name="keywords" content="<?php echo $kona_landing_pageRecord['seo_meta_keywords'] ?>" />
<link rel="shortcut icon" href="favicon.ico" type="image/x-icon" />
<link href="css/main.css" rel="stylesheet" type="text/css" media="screen" />
<link href="css/landing.css" rel="stylesheet" type="text/css" media="screen" />
<!-- Magnific Popup core CSS file -->
<link rel="stylesheet" href="magnific-popup/magnific-popup.css"> 
<style type="text/css" media="screen">
<!--
@import url("css/main.css");
@import url("css/landing.css");
-->
</style>
<link rel="stylesheet" href="css/colorbox.css" />
<script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.1/jquery.min.js"></script>
</head>
<body>
<!-- Start main wrapper -->
         <div class="wrapper">
           <?php  include("/includes/header.php"); //Header code ?>
           <!-- Start center content div -->
           <div id="centerContentSecondary">
              <?php  include("/includes/breadcrumb.php"); //Breadcrumb code ?>
            <!-- Start secondary center div -->
          <div class="secondaryCenter">
             <!-- IE7-/Win: A box with position:absolute next to a float may disappear -->
                     <div style="height:1px;">&nbsp;</div>
              <!-- Top corners tour description -->
              <div id="tourTopCorners">
              <!-- /Top coner tour description -->
              </div>
              <div style="width: 904px; background-color: #f1f1f1;"></div>
              <!-- Tour description main -->
            <div id="tourMainDesc">
           <!-- Start Mast Image -->
           <div id="hiloMastImg2">
           	<img src="/images/tours/landingPageKonaMast.jpg" alt="Hilo Tours" width="861" height="115" />
            <!-- / Mast image -->
            </div>
            <!-- Tour Descriptions -->
           <div id="tourDescriptionsNew">
           <?php $groupNumber = 1; ?>
           <?php $fbGalleryObjs = array(); ?>
           <?php foreach ($kona_tour_descriptionsRecords as $record): ?>
               <div class="leftImg">
               <?php foreach ($record['tour_logo'] as $upload): ?>
            		<a href="<?php echo createPageLink($record['slug'] ); ?>"><img src="<?php echo $upload['urlPath'] ?>" width="<?php echo $upload['width'] ?>" height="<?php echo $upload['height'] ?>" alt="" /></a><br/>
        		<?php endforeach; ?>
                <?php if(isset($record['book_it_now_button']) && $record['book_it_now_button'] != ""): ?>
                    <div class="reserveBtnSec" style="padding-left:16px;">
                    	<a href="https://www.kapohokine.com/reservationFareHarbor.php?<?php echo $record['book_it_now_button']; ?>" name="reserveNow" id="reserve"><img src="/images/reserveNow.jpg" width="181" height="39" alt="Book It Now!" title="Book It Now!" /></a>
                    </div>
                <?php else: ?>
                <div class="reserveBtnSec" style="padding-left:16px;">
<form id="reserve" name="reserveNow" method="post" action="http://www.kapohokine.com/reservation.php">
<input type="image" title="Book It Now!" alt="Book It Now!" value="Reserve Now" src="/images/reserveNow.jpg">
</form>
</div>
				<? endif; ?>
      			<!-- STEP2a: /Display Uploads -->
                
               </div>
               <div class="rightDesc">
                    <h1><a href="<?php echo createPageLink($record['slug'] ); ?>"><?php echo $record['headline_text'] ?></h1></a>
					<?php echo maxWords($record['description'], 30) . '...' ?>
                    <br /><a href="<?php echo createPageLink($record['slug'] ); ?>">More Info</a><br /><br />
                     <? if(isset($record['facebook_album_id']) && $record['facebook_album_id'] != ""):?>
					<?
						$fbConfig = array(
								"fbID" => @$record["facebook_album_id"],
								"fb_cache_file" => @$record["fb_album_cache_file"]
							);
						$fbGalleryObjs[$groupNumber] = new fbGallery($fbConfig); 
						$fbGalleryObjs[$groupNumber]->getPhotos($groupNumber);
					?>
                    <?php else: ?>
                     <?php foreach ($record['gallery'] as $upload): ?>
                 	<a class="group<?php echo $groupNumber; ?>" href="<?php echo $upload['urlPath'] ?>" title="<?php echo $upload['info1']; ?>"><img src="<?php echo $upload['thumbUrlPath'] ?>" width="75" alt="" style="border:5px solid #ededed;" /></a>
                 <?php endforeach; ?>
                 <? endif; ?>
               </div>
               <div style="clear:both">&nbsp;</div>
               <?php $groupNumber++; ?>
           <?php endforeach; ?>
            <div style="clear:both">&nbsp;</div>
           <!-- /Tour Descriptions -->
           </div>
              <!--/ Tour decription main -->
            </div>
            <!-- Tour description bottom corners -->
              <div id="tourBottomCorners">
              <!-- /Tour description bottom corners -->
              </div>
              <!-- clearing div to fix ie7 float bug -->
              <div style="clear:both;">
              <!-- /clearing div -->
            </div>
              <!-- /Center Secondary content div -->
            </div>
           </div>
  <?php include("/includes/footerSecondary.php"); // Banner Ads, Footer Links, Social Media Links ?>
      <!-- Start clearing div to keep footer sticky -->
            <div class="push">
            <!-- End clearing div -->
           </div>
         <!-- /Main wrapper -->
         </div>
         
         <div class="footer">&nbsp;</div>
         <?php include("/includes/analytics.html"); // Google Analytics code ?>
         <script src="magnific-popup/jquery.magnific-popup.js"></script> 
</body>
</html>