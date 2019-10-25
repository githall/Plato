/*
	The MIT License (MIT)

	Copyright (c) <2013> <Ren Aysha>

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/

if (typeof Object.create !== 'function') {
    Object.create = function (obj) {
        function F() { }
        F.prototype = obj;
        return new F();
    };
}

(function ($, win, doc, undefined) {

    "use strict";

    var state = win.history.state || {},
        anchorific = function (options) {

            var dataKey = "anchorific",
                dataIdKey = dataKey + "Id";

            var defaults = {
                navigation: '.anchorific', // position of navigation
                headers: 'h1, h2, h3, h4, h5, h6', // custom headers selector
                speed: 200, // speed of sliding back to top
                anchorClass: 'anchor', // class of anchor links
                anchorText: '#', // prepended or appended to anchor headings
                top: '.top', // back to top button or link class
                spy: true, // scroll spy
                position: 'append', // position of anchor text
                spyOffset: !0 // specify heading offset for spy scrolling
            };

            var methods = {
                $anchor: null,
                init: function ($caller, methodName) {

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

                    var self = this;
                    self.$elem = $caller;
                    self.opt = $.extend({}, defaults, options);
                    self.headers = self.$elem.find(self.opt.headers);
                    self.previous = 0;

                    if (self.headers.length !== 0) {
                        self.first = parseInt(self.headers.prop('nodeName').substring(1), null);
                    }

                    self.build();

                },
                build: function () {

                    var self = this,
                        obj,
                        navigations = function () { };

                    // when navigation configuration is set
                    if (self.opt.navigation) {
                        $(self.opt.navigation).append('<ul />');
                        self.previous = $(self.opt.navigation).find('ul').last();
                        navigations = function (obj) {
                            return self.navigations(obj);
                        };
                    }

                    for (var i = 0; i < self.headers.length; i++) {
                        obj = self.headers.eq(i);
                        navigations(obj);
                        self.anchor(obj);
                    }

                    if (self.opt.spy)
                        self.spy();
                },
                navigations: function (obj) {
                    var self = this, link, list, which, name = self.name(obj);

                    if (obj.attr('id') !== undefined)
                        name = obj.attr('id');

                    link = $('<a />').attr('href', '#' + name).text(obj.text());
                    link.click(function (e) {
                        e.preventDefault();
                        $().scrollTo({
                            offset: -120,
                            target: $('#' + name),
                            onComplete: function () {

                            }
                        }, "go"); // initialize scrollTo
                    });

                    list = $('<li />').append(link);

                    which = parseInt(obj.prop('nodeName').substring(1), null);
                    list.attr('data-tag', which);

                    self.subheadings(which, list);

                    self.first = which;
                },
                subheadings: function (which, a) {
                    var self = this, ul = $(self.opt.navigation).find('ul'),
                        li = $(self.opt.navigation).find('li');

                    if (which === self.first) {
                        self.previous.append(a);
                    } else if (which > self.first) {
                        li.last().append('<ul />');
                        // can't use cache ul; need to find ul once more
                        $(self.opt.navigation).find('ul').last().append(a);
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
                anchor: function (obj) {

                    var self = this,
                        name = self.name(obj),
                        anchor,
                        text = self.opt.anchorText,
                        klass = self.opt.anchorClass,
                        id;

                    if (obj.attr('id') === undefined) {
                        obj.attr('id', name);
                    }

                    id = obj.attr('id');

                    anchor = $('<a />').attr('href', '#' + id).html(text).addClass(klass);
                    anchor.click(function (e) {

                        e.preventDefault();
                        e.stopPropagation();

                        // Set clicked anchor
                        methods.updateAnchor($(this));

                        // Scroll to clicked anchor
                        $().scrollTo({
                            offset: -120,
                            target: methods.$anchor                           
                        }, "go");

                    });

                    if (self.opt.position === 'append') {
                        obj.append(anchor);
                    } else {
                        obj.prepend(anchor);
                    }
                },
                spy: function () {
                    var self = this, previous, current, list, top, prev;

                    $(win).scroll(function (e) {

                        // get the header on top of the viewport
                        current = self.headers.map(function (e) {
                            if (($(this).offset().top - $(win).scrollTop()) < self.opt.spyOffset) {
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
                updateAnchor: function ($anchor) {
                    this.$anchor = $anchor;
                },
                updateState: function () {

                    // We need an anchor to update the state
                    if (this.$anchor === null) {
                        return;
                    }

                    var $card = this.$anchor.closest(".card"),
                        $parent = $card.find('[data-infinite-scroll-offset]');

                    var offset = 0;
                    if ($parent.length > 0) {
                        var o = parseInt($parent.data("infiniteScrollOffset"));
                        if (!isNaN(o)) {
                            offset = o;
                        }
                    }

                    console.log(offset);

                }
            };

            return {
                init: function () {

                    var options = {};
                    var methodName = null;
                    var func = null;
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
                            methods.init($(this), methodName, func);
                        });
                    } else {
                        // $().anchorific()
                        var $caller = $('[data-provide="infiniteScroll"]');
                        if ($caller.length > 0) {
                            if (!$caller.data(dataIdKey)) {
                                var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                                $caller.data(dataIdKey, id);
                                $caller.data(dataKey, $.extend({}, defaults, options));
                            } else {
                                $caller.data(dataKey, $.extend({}, $caller.data(dataKey), options));
                            }
                            methods.init($caller, methodName, func);
                        }

                    }

                }
            };

        }();

    $.fn.extend({
        anchorific: anchorific.init
    });

    // ---------------

    var app = win.$.Plato,
        opts = {
            navigation: '.anchorific', // position of navigation
            headers: 'h1, h2, h3, h4, h5, h6', // headers that you wish to target
            speed: 200, // speed of sliding back to top
            anchorClass: 'anchor', // class of anchor links
            anchorText: '#', // prepended or appended to anchor headings         
            spy: false, // scroll spy
            position: 'append', // position of anchor text
            spyOffset: 120 // specify heading offset for spy scrolling
        };

    app.ready(function () {

        // Add table of contents generated from headers
        //$("body").append($('<nav class="anchorific"></nav>'));        

        // anchorific
        $('[data-provide="markdownBody"]')
            .anchorific(opts);        
     
        // Activate anchorific when loaded via infiniteScroll load
        $().infiniteScroll({
            onStateUpdated: function (state) {

                // The infiniteScroll plug-in will update the browser state
                // to include the first infiniteScroll marker offset visible 
                // within the viewport, for this reason we override this 
                

                $('[data-provide="markdownBody"]')
                    .anchorific("updateState");

            }
        }, function ($ele) {
            $ele.find('[data-provide="markdownBody"]')
                .anchorific(opts);
        }, "ready");

    });

})(jQuery, window, document);