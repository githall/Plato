// <reference path="~/js/app/plato.js" />
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
            navigation: '.anchorific', // position of navigation
            headers: 'h1, h2, h3, h4, h5, h6', // custom headers selector                
            anchorClass: 'anchor text-muted', // class of anchor links                
            anchorTitle: null, // title text displayed when you hover over an anchor link
            iconCss: "fal fa-link", // the anchor link header icon
            spy: true, // scroll spy enabled
            position: 'append', // position of anchor icon relative to the header
            spyOffset: $().layout("getHeaderHeight") || !0, // specify heading offset for spy scrolling          
            onClick: function ($caller, $header, $link) { },
            emptyText: "No sections found!"
        };

        var methods = {
            init: function ($caller, methodName, func) {

                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
                        return this[methodName].apply(this, [$caller, func]);
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
                    $navs = $(navSelector),
                    navigations = function () { };

                // Ensure we have navigation                
                if ($navs.length > 0) {
                    if (this.headers.length > 0) {      
                        $navs.empty().append('<ul />');
                        navigations = function ($el, $nav, obj) {
                            return self.navigations($el, $nav, obj);
                        };
                    } else {
                        $nav.empty().text($caller.data(dataKey).emptyText);
                    }
                }

                // Build navigation & anchors
                for (var i = 0; i < self.headers.length; i++) {
                    obj = self.headers.eq(i);

                    for (var x = 0; x < $navs.length; x++) {   
                        var $nav = $($navs[x]);                       
                        self.previous = $nav.find('ul').last();
                        navigations($caller, $nav, obj);
                    }

                    self.anchor($caller, obj);
                }

            },
            navigations: function ($caller, $nav, obj) {

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
                    var href = $(this).attr("href"),
                        $header = $caller.find(href);
                    if ($header.length > 0) {
                        // Scroll to header for anchor                        
                        $().scrollTo({
                            offset: -$caller.data(dataKey).spyOffset,
                            target: $header,
                            onComplete: function () { }
                        }, "go");
                        if ($caller.data(dataKey).onClick) {
                            $caller.data(dataKey).onClick($caller, $header, $(this));
                        }
                    }
                });

                list = $('<li />').append(link);

                which = parseInt(obj.prop('nodeName').substring(1), null);
                list.attr('data-tag', which);

                self.subheadings($caller, $nav, which, list);

                self.first = which;

            },
            subheadings: function ($caller, $nav, which, a) {

                var self = this,
                    
                    ul = $nav.find('ul'),
                    li = $nav.find('li');

                if (which === self.first) {
                    self.previous.append(a);
                } else if (which > self.first) {
                    li.last().append('<ul />');
                    // can't use cache ul; need to find ul once more
                    $($nav).find('ul').last().append(a);
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
                    var href = $(this).attr("href"),
                        $header = $caller.find(href);
                    if ($header.length > 0) {
                        // Scroll to header for anchor
                        $().scrollTo({
                            offset: -$caller.data(dataKey).spyOffset,
                            target: $header
                        }, "go");
                        if ($caller.data(dataKey).onClick) {
                            $caller.data(dataKey).onClick($caller, $header, $(this));
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
                    prev,
                    offset = $caller.data(dataKey).spyOffset,
                    set = function () {

                        // get the header on top of the viewport
                        current = self.headers.map(function (e) {
                            if ($(this).offset().top - offset - $(win).scrollTop() < offset) {
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
                    };

                set();
                $(win).scroll(function (e) {
                    set();
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

    var app = win.$.Plato;

    app.ready(function () {

        // Locals
        var $elem = $(".entity-body"),
            $target = null,
            opts = {
                anchorTitle: app.T("Link to this section"),
                emptyText: app.T("No sections found"),
                onClick: function ($caller, $header, $link) {
                    // Track targeted header so we can update the
                    // history state after infiniteScrolls scrollEnd event
                    $target = $header;
                }
            };

        // Apply anchorific
        $elem.anchorific(opts);

        // Update state when an anchor is clicked
        var updateState = function () {

            // Ensure we have a targeted header
            if (!$target) {
                return;
            }

            // Get canonical URL or use current URL if not available
            var hash = "",
                $canonical = $('link[rel="canonical"]'),
                url = $canonical.length > 0
                    ? $canonical.attr('href')
                    : win.location.href.split("#")[0];

            // Build new anchor from targeted header using the header id
            if ($target.attr("id")) {
                hash = "#" + $target.attr("id");
            }

            // Replace URL state
            if (url && url !== "") {
                win.history.replaceState(win.history.state || {}, doc.title, url + hash);
            }

            // Clear header, until next time...
            $target = null;

        };

        // Ensure infiniteScroll is available
        if ($('[data-provide="infiniteScroll"]').length > 0) {
            if ($().infiniteScroll) {
                // Update state if anchor was clicked after infiniteScrolls scrollEnd event
                $().infiniteScroll("scrollEnd", function () {
                    updateState();
                });
            }
        } else {
            // Update state after scroll when infiniteScroll is not available
            $().scrollSpy({
                onScrollEnd: function () {
                    updateState();
                }
            });
        }

        // Activate anchorific when previewing within markdown editor
        if ($().markdown) {
            $().markdown("preview", function ($elem) {
                $elem.anchorific(opts);
            });
        }

    });

})(jQuery, window, document);