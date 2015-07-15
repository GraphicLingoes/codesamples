<?php defined('SYSPATH') or die('No direct script access.');

class Controller_Map extends Bwfl_Auth_Controller_Template
{
	public $template = 'templates/innernosidebar';
	
	public function before()
	{				
		parent::before();	
	}

	public function action_index()
	{				
		$user_routes = $this->_user->routes->find_all();
		//echo '<pre>' . print_r($user_routes, 1) . '</pre>';
		$this->template->content = View::factory('blocks/content/map')
			->set('user', $this->_user)
			->set('user_routes',$user_routes);
		
		$this->template->page_title = 'Map a Route | BeeWell For Life';
		$this->template->page_desc = $this->template->page_title;
	}
	
	public function action_save_route()
	{	
		$route_name = Arr::get($_POST, 'bwflRouteName');
		$latitude_array = Arr::get($_POST, 'lat');
		$longitude_array = Arr::get($_POST, 'lng');
		$miles = Arr::get($_POST,'bwflRouteMiles');
		
		$data = array();
		$data['user_id'] = $this->_user;;
		$data['name'] = addslashes($route_name);
		$data['miles'] = $miles;
		$data['lat'] = $latitude_array;
		$data['lng'] = $longitude_array;
		
		$this->_user->routes->saveRoute($data);
		
		$user_routes = $this->_user->routes->find_all();
		$this->template->content = View::factory('blocks/content/map')
			->set('user', $this->_user)
			->set('user_routes',$user_routes)
			->set('longitudes',$longitude_array);
		$this->template->page_title = 'Map a Route | BeeWell For Life Route Saved';
		$this->template->page_desc = $this->template->page_title;
		
	}
	
	public function action_delete_route($id)
	{
		$route = ORM::factory('bwfl_user_route', $id);
		try 
		{
			$route->delete();
		}
		catch(Exception $e)
		{
			Bwfl_Flash::write('error', $e->getMessage());
			return;
		}
		
		$user_routes = $this->_user->routes->find_all();
		$this->template->content = View::factory('blocks/content/map')
			->set('user', $this->_user)
			->set('user_routes',$user_routes);
		$this->template->page_title = 'Map a Route | BeeWell For Life Route Saved';
		$this->template->page_desc = $this->template->page_title;
		
		Bwfl_Flash::write('success', 'Success, route has been deleted.');
	}
	
	public function action_edit_route($id)
	{
		$route = ORM::factory('bwfl_user_route', $id);
		if($route->loaded())
		{
			$route_name = preg_replace('/-/',' ',$route->name);
			$route_name = Security::xss_clean($route_name);
			$coordinates = $route->coordinates->find_all();
			$coordinatesArray = array();
			foreach($coordinates as $coordinate)
			{
				$coordinatesArray[] = array($coordinate->latitude,$coordinate->longitude);
			}
			$jsonCoordinates = json_encode($coordinatesArray);
			$user_routes = $this->_user->routes->find_all();
			
			$this->template->content = View::factory('blocks/content/map')
				->set('user', $this->_user)
				->set('user_routes',$user_routes)
				->set('route_name',$route_name)
				->set('json_coordinates',$jsonCoordinates);
			$this->template->page_title = 'Map a Route | BeeWell For Life Route Saved';
			$this->template->page_desc = $this->template->page_title;
		}
		else
		{
			$this->request->redirect('/map');
		}
	}
		
} // End Map a Route
