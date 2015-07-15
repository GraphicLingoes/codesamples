BaseApp = function () {
    /// <summary>
    /// Main init method used to set up dependent objects nested in the ba app object.
    /// </summary>
    var init = function () {
        ba = {};
        ba.config = ba.config || {};
        ba.ds = ba.ds || {};
        ba.mods = ba.mods || {};
        ba.models = ba.models || {};
        ba.requireConfig = {
            loaded: {},
            callbackQueue: {},
            thirdPartyReady: false
        };
        ba.runApp = BaseApp.runApp;
    };
    /// <summary>
    /// _require method used to delay loading of objects based on a list
    /// of that object's dependencies which are passed in using the "objectParams" parameter.
    /// <code>
    /// BaseApp._require([
    ///     { from: "fileName.js" },
    ///     { objects: ["stateManager", "utils", "formValidator"] }
    /// ], function () {
    ///     ba.mods.MyModule = function () {
    ///         var myMethod = function () {
    ///             // do stuff
    ///         };
    /// 
    ///         return { 
    ///             myMethod: myMethod 
    ///         };
    ///     }();
    /// });
    /// </code>
    /// </summary>
    /// <param name="objectParams" type="object"></param>
    /// <param name="requireCallback" type="function"></param>
    var _require = function (objectParams, requireCallback) {
        // Make sure objectParams is an object
        if (typeof objectParams === "object") {
            var _from = (typeof objectParams[0].from != "undefined") ? objectParams[0].from : false;
            var _objects = (typeof objectParams[1].objects != "undefined") ? objectParams[1].objects : false;
            // See what is loaded
            _checkWhatIsLoaded(_objects, function () {
                // If not all laoded add current _require call to queue
                if (!_allLoaded(_from, _objects)) {
                    _setQueue(_from, _objects, requireCallback);
                    // Run queue in case something has loaded
                    _runQueue();
                    // Return out of _require method to release it's execution
                    // for the next method.
                    return;
                }
                // Assuming everything is loaded run callback to load object right away.
                requireCallback();
                // Check queue again and run it.
                // TODO: Determine if we should just run it.
                if (_queuePending()) {
                    _runQueue();
                }
            });    
        }

    };
    /// <summary>
    /// _allLoaded method used to poll ba.requireConfig.loaded object
    /// to see if current object's dependent files are loaded.
    /// </summary>
    /// <param name="from" type="string"></param>
    /// <param name="compareObjects" type="object"></param>
    /// <returns type="boolean"></returns>
    var _allLoaded = function (from, compareObjects) {
        var retVal = true;
        var fileName;
        var _compareObjects = compareObjects || false;
        if (compareObjects !== false) { 
            for (var k in ba.requireConfig["loaded"]) {
                if (ba.requireConfig["loaded"][k] === false) {
                    // Dynamically build file name based on k
                    fileName = k + ".js";
                    // Only compare against values that are required for current object
                    // being loaded.
                    if (_compareObjects.indexOf(k) != -1) {
                        // Check to see if loaded
                        if (fileName != from) {
                            retVal = false;
                        }
                    }
                    // Check if thridparty object
                    // NOTE: START: Depreciated code, currently not being hit.
                     if (typeof _compareObjects[0]["thirdPartyObj"] !== "undefined") {
                        var thirdPartyName = _compareObjects[0].thirdPartyObj + "_" + _compareObjects[0].propertyName;
                        if (ba.requireConfig["loaded"][thirdPartyName] === false) {
                            retVal = false;
                        }
                     }
                    // END: Depreciated code
                }
            }
        }
        return retVal;
    };
    /// <summary>
    /// _checkWhatIsLoaded method used to poll a list of dependent objects
    /// to see if they have been loaded. If they have been loaded a boolean flag
    /// will be set in the ba.requireConfig.loaded object for the current object.
    /// The ba.requireConfig.loaded objects is later polled to see if dependent objects
    /// are loaded before running callback method.
    /// </summary>
    /// <param name="dependentObjectList" type=""></param>
    /// <param name="callback" type="function"></param>
    var _checkWhatIsLoaded = function (dependentObjectList, callback) {
        // Iterate through object collection to see if dependent objects exist in the global scope yet.
        for (var i = 0; i < dependentObjectList.length; i++) {
            // Third party object check
            var _thirdPartyObj = dependentObjectList[i].thirdPartyObj || false;
            // If not third party object do normal checks
            if (!_thirdPartyObj) {
                // Check to see if object is nested inside another object.
                // We do this by checking for the existence of a dot "." in the name.
                if (dependentObjectList[i].indexOf(".") != -1) {
                    // If nested object separate the name to drill into and see if it is loaded.
                    var separateObjs = dependentObjectList[i].split(".");
                    var subObjectLoaded = _checkNestedObj(separateObjs);
                    // If object is not loaded yet, recurse back into _require
                    // and check again after 100 miliseconds.
                    if (!subObjectLoaded) {
                        ba.requireConfig["loaded"][separateObjs.join(".")] = false;
                    } else {
                        ba.requireConfig["loaded"][separateObjs.join(".")] = true;
                    }
                } else {
                    // Otherwise object is not nested so we can check for it at the top level
                    // of the ba object.
                    var objectLoaded = ba[dependentObjectList[i]] || false;
                    // If object is not loaded yet, recurse back into _require 
                    // and check again after 100 miliseconds.
                    if (!objectLoaded) {
                        ba.requireConfig["loaded"][dependentObjectList[i]] = false;
                    } else {
                        ba.requireConfig["loaded"][dependentObjectList[i]] = true;
                    }
                }
            } else {
                // If object is a thirdparty object we check here to see if it is loaded.
                // NOTE: this is depreciated and not currently used. Leaving it in for now.
                var propertyName = dependentObjectList[i].propertyName;
                var thirdPartyObjectLoaded = _evalThirdPartyObject(dependentObjectList[i]);
                if (!thirdPartyObjectLoaded || thirdPartyObjectLoaded.length <= 0) {
                    ba.requireConfig["loaded"][dependentObjectList[i].thirdPartyObj + "_" + propertyName] = false;
                } else {
                    ba.requireConfig["loaded"][dependentObjectList[i].thirdPartyObj + "_" + propertyName] = true;
                }
            } // End Thirdparty if / else
        } // End Loop

        callback();
    };
    /// <summary>
    /// _queuePending method used to check if there are current objects
    /// waiting to be loaded.
    /// </summary>
    /// <returns type="boolean"></returns>
    var _queuePending = function () {
        var pendingQueue = ba.requireConfig.callbackQueue.toString();
        if (pendingQueue.length > 0) {
            return true;
        }
        return false;
    };
    /// <summary>
    /// _setQueue method used to store callback methods used to laod objects
    /// and their dependencies due to not all of those dependencies being loaded
    /// the first time around.
    /// </summary>
    /// <param name="index" type="string"></param>
    /// <param name="dependencies" type="object"></param>
    /// <param name="callbackFunction" type="function"></param>
    var _setQueue = function (index, dependencies, callbackFunction) {

        ba.requireConfig.callbackQueue[index] = {
            objects: dependencies,
            callback: callbackFunction
        };
    };
    /// <summary>
    /// _runQueue method used to iterate through current required objects queue.
    /// If there are objects that have been queued because all of their dependencis have
    /// not loaded yet they will be loaded assuming dependent files now exist.
    /// Once they are loaded they are deleted from queue.
    /// </summary>
    var _runQueue = function () {
        for (var k in ba.requireConfig.callbackQueue) {
            _checkWhatIsLoaded(ba.requireConfig.callbackQueue[k].objects, function () {
                if (!_allLoaded(k, ba.requireConfig.callbackQueue[k].objects)) {
                    return;
                }
                // Run callback to load object
                ba.requireConfig.callbackQueue[k].callback();
                // Remove object from queue
                delete ba.requireConfig.callbackQueue[k];
            });
        }
    }
    /// <summary>
    /// _checkNestedObj method is used to check objects nested more than
    /// one level deep have loaded such as ba.ds.defaultMod vs ba.defaultMod.
    /// </summary>
    /// <param name="separatedObjs" type=""></param>
    /// <returns type="mixed">Returns a boolean or object</returns>
    var _checkNestedObj = function (separatedObjs) {
        var retVal = false;
        var _separatedObjs = separatedObjs;
        switch (_separatedObjs.length) {
            case 2:
                retVal = ba[_separatedObjs[0]][_separatedObjs[1]] || false;
                break;
            case 3:
                retVal = ba[_separatedObjs[0]][_separatedObjs[1]][_separatedObjs[2]] || false;
                break;
            case 4:
                retVal = ba[_separatedObjs[0]][_separatedObjs[1]][_separatedObjs[2]][_separatedObjs[3]] || false;
                break;
            case 5:
                retVal = ba[_separatedObjs[0]][_separatedObjs[1]][_separatedObjs[2]][_separatedObjs[3]][_separatedObjs[4]] || false;
                break;
                //NOTE: Currently we only support 5 levels nested.
        }

        return retVal;
    };
    /// <summary>
    /// Method used to see if third party objects are loaded in the window object.
    /// </summary>
    /// <param name="thirdPartyObject" type=""></param>
    /// <returns type="mixed">Returns boolean or object</returns>
    var _evalThirdPartyObject = function (thirdPartyObject) {
        var retVal = true;
        var propertyName = thirdPartyObject.propertyName;
        retVal = window[thirdPartyObject.thirdPartyObj][propertyName] || false;
        if (retVal === false) {
            if (typeof thirdPartyObject.initMethod !== "undefined") {
                if (typeof window[thirdPartyObject.thirdPartyObj] !== "undefined") {
                    thirdPartyObject.initMethod();
                    retVal = window[thirdPartyObject.thirdPartyObj][propertyName] || false;
                }
            }
        }

        return retVal;
    }
    /// <summary>
    /// the runApp method is used to run the inital Sammy.js route. It polls a
    /// boolean flag in the ba object to see if all third party objects have loaded.
    /// If not setInterval is used to create a interval timer to continue to poll
    /// until all third party objects are loaded so app can successfully run route.
    /// </summary>
    /// <param name="route" type="string"></param>
    var runApp = function (route) {
        if (ba.requireConfig.thirdPartyReady) {
            baRouter.run(route);
        } else {
            var initAppInterval = setInterval(function () {
                if (ba.requireConfig.thirdPartyReady) {
                    clearInterval(initAppInterval);
                    baRouter.run(route);
                }
            }, 100);
        }
    }

    return {
        init: init,
        _require: _require,
        runApp: runApp
    };
}();
// Init the app to set up dependent objects for Sammy.js routes
BaseApp.init();