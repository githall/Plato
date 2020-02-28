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
                            //app.ui.initToolTips($content);
                            // confirm
                            //$content.find('[data-provide="confirm"]').confirm();

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
                dictDefaultMessage:
                    'Drag & drop files here or <a id="#dzUpload" class=\"dz-clickable\" href="#">click to browse</a>'
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

                var opts = $caller.data(dataKey).dropZoneOptions;
                var url = this._getUrl($caller);
                console.log(url);

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

                        var allowedUploadExtensions = $caller.data(dataKey).allowedUploadExtensions;

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

                                return true;

                            });

                        this.on('drop',
                            function (e) {


                            });

                        this.on('success',
                            function (file, response) {

                                console.log(response);

                                if (response.statusCode === 200) {

                                    if (response && response.result) {
                                        for (var i = 0; i < response.result.length; i++) {

                                            var result = response.result[i],                                              
                                                chunk = "";

                                            if (result.id > 0) {

                                                // Image or file?
                                                if (result.isImage) {
                                                    chunk = '![' + result.name + '](/media/' + result.id + ')';
                                                } else {
                                                    chunk = '[' + result.name + '](/media/' + result.id + ') - ' + result.friendlySize;
                                                }

                                            }

                                            var $div = $("<div>", {
                                                class: "attachment-preview"
                                            }).text(chunk);

                                            $caller.append($div);

                                        }
                                    }

                                }

                            });

                        this.on('error',
                            function (file, error, xhr) {
                                console.log('Error:', error);
                            });
                    };
                }

                // Init dropzone
                var dropzone = new Dropzone($caller[0], opts);

                // Store dropzone object in data for access within event handlers (i.e. paste)
                $caller.data("dropzone", dropzone);

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
