    <div class="container">
      <div class="starter-template">
        <div class="well">
        <?php if(isset($ErrorMessage)):?>
        	<div class="alert alert-danger" role="alert">
        		<?php echo $ErrorMessage; ?>
        	</div>
        <?php else: ?>
        	<div class="alert alert-info" role="alert">
        		<?php echo $UploadStatus; ?>
        	</div>
        <?php endif; ?>
		</div>
      </div>
    </div><!-- /.container -->