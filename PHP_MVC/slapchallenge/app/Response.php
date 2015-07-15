<?
Class Response {
	/**
	 * [$responseStore private dataStore for response object]
	 * @var boolean
	 */
	private $responseStore = false;
	/**
	 * [$headers commone HTTP headers]
	 * @var array
	 */
	private $headers = array(
			"200" => "OK",
			"201" => "Created",
			"204" => "No Content",
			"301" => "Moved",
			"302" => "Found",
			"304" => "Not Modified",
			"307" => "Temporary Redirect",
			"400" => "Bad Request",
			"401" => "Not Authorized",
			"403" => "Forbidden",
			"404" => "Not Found",
			"406" => "Not Acceptable",
			"500" => "Internal Server Error",
			"503" => "Service Unavailable"
		);
	/**
	 * [__construct description]
	 */
	public function __construct() {
		$this->responseStore = array(
			"errCode" => 0,
			"result" => array()
			);
	}
	/**
	 * [set description]
	 * @param [type] $key [description]
	 * @param [type] $val [description]
	 */
	public function set($key, $val)
	{
		$this->responseStore[$key] = $val;
	}
	/**
	 * [renderResonse description]
	 * @return [type] [description]
	 */
	public function renderResonse() {
		echo json_encode($this->responseStore);
	}

}