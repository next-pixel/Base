// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

$(function(){
    var hash = location.hash;
    var target = hash.length > 0 ? hash.substr(1) : "dashboard";
    var link = $(".navview-menu a[href*="+target+"]");
    var menu = link.closest("ul[data-role=dropdown]");
    var node = link.parent("li").addClass("active");

    function getContent(target){
        window.on_page_functions = [];
        $.get(target).then(
            function(response){
                $("#content-wrapper").html(response);

                window.on_page_functions.forEach(function(func){
                    Metro.utils.exec(func, []);
                });
            }
        );
    }

    getContent(target);

    if (menu.length > 0) {
        menu.data("dropdown").open();
    }

    $(".navview-menu").on(Metro.events.click, "a", function(e){
        var href = $(this).attr("href");
        var pane = $(this).closest(".navview-pane");
        var hash;

        if (Metro.utils.isValue(href) > -1) {
            document.location.href = href;
            return false;
        }

        if (href === "#") {
            return false;
        }

        hash = href.substr(1);
        href = hash;

        getContent(hash);

        if (pane.hasClass("open")) {
            pane.removeClass("open");
        }

        pane.find("li").removeClass("active");
        $(this).closest("li").addClass("active");

        window.history.pushState(href, href, "#"+hash);

        return false;
    })
});