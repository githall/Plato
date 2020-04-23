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

    /* files */
    var editFile = function (options) {

        var dataKey = "editFile",
            dataIdKey = dataKey + "Id";

        var defaults = {
            fileId: 0
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

            },
            bind: function ($caller) {

                // The id of the file we are editing.
                // If 0 we are adding a new file
                var fileId = this._getFileId($caller),
                    returnUrl = this._getReturnUrl($caller),
                    dropzoneMsg = fileId === 0
                        ? 'Add files by dropping here or <a id="#dzUpload" class=\"dz-clickable\" href="#">click to browse</a>'
                        : 'Update this file by dropping a new file here or <a id="#dzUpload" class=\"dz-clickable\" href="#">click to browse</a>';

                // Configure dropzone
                $caller.find('[data-provide="file-dropzone"]')
                    .fileDropzone({
                        allowedExtensions: app.Files.allowedExtensions,
                        maxFileSize: app.Files.maxFileSize,
                        dropZoneOptions: {
                            url: win.$.Plato.defaults.pathBase + '/api/files/post',
                            fallbackClick: false,
                            autoProcessQueue: true,
                            autoDiscover: false,
                            disablePreview: true,
                            uploadMultiple: false,
                            dictDefaultMessage: '<p class=\"text-center\"><i class=\"fal fa-arrow-from-top fa-flip-vertical fa-2x d-block text-muted mb-2\"></i>' +
                                dropzoneMsg + '</p>'
                        },
                        onQueuecomplete: function (errors) {                            
                            if (errors.length === 0) {
                                win.location = returnUrl;
                            }
                        },                      
                        onError: function (file, error, xhr) {
                        }
                    });

            },
            _getFileId: function ($caller) {
                return $caller.data("fileId") || $caller.data(dataKey).fileId;
            },
            _getReturnUrl: function ($caller) {
                return $caller.data("returnUrl") || $caller.data(dataKey).returnUrl;
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
                    // $(selector).files()
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
                    // $().files()
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

    /* fileDropdown */
    var fileDropdown = function (options) {

        var dataKey = "fileDropdown",
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
                        var $btns = $el.find('[data-provide="delete-file"]');
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

                                var id = parseInt($(this).data("fileId"));
                                if (isNaN(id)) {
                                    throw new Error("A data-file-id attribute is required!");
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
                $caller.find('[data-provide="file-dropzone"]')
                    .fileDropzone({
                        allowedExtensions: app.Files.allowedExtensions,
                        maxFileSize: app.Files.maxFileSize,
                        onDrop: function () {
                            $caller.find(".dropdown").each(function () {
                                $(this).find(".dropdown-menu").removeClass('show');
                            });
                        },
                        onAddedFile: function (file) {
                        },
                        onComplete: function (file, errors) {
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
                    // $(selector).fileDropdown()
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
                    // $().fileDropdown()
                    var $caller = $('[data-provide="file-dropdown"]');
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

    // fileDropzone
    var fileDropzone = function () {

        var dataKey = "fileDropzone",
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
            maxFiles: 10,
            maxFileSize: 2097152, // 2mb     
            dropZoneOptions: {
                url: win.$.Plato.defaults.pathBase + '/api/files/post',
                fallbackClick: false,
                autoProcessQueue: true,
                autoDiscover: false,
                disablePreview: true,
                uploadMultiple: false,             
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
            onComplete: function (file, errors) {
                // triggers when a file is successfully uploaded
            },
            onQueuecomplete: function () {
                // triggers when all files have completed
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
                    url = this._getUrl($caller),
                    maxFiles = this._getMaxFiles($caller);

                if (url === null) {
                    throw new Error("An post url is required for the dropzone!");
                }

                if (!opts.init) {

                    // Set dropzone max file size & max files
                    opts.maxFilesize = Math.ceil($caller.data(dataKey).maxFileSize / (1024 * 1024));
                    opts.maxFiles = maxFiles;

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

                        function showErrorsDialog() {

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

                        // ------------

                        this.on("addedfiles",
                            function (files) {
                                
                                errors = []; // reset errors when new files are added

                                var valid = true, file = null;
                                for (var i = 0; i < files.length; i++) {
                                    file = files[i];
                                    // file.upload may be null when dropping files
                                    if (file.upload) { 
                                        if (!validateFileExtension(file)) {
                                            valid = false;
                                        }
                                    }
                                   
                                }

                                return valid;
                            });

                        // When a file is added to the list
                        this.on('addedfile',
                            function (file) {
                         
                                // Validate extension                                                                                        
                                if (!validateFileExtension(file)) {     
                                    errors.push(app.T("Some files are not allowed. Allowed types are ") +
                                        allowedExtensions.join(", ") + "\n\n" +
                                        app.T("Files that are allowed have still been uploaded"));
                                    showErrorsDialog();
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

                        // The user dropped something onto the dropzone
                        this.on('drop',
                            function (e) {
                                if ($caller.data(dataKey).onDrop) {
                                    $caller.data(dataKey).onDrop(e);
                                }
                            });

                        // Gets called periodically whenever the file upload progress changes.
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

                        // The file has been uploaded successfully. Gets the server response as second argument. (This event was called finished previously)
                        this.on('success',
                            function (file, response) {

                                // Compile any server side validation errors                                     
                                if (response.statusCode === 200) {
                                    if (response && response.result) {                                        
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

                                // Raise success
                                if ($caller.data(dataKey).onSuccess) {
                                    $caller.data(dataKey).onSuccess(response);
                                }

                            });

                        // Called when the upload was either successful or erroneous.
                        this.on("complete",
                            function (file) {
                            // Raise complete
                            if ($caller.data(dataKey).onComplete) {
                            $caller.data(dataKey).onComplete(file, errors);
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

                        // Called when all files in the queue finish uploading.
                        this.on("queuecomplete",
                            function () {

                                // Show errors
                                if (errors.length > 0) {
                                    showErrorsDialog();
                                }

                                // Raise queue complete
                                if ($caller.data(dataKey).onQueuecomplete) {
                                    $caller.data(dataKey).onQueuecomplete(errors);
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
            },
            _getMaxFiles: function ($caller) {
                var attr = $caller.data("dropzoneMaxFiles");
                if (attr !== null && typeof attr !== "undefined") {
                    var value = parseInt(attr);
                    if (!isNaN(value)) {
                        return value;
                    }
                }
                return $caller.data(dataKey).maxFiles;
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
                    // $(selector).fileDropzone
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
                    // $().fileDropzone()
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
        editFile: editFile.init,
        fileDropdown: fileDropdown.init,
        fileDropzone: fileDropzone.init
    });

    // --------

    app.ready(function () {       

        $('[data-provide="edit-file"]')
            .editFile();

        $('[data-provide="file-dropdown"]')
            .fileDropdown();
        
    });

    // infinite scroll load
    $().infiniteScroll(function ($ele) { }, "ready");

}(window, document, jQuery));
