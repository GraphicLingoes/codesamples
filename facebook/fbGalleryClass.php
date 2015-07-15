<?
class fbGallery {
	protected $config = array(
		"fbpiclimit" => 0,
		"thumbnail" => false
	);
	protected $basePath = "/";
	
	public function __construct($initConfig = array()) {
		if ( !empty($initConfig) ) {
			if ( is_array($initConfig) ) {
				$this->set($initConfig);
			}
		}
	}
	
	public function set($key, $value=NULL) {
		if (is_array($key)) {
			foreach ($key as $k=>$value) {
				$this->config[$k] = $value;	
			}
			return;
		}
		
		$this->config[$key] = $value;
	}
	
	public function get($key) {
		return $this->config[$key];
	}
	
	public function getPhotos($pid = "") {
		if($this->get("fbID") != "") {
			// First try to grab photos json from cache file
			$fbjson = json_decode($this->getCacheFile($this->get("fb_cache_file")), true);
			$timestampNow = time();
			$timeElapsedSinceLastPull = $timestampNow - $fbjson["lastUpdated"];
			// If cache is older than 15 minutes make cURL request to FB api and refresh content
			if ( $timeElapsedSinceLastPull >= 900 ) {
				$fbjson = $this->makeCurlReq();
			}
			// Since grabbing from timeline in case of multile galleries we do not need first item in array
			if($this->get("fbpiclimit") > 2) {
				$fbGallery = $fbjson["data"];
				array_shift($fbGallery);
			} else {
				$fbGallery = $fbjson;
			}
			// Set gallery to "photos" index of config
			$this->set("photos", $fbGallery);
			$this->renderPhotos($pid);
		}
	}
	
	public function getCacheFile($fileURL) {
		if($fileURL != "") {
			$fbCacheJson = file_get_contents($this->basePath . $fileURL);
			return $fbCacheJson;	
		}
		
		return false;
	}
	
	private function makeCurlReq() {
		// Set up local variables
		$fbID = $this->get("fbID");
		$fbpiclimit = $this->get("fbpiclimit");
		// Set up api url for cURL request
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
		$this->writeToFile($outputToWrite);
		$fbjson = json_decode($outputToWrite, true);
		return $fbjson;
	}
	
	private function writeToFile($content) {
		$handle = fopen($this->basePath . $this->get("fb_cache_file"), "w+");
		fwrite($handle, $content);
		fclose($handle);
	}
	
	public function renderPhotos($pid) {
		if($this->get("photos")) {
			$fbGallery = $this->get("photos");
			$fbpiclimit = $this->get("fbpiclimit");
			$i = 1;
			if($fbpiclimit > 2) {
				foreach ($fbGallery as $album) {
					$photos = array_reverse($album["photos"]["data"]);
					if(!isset($fb_timeline_limit)) {
						echo '<div style="width: 200px;margin: 0 auto;">' . "\n";
						echo '<a href="#" id="fbGal' . $i . '_' . $pid . '"><img src="' . $photos[0]["source"] . '" width="200" style="border: 5px solid #fff;" /></a>&nbsp;&nbsp;' . "\n";
						echo '</div>';
					} else {
						echo '<a href="#" id="fbGal' . $i . '_' . $pid . '"><img src="' . $photos[0]["source"] . '" width="100" style="border: 5px solid #fff;" /></a>&nbsp;&nbsp;' . "\n";
					}
					$i++;
				}
			} else {
				$photos = array_reverse($fbGallery["photos"]["data"]);
				if($this->get("thumbnail")) {
					echo '<a href="#" id="fbGal' . $i . '_' . $pid . '">' . $this->get("thumbnail") . '</a>&nbsp;&nbsp;' . "\n";
				} else {
					if(!isset($fb_timeline_limit)) {
							echo '<div style="width: 200px;margin: 0 auto;">' . "\n";
							echo '<a href="#" id="fbGal' . $i . '_' . $pid . '"><img src="' . $photos[0]["source"] . '" width="200" style="border: 5px solid #fff;" /></a>&nbsp;&nbsp;' . "\n";
							echo '</div>';
						} else {
							echo '<a href="#" id="fbGal' . $i . '_' . $pid . '"><img src="' . $photos[0]["source"] . '" width="100" style="border: 5px solid #fff;" /></a>&nbsp;&nbsp;' . "\n";
						}
					}
			}
		}
		$this->renderScripts($pid);
	}
	
	public function renderScripts($pid) {
		if($this->get("photos")) {
			$fbpiclimit = $this->get("fbpiclimit");
			$fbGallery = $this->get("photos");
			echo "<script>\n";
			echo "$(document).ready(function() {\n";
	  		$i = 1;
			if($fbpiclimit > 2) {
				foreach ($fbGallery as $album) {
					$magnific_popupJS = '$("#fbGal' . $i . '_' . $pid . '").magnificPopup({' . "\n";
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
				$magnific_popupJS = '$("#fbGal' . $i . '_' . $pid . '").magnificPopup({' . "\n";
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
			echo "});\n";
			echo "</script>\n";	
		}
	}
}
?>