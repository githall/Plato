﻿
if (typeof jQuery === "undefined") {
    throw new Error("Plato requires jQuery");
}

if (typeof $.Plato.Context === "undefined") {
    throw new Error("$.Plato.Context Required");
}

/* markdown */
$(function (win, doc, $) {

    'use strict';
    
    var markdown = function () {

        var dataKey = "markdownEditor",
            dataIdKey = dataKey + "Id";

        var defaults = {
            event: "show.bs.tab"
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
                
                methods.bind($caller);
                
            },
            getUniqueId: function ($caller) {

                return parseInt($caller.attr("data-markdown-id")) || 0;
            },
            bind: function ($caller) {
                
                var event = $caller.data(dataKey).event;
                if (event) {
                    var $tabs = $caller.find('a[data-toggle="tab"]');
                    $tabs.on(event,
                        function(e) {
                            methods.showTab($caller, e.target.href.split("#")[1]);
                        });
                }

            },
            showTab: function($caller, tabId) {

                var id = this.getUniqueId($caller);

                switch (tabId) {
                    case "write_" + id:
                    break;
                    case "preview_" + id:

                    var $editor = $caller.find("textarea");
                    this.getHtml({
                            markdown: $editor.val()
                        },
                        function (data) {
                            if (data.statusCode === 200) {
                                $caller
                                    .find("#preview_" + id)
                                    .empty()
                                    .html(data.html);
                            }
                        });

                   
                    break;
                }

            },
            getHtml: function (params, fn) {

                win.$.Plato.Http({
                    url: "/api/markdown/parse",
                    method: "GET",
                    async: false,
                    data: params
                }).done(function (data) {
                    fn(data);
                });

            }
            //show: function ($caller) {

            //    var delay = $caller.data("notify").delay || $caller.data("notifyDelay");

            //    var $target = notify.getElement($caller);
            //    $target.addClass("i-notify-visible");

            //    if (delay > 0) {
            //        win.setTimeout(function () {
            //            notify.hide($caller);
            //        },
            //            delay);
            //    }

            //},
            //hide: function ($caller) {
            //    $(".i-notify").removeClass("i-notify-visible");
            //},
            //getElement: function ($caller) {

            //    var text = $caller.data("notifyText") || $caller.data("notify").text,
            //        css = $caller.data("notifyCss") || $caller.data("notify").css,
            //        closeButton = $caller.data("notifyCloseButton") || $caller.data("notify").closeButton,
            //        iconCss = $caller.data("notifyIconCss") || $caller.data("notify").iconCss;

            //    // create alert html
            //    var s = "<div class=\"" + css + "\"><div>";
            //    s += (iconCss ? "<i class=\"" + iconCss + "\"></i>" : "");
            //    s += text;

            //    if (closeButton === true) {
            //        s += "<a class=\"i-notify-close\" href=\"#\"><i class=\"fa fa-times\"></i></a>";
            //    }
            //    s += "</div></div>";

            //    // create and add to dom
            //    var $alert = $(s);
            //    $("body").append($alert);

            //    $("body")
            //        .find(".i-notify-close")
            //        .unbind("click")
            //        .bind("click",
            //            function (e) {
            //                e.preventDefault();
            //                $($caller).inotify("hide");
            //            });

            //    return $alert;
            //}
        }

        return {
            init: function () {

                var options = {};
                var methodName = null;
                for (var i = 0; i < arguments.length; ++i) {
                    var a = arguments[i];
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
                
                if (this.length > 0) {
                    // $(selector).markdownEditor
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
                    // $().markdownEditor 
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

        }

    }();

    $.fn.extend({
        markdownEditor: markdown.init
    });
    
    $(doc).ready(function () {

        $('[data-provide="markdown-container"]')
            .markdownEditor();
     
    });

}(window, document, jQuery));
