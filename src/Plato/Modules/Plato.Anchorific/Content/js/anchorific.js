﻿// <reference path="~/js/app/plato.js" />
// <reference path="~/js/vendors/jquery.js" />

if (typeof window.jQuery === "undefined") {
    throw new Error("jQuery 3.3.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

(function ($, win, doc, undefined) {

    "use strict";

    var anchorific = function () {

        var dataKey = "anchorific",
            dataIdKey = dataKey + "Id";

        var defaults = {
            title: "Table of Contents", // the title for the automatically generated table of contents
            navigation: '.anchorific', // position of navigation
            headers: 'h1, h2, h3, h4, h5, h6', // custom headers selector                
            anchorClass: 'anchor text-muted', // class of anchor links                
            anchorTitle: null, // title text displayed when you hover over an anchor link
            iconCss: "fal fa-link", // the anchor link header icon
            spy: false, // scroll spy enabled
            position: 'append', // position of anchor icon relative to the header
            spyOffset: $().layout("getHeaderHeight") || !0 // specify heading offset for spy scrolling          
        };

        var methods = {
            _$header: null,
            init: function ($caller, methodName, func) {

                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
                        this[methodName].apply(this, [$caller, func]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return null;
                }

                this.bind($caller);

            },
            bind: function ($caller) {

                this.headers = $caller.find($caller.data(dataKey).headers);
                this.previous = 0;

                if (this.headers.length !== 0) {
                    this.first = parseInt(this.headers.prop('nodeName').substring(1), null);
                }

                // Build contents
                this.build($caller);

                // Bind spy event
                if ($caller.data(dataKey).spy) {
                    this.spy($caller);
                }

            },
            build: function ($caller) {

                var self = this,
                    obj,
                    navSelector = $caller.data(dataKey).navigation,
                    $nav = $(navSelector),
                    navigations = function () { };
                
                // Prepend .anchorific to caller if we don't have an existing .anchorific element
                if ($nav.length === 0) {
                    $caller.prepend($('<nav class="anchorific"></nav>'));
                    // Get added navigation
                    $nav = $(navSelector);
                }

                // Ensure we have navigation                
                if ($nav.length > 0) {                    
                    if (this.headers.length > 0) {
                        $nav.empty().append('<ul />');
                        self.previous = $nav.find('ul').last();
                        navigations = function ($el, obj) {
                            return self.navigations($el, obj);
                        };
                    } else {
                        $nav.empty().append('<ul />');
                    }
                }

                // Build navigation & anchors
                for (var i = 0; i < self.headers.length; i++) {
                    obj = self.headers.eq(i);
                    navigations($caller, obj);
                    self.anchor($caller, obj);
                }


            },
            navigations: function ($caller, obj) {

                var self = this,
                    link,
                    list,
                    which,
                    name = self.name(obj);

                if (obj.attr('id') !== undefined) {
                    name = obj.attr('id');
                }

                link = $('<a />')
                    .attr('href', '#' + name)
                    .text(obj.text());

                link.click(function (e) {

                    // Prevent defaults
                    e.preventDefault();
                    e.stopPropagation();

                    if ($(this).attr("href")) {
                        methods._$header = $($(this).attr("href"));
                        if (methods._$header.length > 0) {
                            // Scroll to header for anchor
                            $().scrollTo({
                                offset: -$caller.data(dataKey).spyOffset,
                                target: methods._$header
                            }, "go");
                        }
                    }

                });

                list = $('<li />').append(link);

                which = parseInt(obj.prop('nodeName').substring(1), null);
                list.attr('data-tag', which);

                self.subheadings($caller, which, list);

                self.first = which;
            },
            subheadings: function ($caller, which, a) {

                var self = this,
                    navSelector = $caller.data(dataKey).navigation,
                    ul = $(navSelector).find('ul'),
                    li = $(navSelector).find('li');

                if (which === self.first) {
                    self.previous.append(a);
                } else if (which > self.first) {
                    li.last().append('<ul />');
                    // can't use cache ul; need to find ul once more
                    $(navSelector).find('ul').last().append(a);
                    self.previous = a.parent();
                } else {
                    $('li[data-tag=' + which + ']').last().parent().append(a);
                    self.previous = a.parent();
                }
            },
            name: function (obj) {
                var name = obj.text().replace(/[^\w\s]/gi, '')
                    .replace(/\s+/g, '-')
                    .toLowerCase();
                return name;
            },
            anchor: function ($caller, obj) {

                var name = this.name(obj),
                    $anchor,
                    iconCss = $caller.data(dataKey).iconCss,
                    title = $caller.data(dataKey).anchorTitle,
                    css = $caller.data(dataKey).anchorClass,
                    id;

                if (obj.attr('id') === undefined) {
                    obj.attr('id', name);
                }
                id = obj.attr('id');

                $anchor = $('<a />', {
                    "href": '#' + id,
                    "class": css
                }).append($("<i />").addClass(iconCss));

                if (title) {
                    $anchor.attr("title", title);
                }

                $anchor.click(function (e) {

                    // Prevent defaults
                    e.preventDefault();
                    e.stopPropagation();

                    if ($(this).attr("href")) {
                        methods._$header = $($(this).attr("href"));
                        if (methods._$header.length > 0) {
                            // Scroll to header for anchor
                            $().scrollTo({
                                offset: -$caller.data(dataKey).spyOffset,
                                target: methods._$header
                            }, "go");
                        }
                    }

                });

                if ($caller.data(dataKey).position === 'append') {
                    obj.append($anchor);
                } else {
                    obj.prepend($anchor);
                }

            },
            spy: function ($caller) {

                var self = this,
                    previous,
                    current,
                    list,
                    top,
                    prev;

                $(win).scroll(function (e) {

                    // get the header on top of the viewport
                    current = self.headers.map(function (e) {
                        if ($(this).offset().top - $(win).scrollTop() < $caller.data(dataKey).spyOffset) {
                            return this;
                        }
                    });

                    // get only the latest header on the viewport
                    current = $(current).eq(current.length - 1);

                    if (current && current.length) {
                        // get all li tag that contains href of # ( all the parents )
                        list = $('li:has(a[href="#' + current.attr('id') + '"])');

                        if (prev !== undefined) {
                            prev.removeClass('active');
                        }

                        list.addClass('active');
                        prev = list;
                    }
                });
            },
            clearHeader: function ($caller) {
                methods._$header = null;
            },
            getHeader: function ($caller) {
                return methods._$header;
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
                    // $(selector).anchorific()
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
                    // $().anchorific()
                    var $caller = $(".entity-body");
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

    // ---------------

    $.fn.extend({
        anchorific: anchorific.init
    });

    // ---------------

    //var app = win.$.Plato;
    //app.ready(function () {
        
    //    var opts = {
    //        title: app.T("Contents"),
    //        anchorTitle: app.T("Link to this section")
    //    };

    //    // Apply anchorific only to entity bodies
    //    $(".entity-body").anchorific(opts);

    //    // Update state if anchor was clicked after scrollEnd event
    //    $().infiniteScroll("scrollEnd", function () {

    //        var $header = $(".entity-body").anchorific("getHeader");

    //        // Ensure we have a clicked anchor
    //        if (!$header) {
    //            return;
    //        }

    //        // Get url minus any existing anchor
    //        var url = win.location.href.split("#")[0];
    //        var hash = "";
    //        if ($header.attr("id")) {
    //            hash = "#" + $header.attr("id");
    //        }
            
    //        // Replace state
    //        if (url !== "") {           
    //            win.history.replaceState(win.history.state || {}, doc.title, url + hash);
    //        }

    //        $header = null;

    //    });

    //    // Activate anchorific when loaded via infiniteScroll load
    //    $().infiniteScroll("ready", function ($ele) {
    //        $(".entity-body").anchorific(opts);
    //        //$ele.find('[data-provide="markdownBody"]').anchorific(opts);
    //    });

    //});

})(jQuery, window, document);