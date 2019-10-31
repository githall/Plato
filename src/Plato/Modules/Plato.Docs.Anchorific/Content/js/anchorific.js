// <reference path="~/js/app/plato.js" />
// <reference path="~/js/vendors/jquery.js" />

if (typeof window.jQuery === "undefined") {
    throw new Error("jQuery 3.3.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

(function ($, win, doc) {

    "use strict";

    // ---------------

    var app = win.$.Plato;        

    app.ready(function () {        

        // Locals
        var $elem = $(".doc-body"),
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

        // Update state if anchor was clicked after infiniteScrolls scrollEnd event
        $().infiniteScroll("scrollEnd", function () {
            
            // Ensure we have a targeted header
            if (!$target) {
                return;
            }

            // Get url minus any existing anchor tag
            var hash = "",
                url = win.location.href.split("#")[0];
            // Build new anchor from targeted header using the header id
            if ($target.attr("id")) {
                hash = "#" + $target.attr("id");
            }

            // Replace url state
            if (url && url !== "") {           
                win.history.replaceState(win.history.state || {}, doc.title, url + hash);
            }          

            // Clear header, until next time...
            $target = null;

        });

        // Activate anchorific when loaded via infiniteScroll load
        $().infiniteScroll("ready", function ($ele) {
            $elem.anchorific(opts);            
        });

    });

})(jQuery, window, document);