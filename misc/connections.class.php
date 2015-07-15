<?
class MySQL{
	public function __construct($db){
		$this->localhost = 'localhost';
		$this->username = '';
		$this->password = '';
		$this->database = $db;
		
		//Establish connection
		$status = $this->open_dbc();
		if(!$status){
			return $status;	
		}
	}
	// MySQL functions
	// FUNCTION mysql_insert(mixed $sql,[string $type]);
	// If $sql is an array function will look for 2 indexs, tableName and params
	// params is an array, the params array will be passed through
	// the mysql_restr() function to sanitize values. Once sanitized the query will
	// be constructed and ran, otherwise if $sql is a string query will be ran immediately
	// and must be sanitzied prior to being passed into function.
	public function mysql_insert($sql,$type=""){
		if(is_array($sql)){
			$query_builder = "INSERT INTO ".$sql['tableName']." (";
			foreach($sql['params'] as $key=>$val){
				$query_builder .= $key.",";
			}
			$query_builder = substr($query_builder,0,-1);
			$query_builder .= ") VALUES(";
			$mres_params = $this->mysql_restr($sql['params']);
			foreach($mres_params as $key=>$val){
				$query_builder .= "'".$val."',";
			}
			$query_builder = substr($query_builder,0,-1);
			$query_builder .= ")";
			$result = mysql_query($query_builder);
		} else {
			$result = mysql_query($sql);
		}
		if($result){
			return true;	
		} else {
			$this->set_error($type);
			return false;	
		}
		return false;
	}
	
	public function mysql_return($sql,$column,$error_type="default",$section=""){
		$this->open_dbc();
		$result = mysql_query($sql);
		if($result){
			$return_array = array();
			while($row = mysql_fetch_assoc($result)){
				$return_array[] = $row[$column];	
			}
			mysql_free_result($result);
			$this->close_dbc();
			return $return_array;
		} else {
			$this->close_dbc();
			$this->set_error($error_type,$section);
			return false;	
		}
		return false;
	}
	
	// FUNCTION mysql_update: This function accepts two parameters to run updates
	// mysql_update(mixed $sql,[str $error_type]);
	// If $sql is an array function will look for 3 indexs, tableName, params and where
	// params and where both contain arrays, the params array will be passed through
	// the mysql_restr() function to sanitize values. Once sanitized the query will
	// be constructed and ran, otherwise if $sql is a string query will be ran immediately
	// and must be sanitzied prior to being passed into function.
	public function mysql_update($sql,$error_type="default"){
		$this->open_dbc();
		if(is_array($sql)){
			$query_builder = "UPDATE ".$sql['tableName']." SET ";
			$mres_params = $this->mysql_restr($sql['params']);
			foreach($mres_params as $key=>$val){
				$query_builder .= $key."='".$val."',";
			}
			$query_builder = substr($query_builder,0,-1);
			$query_builder .= " ".$sql['where']['clause']."'".mysql_real_escape_string($sql['where']['value'])."'";
			$result = mysql_query($query_builder);
		} else {
			$result = mysql_query($sql);
		}
		$affected_rows = mysql_affected_rows();
		if($affected_rows > 0){
			$this->close_dbc();
			return true;
		} else if($affected_rows == 0) {
			$this->close_dbc();
			$this->set_error('affected rows',$section);
			return false;
		} else {
			$this->close_dbc();
			$this->set_error($error_type,$section);
			return false;	
		}
		return false;
	}
	
	// FUNCTION mysql_restr: recursive function that will loop through params array and apply
	// mysql_real_escape_string to each value then will rebuild original
	// array with orignal keys and pass back for use in query
	private function mysql_restr($params){
		if(is_array($params)){
			foreach($params as $key=>$val){
				$params[$key] = $this->mysql_restr($val);	
			}
		} elseif(is_string($query)){
			$params = mysql_real_escape_string($query);	
		}
		return $params;
	}
	// Open DB connection
	private function open_dbc(){
		$this->link = mysql_connect($this->localhost, $this->username,$this->password);
		if($this->link){
			mysql_select_db($this->database);
			return false;
		} else {
			return($this->error_response = "There has been an error connecting to the database.\nPlease contact site administrator.");	
		}
		return true;
	}
	// Close DB connection
	private function close_dbc(){
		mysql_close($this->link);
	}
	
	private function set_error($type,$section=""){
		switch($type){
			default:
				$this->error_response = 'There has been an error.';
			break;
			case 'email':
				$this->error_response = 'Email already exists in our database.';
			break;
			case 'submission':
				$this->error_response = "There has been an error with your submission.\nPlease try again.";
			break;
			case 'Newsletter Signup':
				$this->error_response = "There has been an error updating your email information";
			break;
			case 'affected rows':
				$this->error_response = "Query did not match email.";
			break;
			case 'internal':
				$message = "DATE: ".date('(l) m-d-o | g:i a',time())."\n";
				$message .= "ERROR: ".mysql_error()."\n";
				$message .= "SECTION: ".$section;
				mail('youremail@you.com','ERROR FROM: '.$section,$message);
			break;
		}
	}
	
	public function __destruct(){
			
	}
}

?>
