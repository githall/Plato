
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
                spyOffset: !0, // specify heading offset for spy scrolling
                onAnchorClick: null
            };

            var methods = {    
                opt: defaults,
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
                                              
                    this.headers = $caller.find($caller.data(dataKey).headers);
                    this.previous = 0;

                    if (this.headers.length !== 0) {
                        this.first = parseInt(this.headers.prop('nodeName').substring(1), null);
                    }

                    this.build($caller);

                },
                build: function ($caller) {

                    var self = this,
                        obj,
                        navigations = function () { };

                    // when navigation configuration is set
                    if ($caller.data(dataKey).navigation) {
                        $($caller.data(dataKey).navigation).append('<ul />');
                        self.previous = $($caller.data(dataKey).navigation).find('ul').last();
                        navigations = function ($caller, obj) {
                            return self.navigations($caller, obj);
                        };
                    }

                    for (var i = 0; i < self.headers.length; i++) {
                        obj = self.headers.eq(i);
                        navigations($caller, obj);
                        self.anchor($caller, obj);
                    }

                    if (self.opt.spy) {
                        self.spy($caller);
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
                        
                    link = $('<a />').attr('href', '#' + name).text(obj.text());
                    link.click(function (e) {
                        if ($caller.data(dataKey).onAnchorClick) {
                            $caller.data(dataKey).onAnchorClick($(this), e);
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
                        ul = $($caller.data(dataKey).navigation).find('ul'),
                        li = $($caller.data(dataKey).navigation).find('li');

                    if (which === self.first) {
                        self.previous.append(a);
                    } else if (which > self.first) {
                        li.last().append('<ul />');
                        // can't use cache ul; need to find ul once more
                        $($caller.data(dataKey).navigation).find('ul').last().append(a);
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

                    var self = this,
                        name = self.name(obj),
                        anchor,
                        text = $caller.data(dataKey).anchorText,
                        klass = $caller.data(dataKey).anchorClass,
                        id;

                    if (obj.attr('id') === undefined) {
                        obj.attr('id', name);
                    }

                    id = obj.attr('id');

                    anchor = $('<a />').attr('href', '#' + id).html(text).addClass(klass);
                    anchor.click(function (e) {                                   
                        if ($caller.data(dataKey).onAnchorClick) {
                            $caller.data(dataKey).onAnchorClick($(this), e);
                        }                        
                    });

                    if ($caller.data(dataKey).position === 'append') {
                        obj.append(anchor);
                    } else {
                        obj.prepend(anchor);
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
                            if (($(this).offset().top - $(win).scrollTop()) < $caller.data(dataKey).spyOffset) {
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
        $currentAnchor = null,
        offset = 120,
        opts = {
            navigation: '.anchorific', // position of navigation
            headers: 'h1, h2, h3, h4, h5, h6', // headers that you wish to target      
            anchorClass: 'anchor', // class of anchor links
            anchorText: '#', // prepended or appended to anchor headings         
            spy: true, // scroll spy
            position: 'append', // position of anchor text
            spyOffset: offset, // specify heading offset for spy scrolling
            onAnchorClick: function ($anchor, e) {          

                e.preventDefault();
                e.stopPropagation();

                $currentAnchor = $anchor;

                // Scroll to clicked anchor
                $().scrollTo({
                    offset: -offset,
                    target: $anchor
                }, "go");

            }
        };

    app.ready(function () {

        // Add table of contents generated from headers
        //$("body").append($('<nav class="anchorific"></nav>'));        

        // anchorific
        $('[data-provide="markdownBody"]')
            .anchorific(opts);        
     
        // Activate anchorific when loaded via infiniteScroll load
        $().infiniteScroll({
            onStateUpdated: function ($caller, state, stateParts) {

                // The infiniteScroll plug-in will update the browser state
                // to include the first infiniteScroll marker offset visible 
                // within the viewport, here we override this and instead
                // get the parent marker offset for the anchor we've clicked                
            
                // We need an anchor to update the state
                if ($currentAnchor === null) {
                    return;
                }

                var $card = $currentAnchor.closest(".card"),
                    $parent = $card.find('[data-infinite-scroll-offset]');

                var offset = "";
                if ($parent.length > 0) {
                    var o = parseInt($parent.data("infiniteScrollOffset"));
                    if (!isNaN(o)) {
                        offset = "/" + o;
                    }
                }
               
                var anchorUrl = stateParts.parts.url +
                    offset +
                    stateParts.parts.qs +
                    $currentAnchor.attr("href");
                history.replaceState(state, doc.title, anchorUrl);

            }
        }, function ($ele) {
            $ele.find('[data-provide="markdownBody"]')
                .anchorific(opts);
        }, "ready");

    });

})(jQuery, window, document);