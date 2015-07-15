// Handles all form validation
wf.formValidator = function () {
    /**
    * private object _regexObj local store for validation regexs
    * 
    * @access private
    */
    var _regexObj = {
        space: / /,
        any: /[^/]/,
        int: /^[\d]+$/,
        alpha: /^[a-zA-Z\._\-\s]+$/,
        alphanumeric: /^[a-zA-Z\d\._\-\s]+$/,
        password: /^[^{}<>';]+$/,
        phoneNumber: /^\([\d]{3}\)[\d]{3}-[\d]{4}$|^[\d]{3}.[\d]{3}.[\d]{4}$|^[\d]{3}-[\d]{3}-[\d]{4}$/,
        email: /^(([^<>\{\}\[\]\\.,;:\s@\"]+(\.[^<>\{\}\[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[\d]{1,3}\.[\d]{1,3}\.[\d]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-\d]+\.)+[a-zA-Z]{2,}))$/,
        emailOrPhone: /^(([^<>\{\}\[\]\\.,;:\s@\"]+(\.[^<>\{\}\[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[\d]{1,3}\.[\d]{1,3}\.[\d]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-\d]+\.)+[a-zA-Z]{2,}))$|(^[\d]+$)|^\([\d]{3}\)\s*[\d]{3}-[\d]{4}$|^[\d]{3}.[\d]{3}.[\d]{4}$|^[\d]{3}-[\d]{3}-[\d]{4}$/
    };

    /**
    * validate method used for core validation
    *
    * @param array params
    *   @subparam bool [req] Optional property, set to true if field is required
    *   @subparam string field Name of input field to validate
    *   @subparam string [type] Optional predefined REGEX to be used in validation
    *   @subparam regex [regex] Optional custom regex to be used to validate against
    * @access public
    */
    var validate = function (params) {
       
        var _passed = [];
        //for (var k in params) {
        //    for (var key in params[k]) {
        //        console.log(key + ' => ' + params[k][key]);
        //    }
        //}
        // check to make sure parameters are set
        if (typeof params == "object") {
            resetErrorText();
            var paramsLength = params.length;
            var REGEX, fieldVal, _req, regex, setErrTxt;
            // loop through parameters to validate each field
            for (var i = 0; i < paramsLength; i++) {
                // check if field is required before doing anything
                regex = params[i].regex || false;
                _req = params[i].req || false;
                if (params[i].testValue) {
                    fieldVal = params[i].value;
                    setErrTxt = false;
                } else {
                    fieldVal = $("#" + params[i].field).val();
                    setErrTxt = true;
                }
                if ( _req ) { 
                    // if field blank return false
                    if ((!fieldVal || fieldVal == "") && fieldVal !== undefined) {
                        _passed[i] = false;
                        if (setErrTxt) setErrorText(params[i].field, "field cannot be blank");
                        continue;
                    }
                }
                if (fieldVal) {
                    // set correct regex and validate field
                    REGEX = !regex ? _getREGEX(params[i].type) : regex;
                    if (!REGEX.test(fieldVal)) {
                        _passed[i] = false;
                        if (setErrTxt) setErrorText(params[i].field, $$._("error_" + params[i].field));
                        continue;
                    }
                }
                // if we make it this far field has validated
                _passed[i] = true;
            }
            
            // loop through passed array to see if anything failed so we can let the form know
            var _passedLength = _passed.length
            for (var k = 0; k < _passedLength; k++) {
                if (!_passed[k]) {
                    return false;
                }
            }
            // if we make it this far everything has validated
            return true;
        }
        // something went wrong return false.
        return false;
    };

    /**
    * _getREGEX method used to retreive pre-defined regex
    * 
    * @param string type Any of the pre-defined options: alpha, alphaNum, int, any, email
    * @access private
    */
    var _getREGEX = function (type) {
        var returnREGEX;
        switch (type) {
            case 'alpha':
                returnREGEX =  _regexObj.alpha;
            case 'alphaNum':
                returnREGEX =  _regexObj.alphanumeric;
                break;
            case 'int':
                returnREGEX =   _regexObj.int;
                break;
            case 'any':
                returnREGEX =  _regexObj.any;
                break;
            case 'password':
                returnREGEX = _regexObj.password;
                break;
            case 'email':
                returnREGEX =  _regexObj.email;
                break;
            case 'emailOrPhone':
                returnREGEX = _regexObj.emailOrPhone;
                break;
        }

        return returnREGEX;
    };

    /**
    * setErrorText method used to inject error message into the DOM next to input field with error
    * 
    * @param mixed id Identifier of DOM element to inject error message
    * @param string message Error message to be used
    * @access public
    */
    var setErrorText = function (id, message) {
        var domElement = $("span[id='e." + id + "']");
        $(domElement).text(message);
    };
    /**
    * resetErrorText method used to clear out old error messages
    *
    * @access public
    */
    var resetErrorText = function () {
        $('[id^="e."]').html("");
        $('[id*=" e."]').html("");
    }

    return {validate:validate, setErrorText:setErrorText, resetErrorText: resetErrorText};
}();