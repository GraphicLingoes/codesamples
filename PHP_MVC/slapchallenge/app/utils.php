<?
// TODO: include in index.php file and finish sanitation functions
Class utils {
	protected $servername = "localhost";
	protected $username = "root";
	protected $password = "";
	protected $dbname = "slapchallenge";
	protected $conn = null;
	
	public function __construct() {
		
	}

	public function sanitize($data = null) {
		if(is_array($data)) {

		}

	}
	
	public function OpenConn() {
		try {
		$this->conn = new PDO("mysql:host=".$this->servername.";dbname=".$this->dbname, $this->username, $this->password);
		// set the PDO error mode to exception
		$this->conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
		}
		catch (PDOException $e)
		{
			echo $sql . "<br>" . $e->getMessage(); 
		}
	}
	
	public function CloseConn() {
		$this->conn = null;
	}
	
	public function Query($sql, $params=NULL, $fetchAssoc=false) {
		try {
			if(!is_null($this->conn)) {
				if(!is_null($sql)) {
					$stmt = $this->conn->prepare($sql);
				}
				if(is_array($params)) {
					foreach($params as $key => $val) {
						$tmpArray = explode("=>", $val);
						switch(strtolower($tmpArray[1])) {
							case "int":
								$stmt->bindParam(":" . $key, $tmpArray[0], PDO::PARAM_INT);
								break;
							case "str":
							case "string":
								$stmt->bindParam(":" . $key, $tmpArray[0],  PDO::PARAM_STR);
								break;
							case "bool":
								$stmt->bindParam(":" . $key, $tmpArray[0],  PDO::PARAM_BOOL);
								break;
							case "null":
								$stmt->bindParam(":" . $key, $tmpArray[0],  PDO::PARAM_NULL);
								break;
						}
					}
				}

				if($fetchAssoc)
				{
					$stmt->execute();
					return $stmt->fetchAll();
				}
				else 
				{
					return $stmt->execute();
				}
				
				
			}
			else
			{
				throw new PDOException('DB connection is null');
			}
		}
		catch (PDOException $e)
		{
			throw new Exception($e->getMessage()); 
		}
		
	}
	
	public function LastInsertID() {
		if(!is_null($this->conn)) {
			return $this->conn->lastInsertId();
		}
	}
}