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
	
	public function GetFighters($utils) {
		$returnArray = array();
		
		$utils->OpenConn();
		$sql = 'SELECT * FROM applicant a inner join vitals_start vs on a.idApplicant = vs.idApplicant;';
		$returnArray = $utils->Query($sql, null, true);
		
		$utils->CloseConn();
		
		return $returnArray;
	}
	
	public function GetWinner($utils) {
		$returnArray = array();
		$utils->OpenConn();
		$sql = 'SELECT MAX(c.idApplicant) AS idApplicant, MAX(a.FullName) AS FullName, COUNT(c.Win) AS Wins FROM challenges c' .
		' INNER JOIN applicant a ON c.idApplicant = a.idApplicant WHERE c.Win = 1 GROUP BY c.idApplicant;';
		
		$returnArray = $utils->Query($sql, null, true);
		
		return $returnArray;
	}
	
	public function CalculateWinner($applicantData) {
		if(is_array($applicantData)) {
			$currentWinnerScore = 0;
			$currentWinner = null;
			foreach($applicantData as $item) {
				if ($item["Wins"] > $currentWinnerScore) {
					$currentWinnerScore = $item["Wins"];
					$currentWinner = $item["FullName"];
				}
			}
			// Calculate percentage
			$percentage = ($currentWinnerScore * 100) / 9;
			return array(
					"Winner" => $currentWinner
					,"WinnerPercent" => $percentage
			);
		}
		
		return false;
	}
}