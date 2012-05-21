/*
* jQuery pager plugin
* Version 1.0 (12/22/2008)
* @requires jQuery v1.2.6 or later
*
* Example at: http://jonpauldavies.github.com/JQuery/Pager/PagerDemo.html
*
* Copyright (c) 2008-2009 Jon Paul Davies
* Dual licensed under the MIT and GPL licenses:
* http://www.opensource.org/licenses/mit-license.php
* http://www.gnu.org/licenses/gpl.html
* 
* Read the related blog post and contact the author at http://www.j-dee.com/2008/12/22/jquery-pager-plugin/
*
* This version is far from perfect and doesn't manage it's own state, therefore contributions are more than welcome!
*
* Usage: .pager({ pagenumber: 1, pagecount: 15, buttonClickCallback: PagerClickTest });
*
* Where pagenumber is the visible page number
*       pagecount is the total number of pages to display
*       buttonClickCallback is the method to fire when a pager button is clicked.
*
* buttonClickCallback signiture is PagerClickTest = function(pageclickednumber) 
* Where pageclickednumber is the number of the page clicked in the control.
*
* The included Pager.CSS file is a dependancy but can obviously tweaked to your wishes
* Tested in IE6 IE7 Firefox & Safari. Any browser strangeness, please report.
*/
(function ($) {
    $.fn.pager = function (options) {
        var opts = $.extend({}, $.fn.pager.defaults, options);
        return this.each(function () {
            if (options.endPoint == undefined) {
                options.endPoint = 9;
            }
            if (options.thisPage == undefined) {
                options.thisPage = "true";
            }
            if (options.showEnd == undefined) {
                options.showEnd = "true";
            }
            if (options.showNext == undefined) {
                options.showNext = "true";
            }
            $(this).empty().append(renderpager(parseInt(options.pagenumber), parseInt(options.pagecount), parseInt(options.endPoint), options.showNext, options.showEnd, options.thisPage, options.buttonClickCallback));
            $(".pages li").mouseover(function () {
                document.body.style.cursor = "pointer";
            }).mouseout(function () {
                document.body.style.cursor = "auto";
            });
        });
    };
    function renderpager(pagenumber, pagecount, endPoint, showNext, showEnd, thisPage, buttonClickCallback) {
        var $pager = $("<ul class=\"pages\"></ul>");
        if (showEnd == "true") {
            $pager.append(renderButton("\u9996\u9875", pagenumber, pagecount, buttonClickCallback));
        }
        if (showNext == "true") {
            $pager.append(renderButton("\u4e0a\u4e00\u9875", pagenumber, pagecount, buttonClickCallback));
        }
        var startPoint = 1;
        var temp = 2;
        switch (endPoint) {
            case 3:
                temp = 1;
                break;
            case 5:
                temp = 2;
                break;
            case 7:
                temp = 3;
                break;
            case 9:
                temp = 4;
                break;
            case 11:
                temp = 5;
                break;
            case 13:
                temp = 6;
                break;
            case 15:
                temp = 7;
                break;
        }
        if (pagenumber > temp) {
            startPoint = pagenumber - temp;
            endPoint = pagenumber + temp;
        }
        if (endPoint > pagecount) {
            startPoint = pagecount - (temp * 2);
            endPoint = pagecount;
        }
        if (startPoint < 1) {
            startPoint = 1;
        }
        for (var page = startPoint; page <= endPoint; page++) {
            var currentButton = $("<li class=\"page-number\">" + (page) + "</li>");
            page == pagenumber ? currentButton.addClass("pgCurrent") : currentButton.click(function () {
                buttonClickCallback(this.firstChild.data);
            });
            currentButton.appendTo($pager);
        }
        if (showNext == "true") {
            $pager.append(renderButton("\u4e0b\u4e00\u9875", pagenumber, pagecount, buttonClickCallback));
        }
        if (showEnd == "true") {
            $pager.append(renderButton("\u672b\u9875", pagenumber, pagecount, buttonClickCallback));
        }
        if (thisPage != "false") {
            //var span = $("<span class='pageCount'>\u5171 <font color='#f37c2c'>" + pagecount + "</font> \u9875 \u5f53\u524d\u7b2c <font color='#669900'>" + pagenumber + "</font> \u9875</span>");
            var span = $("<span class='pageCount'>" + pagenumber + "/" + pagecount + "</span>");
            $pager.append(span);
        }
        return $pager;
    }
    function renderButton(buttonLabel, pagenumber, pagecount, buttonClickCallback) {
        var $Button = $("<li class=\"pgNext\">" + buttonLabel + "</li>");
        var destPage = 1;
        switch (buttonLabel) {
            case "\u9996\u9875":
                destPage = 1;
                break;
            case "\u4e0a\u4e00\u9875":
                destPage = pagenumber - 1;
                break;
            case "\u4e0b\u4e00\u9875":
                destPage = pagenumber + 1;
                break;
            case "\u672b\u9875":
                destPage = pagecount;
                break;
        }
        if (buttonLabel == "\u9996\u9875" || buttonLabel == "\u4e0a\u4e00\u9875") {
            pagenumber <= 1 ? $Button.addClass("pgEmpty") : $Button.click(function () {
                buttonClickCallback(destPage);
            });
        } else {
            pagenumber >= pagecount ? $Button.addClass("pgEmpty") : $Button.click(function () {
                buttonClickCallback(destPage);
            });
        }
        return $Button;
    }
    $.fn.pager.defaults = { pagenumber: 1, pagecount: 1, endPoint: 9 };
})(jQuery);


