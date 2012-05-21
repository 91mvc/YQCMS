using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YQCMS
{
    ////在主站点注册区域AreasCMS,需要把站点部署到子目录时会用到
    //它实现了AreaRegistration基类。然后我们注册区域路由就会在Global.asax的Application_Start事件方法中去执行注册到主站点的路由表中
    //public class CMSRegistration : AreaRegistration
    //{
    //    public override string AreaName
    //    {
    //        get
    //        {
    //            return "AreasCMS";
    //        }
    //    }

    //    public override void RegisterArea(AreaRegistrationContext context)
    //    {

    //        //分页
    //        //这里配置了html后缀的url路由，如果访问提示找不到页面，那么可能需要在IIS中配置.html后缀的映射文件
    //        //可执行文件路径为c:\windows\microsoft.net\framework\v4.0.30319\aspnet_isapi.dll，选中“脚本引擎”前面复选框，去掉“检查文件是否存在”前面的复选框。

    //        //首页
    //        context.MapRoute(
    //            "cms_Index",
    //            "cms/{pageNo}",
    //            new { controller = "Home", action = "Index", pageNo = UrlParameter.Optional },
    //            new { pageNo = "[0-9]*" }
    //        );

    //        //分类,id号查看
    //        context.MapRoute(
    //            "cms_Rss",
    //            "cms/rss/",
    //            new { controller = "Home", action = "Rss"}
    //        );

    //        //分类,id号查看
    //        context.MapRoute(
    //            "cms_Category",
    //            "cms/cate/{id}/{pageNo}",
    //            new { controller = "Home", action = "Category", pageNo = UrlParameter.Optional },
    //            new { id = "[0-9]+", pageNo = "[0-9]*" }
    //        );

    //        //文章,id查看
    //        context.MapRoute(
    //            "cms_Article",
    //            "cms/archive/{id}",
    //            new { controller = "Home", action = "Article" },
    //            new { id = "[0-9]+" }
    //        );

    //        //文章查看,别名查看
    //        context.MapRoute(
    //            "cms_ArticleByKey",
    //            "cms/article/{key}",
    //            new { controller = "Home", action = "ArticleByKey" },
    //            new { key = @"^[a-zA-Z0-9\-]+$" }

    //        );

    //        //相册查看,id查看
    //        context.MapRoute(
    //            "cms_Album",
    //            "cms/album/{id}",
    //            new { controller = "Home", action = "Album" },
    //            new { id = "[0-9]+" }
    //        );

    //        //标签查看
    //        context.MapRoute(
    //            "cms_Tag",
    //            "cms/tag/{key}/{pageNo}",
    //            new { controller = "Home", action = "Tag", pageNo = UrlParameter.Optional },
    //            new { pageNo = "[0-9]*" }
    //        );

    //        //文章归档
    //        context.MapRoute(
    //            "cms_Archives",
    //            "cms/Archives/{year}/{month}/{pageNo}",
    //            new { controller = "Home", action = "Archives", pageNo = UrlParameter.Optional },
    //            new { year = "[0-9]+", month = "[0-9]+", pageNo = "[0-9]*" }
    //        );

    //        //文章按天归档
    //        context.MapRoute(
    //            "cms_ArchivesDate",
    //            "cms/Archives/{year}-{month}-{day}/{pageNo}",
    //            new { controller = "Home", action = "ArchivesDate", pageNo = UrlParameter.Optional },
    //            new { year = "[0-9]+", month = "[0-9]+", day = "[0-9]+", pageNo = "[0-9]*" }
    //        );

    //        //留言查看
    //        context.MapRoute(
    //            "cms_NoteBook",
    //            "cms/NoteBook/{pageNo}",
    //            new { controller = "Home", action = "NoteBook", pageNo = UrlParameter.Optional },
    //            new { pageNo = "[0-9]*" }
    //        );

    //        //分类,别名查看
    //        context.MapRoute(
    //            "cms_CategoryByKey",
    //            "cms/{key}/{pageNo}",
    //            new { controller = "Home", action = "CategoryByKey", pageNo = UrlParameter.Optional },
    //            new { key = @"^[a-zA-Z0-9\-]+$",pageNo = "[0-9]*" }
    //        );

    //        //默认路由
    //        context.MapRoute(
    //            "cms_Default", // 路由名称
    //            "cms/{controller}/{action}/{id}", // 带有参数的 URL
    //            new { controller = "Home", action = "Index", id = UrlParameter.Optional } // 参数默认值
    //        );
    //    }
    //}

}