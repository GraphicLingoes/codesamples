  <div class="container">
      <div class="starter-template">
        <div class="well">
       		<div id="fightArena" class="alert alert-success" role="alert">
       			<h1>And the winner is....</h1>
       			<?php if(isset($winnersViewData)): ?>
       				<?php echo $winnersViewData["Winner"];?> with a winning percentage of <?php echo $winnersViewData["WinnerPercent"];?>%
       			<?php else: ?>
       				
       			<?php endif;?>
       		</div>
       		
		</div>
      </div>
	
    </div><!-- /.container -->