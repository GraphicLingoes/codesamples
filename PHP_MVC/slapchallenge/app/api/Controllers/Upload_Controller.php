<?php
class Upload_Controller extends Base_Controller {
	protected $utils = null;
	
	public function __construct() {
		$this->utils = new utils();
	}
	public function action_index () {
		try {
		$target_dir = "./app_data/";
		$target_file = $target_dir . basename($_FILES["contestantsUpload"]["name"]);
		if(isset($_POST["submit"])) {
		    // First check mime type to ensure correct file is being uploaded.
			$check = getimagesize($_FILES["contestantsUpload"]["tmp_name"]);
		    $mimes = array('application/vnd.ms-excel','text/plain','text/csv','text/tsv');
		    if(in_array($_FILES['contestantsUpload']['type'],$mimes)){
			    // Next check size of file to ensure it does not violate limit.
		    	if ($_FILES["contestantsUpload"]["size"] > 500000) {
		    		// NOTE: in a production environment we would not give an error message
		    		// with this much detail based on OWASP recommendations.
		    		throw new Exception('Error, file exceeds maximum file size.');
		    	}
				// Next try to save the file to the app_data directory.
		    	if (move_uploaded_file($_FILES["contestantsUpload"]["tmp_name"], $target_file)) {
			        // do nothing at this time.
			    } else {
			        throw new Exception('Error uploading file.');
			    }
		    } else {
		    	throw new Exception('Sorry, wrong file type.');
		    }
		}
				
		} catch (Exception $e) {
			// Handle Error gracefully.
			parent::setViewVars(array(
					"ErrorMessage" => $e->getMessage()
			));
			parent::setTemplate('SlapChallengeMain');
			parent::getView("Upload", "Index");
			parent::render();
			return;
		
		}
	  	// Instantiate model for Upload route.
		$model = parent::getModel('Upload', 'Upload');
		// Parse and store contents in databse.
		if ($model->ParseUpload($_FILES["contestantsUpload"]["name"], $this->utils)) {
			parent::setViewVars(array(
					"UploadStatus" => "File has been uploaded successfully! <a class=\"btn btn-primary\" href=\"/Fight/Index\">Let's Fight</a>"
			));
		}
		else 
		{
			parent::setViewVars(array(
					"UploadStatus" => "There has been an issue please <a href=\"/\">Click Here to Try Again</a>."
			));
		}
		
		parent::setTemplate('SlapChallengeMain');
		parent::getView("Upload", "Index");
		parent::render();
		
	}
}
