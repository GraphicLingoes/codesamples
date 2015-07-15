<?
Abstract class Routes {
	/**
	 * Protected array of all possible routes
	 *
	 * @var  array $routes
	 */
	static protected $routes = array(
			array('/', array('controller' => 'default', 'action' => 'index', 'view' => 'Default')),
			array('/api/alpha:page/alpha:action/:id', array('controller' => 'api')),
			// This is the main api route, keep it simple api/controllerName/actionName
			array('/api/regex:(?P<controller>([a-z]+?))/alpha:action'),
			array('/regex:(?P<controller>([a-z]+?))/alpha:action'),
			array('/regex:(?P<api>([a-z]+?))-(?P<api_controller>([a-z]+?))/alpha:page/alpha:action/:id'),
			array('/regex:(?P<api>([a-z]+?))-(?P<api_controller>([a-z]+?))/alpha:page/alpha:action/:id/:idFoe'),
			array('/alpha:controller/alpha:action')
		);

	/**
	 * Public static method for iterating through routes to add them
	 * 
	 * @param Router $router
	 */
	static public function addRoutes(RouterRegex $router)
	{
		if ($router instanceof RouterRegex) {
			if(!empty(self::$routes)) {
				foreach(self::$routes as $route) {
					if(isset($route[1])) {
						$router->addRoute($route[0],$route[1]);	
					} else {
						$router->addRoute($route[0]);
					}
				}
			}
		}
	}
}