// <reference path="/wwwroot/js/app.js" />

if (typeof window.jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

if (typeof window.$.fn.platoUI === "undefined") {
    throw new Error("$.Plato UI Required");
}

$(function (win, doc, $) {
    
    "use strict";


    // --------

    var app = win.$.Plato,
        featureId = "Plato.Site";

    // --------

    /* gitHubButton */
    var gitHubButton = function () {

        var dataKey = "gitHubButton",
            dataIdKey = dataKey + "Id";

        var defaults = {
            url: "https://api.github.com/repos/InstantASP/plato"
        };
        
        var methods = {
            _xhr: null,            
            init: function ($caller, methodName, func) {

                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
                        return this[methodName].apply(this, [$caller, func]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                this.bind($caller);

            },
            bind: function ($caller) {                               

                function update() {
                    var stars = $caller.find(".star-count");
                    if (methods._xhr) {
                        if (methods._xhr.stargazers_count) {
                            var count = methods._xhr.stargazers_count,
                                text = count + " stargazers";
                            stars.attr("aria-label", text).text(count);
                            stars.show();
                        } else {
                            stars.hide();
                        }
                    } else {
                        stars.hide();
                    }                    
                }

                if (methods._xhr === null) {
                    $.ajax({
                        url: $caller.data(dataKey).url,
                        method: "GET"
                    }).always(function (xhr, textStatus) {
                        methods._xhr = xhr;
                        update();
                    }).fail(function (xhr, ajaxOptions, thrownError) {
                        methods._xhr = null;
                        update();
                    });
                } else {
                    update();
                }

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
                    // $(selector).gitHubButton()
                    return this.each(function () {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        return methods.init($(this), methodName, func);
                    });
                } else {
                    // $().gitHubButton()
                    var $caller = $('[data-provide="leaveSpy"]');
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

    // --------

    $.fn.extend({
        gitHubButton: gitHubButton.init        
    });

    // --------

    var site = {
        init: function () {
            app.logger.logInfo(featureId + " initializing");
            this.bind();
            app.logger.logInfo(featureId + " initialized");
        },
        bind: function () {
            // Placeholder
        }
    };
    
    // --------

    // app ready
    app.ready(function () {

        // Init site
        site.init();

        // gitHubButton
        $(".github-button").gitHubButton();

    });

    // infinite scroll load
    $().infiniteScroll(function ($ele) { }, "ready");

}(window, document, jQuery));
