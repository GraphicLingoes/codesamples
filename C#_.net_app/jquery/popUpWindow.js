$(document).ready(function() {
	$('a[name=launchHelp]').click(function() {
	var winH = (($(window).height()) / 2) - 325;
	var winW = (($(window).width()) / 2) - 400;
	var windowOptions = "width=750,height=650,resizable=yes,scrollbars=yes,left=" + winW + ",top=" + winH;
	window.open($(this).attr('href'), "helpGlossary", windowOptions);
		return false;
	});

	$('a[name=launchHelpEdit]').click(function() {
	// Get the window height and width
	var winH = (($(window).height())/2) - 325;
	var winW = (($(window).width()) / 2) - 400;
	var windowOptions = "width=750,height=700,left=" + winW + ",top=" + winH;
		window.open($(this).attr('href'), "manageHelp", windowOptions);
		return false;
	});
});