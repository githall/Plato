﻿// <reference path="/wwwroot/js/app.js" />

if (typeof window.jQuery === "undefined") {
    throw new Error("jQuery 3.3.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

if (typeof window.$.fn.platoUI === "undefined") {
    throw new Error("$.Plato UI Required");
}

$(function (win, doc, $) {

    'use strict';

    // --------

    var app = win.$.Plato;

    // --------
    
    // references
    var references = function () {

        var dataKey = "references",
            dataIdKey = dataKey + "Id";

        var defaults = {};

        var methods = {
            init: function ($caller, methodName, func) {

                if (methodName) {
                    if (this[methodName]) {
                        this[methodName].apply(this, [$caller, func]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return;
                }

                this.bind($caller);

            },
            bind: function ($caller) {

                var marker = "#";

                $caller.suggester($.extend($caller.data(dataKey)),
                    {
                        // keyBinder options
                        keys: [
                            {
                                match: /(^|\s|\()(#{1}([a-z0-9\-_/]*))$/i,
                                search: function ($input, selection) {

                                    // The result of the search method is tested
                                    // against the match regular expression within keyBinder
                                    // If a match is found the bind method is called 
                                    // otherwise unbind method is called
                                    // This code executes on every key press so should be highly optimized

                                    var chars = $input.val().split(""),
                                        value = null,                                        
                                        startIndex = -1,
                                        start = selection.start - 1,
                                        i;

                                    // Search backwards from caret for marker, until 
                                    // terminators & attempt to get marker position
                                    for (i = start; i >= 0; i--) {
                                        if (chars[i] === marker) {
                                            startIndex = i;
                                            break;
                                        } else {
                                            if (chars[i] === "\n" || chars[i] === " ") {
                                                break;
                                            }
                                        }
                                    }

                                    // Is the character before our marker also a marker?
                                    // For example are we adding a markdown header tag 
                                    // i.e. (## header 2, ### header 3, #### header 4 etc)
                                    var prevChar = startIndex > 0 ? chars[startIndex - 1] : "";                                    
                                    if (prevChar === marker) {
                                        // Reset
                                        startIndex = -1;
                                        value = null;
                                    }

                                    // If we have a marker position search forward from
                                    // the marker position until a terminator to get value
                                    if (startIndex >= 0) {
                                        value = "";
                                        for (i = startIndex; i <= chars.length - 1; i++) {
                                            if (chars[i] === "\n" || chars[i] === " ") {
                                                break;
                                            }
                                            value += chars[i];
                                        }
                                    }                              

                                    return {
                                        startIndex: startIndex,
                                        value: value
                                    };

                                },
                                bind: function ($input, searchResult, e) {
                                    
                                    var keywords = searchResult.value;

                                    // Ensure we have a value
                                    if (!keywords) {
                                        return;
                                    }

                                    // Remove any marker prefix from search keywords
                                    if (keywords.substring(0, 1) === marker) {
                                        keywords = keywords.substring(1, keywords.length);
                                    }
                                    
                                    // Invoke suggester
                                    $caller.suggester({
                                        // pagedList options
                                        page: 1,
                                        pageSize: 5,
                                        itemSelection: {
                                            enable: true,
                                            index: 0,
                                            css: "active"
                                        },
                                        valueField: "keywords",
                                        config: {
                                            method: "GET",
                                            url: win.$.Plato.defaults.pathBase + '/api/search/get?page={page}&size={pageSize}&keywords=' +
                                                encodeURIComponent(keywords),
                                            data: {
                                                sort: "ModifiedDate",
                                                order: "Desc"
                                            }
                                        },
                                        itemCss: "dropdown-item",
                                        itemTemplate:
                                            '<a class="{itemCss}" href="{url}"><div style=\"display:inline-block; width: 85%; overflow:hidden; text-overflow: ellipsis;\"><span class="avatar avatar-sm mr-2"><span style="background-image: url({createdBy.avatar.url});"></span></span>{title}</div>{relevance}</a>',
                                        parseItemTemplate: function (html, result) {

                                            if (result.id) {
                                                html = html.replace(/\{id}/g, result.id);
                                            } else {
                                                html = html.replace(/\{id}/g, "0");
                                            }

                                            if (result.title) {
                                                html = html.replace(/\{title}/g, result.title);
                                            } else {
                                                html = html.replace(/\{title}/g, "(no title)");
                                            }
                                            if (result.excerpt) {
                                                html = html.replace(/\{excerpt}/g, result.userName);
                                            } else {
                                                html = html.replace(/\{excerpt}/g, "(no excerpt)");
                                            }

                                            if (result.url) {
                                                html = html.replace(/\{url}/g, result.url);
                                            } else {
                                                html = html.replace(/\{url}/g, "#");
                                            }

                                            if (result.relevance) {
                                                if (result.relevance > 0) {
                                                    html = html.replace(/\{relevance}/g,
                                                        '<span class="float-right badge badge-primary" data-provide="tooltip" title="Relevancy">' + result.relevance + '%</span>');
                                                } else {
                                                    html = html.replace(/\{relevance}/g, "");
                                                }

                                            } else {
                                                html = html.replace(/\{relevance}/g, "");
                                            }

                                            if (result.createdBy) {

                                                if (result.createdBy.avatar) {
                                                    if (result.createdBy.avatar.url) {
                                                        html = html.replace(/\{createdBy.avatar.url}/g, result.createdBy.avatar.url);
                                                    } else {
                                                        html = html.replace(/\{createdBy.avatar.url}/g, "#");
                                                    }
                                                }
                                            }

                                            if (result.modifiedBy) {

                                                if (result.modifiedBy.avatar) {
                                                    if (result.modifiedBy.avatar.url) {
                                                        html = html.replace(/\{modifiedBy.avatar.url}/g, result.modifiedBy.avatar.url);
                                                    } else {
                                                        html = html.replace(/\{modifiedBy.avatar.url}/g, "#");
                                                    }
                                                }
                                            }

                                            if (result.lastReplyBy) {

                                                if (result.lastReplyBy.avatar) {
                                                    if (result.lastReplyBy.avatar.url) {
                                                        html = html.replace(/\{lastReplyBy.avatar.url}/g, result.lastReplyBy.avatar.url);
                                                    } else {
                                                        html = html.replace(/\{lastReplyBy.avatar.url}/g, "#");
                                                    }
                                                }
                                            }


                                            return html;
                                        },
                                        onPagerClick: function ($self, page, e) {
                                            e.preventDefault();
                                            e.stopPropagation();                                         
                                            $caller.suggester({
                                                page: page
                                            },
                                                "show");
                                        },
                                        onItemClick: function ($self, result, e) {

                                            // Prevent default event
                                            e.preventDefault();

                                            // Focus input, hide suggest & insert result
                                            $caller
                                                .focus()
                                                .suggester("hide")
                                                .suggester({
                                                    insertData: {
                                                        index: searchResult.startIndex,
                                                        value: result.id + "(" + result.title + ")"
                                                    }
                                                },
                                                    "insert");

                                        }
                                    },
                                        "show");

                                },
                                unbind: function ($input, key, e) {
                                    $caller.suggester("hide");
                                }
                            }
                        ]
                    },
                    defaults);
            }
        };

        return {
            init: function () {

                var options = {},
                    methodName = null,
                    func = null;
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
                            case Function:
                                func = a;
                                break;
                        }
                    }
                }

                if (this.length > 0) {
                    // $(selector).references
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
                    // $().references
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

    $.fn.extend({
        references: references.init
    });

    app.ready(function () {

        // #Id
        $('[data-provide="references"]').references();

        // #Id
        $('.md-textarea').references();

        // bind suggesters
        $('.md-textarea').keyBinder("bind");

    });

}(window, document, jQuery));
