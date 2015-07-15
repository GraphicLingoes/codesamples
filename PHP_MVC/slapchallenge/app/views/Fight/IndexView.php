    <div class="container">
      <div class="starter-template">
        <div class="well">
       		<div id="fightArena" class="alert alert-warning" role="alert">
       			<h1>Fight Arena</h1>
       		</div>
       		<strong>Instructions:</strong>Click the "Fight" link under actions to have that contestant fight all other applicants. Do this for each applicant then click the "Winner's" circle link above.
       		<table class="table table-striped" id="applicantsTable">
 				<thead>
 					<td>Name</td>
 					<td>Health</td>
 					<td>Damage</td>
 					<td>Attacks</td>
 					<td>Actions</td>
 				</thead>
 				<tbody>
 				<?php if(isset($applicants) && count($applicants) > 0):?>
 					<?php foreach($applicants as $applicant):?>
 						<tr>
 							<td class="FullName"><?php echo $applicant["FullName"]; ?></td>
 							<td class="Health"><?php echo $applicant["Health"]; ?></td>
 							<td class="Damage"><?php echo $applicant["Damage"]; ?></td>
 							<td class="Attacks"><?php echo $applicant["Attacks"]; ?></td>
 							<td class="FightAction"><a id="<?php echo $applicant["idApplicant"]; ?>" class=\"btn btn-primary\" href="#">Fight!</a></td>
 							
 						</tr>
 					<?php endforeach;?>
 				<?php else: ?>
 					<tr>
 						<td>
 							There are no fighters uploaded, please <a href="/">Click Here</a> to upload.
 						</td>
 					</tr>
 				<?php endif;?>
				</tbody>
			</table>
		</div>
      </div>
	
    </div><!-- /.container -->