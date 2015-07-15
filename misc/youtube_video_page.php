<?
class Youtube_Video_Widget extends WP_Widget {
	function Youtube_Video_Widget() {
		$widget_ops = array('description'=>'A widget that displays YouTube videos on video page.');
		$this->WP_Widget("dwYouTube","YouTube Channel",$widget_ops);
	}
	
	function form($instance) {
		$primary_video = esc_attr($instance['primary_video']);
		$channel = esc_attr($instance['channel']);
		$total_vids = esc_attr($instance['total_vids']);
		?>
        	<p>
            <b>Directions:</b> Enter data below.<br />
            <b>Pirmary Video</b> - Video that plays first on video page. If left blank most recent video will play.<br />
            <b>youTube Channel</b> - youTube Channel videos are pulled from. This field must be filled in.<br />
            <b>Total Videos Per Page</b> - Number of videos that will display on one page. May be left blank.<br />
            </p>
            <p><label for="<?php echo $this->get_field_id('primary_video'); ?>">Primary Video:
            <input class="widefat"
            	   id="<?php echo $this->get_field_id('primary_video'); ?>"
                   name="<?php echo $this->get_field_name('primary_video'); ?>"
                   type="text"
                   value="<?php echo attribute_escape($primary_video); ?>" />
               </label>
            </p>
            <p><label for="<?php echo $this->get_field_id('channel'); ?>">youTube Channel:
            <input class="widefat"
            	   id="<?php echo $this->get_field_id('channel'); ?>"
                   name="<?php echo $this->get_field_name('channel'); ?>"
                   type="text"
                   value="<?php echo attribute_escape($channel); ?>" />
               </label>
            </p>
            <p>
            <label for="<?php echo $this->get_field_id('total_vids'); ?>">Total Videos Per Page:
            <input class="widethin"
            	   id="<?php echo $this->get_field_id('total_vids'); ?>"
                   name="<?php echo $this->get_field_name('total_vids'); ?>"
                   type="text"
                   value="<?php echo attribute_escape($total_vids); ?>" />
               </label>
            </p>
        <?
	}
	
	function update($new_instance, $old_instance) {
		$instance = $old_instance;
		$instance['primary_video'] = strip_tags($new_instance['primary_video']);
		$instance['channel'] = strip_tags($new_instance['channel']);
		$instance['total_vids'] = strip_tags($new_instance['total_vids']);
		return $instance;
	}
	
	function widget($args, $instance) {
		extract($args,EXTRA_SKIP);
		
		if(is_page("Videos")){
			echo $before_widget;
			$primary_video_check = apply_filters('widget_title',$instance['primary_video']);
			$channel = apply_filters('widget_channel',$instance['channel']);
			$primary_video = ($primary_video_check != "" ? $primary_video_check:$this->dw_get_primary_vide($channel));
			$num_of_videos = apply_filters('total_vids',$instance['total_vids']);
			$videoID = (isset($_REQUEST['v']) ? $_REQUEST['v']:$primary_video);
			
			// render embeded video object
			$added_playerURL_params = "&autoplay=1&fs=1&modestbranding=1&orgin=".urlencode('http://www.davidweck.com');
			echo '<div id="dw_video_container"><!-- dw Video Container -->'."\n";
			echo '<div class="dw_primary_video">'."\n";
			echo '<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/swfobject/2.2/swfobject.js"></script>'."\n";
			echo '<div id="ytapiplayer">You need Flash player 8+ and JavaScript enabled to view this video.</div>'."\n";
  			echo '<script type="text/javascript">'."\n";
   			echo 'var params = { allowScriptAccess: "always", allowFullScreen: "true" };'."\n";
    		echo 'var atts = { id: "myytplayer" };'."\n";
   			echo 'swfobject.embedSWF("http://www.youtube.com/e/'.$videoID.'?enablejsapi=1&playerapiid=ytplayer'.$added_playerURL_params.'","ytapiplayer", "640", "390", "8", null, null, params, atts);'."\n";
            echo '</script>'."\n";
			echo '</div>'."\n";
			$this->dw_get_YouTube_vids($channel,$num_of_videos);
			## http://gdata.youtube.com/feeds/api/users/bosufitness/uploads
			echo '</div><!-- /dw Video Container -->'."\n";
			echo $after_widget;
		}
	}
	
	function dw_get_primary_vide($api_link){
		$xmlObject = simplexml_load_file($api_link);
		$media = $xmlObject->entry[0]->children('http://search.yahoo.com/mrss/');
		// get video player URL
		$attrs = $media->group->player->attributes();
		$watch = $this->dw_parse_video_link($attrs['url']);
		$watch_pieces = explode('&',$watch);
		$watchID = substr($watch_pieces[0],2);
		return $watchID;
	}
	function dw_get_YouTube_vids($api_link,$num_of_videos){
		$xmlObject = simplexml_load_file($api_link);
		$video_count = count($xmlObject->entry);
		$final_total = ($video_count < $num_of_videos || $num_of_videos == "" ? $video_count - 1:$num_of_videos - 1);
		for($i=0;$i<=$final_total;$i++){
		  // get nodes in media: namespace for media information
		  $title = $xmlObject->entry[$i]->title;
		  $media = $xmlObject->entry[$i]->children('http://search.yahoo.com/mrss/');
		  
		  // get video player URL
		  $attrs = $media->group->player->attributes();
		  $watch = $this->dw_parse_video_link($attrs['url']);
		  $watch_pieces = explode('&',$watch);
		  $watchID = substr($watch_pieces[0],2);
		  
		  
		  // get video thumbnail
		  $attrs = $media->group->thumbnail[1]->attributes();
		  $thumbnail = $attrs['url']; 
				
		  // get <yt:duration> node for video length
		  $yt = $media->children('http://gdata.youtube.com/schemas/2007');
	      $attrs = $yt->duration->attributes();
	      $length = $attrs['seconds'];
		  
		  // get <yt:stats> node for viewer statistics
		  $yt = $xmlObject->entry[$i]->children('http://gdata.youtube.com/schemas/2007');
		  $attrs = $yt->statistics->attributes();
		  $viewCount = $this->dw_number_format($attrs['viewCount']); 
		  
		  // get <gd:rating> node for video ratings
		  $gd = $xmlObject->entry[$i]->children('http://schemas.google.com/g/2005'); 
		  if ($gd->rating) {
			$attrs = $gd->rating->attributes();
			$rating = number_format($attrs['average'],2,'.',','); 
		  } else {
			$rating = 0; 
		  }
		  //$size = getimagesize($thumbnail);
		  //$resize_array = $this->dw_resize_image($size[0],$size[1],150);
		  echo '<div class="dw_thumbnail_container">'."\n";
		  echo '<div class="dw_thumbnail"><a href="?v='.$watchID.'"><img src="'.$thumbnail.'" /></a></div>'."\n";
		  echo '<div class="dw_video_description"><a href="?v='.$watchID.'" class="dw_description_link">'.$title.'</a><br />Views: '.$viewCount.'<br /></div>';
		  echo '</div>'."\n";
		}
		
	}
	
	//Image resize - helper function not currently used.
	function dw_resize_image($width, $height, $target_width){
		if($width > $height){
			$percentage = $target_width/$width;
		} else {
			$percentage = $target_width/$height;
		}
		$sizes['width'] = round($percentage*$width);
		$sizes['height'] = round($percentage*$height);
		
		return $sizes;
	}
	
	function dw_number_format($num){
		$num=(string)$num;
 		$len=strlen($num);
  		$newnum='';
  		for ($i=0; $i<$len; ++$i) {
			if (($i%3==0) && $i) {
     		$newnum=','.$newnum;
    		}
    		$newnum=$num[$len-$i-1].$newnum;
  		}
  		return $newnum;
	}
	
	function dw_parse_video_link($link){
		$url_parts = parse_url($link);
		$query = $url_parts['query'];
		return $query;
		
	}
}

register_widget('Youtube_Video_Widget');

?>