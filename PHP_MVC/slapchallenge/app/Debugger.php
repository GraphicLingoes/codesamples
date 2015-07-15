<?
Abstract class Debugger {
	/**
	 * TODO: Finish this class.
	 */
	/**
	 * @var string $enabled
	 */
	static protected $enabled = false;

	/**
	 * Static public method to set dubugger mode.
	 * 
	 * @param boolean $mode
	 */
	static public function setMode($mode = false)
	{
		self::$enabled = $mode;
	}

	static public function debug($debuggerInfo = array())
	{
		if (self::$enabled) {
			if(isset($debuggerInfo['method'])) {
				switch ($debuggerInfo['method']) {
					case 'try-catch':
						echo '<pre>' . print_r($debuggerInfo, 1) . '</pre>';
						echo 'made it to try-catch 1';
						self::tryCatch($debuggerInfo);
						break;
					
					default:
						# code...
						break;
				}
			}
		}
	}

	static public function tryCatch($code) {
		try {
			$code = $debuggerInfo['code'];
			$code($debuggerInfo['params']);
		} catch (Exception $e) {
			echo 'ERROR: ' . $e->getMessage();
		}
	}
}