// Global error handler
wf.noticeHandler = function () {
    var _errorClass = "error";
    var _warningClass = "warning";
    var _successClass = "success";
    var _targetDiv = "#noticeDiv";
    var _currentClass = "";
    var _divState = "off";

    /**
    * fire method used to inject error message into DOM element
    *
    * @param mixed message If response error pass in response object for debug info
    * @param string type ("error", "warning" or "success")
    * @param string [call] Optional WipitFETCH.dataservice used
    * @param string [div] Optional parameter if default error div should not be used
    * @access public
    */
    fire = function (message, type, call, div) {
        var targetDiv = div || _targetDiv;
        var divClass = "";
        var _type = type || 'error';
        switch (_type) {
            case 'error':
                divClass = _errorClass;
                break;
            case 'warning':
                divClass = _warningClass;
                break;
            case 'success':
            default:
                divClass = _successClass;
                break;
        }
        show(targetDiv, divClass);
        render(message, targetDiv, call);
    };

    /**
    * show method used to show error div
    *
    * @param string div
    * @param string type Used to set which type of notice is showing
    * @access public
    */
    show = function (div, type) {
        var cssClass = _currentClass || false;
        $(div).show();
        if (cssClass) {
            $(div).removeClass(cssClass);
        }
        $(div).addClass(type);
        _divState = type;
        _currentClass = type;
    };

    /**
    * hide method used to hide error div
    * 
    * @access public
    */
    hide = function () {
        if (_divState != "off") {
            $(_targetDiv).hide();
            _divState = "off";
        }
    };

    /**
    * render method used for constructing notice messages and injecting into target div
    *
    * @param mixed data If error from web service pass in response object for debug info
    * @param string div Target div
    * @param [call] Optional WipitFETCH.dataservice method
    * @access public
    */
    render = function (data, div, call) {
        var closeBtn = '<div id="noticeCloseX"><a href="" name="closeNotice" onclick="event.preventDefault();event.stopPropagation();$(\'' + div + '\').hide();"><i class="icon-remove"></i></a></div>';
        var messageHtml;
        var _call = call || false;
        // if data is an object it is an error so construct error message
        if (typeof data == "object") {
            messageHtml = '<div id="noticeCloseX"><a href="" name="closeNotice" onclick="event.preventDefault();event.stopPropagation();$(\''+div+'\').hide();"><i class="icon-remove"></i></a></div>';
            messageHtml += _call ? "<b>error in " + _call + "</b><br />" : "";
            messageHtml += "<b>errCode:</b> " + data.errCode + "<br />";
            messageHtml += "<b>error:</b> " + data.error + "<br />";
            messageHtml += "<b>errorDebug:</b> " + data.errorDebug + "<br />";
            $(div).html(messageHtml);
            return;
        }
        // otherwise just render message
        $(div).html(closeBtn + data);
    }

    return { fire:fire, show:show, hide:hide, render:render };
}();