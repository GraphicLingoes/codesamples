<? 
// Gigya comments moderation functions 
// created 12/21/2011 by mike@platformic.com
/*	Copyright 2012 Mike Aguilar. - All rights reserved.
 *	Use of this source code without direct authorization is strictly
 *	prohibited.	
 */
#####################################################
## Abstract: When gigya_CM_get_all_comments()
## is called memcache is checked to see if the
## comments array has already been stored. If
## the comments array is not there comments will
## be pulled from the gigya API. This process may
## take between 5-10 seconds. Once comments are
## returned they are stored in an array and added
## to memcache for future use. Any data manipulation
## to the comments array is usually done in the
## gigya_CM_get_all_comments() function then the data
## array is passed to the gigya_CM_build_aggtable()
## function to prepare it for display.
######################################################
function gigya_CM_login($action=""){
	global $site;
	//Include gigya SDK
	//Notify login so admin can post to activity feed
	include_once('/export/home/global_includes/GSSDK.php');
	// Grab avatar image for site
	$fbpi_chkimg_array = array("fb_primary_img.jpeg","fb_primary_img.png","fb_primary_img.gif");
	foreach($fbpi_chkimg_array as $fbpi_filename){
		if (file_exists($_SERVER['DOCUMENT_ROOT']."/".$fbpi_filename)) {
			$fb_og_protocol = $_SERVER['HTTPS'] == 'on' ? 'https' : 'http';
			$fb_og_link = $fb_og_protocol.'://'.$_SERVER['HTTP_HOST']."/".$fbpi_filename;
			$json_thumbnail_piece = ',thumbnailURL:"'.$fb_og_link.'"';
		}
	}
	// Get Gender for user object
	switch($site->user->gender) {
		case 'Male':
			$gigya_gender = 'm';
		break;
		case 'Female':
			$gigya_gender = 'f';
		break;
		case'':
			$gigya_gender = 'm';
		break;
	}
	$gigya_login = new GSRequest($GLOBALS['gigya']['api_key'],$GLOBALS['gigya']['api_secret'],"socialize.notifyLogin");
	if(strlen($site->user->gigyaUID) > 0) {
		$gigyaUID = $site->user->gigyaUID;
		$gigya_login->setParam('siteUID',$gigyaUID);
	} else {
		$gigyaUID = $site->user->ugc_userID;
		$gigya_login->setParam('siteUID',$gigyaUID);
	}
	$gigya_login->setParam('sessionExpiration',3600);
	switch($site->networkID){
		case "162":
			$fname_alias = "Comcast";
			$lname_alias = "SportsNet";
			$nickname = "Comcast SportsNet";
		break;
		default:
			$fname_alias = $site->user->fname;
			$lname_alias = $site->user->lname;
			$nickname = $site->user->username;
		break;	
	}
	$gigya_login->setParam('userInfo','{firstName:"'.$fname_alias.'",lastName:"'.$lname_alias.'",gender:"'.$gigya_gender.'",nickname:"'.$nickname.'"'.$json_thumbnail_piece.'}');
	$gigya_response = $gigya_login->send();
	if($gigya_response->getErrorCode()==0) {
		$response_json = json_decode($gigya_response->getResponseText(),true);
		//mail("mike@platformic.com","login response",$response_json."\n".$gigyaUID."\n".$action);	
		setrawcookie($response_json['cookieName'],$response_json['cookieValue'],0,$response_json['cookiePath'],$response_json['cookieDomain']);	
	} else {
		$response_json = $gigya_response->getResponseText();
		mail("mike@platformic.com","login response",$response_json."\n".$gigyaUID."\n".$action);	
	}
}

function gigya_CM_logout($action=""){
	global $site;
	//Notify logout so there are no surprises
	include_once('/export/home/global_includes/GSSDK.php');
	$gigya_logout = new GSRequest($GLOBALS['gigya']['api_key'],$GLOBALS['gigya']['api_secret'],"socialize.logout");
	if(strlen($site->user->gigyaUID) > 0) {
		$gigya_logout->setParam('uid',$site->user->gigyaUID);
		$id = $site->user->gigyaUID;
	} else {
		$gigya_logout->setParam('uid',$site->user->ugc_userID);
		$id = $site->user->ugc_userID;
	}
	$gigya_response = $gigya_logout->send();
	//$response = $gigya_response->getResponseText();
	//mail("mike@platformic.com","logout response",$response."\n".$id."\n".$action);	
}

function gigya_CM_get_comments(){
	global $site;
	$debug_comments = false;
	$comment_streams = array(htmlspecialchars($_POST['streamID']));
	if($_POST['streamID'] == ""){
		ob_start();
		?>
           	<script type="text/javascript">
               	$("#gigya_comments_holder").html("SteamID missing please return to the contributor dasboard and try again.");
            </script>
		<?
		$site->include_jquery = 1;
		$site->post_output = ob_get_clean();
		return;
	}
	if(isset($GLOBALS['gigya']['CM_per_page'])){
		$per_page = isset($_POST['gigya_CM_per_page']) && $_POST['gigya_CM_per_page'] != $GLOBALS['gigya']['CM_per_page'] ? $_POST['gigya_CM_per_page']:$GLOBALS['gigya']['CM_per_page'];
	} else {
		$per_page = isset($_POST['gigya_CM_per_page']) && $_POST['gigya_CM_per_page'] != 5 ? $_POST['gigya_CM_per_page']:5;	
	}
	$page = isset($_POST['gigya_CM_page']) && $_POST['gigya_CM_page'] != 1 ? $_POST['gigya_CM_page']:1;
	$memkey = "gigya_CM".$comment_streams[0]."_".$site->siteID."_".$site->networkID."_".$site->site_sign;
	$mem_output = $site->memCache->get($memkey);
	if($mem_output != ""){
		$tmp_array = $mem_output;	
	} else {
		$start = NULL;
		$comment_data = array();
		gigya_CM_login("Get Comments");
		include_once('/export/home/global_includes/GSSDK.php');
		//$debug_comments = "";
		foreach($comment_streams as $streamID){
			//$start = isset($_POST['gigya_CM_start']) ? $_POST['gigya_CM_start']:NULL;
			$gigyaCommentsGSRequest = new GSRequest($GLOBALS['gigya']['api_key'],$GLOBALS['gigya']['api_secret'],"comments.getComments");
			$gigyaCommentsGSRequest->setParam("categoryID",$GLOBALS['gigya']['category']);
			$gigyaCommentsGSRequest->setParam("streamID",$streamID);
			$gigyaCommentsGSRequest->setParam("threadLimit",200);
			$gigyaCommentsGSRequest->setParam("threadDepth",5);
			$gigyaCommentsGSRequest->setParam("sort","dateDesc");
			$gigyaCommentsGSRequest->setParam("includeStreamInfo",true);
			$gigyaCommentsGSRequest->setParam("includeUID",true);
			if(!is_null($start)){
				$gigyaCommentsGSRequest->setParam("start",$start);	
			}
			$gigyaGetComments = $gigyaCommentsGSRequest->send();
			if($gigyaGetComments->getErrorCode()!=0) {
				$subject = $gigyaGetComments->getErrorCode()." - SiteID: ".$site->siteID." - Call: comments.getComments";
				$message = "Response Text: ".$gigyaGetComments->getResponseText();
				mail('mike@platformic.com',$subject,$message);
			}
			$raw_comments = json_decode($gigyaGetComments->getData());
			//$debug_comments .='<pre>'.print_r($raw_comments,1).'</pre>';
			// check to see if any comments exists
			if($raw_comments->commentCount != 0){
				$comments = $raw_comments->comments;
				$avatar_URLprotocol = $_SERVER['HTTPS'] == 'on' ? 'https' : 'http';
				// Grab headline from platformic DB
				$sql = "SELECT headline FROM text_block WHERE siteID = '".$site->siteID."' AND blockID = '".$streamID."'";
				$res = $site->_return_data($sql);
				foreach($res as $row){
					$headline = $row['headline'];
				}
				
				foreach($comments as $comment){
					// If comment has replies
					if($comment->descendantsCount > 0){
						if($comment->flagCount == 0){
							
							$inner_comment_data = array();
							$inner_comment_data['id'] = $comment->ID;
							$inner_comment_data['streamID'] = $streamID;
							$inner_comment_data['headline'] = $headline;
							$inner_comment_data['comment_text'] = $comment->commentText;
							$inner_comment_data['flag_count'] = $comment->flagCount;
							$inner_comment_data['photo_url'] = $comment->sender->photoURL != "" ? '<img src="'.$comment->sender->photoURL.'" width="45" height="45" />':'<img src="'.$avatar_URLprotocol.'://'.$_SERVER['HTTP_HOST'].'/common/global_images/icons/avatars/anonymous.jpg" width="45" height="45" />';
							$inner_comment_data['name'] = $comment->sender->name;
							$inner_comment_data['UID'] = $comment->sender->UID;
							$inner_comment_data['is_reply'] = 0;
							$timestamp = $comment->timestamp/1000;
							$comment_time = date("m/d/y g:ia T",$timestamp);
							$inner_comment_data['formatted_time'] = $comment_time;
							$comment_data[$comment->timestamp] = $inner_comment_data;
						}
						
						foreach($comment->replies as $tmp_data){
							$inner_comment_data = array();
							$inner_comment_data['id'] = $tmp_data->ID;
							$inner_comment_data['streamID'] = $streamID;
							$inner_comment_data['headline'] = $headline;
							$inner_comment_data['comment_text'] = $tmp_data->commentText;
							$inner_comment_data['flag_count'] = $tmp_data->flagCount;
							$inner_comment_data['photo_url'] = $tmp_data->sender->photoURL != "" ? '<img src="'.$tmp_data->sender->photoURL.'" width="45" height="45" />':'<img src="'.$avatar_URLprotocol.'://'.$_SERVER['HTTP_HOST'].'/common/global_images/icons/avatars/anonymous.jpg" width="45" height="45" />';
							$inner_comment_data['name'] = $tmp_data->sender->name;
							$inner_comment_data['UID'] = $tmp_data->sender->UID;
							$inner_comment_data['is_reply'] = 1;
							$timestamp = $tmp_data->timestamp/1000;
							$comment_time = date("m/d/y g:ia T",$timestamp);
							$inner_comment_data['formatted_time'] = $comment_time;
							$comment_data[$tmp_data->timestamp] = $inner_comment_data;
							$tack_on_replies = gigya_CM_get_replies($tmp_data,$streamID,$headline,$avatar_URLprotocol,1);
						}
						// Add replies to comment data array
						if(is_array($tack_on_replies) || is_object($tack_on_replies)){
							foreach($tack_on_replies as $key=>$val){
								$comment_data[$key] = $val;	
							}
						}
					} else {
						// Comment does not have replies
						if($comment->flagCount < 1){
							$inner_comment_data = array();
							$inner_comment_data['id'] = $comment->ID;
							$inner_comment_data['streamID'] = $streamID;
							$inner_comment_data['headline'] = $headline;
							$inner_comment_data['comment_text'] = $comment->commentText;
							$inner_comment_data['flag_count'] = $comment->flagCount;
							$inner_comment_data['photo_url'] = $comment->sender->photoURL != "" ? '<img src="'.$comment->sender->photoURL.'" width="45" height="45" />':'<img src="'.$avatar_URLprotocol.'://'.$_SERVER['HTTP_HOST'].'/common/global_images/icons/avatars/anonymous.jpg" width="45" height="45" />';
							$inner_comment_data['name'] = $comment->sender->name;
							$inner_comment_data['UID'] = $comment->sender->UID;
							$inner_comment_data['is_reply'] = 0;
							$timestamp = $comment->timestamp/1000;
							$comment_time = date("m/d/y g:ia T",$timestamp);
							$inner_comment_data['formatted_time'] = $comment_time;
							$comment_data[$comment->timestamp] = $inner_comment_data;
						}
					}
				}
			} // End foreach loop
		}
		gigya_CM_logout("Get Comments");
		// Sort data array in descending order
		// Add to memchace
		krsort($comment_data);
		$tmp_array = array();
		foreach($comment_data as $comment_array){
			$tmp_array[] = $comment_array;	
		}
		//die('<pre>'.print_r($comment_data,1).'</pre>');
		$site->memCache->add($memkey,$tmp_array,false,240);
	}
	###################################
	#### If function called from search
	###################################
	if($_POST['gigya_CM_is_search']){
		if($_POST['gigya_CM_search_type'] != "none"){
			$tmp_array = gigya_CM_search($tmp_array,$_POST['gigya_CM_search_phrase'],$_POST['gigya_CM_search_type']);
			$page = count($tmp_array) > $per_page ? $page:1;
		}
	}
	// END search manipulation
	$comment_output = gigya_CM_build_aggtable($tmp_array,$per_page,$page,$debug_comments);
	//$comment_output = "<pre>".print_r($tmp_array,1)."</pre>";
	$comment_output = stripslashes(stripslashes($comment_output));
	$comment_output = preg_replace("/\n/","\\n",$comment_output);
	$comment_output = preg_replace("/\r/","\\n",$comment_output);
	$comment_output = preg_replace("/'/","\'",$comment_output);
	// JS
	ob_start();
	?>
	<script type="text/javascript">
		$("#gigya_comments_holder").html('<?php echo $comment_output; ?>');
		// Handle pagination
		$("#gigya_CM_nextBtn").click(function(e){
			e.preventDefault();
			// Check if search params exist
			var gCMsearchParams = gigyaCMSearchPresent();
			// Set perPage variable before injecting new html
			var perPage = $("#gigya_CM_aggperpage").val();
			var goToPage = $("#gigya_CM_nextBtn").attr('href');
			$("#gigya_comments_holder").html('<?php echo '<div align="center"><img src="'.ICON_URL.'/loading.gif" /><br />Loading</div>'; ?>');
			var streamID = "<?php echo $comment_streams; ?>";
			qpost("module:gigya_comments~function:gigya_CM_get_comments~streamID:"+streamID+"~gigya_CM_per_page:"+perPage+"~gigya_CM_page:"+goToPage+gCMsearchParams);
		});
		$("#gigya_CM_prevBtn").click(function(e){
			e.preventDefault();
			// check if search params exist
			var gCMsearchParams = gigyaCMSearchPresent();
			// Set perPage variable before injecting new html
			var perPage = $("#gigya_CM_aggperpage").val();
			var goToPage = $("#gigya_CM_prevBtn").attr('href');
			$("#gigya_comments_holder").html('<?php echo '<div align="center"><img src="'.ICON_URL.'/loading.gif" /><br />Loading</div>'; ?>');
			var streamID = "<?php echo $comment_streams; ?>";
			qpost("module:gigya_comments~function:gigya_CM_get_comments~streamID:"+streamID+"~gigya_CM_per_page:"+perPage+"~gigya_CM_page:"+goToPage+gCMsearchParams);
		});
		$("#gigya_CM_reload_link").click(function(e){
			e.preventDefault();
			qpost("module:gigya_comments~function:gigya_CM_reload_comments");
		});
	</script>
	<?
	$site->include_jquery = 1;
	$js_output = ob_get_clean();
	$site->post_output = $js_output;
}

function gigya_CM_get_allcomments(){
	global $site;
	$debug_comments = false;
	if(isset($GLOBALS['gigya']['CM_per_page'])){
		$per_page = isset($_POST['gigya_CM_per_page']) && $_POST['gigya_CM_per_page'] != $GLOBALS['gigya']['CM_per_page'] ? $_POST['gigya_CM_per_page']:$GLOBALS['gigya']['CM_per_page'];
	} else {
		$per_page = isset($_POST['gigya_CM_per_page']) && $_POST['gigya_CM_per_page'] != 5 ? $_POST['gigya_CM_per_page']:5;	
	}
	$page = isset($_POST['gigya_CM_page']) && $_POST['gigya_CM_page'] != 1 ? $_POST['gigya_CM_page']:1;
	$memkey = "gigya_CM_agg_".$site->siteID."_".$site->networkID."_".$site->site_sign;
	$mem_output = $site->memCache->get($memkey);
	if($mem_output != ""){
		$tmp_array = $mem_output;	
	} else {
		$start = NULL;
		$comment_streams = explode(",",gigya_CM_get_blockIDs());
		$comment_data = array();
		gigya_CM_login("Get Comments2");
		include_once('/export/home/global_includes/GSSDK.php');
		//$debug_comments = "";
		foreach($comment_streams as $streamID){
			//$start = isset($_POST['gigya_CM_start']) ? $_POST['gigya_CM_start']:NULL;
			$gigyaCommentsGSRequest = new GSRequest($GLOBALS['gigya']['api_key'],$GLOBALS['gigya']['api_secret'],"comments.getComments");
			$gigyaCommentsGSRequest->setParam("categoryID",$GLOBALS['gigya']['category']);
			$gigyaCommentsGSRequest->setParam("streamID",$streamID);
			$gigyaCommentsGSRequest->setParam("threadLimit",150);
			$gigyaCommentsGSRequest->setParam("threadDepth",5);
			$gigyaCommentsGSRequest->setParam("sort","dateDesc");
			$gigyaCommentsGSRequest->setParam("includeStreamInfo",true);
			$gigyaCommentsGSRequest->setParam("includeUID",true);
			if(!is_null($start)){
				$gigyaCommentsGSRequest->setParam("start",$start);	
			}
			$gigyaGetComments = $gigyaCommentsGSRequest->send();
			if($gigyaGetComments->getErrorCode()!=0) {
				$subject = $gigyaGetComments->getErrorCode()." - SiteID: ".$site->siteID." - Call: comments.getComments";
				$message = "Response Text: ".$gigyaGetComments->getResponseText();
				mail('mike@platformic.com',$subject,$message);
			}
			$raw_comments = json_decode($gigyaGetComments->getData());
			//$debug_comments .='<pre>'.print_r($raw_comments,1).'</pre>';
			// check to see if any comments exists
			if($raw_comments->commentCount != 0){
				$comments = $raw_comments->comments;
				$avatar_URLprotocol = $_SERVER['HTTPS'] == 'on' ? 'https' : 'http';
				// Grab headline from platformic DB
				$sql = "SELECT headline FROM text_block WHERE siteID = '".$site->siteID."' AND blockID = '".$streamID."'";
				$res = $site->_return_data($sql);
				foreach($res as $row){
					$headline = $row['headline'];
				}
				
				foreach($comments as $comment){
					// If comment has replies
					if($comment->descendantsCount > 0){
						if($comment->flagCount == 0){
							
							$inner_comment_data = array();
							$inner_comment_data['id'] = $comment->ID;
							$inner_comment_data['streamID'] = $streamID;
							$inner_comment_data['headline'] = $headline;
							$inner_comment_data['comment_text'] = $comment->commentText;
							$inner_comment_data['flag_count'] = $comment->flagCount;
							$inner_comment_data['photo_url'] = $comment->sender->photoURL != "" ? '<img src="'.$comment->sender->photoURL.'" width="45" height="45" />':'<img src="'.$avatar_URLprotocol.'://'.$_SERVER['HTTP_HOST'].'/common/global_images/icons/avatars/anonymous.jpg" width="45" height="45" />';
							$inner_comment_data['name'] = $comment->sender->name;
							$inner_comment_data['UID'] = $comment->sender->UID;
							$inner_comment_data['is_reply'] = 0;
							$timestamp = $comment->timestamp/1000;
							$comment_time = date("m/d/y g:ia T",$timestamp);
							$inner_comment_data['formatted_time'] = $comment_time;
							$comment_data[$comment->timestamp] = $inner_comment_data;
						}
						
						foreach($comment->replies as $tmp_data){
							$inner_comment_data = array();
							$inner_comment_data['id'] = $tmp_data->ID;
							$inner_comment_data['streamID'] = $streamID;
							$inner_comment_data['headline'] = $headline;
							$inner_comment_data['comment_text'] = $tmp_data->commentText;
							$inner_comment_data['flag_count'] = $tmp_data->flagCount;
							$inner_comment_data['photo_url'] = $tmp_data->sender->photoURL != "" ? '<img src="'.$tmp_data->sender->photoURL.'" width="45" height="45" />':'<img src="'.$avatar_URLprotocol.'://'.$_SERVER['HTTP_HOST'].'/common/global_images/icons/avatars/anonymous.jpg" width="45" height="45" />';
							$inner_comment_data['name'] = $tmp_data->sender->name;
							$inner_comment_data['UID'] = $tmp_data->sender->UID;
							$inner_comment_data['is_reply'] = 1;
							$timestamp = $tmp_data->timestamp/1000;
							$comment_time = date("m/d/y g:ia T",$timestamp);
							$inner_comment_data['formatted_time'] = $comment_time;
							$comment_data[$tmp_data->timestamp] = $inner_comment_data;
							$tack_on_replies = gigya_CM_get_replies($tmp_data,$streamID,$headline,$avatar_URLprotocol,1);
						}
						// Add replies to comment data array
						if(is_array($tack_on_replies) || is_object($tack_on_replies)){
							foreach($tack_on_replies as $key=>$val){
								$comment_data[$key] = $val;	
							}
						}
					} else {
						// Comment does not have replies
						if($comment->flagCount < 1){
							$inner_comment_data = array();
							$inner_comment_data['id'] = $comment->ID;
							$inner_comment_data['streamID'] = $streamID;
							$inner_comment_data['headline'] = $headline;
							$inner_comment_data['comment_text'] = $comment->commentText;
							$inner_comment_data['flag_count'] = $comment->flagCount;
							$inner_comment_data['photo_url'] = $comment->sender->photoURL != "" ? '<img src="'.$comment->sender->photoURL.'" width="45" height="45" />':'<img src="'.$avatar_URLprotocol.'://'.$_SERVER['HTTP_HOST'].'/common/global_images/icons/avatars/anonymous.jpg" width="45" height="45" />';
							$inner_comment_data['name'] = $comment->sender->name;
							$inner_comment_data['UID'] = $comment->sender->UID;
							$inner_comment_data['is_reply'] = 0;
							$timestamp = $comment->timestamp/1000;
							$comment_time = date("m/d/y g:ia T",$timestamp);
							$inner_comment_data['formatted_time'] = $comment_time;
							$comment_data[$comment->timestamp] = $inner_comment_data;
						}
					}
				}
			} // End foreach loop
		}
		gigya_CM_logout("Get Comments2");
		// Sort data array in descending order
		// Add to memchace
		krsort($comment_data);
		$tmp_array = array();
		foreach($comment_data as $comment_array){
			$tmp_array[] = $comment_array;	
		}
		//die('<pre>'.print_r($comment_data,1).'</pre>');
		$site->memCache->add($memkey,$tmp_array,false,240);
	}
	###################################
	#### If function called from search
	###################################
	if($_POST['gigya_CM_is_search']){
		if($_POST['gigya_CM_search_type'] != "none"){
			$tmp_array = gigya_CM_search($tmp_array,$_POST['gigya_CM_search_phrase'],$_POST['gigya_CM_search_type']);
			$page = count($tmp_array) > $per_page ? $page:1;
		}
	}
	// END search manipulation
	$comment_output = gigya_CM_build_aggtable($tmp_array,$per_page,$page,$debug_comments);
	//$comment_output = "<pre>".print_r($tmp_array,1)."</pre>";
	$comment_output = stripslashes(stripslashes($comment_output));
	$comment_output = preg_replace("/\n/","\\n",$comment_output);
	$comment_output = preg_replace("/\r/","\\n",$comment_output);
	$comment_output = preg_replace("/'/","\'",$comment_output);
	// JS
	ob_start();
	?>
	<script type="text/javascript">
		$("#gigya_comments_holder").html('<?php echo $comment_output; ?>');
		// Handle pagination
		$("#gigya_CM_nextBtn").click(function(e){
			e.preventDefault();
			// Check if search params exist
			var gCMsearchParams = gigyaCMSearchPresent();
			// Set perPage variable before injecting new html
			var perPage = $("#gigya_CM_aggperpage").val();
			var goToPage = $("#gigya_CM_nextBtn").attr('href');
			$("#gigya_comments_holder").html('<?php echo '<div align="center"><img src="'.ICON_URL.'/loading.gif" /><br />Loading</div>'; ?>');
			var streamIDs = "<?php echo $gigya_CM_aggregateIDs != "" ? $gigya_CM_aggregateIDs:gigya_CM_get_blockIDs(30); ?>";
			qpost("module:gigya_comments~function:gigya_CM_get_allcomments~streamIDs:"+streamIDs+"~gigya_CM_per_page:"+perPage+"~gigya_CM_page:"+goToPage+gCMsearchParams);
		});
		$("#gigya_CM_prevBtn").click(function(e){
			e.preventDefault();
			// check if search params exist
			var gCMsearchParams = gigyaCMSearchPresent();
			// Set perPage variable before injecting new html
			var perPage = $("#gigya_CM_aggperpage").val();
			var goToPage = $("#gigya_CM_prevBtn").attr('href');
			$("#gigya_comments_holder").html('<?php echo '<div align="center"><img src="'.ICON_URL.'/loading.gif" /><br />Loading</div>'; ?>');
			var streamIDs = "<?php echo $gigya_CM_aggregateIDs != "" ? $gigya_CM_aggregateIDs:gigya_CM_get_blockIDs(30); ?>";
			qpost("module:gigya_comments~function:gigya_CM_get_allcomments~streamIDs:"+streamIDs+"~gigya_CM_per_page:"+perPage+"~gigya_CM_page:"+goToPage+gCMsearchParams);
		});
		$("#gigya_CM_reload_link").click(function(e){
			e.preventDefault();
			qpost("module:gigya_comments~function:gigya_CM_reload_comments");
		});
	</script>
	<?
	$site->include_jquery = 1;
	$js_output = ob_get_clean();
	$site->post_output = $js_output;
}

function gigya_CM_get_replies($data,$streamID,$headline,$protocol,$round,$has_replies=false){
	// Check if incomming reply has relpy
	// If yes recurse through function again
	// If not return
	global $temp_array;
	if($round){
		$incomming = $data->replies;
		if(is_array($incomming) || is_object($incomming)){
			foreach($incomming as $reply1){
				if($inner_reply->flagCount < 1){
					$inner_comment_data = array();
					$inner_comment_data['id'] = $reply1->ID;
					$inner_comment_data['streamID'] = $streamID;
					$inner_comment_data['headline'] = $headline;
					$inner_comment_data['comment_text'] = $reply1->commentText;
					$inner_comment_data['flag_count'] = $reply1->flagCount;
					$inner_comment_data['photo_url'] = $reply1->sender->photoURL != "" ? '<img src="'.$reply1->sender->photoURL.'" width="45" height="45" />':'<img src="'.$protocol.'://'.$_SERVER['HTTP_HOST'].'/common/global_images/icons/avatars/anonymous.jpg" width="45" height="45" />';
					$inner_comment_data['name'] = $reply1->sender->name;
					$inner_comment_data['UID'] = $comment->sender->UID;
					$inner_comment_data['is_reply'] = 1;
					$timestamp = $reply1->timestamp/1000;
					$comment_time = date("m/d/y g:ia T",$timestamp);
					$inner_comment_data['formatted_time'] = $comment_time;
					$temp_array[$reply1->timestamp] = $inner_comment_data;
				}
				if($reply1->replies){
					gigya_CM_get_replies($reply1->replies,$streamID,$headline,$protocol,0,true);
				}
			}
			return $temp_array;
		}
	}
	
	if($has_replies){
		$replies = $data;
		if(is_array($replies) || is_object($replies)){
			foreach($replies as $inner_reply){
				$inner_comment_data = array();
				if($inner_reply->flagCount < 1){
					$inner_comment_data['id'] = $inner_reply->ID;
					$inner_comment_data['streamID'] = $streamID;
					$inner_comment_data['headline'] = $headline;
					$inner_comment_data['comment_text'] = $inner_reply->commentText;
					$inner_comment_data['flag_count'] = $inner_reply->flagCount;
					$inner_comment_data['photo_url'] = $inner_reply->sender->photoURL != "" ? '<img src="'.$inner_reply->sender->photoURL.'" width="45" height="45" />':'<img src="'.$protocol.'://'.$_SERVER['HTTP_HOST'].'/common/global_images/icons/avatars/anonymous.jpg" width="45" height="45" />';
					$inner_comment_data['name'] = $inner_reply->sender->name;
					$inner_comment_data['UID'] = $comment->sender->UID;
					$inner_comment_data['is_reply'] = 1;
					$timestamp = $inner_reply->timestamp/1000;
					$comment_time = date("m/d/y g:ia T",$timestamp);
					$inner_comment_data['formatted_time'] = $comment_time;
					$temp_array[$inner_reply->timestamp] = $inner_comment_data;
				}
				
				if($inner_reply->replies){
					gigya_CM_get_replies($inner_reply->replies,$streamID,$headline,$protocol,0,true);
				}
			}
		}
		return $temp_array;
	}
	
}

function gigya_CM_build_aggtable($data_array,$per_page,$page=1,$debug_comments=false){
	global $site;
	$page_txt = $page != 1 ? $page:1;
	$header_table = new HTMLtable('wi=100%,bc='.ADMIN_TABDATA_ALTCOLOR,'off');
	$data_table = new HTMLtable('wi=100%,cp=6px,bc='.ADMIN_TABDATA_ALTCOLOR,'on','','','admDataTable','plt_admin_datatable plt_color_table');
	// Debugging, var dump comments object on screen.
	if($debug_comments != false){
		$data_table->row(array($debug_comments),"al=left");
	}
	// END Debug
	if(is_array($data_array)){
		$size = count($data_array);
		$total_pages = ceil($size/$per_page);
		$total_pages = $total_pages == 0 ? 1:$total_pages;
		$next = $page_txt == $total_pages ? "":'<a href="'.($page+1).'" id="gigya_CM_nextBtn" name="gigya_CM_nextBtn" title="Next Page">Next >></a>';
		$previous = $page_txt > 1 ? '<a href="'.($page-1).'" id="gigya_CM_prevBtn" name="gigya_CM_prevBtn" title="Previous Page"><< Previous</a>':""; 
		$header_table->row(array('<strong>Total Comments: </strong>'.$size." <a href='#' id=\"gigya_CM_reload_link\" name=\"gigya_CM_reload_link\">".u_icon('arrow_rotate_clockwise',"","Reload from Gigya")."</a>",'Page '.$page_txt.' of '.$total_pages."&nbsp;&nbsp;&nbsp;".$previous." ".$next,submit_input("gigya_CM_remove_btn","Remove Selected",1,"gigyaCMRemoveComment();")),"al=left~center~right,wi=33%,va=top,ht=27px");
		$data_table->row(array($header_table->output()),"wi=100%,al=left,va=top,cs=3");
		$base_URLprotocol = $_SERVER['HTTPS'] == 'on' ? 'https' : 'http';
		$base_url = $base_URLprotocol."://".$_SERVER['HTTP_HOST']."/landing";
		// Account for page number difference in for loop
		$set_i = ($page-1) != 0 ? ($page*$per_page)-$per_page:$page-1;
		$per_page = $page != 1 ? $set_i + $per_page:$per_page;
		$per_page = $per_page < $size ? $per_page:$size;
		//$data_table->row(array("i = ".$set_i." Per Page = ".$per_page),"al=left");
		// Handle paging with for loop
		for($i=$set_i;$i < $per_page; $i++){
			$comment = $data_array[$i];
			//$test_data_array = "<pre>".print_r($comment,1)."</pre>";
			$url = $base_url."?blockID=".$comment['streamID'];
			$headline_link = '<a href="'.$url.'">'.$comment['headline'].'</a>';
			$reply_icon = $comment['is_reply'] ? u_icon('arrow_rotate_anticlockwise',"","Reply")." ":"";
			$replace_text = $comment['is_reply'] ? "Reply comment: ":"Commented on: ";
			$comment_text = preg_replace("/'/","\'",$comment['comment_text']);
			$tmp_CM_cbody = $reply_icon.'<strong>'.$comment['name'].'</strong><br /><em>'.$replace_text.'</em> '.$headline_link.'<br /><em>Comment: </em>'.$comment_text.'<br /><br /><span class="gigya_CM_date">'.$comment['formatted_time'].'</span><br />';
			// Build parent comment
			$data_table->row(array($comment['photo_url'],$tmp_CM_cbody,checkbox("gigya_CM_flagchk",$comment['id']."^".$comment['streamID'],"")." ".u_icon('flag_red',"","Remove Comment")),"al=left~left~center,wi=45px~655px~90px,va=top");
		}
	}
	$output = $data_table->output();
	return $output;	
}

function gigya_CM_get_blockIDs($limit=50,$agg_limit=29,$type="aggregate"){
	global $site;
	$sql_blockIDs = "SELECT blockID, headline FROM text_block WHERE siteID = '".$site->siteID."' ORDER BY blockID DESC LIMIT ".$limit;
	$res = $site->_return_data($sql_blockIDs);
	$article_dropdown = array(-1=>"Select");
	$gigya_CM_aggregateIDs = "";
	$i = 0;
	foreach($res as $row){
		$article_dropdown[$row['blockID']] = substr($row['blockID']." : ".$row['headline'],0,100);
		if($i <= $agg_limit){
			$gigya_CM_aggregateIDs .= $row['blockID'].",";
		}
		$i++;
	}
	$gigya_CM_aggregateIDs = rtrim($gigya_CM_aggregateIDs,",");
	$article_select = do_select_array("platformic_blockIDs",$article_dropdown,"",0,1,0,1);
	switch($type){
		case 'aggregate':
			return $gigya_CM_aggregateIDs;
		break;
		case 'comment_stream':
			$rtrn_array = array($gigya_CM_aggregateIDs,$article_select);
			return $rtrn_array;
		break;	
	}
}
function gigya_CM_search($data_array,$search_phrase,$search_type){
	global $site;
	switch($search_type){
		case 'uname':
			if(is_array($data_array)){
				$ouput_array = array();
				$size = count($data_array);
				for($i=0;$i<$size;$i++){
					$comment = $data_array[$i];
					if(stristr(addslashes($comment['name']),$search_phrase)){
						$ouput_array[] = $comment;	
					}
				}
				return $ouput_array;
			}
		break;
		case 'body':
			if(is_array($data_array)){
				$ouput_array = array();
				$size = count($data_array);
				for($i=0;$i<$size;$i++){
					$comment = $data_array[$i];
					if(stristr(addslashes($comment['comment_text']),$search_phrase)){
						$ouput_array[] = $comment;	
					}
				}
				return $ouput_array;
			}
		break;
		case 'combo':
			if(is_array($data_array)){
				$exploded_search = explode("$$##$$",$search_phrase);
				$uname = $exploded_search[0];
				$search_body = $exploded_search[1];
				$ouput_array = array();
				$size = count($data_array);
				for($i=0;$i<$size;$i++){
					$comment = $data_array[$i];
					if(strstr(addslashes($comment['name']),$uname)){
						if(stristr(addslashes($comment['comment_text']),$search_body)){
							$ouput_array[] = $comment;
						}
					}
				}
				return $ouput_array;
			}
		break;
	}
	return false;
}

function gigya_CM_flag_comment(){
	global $site;
	$commentID_str = $_POST['gigyaCMCommentIDs'];
	if(strstr($commentID_str,",")){
		$exploded_commentIDs = explode(",",$commentID_str);
	} else {
		$exploded_commentIDs = array($commentID_str);
	}
	// Set UID for GS request
	if(strlen($site->user->gigyaUID) > 0) {
		$gigyaUID = $site->user->gigyaUID;
	} else {
		$gigyaUID = $site->user->ugc_userID;
	}
	gigya_CM_login("Flag Comment");
	include_once('/export/home/global_includes/GSSDK.php');
	foreach($exploded_commentIDs as $comment_params){
		$explode_params = explode("^",$comment_params);
		$commentID = $explode_params[0];
		$streamID = $explode_params[1];
		$categoryID = $GLOBALS['gigya']['category'];
		$iterator_limit = 1;		
		for($i=0;$i<$iterator_limit;$i++){
			// Make gigya API Call
			$gigyaFlagGSRequest = new GSRequest($GLOBALS['gigya']['api_key'],$GLOBALS['gigya']['api_secret'],"comments.flagComment");
			$gigyaFlagGSRequest->setParam("commentID",$commentID);	
			$gigyaFlagGSRequest->setParam("categoryID",$categoryID);
			$gigyaFlagGSRequest->setParam("streamID",$streamID);
			$gigyaFlagGSRequest->setParam("uid",$gigyaUID);
			$gigyaSetFlag = $gigyaFlagGSRequest->send();
			// Troubleshooting
			if($gigyaSetFlag->getErrorCode()!=0) {
				$subject = $gigyaSetFlag->getErrorCode()." - SiteID: ".$site->siteID." - Call: comments.flagComment";
				$message = "Response Text: ".$gigyaSetFlag->getResponseText();
				mail('mike@platformic.com',$subject,$message);
			} else {
				//$subject = $gigyaSetFlag->getErrorCode()." - SiteID: ".$site->siteID." - Call: comments.flagComment";
				//$message = "Response Text: ".$gigyaSetFlag->getResponseText();
				//mail('mike@platformic.com',$subject,$message);	
			}
		}
		
		//$memkey = "gigya_CM_agg_".$site->siteID."_".$site->networkID."_".$site->site_sign;
		//$site->memCache->delete($memkey);
		/*$site->post_output = '.<script type="text/javascript">setTimeout("window.location.href=\"login?core_app=gigya_comments\";",400);</script>.';*/
		
	}
	gigya_CM_logout("Flag Comments");
	gigya_CM_reload_comments();
}
function gigya_CM_JS_flag(){
	global $site;
	
	if(strstr($_POST['gigyaCMCommentIDs'],",")){
		$commentID_str = $_POST['gigyaCMCommentIDs'];
	} else {
		$commentID_str = $_POST['gigyaCMCommentIDs'].",";	
	}
	ob_start();
	?>
    	function gigyaResponse(response){
        	var code = response.errorCode;
            var message = response.errorMessage;
            var operation = response.operation;
            alert("Error Code: "+code+"\nError Message: "+message+"\nOperation: "+operation);
        }
        function gigyaCMflagComment(){
        	var gigyaCMCommentIDs = "<?php echo $commentID_str; ?>";
            var gcmComment = gigyaCMCommentIDs.split(",");
            for(var i=0;i < gcmComment.length; i++){
                var tmp = gcmComment[0];
                var tmp2 = tmp.split("^");
                var gcmComID = tmp2[0];
                var gcmStreamID = tmp2[1];
                var gigyaCMFlagParams = {
                    commentID: gcmComID,
                    categoryID: "<?php echo $GLOBALS['gigya']['category']; ?>",
                    streamID: gcmStreamID,
                    callback: gigyaResponse
				}
                gigya.services.comments.flagComment(gigyaCMFlagParams);
			}
        }
        
        setTimeout("gigyaCMflagComment();",200);
    <?
	$flag_JS = ob_get_clean();
	$site->post_output = '&nbsp;<script language="javascript" src="http://cdn.gigya.com/js/socialize.js?apiKey='.$GLOBALS['gigya']['api_key'].'"></script><script type="text/javascript">'.$flag_JS.'</script>&nbsp;';
		
}

function gigya_CM_reload_comments(){
	global $site;
	if(isset($_GET['blockID'])){
		$memkey = "gigya_CM".$_GET['blockID']."_".$site->siteID."_".$site->networkID."_".$site->site_sign;
	} else {
		$memkey = "gigya_CM_agg_".$site->siteID."_".$site->networkID."_".$site->site_sign;	
	}
	$site->memCache->delete($memkey);
	$url_protocol = $_SERVER['HTTPS'] == 'on' ? 'https' : 'http';
	$vars_string = isset($_GET['blockID']) ? "~function:gigya_CM_get_comments~streamID:".$_GET['blockID']:"~function:gigya_CM_get_allcomments";
	/*$site->post_output = '.<script type="text/javascript">window.location.href="'.$url_protocol.'://'.$_SERVER['HTTP_HOST'].'/login?'.$coreapp.'";</script>.';*/
	ob_start();
	?>
    	$("#gigya_comments_holder").html('<?php echo '<div align="center"><img src="'.ICON_URL.'/loading.gif" /><br />Loading</div>'; ?>');
        function gigyaCMFlagReload(){
        	qpost("module:gigya_comments<?php echo $vars_string; ?>");
        }
        
        setTimeout("gigyaCMFlagReload();",200);
    <?
	$JS = ob_get_clean();
	$site->post_output = '&nbsp;<script type="text/javascript">'.$JS.'</script>&nbsp;';
}
//$admin_outer->row(array('<strong>Comment Stream: </strong>'.$article_select),"al=left");
?>