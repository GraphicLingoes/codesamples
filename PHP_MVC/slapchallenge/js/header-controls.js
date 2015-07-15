$(document).ready(function(){
	// Header toggle button up
	$('.header-controls-toggle-btm').click(function(e){
		e.preventDefault();
		$('#header-controls').animate({
			height: 18
		}, 100,function(){
			$('.header-controls-toggle-top').show();
		});
	});

	// Header toggle button down
	$('.header-controls-toggle-top').click(function(e){
		e.preventDefault();
		$('.header-controls-toggle-top').hide();
		$('#header-controls').animate({
			height: 242
		}, 100);
	});
});