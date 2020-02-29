// <reference path="/wwwroot/js/plato.js" />

if (typeof $().modal === 'undefined') {
    throw new Error("BootStrap 4.1.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

$(function (win, doc, $) {

    "use strict";

    // Plato Global Object
    var app = win.$.Plato;

    /* attachmentDropdown */
    var attachmentDropdown = function (options) {

        var dataKey = "attachmentDropdown",
            dataIdKey = dataKey + "Id";

        var defaults = {      
            loaderTemplate: '<p class="text-center"><i class="fal fa-spinner fa-spin"></i></p>', // a handlebars style template for auto complete list items
        };

        var methods = {
            init: function ($caller, methodName, func) {

                if (func) {
                    return func(this);
                }
                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return null;
                }

                this.bind($caller);

                return null;

            },
            bind: function ($caller) {

                $caller.on('shown.bs.dropdown',
                    function () {
                        methods.populate($caller);
                    });

            },
            populate: function ($caller) {

                var url = methods.getUrl($caller);
                if (!url) {
                    throw new Error("Could not determine a valid url to load within the dropdown!");
                }
                
                app.http({
                    method: "GET",
                    url: url
                }).done(function (response) {
                    var $content = $caller.find(".dropdown-menu-content");
                    if ($content.length > 0) {
                        $content.empty();
                        if (response !== "") {
                            $content.html(response);

                            // Enable tooltips within loaded content
                            app.ui.initToolTips($content);
                            // confirm
                            $content.find('[data-provide="confirm"]').confirm();

                        }
                    }

                    // onLoad event
                    //if ($caller.data(dataKey).onLoad) {
                    //    $caller.data(dataKey).onLoad($caller, response.result);
                    //}
                });

            },
            getUrl: function ($caller) {           

                if (!$caller.data(dataKey).url) {

                    // get url from caller
                    if ($caller.attr("href")) {
                        $caller.data(dataKey).url = $caller.attr("href");
                    }

                    // get url from child trigger
                    $caller.find("a").each(function () {
                        if ($(this).hasClass("dropdown-toggle") ||
                            $(this).attr("data-toggle")) {
                            if ($(this).attr("href")) {
                                $caller.data(dataKey).url = $(this).attr("href");
                                return;
                            }
                        }
                    });

                }

                return $caller.data(dataKey).url;                

            }

        };

        return {
            init: function () {

                var options = {};
                var methodName = null;
                for (var i = 0; i < arguments.length; ++i) {
                    var a = arguments[i];
                    if (a) {
                        switch (a.constructor) {
                            case Object:
                                $.extend(options, a);
                                break;
                            case String:
                                methodName = a;
                                break;
                            case Boolean:
                                break;
                            case Number:
                                break;
                            case Function:
                                break;
                        }
                    }
                }

                if (this.length > 0) {
                    // $(selector).labelSelectDropdown()
                    return this.each(function () {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        methods.init($(this), methodName);
                    });
                } else {
                    // $().labelSelectDropdown()
                    if (methodName) {
                        if (methods[methodName]) {
                            var $caller = $("body");
                            $caller.data(dataKey, $.extend({}, defaults, options));
                            methods[methodName].apply(this, [$caller]);
                        } else {
                            alert(methodName + " is not a valid method!");
                        }
                    }
                }

            }
        };

    }();

    // attachmentDropzone
    var attachmentDropzone = function () {

        var dataKey = "attachmentDropzone",
            dataIdKey = dataKey + "Id";

        var defaults = {
            progressPreview: "#progress",
            allowedUploadExtensions: [
                "txt",
                "html",
                "zip",
                "png",
                "gif",
                "bmp",
                "jpg",
                "jpeg"
            ],
            dropZoneOptions: {
                url: '/api/attachments/streaming/upload',
                fallbackClick: false,
                autoProcessQueue: true,
                autoDiscover: false,
                disablePreview: true,
                uploadMultiple: false,
                dictDefaultMessage:
                    'Drag & drop files here or <a id="#dzUpload" class=\"dz-clickable\" href="#">click to browse</a>'
            },
            onAddedFile: function (file) {
                // triggers when a file is added
            },
            onDrop: function (e) {
                // triggers when a file is dropped into the dropzone
            },
            onSuccess: function (response) {
                // triggers when a file is successfully uploaded
            },
            onError: function (file, error, xhr) {
                // triggers when an upload error occurrs
            }
        };

        var methods = {
            init: function ($caller, methodName) {
     
                if (methodName) {
                    if (this[methodName]) {
                        this[methodName].apply(this, [$caller]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                this.bind($caller);
                
            },
            bind: function ($caller) {

                //methods._showFullScreenProgress($caller);

                var opts = $caller.data(dataKey).dropZoneOptions,
                    url = this._getUrl($caller);
                if (url === null) {
                    throw new Error("An upload url is required for the dropzone!");
                }

                if (!opts.init) {

                    // Get Plato options
                    if (app.defaults) {

                        var token = app.defaults.getCsrfCookieToken();
                        if (token === "") {
                            alert("An error occurred. No valid CSRF token could be obtained for the request.");
                        }

                        // Configure drop zone requests from Plato options
                        opts.url = app.defaults.url + url;

                        // Configure request headers
                        opts.headers = {
                            "Authorization": "Basic " + app.defaults.apiKey,
                            "X-Csrf-Token": token
                        };

                    }

                    opts.init = function () {

                        var $progressPreview = methods._getProgressPreview($caller),                           
                            allowedUploadExtensions = $caller.data(dataKey).allowedUploadExtensions;                                        

                        function getProgressId(file) {
                            return "progress-" + file.upload.uuid;
                        }

                        function getProgress(file) {

                            var $row = $("<div>", {
                                "id": getProgressId(file),
                                "class": "progress-row"
                            });

                            var $info = $("<div>", {                          
                                "class": "progress-info mb-2"
                            });

                            var $bar = $("<div>", {
                                "class": "progress"
                            });
                            
                            $bar.append($("<div>", {
                                "class": "progress-bar",
                                "role": "progressbar",
                                "style": "width: 0;"
                            }));

                            $row.append($info);
                            $row.append($bar);
                            
                            return $row;

                        }

                        this.on('addedfile',
                            function (file) {

                                // Validate file extension
                                var fileName = file.upload.filename;
                                var allowed = false;
                                if (fileName && allowedUploadExtensions) {
                                    var bits = fileName.split(".");
                                    var fileExtension = bits[bits.length - 1];
                                    for (var i = 0; i < allowedUploadExtensions.length; i++) {
                                        var allowedExtension = allowedUploadExtensions[i];
                                        if (fileExtension.toLowerCase() === allowedExtension.toLowerCase()) {
                                            allowed = true;
                                        }
                                    }
                                }

                                // Allowed?
                                if (allowed === false) {
                                    alert("File type is not allowed.\n\nAllowed types are " +
                                        allowedUploadExtensions.join(", "));
                                    this.removeFile(file);
                                    return false;
                                }

                                // Append a progress bar for the upload
                                if ($progressPreview) {                             
                                    $progressPreview.append(getProgress(file));
                                }                            

                                // Raise event
                                if ($caller.data(dataKey).onAddedFile) {
                                    $caller.data(dataKey).onAddedFile(file);
                                }

                                return true;

                            });

                        this.on('drop',
                            function (e) {
                                if ($caller.data(dataKey).onDrop) {
                                    $caller.data(dataKey).onDrop(e);
                                }
                            });

                        this.on('uploadprogress',
                            function (file, progress, bytesSent) {                                
                                if ($progressPreview) {
                                    progress = Math.floor(bytesSent / file.size * 100);
                                    var rowSelector = '#' + getProgressId(file);
                                    var $row = $progressPreview.find(rowSelector);
                                    $row.find(".progress-info").text(progress + "%");
                                    $row.find(".progress-bar").width(progress + "%");
                                }
                            });

                        this.on('success',
                            function (file, response) {
                                
                                if (response.statusCode === 200) {
                                    if (response && response.result) {
                                        for (var i = 0; i < response.result.length; i++) {
                                            var result = response.result[i];
                                            if (result.id > 0) {                                                
                                                // Bootstrap notify
                                                app.ui.notify({
                                                        // options
                                                        message: result.name + app.T(" Uploaded Successfully")
                                                    },
                                                    {
                                                        // settings                                                     
                                                        type: 'success',
                                                        allow_dismiss: true
                                                    });
                                            }
                                        }
                                    }
                                }

                                // Remove progress 
                                if ($progressPreview) {
                                    var selector = '#' + getProgressId(file),
                                        $progrsss = $progressPreview.find(selector);
                                    if ($progrsss.length > 0) {
                                        $progrsss.remove();
                                    }                                    
                                }

                                if ($caller.data(dataKey).onSuccess) {
                                    $caller.data(dataKey).onSuccess(response);
                                }


                            });
                    
                        this.on('error',
                            function (file, error, xhr) {

                                var s = '<h6>' + app.T("An error occurred!") + '</h6>';
                                s += app.T("Information is provided below...") + "<br/><br/>";
                                s += '<textarea style="min-height: 130px;" class="form-control">' + error + '</textarea>';                             

                                // Bootstrap notify
                                app.ui.notify({
                                        // options
                                        message: s
                                    },
                                    {
                                        // settings
                                        mouse_over: "pause",
                                        type: 'danger',
                                        allow_dismiss: true
                                    });

                                // Remove progress 
                                if ($progressPreview) {
                                    var selector = '#' + getProgressId(file),
                                        $progrsss = $progressPreview.find(selector);
                                    if ($progrsss.length > 0) {
                                        $progrsss.remove();
                                    }
                                }

                                if ($caller.data(dataKey).onError) {
                                    $caller.data(dataKey).onError(file, error, xhr);
                                }

                            });
                    };
                }

                // Init dropzone
                var dropzone = new Dropzone($caller[0], opts);

                // Store dropzone object in data for access within event handlers (i.e. paste)
                $caller.data("dropzone", dropzone);

            },
            _showFullScreenProgress: function ($caller) {

                var $overlay = $(".upload-overlay");
                if (!$overlay.hasClass("visible")) {
                    $overlay.addClass("visible");
                }
            },
            _hideFullScreenProgress: function ($caller) {

                var $overlay = $(".upload-overlay");
                if ($overlay.hasClass("visible")) {
                    $overlay.removeClass("visible");
                }

            },
            _getProgressPreview: function ($caller) {
                var selector = $caller.data("progressPreview") || $caller.data(dataKey).progressPreview;
                if (selector) {
                    var $preview = $(selector);
                    if ($preview.length > 0) {
                        return $preview;
                    }
                }
                return null;
            },
            _getUrl: function ($caller) {
                return $caller.data("dropzoneUrl") ||
                    $caller.data(dataKey).dropZoneOptions.url;
            }
        };

        return {
            init: function () {

                var options = {},
                    methodName = null,
                    func = null;
                for (var i = 0; i < arguments.length; ++i) {
                    var a = arguments[i];
                    switch (a.constructor) {
                        case Object:
                            $.extend(options, a);
                            break;
                        case String:
                            methodName = a;
                            break;
                        case Function:
                            func = a;
                            break;
                    }
                }

                console.log(this.length);

                if (this.length > 0) {
                    // $(selector).attachments
                    return this.each(function () {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        methods.init($(this), methodName);
                    });
                } else {
                    // $().attachments()
                    var $caller = $('[data-provide="attachment-dropzone"]');
                    if ($caller.length > 0) {
                        if (!$caller.data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $caller.data(dataIdKey, id);
                            $caller.data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $caller.data(dataKey, $.extend({}, $caller.data(dataKey), options));
                        }
                        return methods.init($caller, methodName, func);
                    }
                }

            }
        };

    }();

    $.fn.extend({        
        attachmentDropdown: attachmentDropdown.init,
        attachmentDropzone: attachmentDropzone.init
    });

    // --------

    app.ready(function () {
  
        $('[data-provide="attachment-dropdown"]')
            .attachmentDropdown();

        $('[data-provide="attachment-dropzone"]')
            .attachmentDropzone();

    });

    // infinite scroll load
    $().infiniteScroll(function ($ele) { }, "ready");

}(window, document, jQuery));
