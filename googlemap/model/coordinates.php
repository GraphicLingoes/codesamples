<?php defined('SYSPATH') or die('No direct script access.');
class Model_BWFL_Coordinates extends ORM
{
	protected $_table_name = 'user_route_coordinates';
	
	protected $_belongs_to = array(
		'coordinates' => array(
			'model' => 'bwfl_routes',
			'foreign_key' => 'route_id',			
		),
	);
	
	public function rules()
	{
		return array(
			'route_id' => array(
				array('not_empty'),
				array('numeric'),
			),
			'latitude' => array(
				array('regex', array(':value', '/^[0-9\.\-]+$/')),
			),
			'longitude' => array(
				array('regex', array(':value', '/^[0-9\.\-]+$/')),
			),
		);
	}
	
	public static function saveCoordinates($lat_data,$lng_data,$routeID)
	{
		// Check if more than one set of coordinates exists
		if ( is_array($lat_data) )
		{
			
			// Determine size to iterate through lat and lng values to insert on each iteration
			$lat_size = sizeof($lat_data);
			for( $i = 0; $i < $lat_size; $i++)
			{
				$insert = DB::insert('user_route_coordinates')
						->columns(array('route_id','latitude','longitude'))
						->values(array($routeID,$lat_data[$i],$lng_data[$i]));
				$insert->execute();
			}
			
		}
		else // There are not more than one lat and lng set
		{
			$insert = DB::insert('user_route_coordinates')
						->columns(array('route_id','latitude','longitude'))
						->values(array($routeID,$lat_data[$i],$lng_data[$i]));
			$insert->execute();
		}
	}
}