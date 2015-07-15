$(document).ready(function() {
	$('a[name=closeErrorBox]').click(function() {
		$(this).fadeOut("slow");
		$('.errorMessageSearch').fadeOut("slow");
	});

	$('a[name=closeNoticeBox]').click(function() {
		$(this).fadeOut("slow");
		$('.searchNotice').fadeOut("slow");
	});

	$('a[name=closeSuccessBox]').click(function() {
		$(this).fadeOut("slow");
		$('.success').fadeOut("slow");
	});

});