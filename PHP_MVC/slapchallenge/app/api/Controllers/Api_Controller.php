<?
class Api_Controller extends Base_Controller {
	/**
	 * Run our request
	 *
	 * @param  string $url
	 */
	public function dispatch($url, $default_data = array())
	{
		try {
			if (!$this->router) {
				throw new Exception("Router not set");
			}

			$route = $this->router->getRoute($url);

			$controller = ucfirst($route['controller']);
			$action = ucfirst($route['action']);

			unset($route['controller']);
			unset($route['action']);

			// Get our model
			$model = $this->getModel($controller);

			$data = $model->{$action}($route);
			$data = $data + $default_data;

			// Get our view
			$view = $this->getView($controller, $action);

			echo $view->render($data);


		} catch (Exception $e) {
			try {
				if ($url != '/error') {
					$data = array('message' => $e->getMessage());
					$this->dispatch("/error", $data);
				} else {
					throw new Exception("Error Route undefined");
				}
			} catch (Exception $e) {
				echo "<h1>An unknown error occurred.</h1>";
			}

		}
	}
}