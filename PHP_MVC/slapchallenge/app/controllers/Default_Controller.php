<?
class Default_Controller extends Base_Controller {
	private $controller = 'Default';
	/**
	 * Run our request
	 *
	 * @param  string $url
	 */
	public function action_index()
	{
		try {
			if (!parent::$router) {
				throw new Exception("Router not set");
			}
			// Get our view
			parent::setViewVars(array(
					"testVariable" => "set from dispatch",
					"innerArray" => array("testing" => "Inner Array", "anotherIndex" => "Testing")
				));
			parent::setTemplate('SlapChallengeMain');
			parent::getView($this->controller, 'Index');
			parent::render();

		} catch (Exception $e) {
			

		}
	}
}