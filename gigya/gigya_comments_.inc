<? 
// Gigya comments moderation dashboard 
// created 12/21/2011 by mike@platformic.com
/*	Copyright 2012 Mike Aguilar. - All rights reserved.
 *	Use of this source code without direct authorization is strictly
 *	prohibited.	
 */
#########################################
## Abstract: This module uses the Gigya
## REST api to grab comments from
## specific stream then provides
## administrator with a means to flag
## a comment to remove it from the site.
#########################################
//css
ob_start();
?>
<style type="text/css" media="all">
	#gigya_CM_wrapper {padding:9px;width:auto;height:auto;}
	#gigya_CM_thumb {float:left;width:45px;height:45px;border:2px solid #fff;}
	#gigya_CM_body {float:left;padding-bottom:12px;padding-left:12px;width:670px;height:auto;}
	.gigya_CM_date {font-size:10px;};
</style>
<?
$gigya_CM_css = ob_get_clean();
$admin_outer = new HTMLtable("wi=100%","off","","","admOuter","plt_admin_outer");
$admin_outer->row(array("<h1>Gigya Comments Moderation</h1>"), "al=left");

$admin_outer->row(array(gap()),'al=left');
// Search Bar
$per_page = $GLOBALS['gigya']['CM_per_page'];
$article_per_page = do_select_array("gigya_CM_aggperpage",array(5=>"5",10=>"10",25=>"25",50=>"50"),$per_page,0,1,0,1);
$admin_outer->row(array("Search by username: ".text_input("gigya_CM_search_uname","",18)." | Search comment body: ".text_input("gigya_CM_search_cbody","",18).' | Per Page: '.$article_per_page),"al=left");
$admin_outer->row(array(gap()."".gap()),'al=left');
// Set up Gigya content
$admin_outer->row(array('<div id="gigya_comments_holder">'.hidden_input("gigya_CM_init",1).'</div>'),"al=left");
// JS
ob_start();
?>
<script type="text/javascript">
if($("#gigya_CM_init").val() == 1){
	$("#gigya_comments_holder").html('<?php echo '<div align="center"><img src="'.ICON_URL.'/loading.gif" /><br />Loading</div>'; ?>');
	qpost("module:gigya_comments~function:gigya_CM_get_allcomments~gigya_CM_is_search:0~streamIDs:<?php echo $gigya_CM_aggregateIDs; ?>");
}
$("#platformic_blockIDs").change(function(){
	var streamID = $(this).val();
	// check if search params exist
	var gCMsearchParams = gigyaCMSearchPresent();
	if(streamID != -1){
		var perPage = $("#gigya_CM_perpage").val();
		qpost("module:gigya_comments~function:gigya_CM_get_comments~streamID:"+streamID+"~gigya_CM_per_page:"+perPage+gCMsearchPhrase);	
	} else {
		alert('Please select steam to view comments.');	
	}
});

$("#gigya_CM_aggperpage").change(function(){
	setTimeout('gigyaCMpageChange();',200);
});
$("#gigya_CM_search_uname").keyup(throttle(function (event) {
	unameField = $("#gigya_CM_search_uname").val();
	setTimeout('gigyaCMsearchUname();',200);
	return;
}, 250));

$("#gigya_CM_search_cbody").keyup(throttle(function (event) {
	bodyField = $("#gigya_CM_search_cbody").val();
	setTimeout('gigyaCMsearchBody();',200);
	return;
}, 250));
function gigyaCMpageChange(){
	// check if search params exist
	var gCMsearchParams = gigyaCMSearchPresent();
	// Set perPage variable before injecting new html
	var perPage = $("#gigya_CM_aggperpage").val();
	$("#gigya_comments_holder").html('<?php echo '<div align="center"><img src="'.ICON_URL.'/loading.gif" /><br />Loading</div>'; ?>');
	var streamIDs = "<?php echo $gigya_CM_aggregateIDs; ?>";
	qpost("module:gigya_comments~function:gigya_CM_get_allcomments~streamIDs:"+streamIDs+"~gigya_CM_per_page:"+perPage+gCMsearchParams);
}
function gigyaCMsearchUname(){
	var perPage = $("#gigya_CM_aggperpage").val();
	$("#gigya_comments_holder").html('<?php echo '<div align="center"><img src="'.ICON_URL.'/loading.gif" /><br />Loading</div>'; ?>');
	// check if search params exist
	var gCMsearchParams = gigyaCMSearchPresent();
	qpost("module:gigya_comments~function:gigya_CM_get_allcomments~streamIDs:<?php echo $gigya_CM_aggregateIDs; ?>~gigya_CM_per_page:"+perPage+gCMsearchParams);
}
function gigyaCMsearchBody(){
	var perPage = $("#gigya_CM_aggperpage").val();
	$("#gigya_comments_holder").html('<?php echo '<div align="center"><img src="'.ICON_URL.'/loading.gif" /><br />Loading</div>'; ?>');
	// check if search params exist
	var gCMsearchParams = gigyaCMSearchPresent();
	qpost("module:gigya_comments~function:gigya_CM_get_allcomments~streamIDs:<?php echo $gigya_CM_aggregateIDs; ?>~gigya_CM_per_page:"+perPage+gCMsearchParams);
}
function gigyaCMSearchPresent(){
	gCMuname = $("#gigya_CM_search_uname").val();
	gCMbody = $("#gigya_CM_search_cbody").val();
	if(gCMuname != "" && gCMbody == ""){
		return "~gigya_CM_is_search:1~gigya_CM_search_phrase:"+gCMuname+"~gigya_CM_search_type:uname~";
	}
			
	if(gCMuname == "" && gCMbody != ""){
		return "~gigya_CM_is_search:1~gigya_CM_search_phrase:"+gCMbody+"~gigya_CM_search_type:body~";
	}
			
	if(gCMuname != "" && gCMbody != ""){
		return "~gigya_CM_is_search:1~gigya_CM_search_phrase:"+gCMuname+"$$##$$"+gCMbody+"~gigya_CM_search_type:combo~";
	}
	
	if(gCMuname == "" && gCMbody == ""){
		return "~gigya_CM_is_search:0~";	
	}
	return "";
}
function gigyaCMRemoveComment(){
	var gigyaCMComValStr = "";
	$("input[type='checkbox']:checked").each(function(){
		gigyaCMComValStr += $(this).val() + ",";
	});
	var gigyaCMComValStrTrim = gigyaCMComValStr.substring(0, gigyaCMComValStr.length - 1);
	if(gigyaCMComValStrTrim != ""){
		if(confirm("Are you sure you want remove these comments?\nThis process is irreversible from this dashborad.")){
			gigyaCMFlagComment(gigyaCMComValStrTrim);	
		} else {
			return false;
		}
	} else {
		alert("You have not selected any comments.");	
	}
	return false;
	
}
function gigyaCMFlagComment(commentIDStr){
	qpost("module:gigya_comments~function:gigya_CM_flag_comment~gigyaCMCommentIDs:"+commentIDStr);
}
</script>
<?
$site->include_jquery = 1;
$gigya_CM_JS = ob_get_clean();
$module_output = $gigya_CM_css.$admin_outer->output().$gigya_CM_JS;

?>