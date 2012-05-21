var jsonNav = category;
//如果网站定义在子目录，需修改此处，eg:/cms/
var webPath = "/";
//无限极菜单
function ViewNav(parentid, layer) {
    var layer = layer + 1;
    var navItemStr;
    var currentParent;
    var currentItem;

    if (parentid == 0) {
        currentParent = $(".menu");
        currentParent.append("<ul class=\"topnav\"><li id=\"Nav_0\" v=\"0\"><a href=\"" + webPath + "\">主页</a></li></ul>");
    }
    else {
        currentParent = $("#Nav_" + parentid);
        if (layer == 2)
        { currentParent.append("<ul class=\"subnav\" id=\"PNav_" + parentid + "\"></ul>"); }
        else
        { currentParent.append("<ul class=\"subnav2\" id=\"PNav_" + parentid + "\"></ul>"); }
    }
    currentParent = currentParent.find("ul");

    //循环取json中的数据
    $.each(jsonNav, function (i) {

        if ((jsonNav[i].IsNav == 1) && (jsonNav[i].ParentId == parentid)) {
            //添加
            var urlstr = webPath+"cate/" + jsonNav[i].CateId;
            if (jsonNav[i].ReName != "")
            { urlstr = webPath + jsonNav[i].ReName+"/"; }
            navItemStr = "<li id=\"Nav_" + jsonNav[i].CateId + "\" v=\"" + jsonNav[i].CateId + "\"><a href=\"" + urlstr + "\">" + jsonNav[i].CateName + "</a></li>";
            currentParent.append(navItemStr)
            currentItem = $("#Nav_" + jsonNav[i].CateId);

            //有子栏目时添加下拉样式标签以及hasSub样式（用于显示子栏目的操作）
            if (jsonNav[i].SubCount > 0 && jsonNav[i].Type != "5") {
                if (layer == 1)
                { currentItem.append("<span></span>"); }
                else {
                    currentItem.find("a").css({ "background-position": "145px 15px" });
                }
                currentItem.addClass("hasSub");
                ViewNav(jsonNav[i].CateId, layer);
            }

            if (layer > 2)
            { currentItem.find("a").css({ "float": "left", "width": "150px" }); }
        }
    });
}

//替换掉编辑器自动加的换行
function ReplaceKESpace(str) {
    var re = /(<p>(\s|\s*&nbsp;\s*|\s*<br\s*\/?\s*>\s*)*<\/p>)+/ig;
    var re2 = /((\s|\s*&nbsp;\s*|\s*<br\s*\/?\s*>\s*)*)+/ig;
    var newstr = str;
    if (newstr.replace(re, "").replace(re2, "") == "") {
        newstr = "";
    }
    else {
        newstr = newstr.replace(re, "<br/>");
    }
    return newstr;
}

//取得当前时间
function GetCurrentTime() {
    var date = new Date();
    var now = "";
    now = date.getFullYear() + "-"; 
    now = now + (date.getMonth() + 1) + "-"; 
    now = now + date.getDate() + " ";
    now = now + date.getHours() + ":";
    now = now + date.getMinutes() + ":";
    now = now + date.getSeconds();
    return now;
}

//文章列表修饰图异步加载
function LoadSummaryImg() {
    if (summaryimgs) {
        $.each(summaryimgs, function (i, n) {
            $("#simg_" + n["id"]).append("<img src=\"" + n["img"] + "\"/>");
        });
    }
}

//加载日历
function initArticleCalendar() {
    var datepickerYear = $(".ui-datepicker-year").html();
    //英文月份：alert($.datepicker.regional[''].monthNames);
    var datepickerMonth = $.inArray($(".ui-datepicker-month").html(), $.datepicker.regional['zh-CN'].monthNames) + 1;
    $(".ui-datepicker-calendar a").each(function (index, domEle) {

        var datepickerDay = $(this).html();
        var datepickerDate = datepickerYear + "-" + datepickerMonth + "-" + datepickerDay;

        if ($.inArray(datepickerDate, strArticleDates) > -1) {
            $(this).parent().attr("onclick", "null");
            $(this).attr("href", webPath + "Archives/" + datepickerDate + "/").css({ "background": "#680707" });
        }
        else {
            $(this).parent().attr("onclick", "function(){return false;}").html("<div class=\"ui-state-word\">" + datepickerDay + "</div>");
        }
    });
}

$(function () {
    //登录信息
    $.get(webPath + "Account/CheckLogin/", { rn: Math.random() }, function (re) {
        $("#logindisplay").html(re);
    })

});