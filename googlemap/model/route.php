<?php defined('SYSPATH') or die('No direct script access.');
class Model_BWFL_User_Route extends ORM
{
	protected $_table_name = 'user_routes';
	
	protected $_belongs_to = array(
		'routes' => array(
			'model' => 'bwfl_user',
			'foreign_key' => 'user_id',			
		),
	);
	
	protected $_has_many = array(
		'coordinates' => array(
			'model' => 'bwfl_coordinates',
			'foreign_key' => 'route_id'
		),
	);
	
	public function rules()
	{
		return array(
			'user_id' => array(
				array('not_empty'),
				array('regex', array(':value', '/^[0-9]+$/')),
			),
			'name' => array(
				array('not_empty'),
				array('max_length', array(':value', 22)),
				array('regex', array(':value', '/^[0-9a-zA-Z\-\.\!\\\@\#\$\%\^\&\*\(\)\']+$/')),
			),
			'miles' => array(
				array('not_empty'),
				array('regex', array(':value', '/^[0-9.]+$/')),
			),
			'date_created' => array(
				array('not_empty'),
				array('regex', array(':value', '/^[0-9\-]+$/')),
			),
		);
	}
	
	public static function saveRoute($data)
	{
		/**
			Load ORM object for current user and check routes
			If a route with the current name exists delete and
			add again, otherwise just add it
		 */
		$uid = $data['user_id'];
		$route_name = preg_replace('/\s/','-',$data['name']);
		$check_name_sql = "SELECT id FROM user_routes WHERE name=:name AND user_id=:id";
		$result = DB::query(Database::SELECT, $check_name_sql)
				->param(':name', $route_name)
				->param(':id', $uid)
				->execute();
		if($result->offsetExists(0))
		{
			foreach($result as $row)
			{
				$delete_routeID = $row['id'];
			}
			$deleteSQL = "DELETE FROM user_routes WHERE id = " . $delete_routeID . " AND user_id = " . $uid;
			$delete = DB::query('delete', $deleteSQL)->execute();
		}
		
		try{
			$route = ORM::factory('bwfl_user_route');
			$route->miles = $data['miles'];
			$route->name = $route_name;
			$route->user_id = $data['user_id'];
			$route->date_created = date('Y-m-d',time());
			$route->save();
			
			$routeID = $route->pk();
			$coordinates = ORM::factory('bwfl_coordinates');
		
			if (isset($data['lat']) && isset($data['lng']))
			{
				if ( is_array($data['lat']) && is_array($data['lng']) )
				{	
					$coordinates->saveCoordinates($data['lat'],$data['lng'],$routeID);
				} 
			}
		}
		catch(ORM_Validation_Exception $e)
		{
			Bwfl_Flash::write('error', $e->errors('model'));
			return;
		}
		catch(Exception $e)
		{
			Bwfl_Flash::write('error', $e->getMessage());
			return;
		}
		$success_clean_name = Security::xss_clean(stripslashes($data['name']));
		Bwfl_Flash::write('success', 'Your '. strip_tags($data['miles']) . ' mile route "'. $success_clean_name . '" has been saved. <br /> Go back to the <a href="/">Dashboard</a> to log your miles.');
		//return $routeID;
	}
}