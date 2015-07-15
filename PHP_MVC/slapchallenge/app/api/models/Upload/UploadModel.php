<?php
class UploadModel {
	/**
	 * [$request private data store for request body content]
	 * @var boolean
	 */
	private $request = false;

	/**
	 * [__construct Constructor method for Styles Model class. Set request body content to private member for easy access.]
	 * @param Request $request [Data store object that contains request body content.]
	 */
	public function __construct(Request $request)
	{
		if(is_a($request, 'Request'))
		{
			$this->request = $request;
		}
	}
	/**
	 * [PasrseUpload method used to iterate through content so upload and store in databse. For this asignment
	 * we are hard coding the indexes to save time. This can be made dynamic in the future.]
	 * @param string $fileName
	 */
	public function ParseUpload($fileName, $utils) {
		// First check to see if $fileName valid
		if(isset($fileName)) {
			$this->ResetData($utils);
			$row = 1;
			$pathToUploads = "././app_data/";
			if (($handle = fopen($pathToUploads . $fileName, "r")) !== FALSE) {
				$utils->OpenConn();
				while (($data = fgetcsv($handle, 1000, ",")) !== FALSE) {
			        if ($row != 1) {
				        // Insert values into DB
		        		$params = array ("FullName" => $data[0] . "=>str");
			            $stmt = "INSERT INTO applicant (FullName) VALUES (:FullName)";
			            $utils->Query($stmt, $params);
			            $lastInsertID = $utils->LastInsertID();
			            $vitalsParams = array ( "idApplicant" => $lastInsertID . "=>int"
			            		,"Health" => $data[1] . "=>int"
			            		,"Damage" => $data[2] . "=>int"
			            		,"Attacks" => $data[3] . "=>int"
			            		,"Dodge" => $data[4] . "=>int"
			            		,"Critical" => $data[5] . "=>int"
			            		,"Initiative" => $data[6] . "=>int"
			            	);
			           	$stmtVitals = "INSERT INTO vitals_start (idApplicant, Health, Damage, Attacks, Dodge, Critical, Initiative)" .
					           	" VALUES (:idApplicant, :Health, :Damage, :Attacks, :Dodge, :Critical, :Initiative)";
			           	
			           	$utils->Query($stmtVitals, $vitalsParams);
			        }
			        $row++;
			    }
			    $utils->CloseConn();
			    fclose($handle);
			}
		}
		
		return true;
	}
	
	public function ResetData($utils){
		$utils->OpenConn();
		$stmt = "DELETE FROM applicant; DELETE FROM vitals_start; DELETE FROM challenges;";
		$utils->Query($stmt);
		$utils->CloseConn();
	}
}