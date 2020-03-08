﻿// <reference path="~/js/vendors/jquery.js" />
// <reference path="~/js/vendors/bootstrap.js" />

/*======================================================================*/
// Plato
// © InstantASP Ltd.
/*======================================================================*/

if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

$(function (win, doc, $) {

    "use strict";

    /* Private */
    /* ---------------------------------------------*/

    /* Plato Simple Logging */
    var platoLogger = {
        info: "Info",
        warning: "Warning",
        error: "Error",
        prevLogDate: null,
        logInfo: function(message) {
            this.log(this.info, message);
        },
        logWarning: function(message) {
            this.log(this.warning, message);
        },
        logError: function(message) {
            this.log(this.error, message);
        },
        log: function(level, message) {
            if (!$.Plato.defaults.debug) {
                return;
            }
            var difference = this.getDifferenceInMilliseconds();
            this.prevLogDate = new Date();
            console.log(level + ": " + message + " - " + difference + "ms elapsed since previous log entry.");
        },
        getDifferenceInMilliseconds: function() {
            if (this.prevLogDate === null) {
                this.prevLogDate = new Date();
            } else {
                return (this.prevLogDate - new Date()) / 10000;
            }
            return 0;
        }
    };

    /* Plato Client Localization */
    var platoLocale = {
        init: function () {

            if (win.$.Plato.defaults === null) {
                throw new Error("win.$.Plato.defaults Required");
            }

            platoLogger.logInfo("Initializing platoLocale");
            
            // We need a locale
            if (win.$.Plato.defaults.locale === "") {
                platoLogger.logError("platoLocale could not be initialized as the $.Plato.defaults.locale property is empty!");
                return;
            }

            // We need a url
            if (win.$.Plato.defaults.url === "") {
                platoLogger.logError("platoLocale could not be initialized as the $.Plato.defaults.url property is empty!");
                return;
            }

            // append a forward slash if needed
            var baseUrl = win.$.Plato.defaults.url;
            if (baseUrl.substring(baseUrl.length - 1, baseUrl.length) !== "/") {
                baseUrl += "/";
            }

            var url = baseUrl + "js/app/locale/app." + win.$.Plato.defaults.locale + ".js";
            platoLogger.logInfo("Loading locale: " + url);
            this._load(url);

        },
        get: function (key) {
            var strings = win.$.Plato.Strings;
            if (typeof strings !== 'undefined' &&
                typeof strings[key] !== 'undefined') {
                return strings[key];
            }
            return key;
        },
        _load: function (url) {

            var head = document.getElementsByTagName('head'),
                buster = parseInt(Math.random() * 1000) + new Date().getTime();

            var script = document.createElement('script');
            script.setAttribute('src', url + '?v=' + buster);

            if (head) {
                head[0].appendChild(script);
                platoLogger.logInfo("Adding locale (" + url + ") to the head element.");
            }

            script.onLoad = function() {
                platoLogger.logInfo("Added locale (" + url + ") to the head element.");
            };
        }
    };
    
    /* Plato UI */
    var platoUi = {
        init: function($ele) {

            if (!$ele) {
                $ele = $("body");
            }

            // init
            this.initToolTips($ele);
            this.initDropDowns($ele);
            this.initAvatar($ele);

        },
        logInfo: function(message) {
            platoLogger.logInfo(message);
        },
        logError: function(message) {
            platoLogger.logError(message);
        },
        initToolTips: function($el) {

            if (!$el) {
                $el = $("body");
            }

            this.logInfo("initToolTips()");

            // Enable Bootstrap tooltips
            if (win.$.Plato.defaults.bsToolTipEnabled) {
                if ($el) {
                    $el.find(win.$.Plato.defaults.bsToolTipSelector).tooltip({ trigger: "hover" });
                    $el.find(win.$.Plato.defaults.bsToolTipAlternativeSelector).tooltip({ trigger: "hover" });

                }
                this.logInfo("Bootstrap tooltips initialized.");
            }
        },
        disposeToolTips: function($el) {
            if (win.$.Plato.defaults.bsToolTipEnabled) {
                if ($el) {
                    $el.find(win.$.Plato.defaults.bsToolTipSelector).tooltip('dispose');
                    $el.find(win.$.Plato.defaults.bsToolTipAlternativeSelector).tooltip('dispose');

                }
                this.logInfo("Bootstrap tooltips disposed.");
            }
        },
        initDropDowns: function($el) {

            if (!$el) {
                $el = $("body");
            }

            this.logInfo("initDropDowns()");

            // Enable nested dropdown support
            $el.find("ul.dropdown-menu [data-toggle='dropdown']").on("click",
                function(event) {

                    // Avoid following the href location when clicking
                    event.preventDefault();
                    // Avoid having the menu to close when clicking
                    event.stopPropagation();
                    // If a menu is already open we close it
                    $("ul.dropdown-menu [data-toggle='dropdown']")
                        .parent()
                        .removeClass('show');
                    // opening the one you clicked on
                    $(this).parent().addClass('show');

                });


        },
        initAvatar: function($el) {

            if (!$el) {
                $el = $("body");
            }

            this.logInfo("initAvatar()");

            // Avatar upload selector with preview
            $el.find(win.$.Plato.defaults.avatarUploadSelector).change(function() {
                function readUrl(input) {
                    if (input.files && input.files[0]) {
                        var reader = new FileReader();
                        reader.onload = function(e) {
                            var previewSelector = $(input).attr("data-upload-preview-selector");
                            var $preview = $(previewSelector).find("span");
                            $preview.css('background-image', ' url(' + e.target.result + ')');
                            $preview.hide()
                                .fadeIn(650);
                        };
                        reader.readAsDataURL(input.files[0]);
                    }
                }

                readUrl(this);
            });

        },
        notify: function(options, settings) {
            // Bootstrap notify
            $.notify(options, settings);
        }
    };

    /* Plato Http */
    var platoHttp = function(config) {        
     
        if (!win.$.Plato.defaults) {
            throw new Error("platoHttp requires a valid $.Plato.defaults object");
        }
        
        var baseUrl = win.$.Plato.defaults.url,
            virtualUrl = config.url;
   
        // Remove forward slash suffix from base url
        if (baseUrl.substring(baseUrl.length - 1, baseUrl.length) === "/") {
            baseUrl = baseUrl.substring(win.$.Plato.defaults.url.length - 1);
        }

        // prefix a forward slash if non is provided for our end point
        if (virtualUrl.substring(0, 1) !== "/") {
            virtualUrl = "/" + virtualUrl;
        }

        // Update to absolute URL
        if (virtualUrl.toLowerCase().indexOf("http://") === -1 &&
            virtualUrl.toLowerCase().indexOf("https://") === -1) {
            config.url = baseUrl + virtualUrl;
        } 
    
        // add basic authentication headers
        var apiKey = win.$.Plato.defaults.apiKey;
        if (apiKey) {
            platoLogger.logInfo("ApiKey: " + apiKey);
            config.beforeSend = function(xhr) {
                xhr.setRequestHeader("Authorization", "Basic " + apiKey);
            };
        } else {
            platoLogger.logInfo("No api key was supplied");
        }

        // set content type & API version
        config.headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'X-Api-Version': '1',
            "X-Csrf-Token": win.$.Plato.defaults.getCsrfCookieToken()
        };

        var http = (function() {

            var getHtmlErrorMessage = function(err) {
                    var s = '<h6>' + platoLocale.get("An error occurred!") + '</h6>';
                s += platoLocale.get("Information is provided below...") + "<br/><br/>";
                    s += '<textarea style="min-height: 130px;" class="form-control">' + err + '</textarea>';
                    return s;
                },
                notify = function(message) {
                    
                    // Bootstrap notify
                    platoUi.notify({
                            // options
                            message: message
                        },
                        {
                            // settings
                            mouse_over: "pause",
                            type: 'danger',
                            allow_dismiss: true
                        });

                },
                onError = function(config, xhr, ajaxOptions, thrownError) {
                    
                    // Error details
                    var err = "$.Plato.Http - " +
                        thrownError +
                        "\n" +
                        JSON.stringify(xhr, null, "     ");

                    // Log
                    platoLogger.logError(err);
                    
                    // Notify
                    notify(getHtmlErrorMessage(err));

                },
                onAlways = function(xhr, textStatus) {
                    
                    switch (xhr.statusCode) {
                        case 401:
                            notify("<h6>" + platoLocale.get("Could not authenticate your request!") + "</h6>");
                            break;
                        case 404:
                            notify("<h6>" + platoLocale.get("404 - Service Not Found!") + "</h6>");
                            break;
                    }
               
                    // Log
                    platoLogger.logInfo("platoHttp - Completed: " + JSON.stringify(xhr, null, "     "));

                };

            return {
                promise: function(config) {
                    return $.ajax(config)
                        .fail(function (xhr, ajaxOptions, thrownError) {
                            // If you attempt to navigate away from the page before the XmlHttp request
                            // completes jQuery raises the failed event with a readyState of 0, this is not 
                            // an error we need to display so simply return in this scenario
                            if (xhr) {
                                if (xhr.readyState !== null && typeof xhr.readyState !== "undefined") {
                                    var readyState = parseInt(xhr.readyState);
                                    if (!isNaN(readyState)) {
                                        if (readyState === 0) {
                                            return;
                                        }
                                    }
                                }
                            }
                            if (onError) {
                                onError(config, xhr, ajaxOptions, thrownError);
                            }
                            if (config.onError) {
                                config.onError(xhr, ajaxOptions, thrownError);
                            }
                        })
                        .always(function(xhr, textStatus) {
                            if (onAlways) {
                                onAlways(xhr, textStatus);
                            }
                            if (config.onAlways) {
                                config.onAlways(xhr, textStatus);
                            }
                        });
                }
            };
        }());

        platoLogger.logInfo("$.Plato.Http - Starting Request: " + JSON.stringify(config, null, "     "));

        return http.promise(config);

    };

    /* Plato Storage */
    var platoStorage = {
        setCookie: function (key, value, expireDays, toJson, path) {

            toJson = toJson || false;
            if (toJson) { value = encodeURIComponent(JSON.stringify(value)); }
            if (!path) { path = "/"; }

            var data = "";
            if (expireDays) {
                var dateExpires = new Date();
                dateExpires.setTime(dateExpires.getTime() + 1000 * 60 * 60 * 24 * expireDays);
                data = '; expires=' + dateExpires.toGMTString();
            }
            document.cookie = key + "=" + value + data + ";path=" + path;
            return this;
        },
        getCookie: function (key) {
            var ckName = key + "=";
            var ckPos = document.cookie.indexOf(ckName);
            if (ckPos !== -1) {
                var ckStart = ckPos + ckName.length;
                var ckEnd = document.cookie.indexOf(";", ckStart);
                if (ckEnd === -1) { ckEnd = document.cookie.length; }
                return unescape(document.cookie.substring(ckStart, ckEnd));
            }
            return null;
        },
        updateCookie: function (key, value, update, delimiter) {
            // if update is false the value will be removed from the cookie
            // if update is true the value will be appended to the cookie
            var cookie = this.getCookie(key),
                temp = new Array();
            delimiter = delimiter || ",";
            if (cookie) {
                // read existing excluding value into temp array
                var values = cookie.split(delimiter);
                for (var i in values) {
                    if (values.hasOwnProperty(i)) {
                        if (values[i] !== value && values[i] !== "") {
                            temp[temp.length] = values[i];
                        }
                    }
                }
            }
            // should we append the current value?
            if (update) { temp[temp.length] = value; }
            // update the cookie
            this.setCookie(key, temp.join(delimiter));
            return this;
        },
        clearCookie: function (key) {
            this.setCookie(key, null, -1);
            return this;
        },
        set: function (key, value) {     
            if (win.localStorage) {             
                win.localStorage.setItem(key, value);
            }            
        },
        get: function (key) {
            if (win.localStorage) {             
                return win.localStorage.getItem(key);
            }
            return null;
        },
        remove: function (key) {
            if (win.localStorage) {
                win.localStorage.removeItem(key);
            }
        }
    };

    /* Plato Text */
    var platoText = {
        htmlEncode: function(input) {
            var ele = document.createElement('span');
            ele.textContent = input;
            return ele.innerHTML;
        }
    };

    /* Plato Social */
    var platoSocial = {
        init: function () {

            // Inject facebook & twitter scripts
            var js,
                fjs = doc.getElementsByTagName('script')[0],
                frag = doc.createDocumentFragment(),
                add = function (url, id) {
                    if (doc.getElementById(id)) {
                        return;
                    }
                    js = doc.createElement('script');
                    js.src = url;
                    id && (js.id = id);
                    frag.appendChild(js);
                };
            add('https://connect.facebook.net/en_US/sdk.js', 'facebook-jssdk');
            add('https://platform.twitter.com/widgets.js');
            fjs.parentNode.insertBefore(frag, fjs);

        },
        facebook: {
            init: function (appId) {
                // We may not have a facebook app id
                if (appId === null || typeof appId === "undefined" || appId === "") {
                    return null;
                }
                // Initialize facebook API
                win.fbAsyncInit = function () {
                    win.FB.init({
                        appId: appId,
                        xfbml: true,
                        version: "v2.8"
                    });
                };
            },
            share: function (url) {
                win.FB.ui({
                    method: 'share',
                    href: url
                },
                    function (response) {
                        if (response && response.post_id) {
                            // published
                        } else {
                            // not published
                        }
                    });
            },
            feed: function (url) {
                win.FB.ui({
                    method: "feed",
                    name: "",
                    link: url,
                    picture: "",
                    caption: "",
                    description: ""
                },
                    function (response) {
                        if (response && response.post_id) {
                            // published
                        } else {
                            // not published
                        }
                    });
            }
        },
        twitter: {
            share: function (url) {
                win.twttr.ready(function (twttr) {
                    twttr.events.bind('tweet',
                        function (event) { });
                });
                var popup = window.open('https://twitter.com/intent/tweet?text=' + url,
                    'popupwindow',
                    'scrollbars=yes,width=800,height=400');
                popup.focus();
            }
        }
    };

    /* Plato Utilities */
    var platoUtils = {       
        addCss: function (path) {
            var head = doc.head;
            if (head) {
                var link = doc.createElement("link");
                link.type = "text/css";
                link.rel = "stylesheet";
                link.href = path;
                head.appendChild(link);
            }
        },
        addScript: function (path) {
            var body = doc.body;
            if (body) {
                var script = doc.createElement("script");             
                script.src = path;
                body.appendChild(script);
            }
        }
    };

    /* Global */
    /* ---------------------------------------------*/

    win.$.Plato = {
        // defaults
        defaults: {
            debug: false,
            url: "",
            locale: "en-US",
            apiKey: "",
            csrfHeaderName: "X-Csrf-Token", // Custom CSRF header
            csrfCookieName: "", // CSRF cookie name
            bsToolTipEnabled: true, // UI tooltips
            bsToolTipAlternativeSelector: "[data-provide='tooltip']",
            bsToolTipSelector: "[data-toggle='tooltip']",
            avatarUploadSelector: "[data-upload='avatar']",
            alerts: {
                autoClose: true,
                autoCloseDelay: 8 // duration in seconds before auto close
            },
            layout: {
                stickyHeaders: true,
                stickySidebars: true,
                stickyAsides: true
            },
            validation: {
                scrollToErrors: true
            },
            getCsrfCookieToken: function() {
                if (this.csrfCookieName !== "") {
                    var storage = win.$.Plato.storage;
                    var cookie = storage.getCookie(this.csrfCookieName);
                    if (cookie) {
                        return cookie;
                    }
                }
                return "";
            }
        },
        // helpers
        T: function (key) {
            // Gets the first matching key from any of the loaded client side locales
            if (this.locale) {
                return this.locale.get(key);
            }
            return key;
        },
        readyMethods: [],
        ready: function(func) {
            this.readyMethods.push(func);
        },
        // facade access - ensures we can easily implement 
        // new objects or extend existing objects 
        // i.e.$.extend(window.$.Plato.logger, myLogger)
        locale: platoLocale,
        logger: platoLogger,
        ui: platoUi,
        http: platoHttp,
        storage: platoStorage,
        text: platoText,
        social: platoSocial,
        utils: platoUtils
    };
    
    /* Initialize */
    /* ---------------------------------------------*/
    
    $(doc).ready(function () {

        // Our main global object
        var app = win.$.Plato;

        // Invoke pushed ready methods
        for (var i = 0; i < app.readyMethods.length; i++) {
            app.readyMethods[i]();
        }

        // Write options to the console
        app.logger.logInfo("$.Plato:\n" + JSON.stringify(app.defaults, null, "     "));

        // init UI
        app.ui.init();

        // init Locales
        app.locale.init();

    });

    // Register an offline service worker for better PWA support
    // Delay registration until everything has loaded to improve first visit perf
    $(win).on("load", function () {        
        if ('serviceWorker' in win.navigator) {
            win.navigator.serviceWorker.register('/service-worker.js');
        }     
    });    

}(window, document, jQuery));
