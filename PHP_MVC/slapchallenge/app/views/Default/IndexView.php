    <div class="container">
      <div class="starter-template">
        <div class="well">
        <h1>Upload Contestants</h1>
        <form action="api/upload/index" method="post" enctype="multipart/form-data">
		    <label for="contestantsUpload">.csv files only please.</label>
		    <input type="file" name="contestantsUpload" id="contestantsUpload">
		    <input type="submit" value="Upload Contestants" name="submit">
		</form>
		</div>
      </div>

    </div><!-- /.container -->