<?php
class FightModel {
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
	
	public function AddChallenge($utils) {
		try {
			$utils->OpenConn();
			
			$params = array(
					"aId" => $this->request->get("idApplicant") . "=>int"
					,"fId" => $this->request->get("idFoe") . "=>int"
					,"W" => $this->request->get("Win") . "=>int"
					,"L" => $this->request->get("Loss") . "=>int"
			);
			$stmtChallenges = 'INSERT INTO challenges (idApplicant, foeId, Win, Loss) VALUES (:aId, :fId, :W, :L);';
			$utils->Query($stmtChallenges, $params);
			
			$utils->CloseConn();
		}
		catch (Exception $e)
		{
			return false;
		}
		
		return true;
	}
}