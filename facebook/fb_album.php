<?
	$fbpiclimit = isset($fb_timeline_limit) ? $fb_timeline_limit : "2";
	$fbID = isset($fb_timeline_id) ? $fb_timeline_id : $record['facebook_album_id'];
	$fb_cache_file = isset($fb_cFile) ? $fb_cFile : $record['fb_album_cache_file'];
	// Check fb_gallery_{tour abrv}.txt for cached gallery results so we're not hammering the FB open graph
	$fbGallery_og_fileCache = file_get_contents($fb_cache_file);
	// Set assoc parameter in json_decode to true to return associative array
	$fbjson = json_decode($fbGallery_og_fileCache, true);
	// Grab current time to compare to last updated time
	$timestampNow = time();
	$timeElapsedSinceLastPull = $timestampNow - $fbjson["lastUpdated"];
	// If time elapsed since last pull is more than 15 minutes pull again
	//http://graph.facebook.com/10151685150767718?fields=photos.limit(10).fields(source,picture),name
	if($timeElapsedSinceLastPull >= 900)
	{
		$curlURL = $fbpiclimit > 2 ? "http://graph.facebook.com/".$fbID."/albums?limit=".$fbpiclimit."&fields=id,name,link,photos.fields(id,picture,source)" : "http://graph.facebook.com/".$fbID."?fields=id,name,link,photos.fields(id,picture,source)";
		$ch = curl_init();  
		curl_setopt($ch, CURLOPT_URL, $curlURL); 
		//return the transfer as a string 
		curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1); 
		$output = curl_exec($ch);
		curl_close($ch);
		$outputSub7 = substr($output, 7);
		$currentTimestamp = time();
		$outputToWrite = "";
		if($fbpiclimit > 2) {
			$outputToWrite = "{\"lastUpdated\":\"".$currentTimestamp."\",\"data\"" . $outputSub7;
		} else {
			$outputToWrite = "{\"lastUpdated\":\"".$currentTimestamp."\",\"id\":\"" . $outputSub7;
		}
		$galleryTxtFile = $fb_cache_file;
		$handle = fopen($galleryTxtFile, "w+");
		fwrite($handle, $outputToWrite);
		fclose($handle);
		$fbjson = json_decode($outputToWrite, true);
		//showMe($fbjson);
	}
	// Shift first element off array since we do not need it
	if($fbpiclimit > 2) {
		$fbGallery = $fbjson["data"];
		array_shift($fbGallery);
	} else {
		$fbGallery = $fbjson;
	}
	
	$i = 1;
	if($fbpiclimit > 2) {
		foreach ($fbGallery as $album) {
			$photos = array_reverse($album["photos"]["data"]);
			if(!isset($fb_timeline_limit)) {
				echo '<div style="width: 200px;margin: 0 auto;">' . "\n";
				echo '<a href="#" id="fbGal' . $i . '"><img src="' . $photos[0]["source"] . '" width="200" style="border: 5px solid #fff;" /></a>&nbsp;&nbsp;' . "\n";
				echo '</div>';
			} else {
				echo '<a href="#" id="fbGal' . $i . '"><img src="' . $photos[0]["source"] . '" width="100" style="border: 5px solid #fff;" /></a>&nbsp;&nbsp;' . "\n";
			}
			$i++;
		}
	} else {
		$photos = array_reverse($fbGallery["photos"]["data"]);
		if(!isset($fb_timeline_limit)) {
				echo '<div style="width: 200px;margin: 0 auto;">' . "\n";
				echo '<a href="#" id="fbGal' . $i . '"><img src="' . $photos[0]["source"] . '" width="200" style="border: 5px solid #fff;" /></a>&nbsp;&nbsp;' . "\n";
				echo '</div>';
			} else {
				echo '<a href="#" id="fbGal' . $i . '"><img src="' . $photos[0]["source"] . '" width="100" style="border: 5px solid #fff;" /></a>&nbsp;&nbsp;' . "\n";
			}
	}
	echo '<p>&nbsp;</p>';
	?>
<script src="magnific-popup/jquery.magnific-popup.js"></script> 
<script>
	$(document).ready(function() {
	  <?
	  		$i = 1;
			if($fbpiclimit > 2) {
				foreach ($fbGallery as $album) {
					$magnific_popupJS = '$("#fbGal' . $i . '").magnificPopup({' . "\n";
					$magnific_popupJS .= "items: [\n";
					$photos = array_reverse($album["photos"]["data"]);
					foreach($photos as $picData) {
						$magnific_popupJS .= "{ src:'" . $picData["source"] . "'},\n";	
					}
					$magnific_popupJSTrim = substr($magnific_popupJS, 0, -1);
					$magnific_popupJSTrim2 = substr($magnific_popupJSTrim, 0 , -1);
					$magnific_popupJSTrim2 .=  "\n],\n";
					$magnific_popupJSTrim2 .= "gallery: { enabled: true },\n";
					$magnific_popupJSTrim2 .= "type: 'image'\n";
					$magnific_popupJSTrim2 .= "});\n\n";
					echo $magnific_popupJSTrim2;
					$i++;
				}
			} else {
				$albumPhotos = $fbGallery["photos"]["data"];
				$magnific_popupJS = '$("#fbGal' . $i . '").magnificPopup({' . "\n";
				$magnific_popupJS .= "items: [\n";
				$photos = array_reverse($albumPhotos);
				foreach($photos as $picData) {
					$magnific_popupJS .= "{ src:'" . $picData["source"] . "'},\n";	
				}
				$magnific_popupJSTrim = substr($magnific_popupJS, 0, -1);
				$magnific_popupJSTrim2 = substr($magnific_popupJSTrim, 0 , -1);
				$magnific_popupJSTrim2 .=  "\n],\n";
				$magnific_popupJSTrim2 .= "gallery: { enabled: true },\n";
				$magnific_popupJSTrim2 .= "type: 'image'\n";
				$magnific_popupJSTrim2 .= "});\n\n";
				echo $magnific_popupJSTrim2;
			}
	  ?>
	});
</script>