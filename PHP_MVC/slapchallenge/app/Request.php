<?
class Request {
	/**
	 * [$dataStore private member used to store request data]
	 * @var array
	 */
	private $dataStore = array();
	/**
	 * [add description]
	 * @param [type] $key   [description]
	 * @param [type] $value [description]
	 */
	public function set ($key, $value=null) 
	{
		if(is_array($key))
		{
			foreach($key as $k=>$val)
			{
				$this->dataStore[$k] = $val;
			}
		} 
		else 
		{
			$this->dataStore[$key] = $value;
		}
	}
	/**
	 * [get description]
	 * @param  [type] $key [description]
	 * @return [type]      [description]
	 */
	public function get ($key)
	{
		return $this->dataStore[$key];
	}
	/**
	 * [remove description]
	 * @param  [type] $key [description]
	 * @return [type]      [description]
	 */
	public function remove ($key) 
	{
		if(is_array($key))
		{
			foreach($key as $k)
			{
				unset($this->dataStore[$k]);
			}
		}
		else
		{
			unset($this->dataStore[$key]);
		}
	}

}