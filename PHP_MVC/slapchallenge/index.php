<?
function showMe($showMeArray) {
	echo '<pre>' . print_r($showMeArray, 1) . '</pre>';
}
// Main index
// Set App Path
$appPath = 'app';

// Define constants
define('SYS_DEBUG', FALSE);
define('SYS_ROUTER', 'RouterRegex');
define('SYS_ROUTES', 'Routes');
define('SYS_REGISTRY', 'Registry');
define('SYS_CONTROLLERS', $appPath . '/controllers');
define('SYS_MODELS', $appPath . '/models');
define('SYS_VIEWS', $appPath . '/views');
define('SYS_TEMPLATES', SYS_VIEWS . '/Templates');
define('SYS_REQUEST', 'Request');
define('SYS_RESPONSE', 'Response');
define('SYS_UTILS', 'utils');
//define('SYS_DEFAULT_CONTROLLER', SYS_CONTROLLERS . '/Default_Controller.php');

// Include necessary app files
include_once($appPath . DIRECTORY_SEPARATOR . "Debugger.php");
include_once($appPath . DIRECTORY_SEPARATOR . SYS_ROUTER . ".php");
include_once($appPath . DIRECTORY_SEPARATOR . SYS_ROUTES . ".php");
include_once($appPath . DIRECTORY_SEPARATOR . SYS_REGISTRY . ".php");
include_once($appPath . DIRECTORY_SEPARATOR . SYS_REQUEST . ".php");
include_once($appPath . DIRECTORY_SEPARATOR . SYS_RESPONSE . ".php");
include_once($appPath . DIRECTORY_SEPARATOR . SYS_UTILS . ".php");
// Set Debugger
Debugger::setMode(SYS_DEBUG);

// Instantiate Router and set routes
$routerName = SYS_ROUTER;
$routes = SYS_ROUTES;
$router = new $routerName;
$routes::addRoutes($router);
$url = $_SERVER['REQUEST_URI'];
$routeData = $router->getRoute($url);
$responseName = SYS_RESPONSE;
$response = new $responseName;
$file = SYS_CONTROLLERS . DIRECTORY_SEPARATOR . ucfirst($routeData['controller']) . "_" . "Controller.php";
$className = ucfirst($routeData['controller']) . "_" . "Controller";
$modelClassName = ucfirst($routeData['controller']);
$action = 'action_' . $routeData['action'];
$controller = $routeData['controller'];

if (!file_exists($file)) {
	header('HTTP/1.1 404 Not Found');
	throw new Exception("Class not found!{$file}");
} else {
	require_once SYS_CONTROLLERS . DIRECTORY_SEPARATOR . 'Base_Controller.php';
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

$request_method = $_SERVER['REQUEST_METHOD']; // TODO: user request verb for api calls.

// Create reference to registry as a global store for persistant data
$registry = SYS_REGISTRY;

// Clean up necessary variables for next request
unset($appPath);
unset($apiPath);
unset($routerName);
unset($routes);
?>