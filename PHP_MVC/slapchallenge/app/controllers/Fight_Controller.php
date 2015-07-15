<?php
class Fight_Controller extends Base_Controller {
	public $controller = 'Fight';
	/**
	 * index action for Fight route.
	 */
	public function action_index() {
		// Grab all the contestants from the DB.
		$model = parent::getModel('Fight', 'Fight');
		$fighterData = $model->GetFighters(new utils());
		
		parent::setViewVars(array(
				"applicants" => $fighterData,
				"JSScripts" => '<script src="/js/slapchallenge.js"></script>'
		));
		
		parent::setTemplate('SlapChallengeMain');
		parent::getView($this->controller, 'Index');
		parent::render();
	}
	
	public function action_winners() {
		$model = parent::getModel('Fight', 'Fight');
		$utils = new utils();
		$winners = $model->GetWinner($utils);
		$winnersViewData = $model->CalculateWinner($winners);
		if($winnersViewData)
		{
			parent::setViewVars(array(
					"winnersViewData" => $winnersViewData
			));
		}
		
		parent::setTemplate('SlapChallengeMain');
		parent::getView($this->controller, 'Winners');
		parent::render();
	}
}