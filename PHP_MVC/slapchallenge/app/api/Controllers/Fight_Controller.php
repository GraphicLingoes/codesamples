<?php
class Fight_Controller extends Base_Controller {
	
	protected $utils = null;
	
	public function __construct() {
		$this->utils = new utils();
	}
	
	public function action_addChallenges() {
		$response = parent::$response;
		$request = parent::$request;
		$model = parent::getModel('Fight', 'Fight');
		
		if($model->AddChallenge(new utils())) 
		{
			$response->set('result', array(
					"passed" => 1
			));
		} 
		else 
		{
			$response->set('error', "1");
		}

		$response->renderResonse();
	}
}