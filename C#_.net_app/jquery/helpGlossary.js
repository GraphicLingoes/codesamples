$(document).ready(function() {
	$('a[name=helpTopicLink]').click(function(event) {
		event.preventDefault();
		var parDiv = $(this).parent();
		var childDiv = $(parDiv).children().find(".subMenu");
		$(childDiv).toggle("fast");
	});

	$('a[name=clickSubMenu]').click(function(event) {
		event.preventDefault();
		var pageID = $(this).attr('href');
		$('#loadedHelpPage').load('helpContentInsert.aspx' + pageID);
	});

	$('a[name=clickSearchPage]').click(function(event) {
		event.preventDefault();
		var pageID = $(this).attr('href');
		$('#loadedHelpPage').load('helpContentInsert.aspx' + pageID);
	});

	$("#loadedHelpPage").live('hover', function() {
		$('a[name=clickEmbededLink]').click(function(event) {
			event.preventDefault();
			var pageID = $(this).attr('href');
			$('#loadedHelpPage').load('helpContentInsert.aspx' + pageID);
		});
	});
});