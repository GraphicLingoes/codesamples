<?
Class Registry {
	/**
	 * @var array The store for all of our objects
	 */
	static private $_store = array();

	/**
	 * Add an object to the object $_store, if a name is not provided the class name will be used
	 * 
	 * @param mixed $object
	 * @param string $name
	 * @return mixed If overwriting an object the previous object will be returned
	 * @throws Exception
	 */
	static public function add($object, $name = null) {
		// Use class name if no name is provided
		$name = (!is_null($name)) ?: get_class($object);
		$name = strtolower($name);

		$return = null;
		if (isset(self::$_store[$name])) {
			$return = self::$_store[$name];
		}

		self::$_store[$name] = $object;
		return $return;
	}

	/**
	 * Get an object from the registry
	 * @param  string $name  Object name or name if provided in add method
	 * @return mixed
	 * @throws Exception
	 */
	static public function get($name) {
		if (!isset(self::$_store[$name])) {
			throw new Exception("Object does not exists in the registry!");
		}

		return self::$_store[$name];
	}

	/**
	 * Check to see if object is stored in the registry
	 * @param  string $name Object name or name if provided in add method
	 * @return bool
	 */
	static public function contains($name)
	{
		if (!isset(self::$_store[$name])) {
			return false;
		}

		return true;
	}

	/**
	 * Remove an object from the registry
	 * @param  string $name Object name or name if provided in the add method
	 * @return void
	 */
	static public function remove($name)
	{
		if (isset(sefl::$_store[$name])) {
			unset(self::$_store[$name]);
		}
	}
}