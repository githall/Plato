// <reference path="/wwwroot/js/app.js" />

if (typeof $().modal === 'undefined') {
    throw new Error("BootStrap 4.1.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

$(function (win, doc, $) {

    "use strict";

    // --------

    var app = win.$.Plato,
        featureId = "Plato.Docs";

    // --------

    ///* stickySideBars */
    //var stickySideBars = function () {

    //    var dataKey = "stickySideBars",
    //        dataIdKey = dataKey + "Id";

    //    // Default options
    //    var defaults = {
    //        stickyHeaders: true,
    //        stickySideBars: true            
    //    };

    //    // CSS selectors for various layout elements
    //    var selectors = {
    //        header: ".layout-header",
    //        body: ".layout-body",
    //        content: ".layout-content",
    //        sideBar: ".layout-sidebar",            
    //        sideBarContent: ".layout-sidebar-content",
    //        footer: ".layout-footer"
    //    };

    //    var methods = {
    //        init: function ($caller, methodName, func) {

    //            if (methodName) {
    //                if (this[methodName]) {
    //                    return this[methodName].apply(this, [$caller, func]);
    //                } else {
    //                    alert(methodName + " is not a valid method!");
    //                }
    //                return;
    //            }

    //            this.bind($caller);

    //        },
    //        bind: function ($caller) {

    //            // Detect key pages
    //            var isIndex = $(".doc-index").length > 0 ? true : false,
    //                isDisplay = $(".doc-display").length > 0 ? true : false,
    //                isCategoryIndex = $(".doc-categories-index").length > 0 ? true : false,
    //                isCategoryDisplay = $(".doc-categories-display").length > 0 ? true : false;

    //            // Only allow sticky sidebars on key pages
    //            if (!isIndex &&
    //                !isDisplay &&
    //                !isCategoryIndex &&
    //                !isCategoryDisplay) {
    //                return;
    //            }

    //            // Layout elements
    //            var $header = $caller.find(selectors.header),
    //                $body = $caller.find(selectors.body),
    //                $content = $caller.find(selectors.content),
    //                $sideBar = $caller.find(selectors.sideBar),
    //                $sideBarContent = $sideBar.find(selectors.sideBarContent),                                        
    //                $footer = $caller.find(selectors.footer);

    //            // Layout options
    //            var sidebarOffsetTop = 0,
    //                stickyHeaders = $caller.data(dataKey).stickyHeaders,
    //                stickySideBars = $caller.data(dataKey).stickySideBars;

    //            // If we don't find our sticky elements disable flags
    //            if ($header.length === 0) { stickyHeaders = false; }
    //            if ($sideBar.length === 0) { stickySideBars = false; }
                
    //            // Apply sticky headers
    //            if (stickyHeaders) {

    //                // Default offset for sticky sidebars
    //                sidebarOffsetTop = $header.outerHeight();

    //                // Important: Set initial height of header
    //                // This ensures other calculations are correct
    //                $header.css({
    //                    "height": sidebarOffsetTop
    //                });
                    
    //            }

              

    //        },
    //        unbind: function ($caller) {
    //            $().sticky("unbind");
    //        }
    //    };

    //    return {
    //        init: function () {

    //            var options = {},
    //                methodName = null,
    //                func = null;
    //            for (var i = 0; i < arguments.length; ++i) {
    //                var a = arguments[i];
    //                switch (a.constructor) {
    //                    case Object:
    //                        $.extend(options, a);
    //                        break;
    //                    case String:
    //                        methodName = a;
    //                        break;
    //                    case Function:
    //                        func = a;
    //                        break;
    //                }
    //            }

    //            if (this.length > 0) {
    //                // $(selector).stickySideBars()
    //                return this.each(function () {
    //                    if (!$(this).data(dataIdKey)) {
    //                        var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
    //                        $(this).data(dataIdKey, id);
    //                        $(this).data(dataKey, $.extend({}, defaults, options));
    //                    } else {
    //                        $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
    //                    }
    //                    return methods.init($(this), methodName, func);
    //                });
    //            } else {
    //                // $().stickySideBars()
    //                var $caller = $(".layout");
    //                if ($caller.length > 0) {
    //                    if (!$caller.data(dataIdKey)) {
    //                        var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
    //                        $caller.data(dataIdKey, id);
    //                        $caller.data(dataKey, $.extend({}, defaults, options));
    //                    } else {
    //                        $caller.data(dataKey, $.extend({}, $caller.data(dataKey), options));
    //                    }
    //                    return methods.init($caller, methodName, func);
    //                }
    //            }

    //        }
    //    };

    //}();

    ///* Register plug-ins */
    //$.fn.extend({       
    //    stickySideBars: stickySideBars.init
    //});

    // --------

    var docs = {
        init: function () {
            app.logger.logInfo(featureId + " initializing");
            this.bind();
            app.logger.logInfo(featureId + " initialized");
        },
        bind: function () {
        }
    };

    // --------

    // Ready
    app.ready(function () {

        // Initialize docs
        docs.init();

        //// Initialize sticky sidebars
        //$(".layout").stickySideBars({
        //    stickyHeaders: app.defaults.layout.stickyHeaders,
        //    stickySideBars: app.defaults.layout.stickySideBars
        //});

    });
    
    // infinite scroll load
    $().infiniteScroll(function ($ele) { }, "ready");
    
}(window, document, jQuery));
