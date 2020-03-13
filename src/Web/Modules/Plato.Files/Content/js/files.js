// <reference path="/wwwroot/js/plato.js" />

if (typeof $().modal === 'undefined') {
    throw new Error("BootStrap 4.1.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

$(function (win, doc, $) {

    "use strict";

    // A global object that allows Plato to configure attachment settings
    win.$.Plato.Files = {
        allowedExtensions: [
            "txt",
            "html",
            "zip",
            "png",
            "gif",
            "bmp",
            "jpg",
            "jpeg"
        ],
        maxFileSize: 0
    };

    // Plato Global Object
    var app = win.$.Plato;

    /* attachments */
    var attachments = function (options) {

        var dataKey = "attachments",
            dataIdKey = dataKey + "Id";

        var defaults = {};

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

            },
            bind: function ($caller) {

                // Configure httpContent
                $caller.find('[data-provide="http-content"]').httpContent({
                    onLoad: function ($el) {
                        // Bind delete
                        var $btns = $el.find('[data-provide="delete-attachment"]');
                        if ($btns.length > 0) {
                            $btns.click(function (e) {

                                e.preventDefault();
                                e.stopPropagation();

                                var $icon = $(this).find("i");
                                if ($icon.length > 0) {
                                    $icon.removeClass("fa-times")
                                        .addClass("fa-spinner")
                                        .addClass("fa-spin");
                                }                             

                                var url = $(this).attr("href");
                                if (url === "") {
                                    throw new Error("A delete url is required!");
                                }

                                var id = parseInt($(this).data("attachmentId"));
                                if (isNaN(id)) {
                                    throw new Error("An attachment id to delete is required!");
                                }

                                app.http({
                                    method: "POST",
                                    url: url,                                  
                                    data: JSON.stringify(id)
                                }).done(function (response) {
                                    $caller.find('[data-provide="http-content"]').httpContent("reload");
                                });

                            });
                        }                  
                    }
                });

                // Configure dropzone
                $caller.find('[data-provide="attachment-dropzone"]')
                    .attachmentDropzone({
                        allowedExtensions: app.Files.allowedExtensions,
                        maxFileSize: app.Files.maxFileSize,
                        onDrop: function () {
                            $caller.find(".dropdown").each(function () {
                                $(this).find(".dropdown-menu").removeClass('show');
                            });
                        },
                        onAddedFile: function (file) {
                        },
                        onComplete: function (file) {
                            $caller.find('[data-provide="http-content"]').httpContent("reload");
                        },
                        onSuccess: function (response) {
                        },
                        onError: function (file, error, xhr) {
                        }
                    });

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

                if (this.length > 0) {
                    // $(selector).attachments()
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
                    var $caller = $('[data-provide="http-content"]');
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

    // attachmentDropzone
    var attachmentDropzone = function () {

        var dataKey = "attachmentDropzone",
            dataIdKey = dataKey + "Id";

        var defaults = {
            progressPreview: "#progress",
            allowedExtensions: [
                "txt",
                "html",
                "zip",
                "png",
                "gif",
                "bmp",
                "jpg",
                "jpeg",
                "pdf"
            ],
            maxFileSize: 2097152, // 2mb     
            dropZoneOptions: {
                url: '/api/attachments/streaming/upload',
                fallbackClick: false,
                autoProcessQueue: true,
                autoDiscover: false,
                disablePreview: true,
                uploadMultiple: false,
                maxFilesize: 256, // 256mb
                dictDefaultMessage:
                    '<p class=\"text-center\"><i class=\"fal fa-arrow-from-top fa-flip-vertical fa-2x d-block text-muted mb-2\"></i>Drag & drop files here or <a id="#dzUpload" class=\"dz-clickable\" href="#">click to browse</a></p>'
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
            },
            onComplete: function (file) {
                // triggers when a file is successfully uploaded
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

                var opts = $caller.data(dataKey).dropZoneOptions,
                    url = this._getUrl($caller);
                if (url === null) {
                    throw new Error("An upload url is required for the dropzone!");
                }

                if (!opts.init) {

                    // Set dropzone max file size
                    opts.maxFilesize = Math.ceil($caller.data(dataKey).maxFileSize / (1024 * 1024));

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
                            maxFileSize = $caller.data(dataKey).maxFileSize,
                            allowedExtensions = $caller.data(dataKey).allowedExtensions,
                            errors = [];

                        function getProgressId(file) {
                            return "progress-" + file.upload.uuid;
                        }

                        function getProgress(file) {

                            var $row = $("<div>", {
                                "id": getProgressId(file),
                                "class": "progress-row m-3"
                            });

                            var $info = $("<div>", {
                                "class": "progress-info text-center mb-2"
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
                        
                        function validateFileExtensions(files) {

                            var valid = true;
                            for (var i = 0; i < files.length; i++) {                           
                                if (!validateFileExtension(files[i])) {
                                    valid = false;
                                }
                            }

                            // Allowed?
                            if (valid === false) {

                                var title = app.T("Some files won't be attached"),
                                    message = app.T("One or more file types you attached are not allowed. File types that are allowed will still be uploaded. Allowed types are...") + "\n\n" +
                                        allowedExtensions.join(", ");

                                // Show error dialog
                                $().dialog({
                                    title: title,
                                    body: {
                                        url: null,
                                        html: message.replace(/\n/g, "<br/>")
                                    },
                                    buttons: [
                                        {
                                            id: "ok",
                                            text: "OK",
                                            css: "btn btn-primary",
                                            click: function ($dialog, $button) {
                                                $().dialog("hide");
                                                return false;
                                            }
                                        }
                                    ]
                                },
                                    "show");

                            }

                            return valid;

                        }

                        function validateFileExtension(file) {

                            if (allowedExtensions === null) {
                                return false;
                            }
                            if (allowedExtensions.length === 0) {
                                return false;
                            }

                            var fileName = file.upload.filename,
                                bits = fileName.split("."),
                                fileExtension = bits[bits.length - 1],
                                allowedExtension = null;
                            for (var i = 0; i < allowedExtensions.length; i++) {
                                allowedExtension = allowedExtensions[i];
                                if (fileExtension.toLowerCase() === allowedExtension.toLowerCase()) {
                                    return true;
                                }
                            }

                            return false;

                        }

                        // ------------

                        this.on("addedfiles",
                            function (files) {
                                errors = []; // reset errors when files are added
                                return validateFileExtensions(files);
                            });

                        this.on('addedfile',
                            function (file) {

                                // Validate extension                                                                                        
                                if (!validateFileExtension(file)) {
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
                                        // Compile any errors                                     
                                        for (var i = 0; i < response.result.length; i++) {
                                            var result = response.result[i];
                                            if (result.error && result.error !== "") {
                                                errors.push(result.error);
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

                        this.on("complete",
                            function (file) {

                            // Show errors
                            if (errors.length > 0) {

                                var messages = "";
                                for (var i = 0; i < errors.length; i++) {
                                    messages += errors[i];
                                    if (i < errors.length - 1) {
                                        messages += "\n\n";
                                    }
                                }

                                // Show error dialog
                                $().dialog({
                                    title: app.T("Some problems occurred"),
                                    body: {
                                        url: null,
                                        html: messages.replace(/\n/g, "<br/>")
                                    },
                                    css: {
                                        body: "modal-body max-h-200 overflow-auto"
                                    },
                                    buttons: [
                                        {
                                            id: "ok",
                                            text: "OK",
                                            css: "btn btn-primary",
                                            click: function ($dialog, $button) {
                                                $().dialog("hide");
                                                return false;
                                            }
                                        }
                                    ]
                                },
                                    "show");

                            }

                            if ($caller.data(dataKey).onComplete) {
                                $caller.data(dataKey).onComplete(file);
                            }
                        });

                        this.on('error',
                            function (file, error, xhr) {                                

                                var s = '<h6>' + app.T("A problem occurred!") + '</h6>';                          
                                s += '<textarea class="form-control">' + error + '</textarea>';                             

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

                if (win.Dropzone) {

                    // Init dropzone
                    var dropzone = new win.Dropzone($caller[0], opts);

                    // Store dropzone object in data for access within event handlers
                    $caller.data("dropzone", dropzone);

                } else {

                    console.log('Dropzone was configured but Dropzone was not detected. Ensure the Plato.Dropzone feature is enabled.');

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
          
                if (this.length > 0) {
                    // $(selector).attachmentDropzone
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
                    // $().attachmentDropzone()
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
        attachments: attachments.init,    
        attachmentDropzone: attachmentDropzone.init
    });

    // --------

    app.ready(function () {       
        
        $('[data-provide="attachments"]')
            .attachments();
        
    });

    // infinite scroll load
    $().infiniteScroll(function ($ele) { }, "ready");

}(window, document, jQuery));
