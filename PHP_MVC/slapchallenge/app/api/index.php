<?
// Set App Path
$appPath = '../../app';
$apiPath = $appPath . '/api';

function showMe($showMeArray) {
	echo '<pre>' . print_r($showMeArray, 1) . '</pre>';
}

// Define constants
define('API_DEBUG', FALSE);
define('API_ROUTER', 'RouterRegex');
define('API_ROUTES', 'Routes');
define('API_REGISTRY', 'Registry');
define('API_REQUEST', 'Request');
define('API_VIEWS_PATH', $apiPath . DIRECTORY_SEPARATOR . 'views');
define('API_CONTROLLERS_PATH', $apiPath . DIRECTORY_SEPARATOR . 'controllers');
define('API_MODELS_PATH', $apiPath . DIRECTORY_SEPARATOR . 'models');
define('API_USER_STYLESHEETS', $appPath . DIRECTORY_SEPARATOR . 'user_content' . DIRECTORY_SEPARATOR . 'css');
define('API_RESPONSE', 'Response');
define('API_UTILS', 'utils');
define('SYS_VIEWS', $apiPath . '/views');
define('SYS_TEMPLATES', SYS_VIEWS . '/Templates');

// Include necessary app files
include_once($appPath . DIRECTORY_SEPARATOR . "Debugger.php");
include_once($appPath . DIRECTORY_SEPARATOR . API_ROUTER . ".php");
include_once($appPath . DIRECTORY_SEPARATOR . API_ROUTES . ".php");
include_once($appPath . DIRECTORY_SEPARATOR . API_REGISTRY . ".php");
include_once($appPath . DIRECTORY_SEPARATOR . API_REQUEST . ".php");
include_once($appPath . DIRECTORY_SEPARATOR . API_RESPONSE . ".php");
include_once($appPath . DIRECTORY_SEPARATOR . API_UTILS . ".php");
// Set Debugger
Debugger::setMode(API_DEBUG);
// Instantiate Router and set routes
$routerName = API_ROUTER;
$routes = API_ROUTES;
$router = new $routerName;
$responseName = API_RESPONSE;
$response = new $responseName;
$routes::addRoutes($router);
$url = $_SERVER['REQUEST_URI'];
$routeData = $router->getRoute($url);
$file = API_CONTROLLERS_PATH . DIRECTORY_SEPARATOR . ucfirst($routeData['controller']) . "_" . "Controller.php";
$className = ucfirst($routeData['controller']) . "_" . "Controller";
$modelClassName = ucfirst($routeData['controller']);
$action = 'action_' . $routeData['action'];
$controller = $routeData['controller'];

if (!file_exists($file)) {
	header('HTTP/1.1 404 Not Found');
	throw new Exception("Class not found!{$file}");
} else {
	require_once API_CONTROLLERS_PATH . DIRECTORY_SEPARATOR . 'Base_Controller.php';
	require_once $file;
	$controller = new $className();
	$controller::setRouter($router);
	$controller::setResponse($response);
	$postData = json_decode(file_get_contents('php://input'), true);
	$request = new Request();
	if(isset($postData))
	{
		$request->set($postData);
		$controller::setRequestVars($request);
	}
	else
	{
		$request->set("");
		$controller::setRequestVars($request);
	}

	if(method_exists($controller, $action))
	{
		$controller->$action();
	}
	else
	{
		throw new Exception("{$action} method does not exist");
	}
}

$request_method = $_SERVER['REQUEST_METHOD']; // TODO: implement verb for RESTful services.


// Create reference to registry as a global store for persistant data
$registry = API_REGISTRY; 


// Clean up necessary variables for next request
unset($appPath);
unset($apiPath);
unset($routerName);
unset($routes);