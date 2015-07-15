wf.request = function () {
    /**
    * [makeRequest method used to make jQuery ajax requests]
    *
    * @param {object} [params] [Ajax request object parameters]
    */
    var makeRequest = function (params) {
        // Set up reqeust parameters and make request
        if (typeof (params) == "object") {
            if (!params.skipSentry) {
                // perform sentryCheck to see if user is still authenticated
                if (wf.utils.sentryCheck()) {
                    return;
                }
            }
            var url = params.url || "";
            var async = params.async === false ? false : true;
            var dataType = params.dataType || "json";
            var contentType = params.contentType || "application/json; charset=utf-8";
            var type = params.type || "POST";
            var data = params.data || {};
            var success = params.success || function (data) { };
            var defaultErrCallback = function (xhr, errorStatus, error) {
                wf.noticeHandler.fire("failed: " + errorStatus + " " + error, 'error', params.errFnName || "", params.errNoticeDiv || "#noticeDiv");
                wf.utils.scrollTop();
            };
            var cache = params.cache || false;
            var error = params.error || defaultErrCallback;
            var done = params.done || function () { };

            $.ajax({
                url: url,
                cache: cache,
                async: async,
                dataType: dataType,
                contentType: contentType,
                type: type,
                data: JSON.stringify(data),
                success: success,
                error: error
            }).done(done);
        } else {
            // TODO: use notice handler to display error
        }
    };

    return { makeRequest: makeRequest };
}();