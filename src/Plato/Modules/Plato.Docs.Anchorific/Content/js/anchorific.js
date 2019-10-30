// <reference path="~/js/app/plato.js" />
// <reference path="~/js/vendors/jquery.js" />
// <reference path="~/js/vendors/bootstrap.js" />

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

        var $elem = $(".doc-body"),
            opts = {
                title: app.T("Contents"),
                anchorTitle: app.T("Link to this section")
            };

        // Apply anchorific only to entity bodies
        $elem.anchorific(opts);

        // Update state if anchor was clicked after scrollEnd event
        $().infiniteScroll("scrollEnd", function () {

            var $header = $elem.anchorific("getHeader");

            // Ensure we have a clicked anchor
            if (!$header) {
                return;
            }

            // Get url minus any existing anchor
            var url = win.location.href.split("#")[0];
            var hash = "";
            if ($header.attr("id")) {
                hash = "#" + $header.attr("id");
            }

            // Replace state
            if (url !== "") {           
                win.history.replaceState(win.history.state || {}, doc.title, url + hash);
            }          

            $elem.anchorific("clearHeader");

        });

        // Activate anchorific when loaded via infiniteScroll load
        $().infiniteScroll("ready", function ($ele) {
            $(".doc-body").anchorific(opts);            
        });

    });

})(jQuery, window, document);