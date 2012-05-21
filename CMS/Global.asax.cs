using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace YQCMS
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            //作用:使路由系统忽略处理ASP.NET的Web资源文件(WebResource.axd或ScriptResource.axd).
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.RouteExistingFiles = true;

            //分页
            //这里配置了html后缀的url路由，如果访问提示找不到页面，那么可能需要在IIS中配置.html后缀的映射文件
            //可执行文件路径为c:\windows\microsoft.net\framework\v4.0.30319\aspnet_isapi.dll，选中“脚本引擎”前面复选框，去掉“检查文件是否存在”前面的复选框。
            //routes.MapRoute(
            //    "CategoryPager",
            //    "{controller}/{cid}_{pageNo}.html",
            //    new { controller = "Home", action = "Category"},
            //    new { cid = "[0-9]+", pageNo = "[0-9]+" }
            //);

            //首页
            routes.MapRoute(
                "Index",
                "{pageNo}",
                new { controller = "Home", action = "Index", pageNo = UrlParameter.Optional },
                new { pageNo = "[0-9]*" }
            );

            //分类,id号查看
            routes.MapRoute(
                "Rss",
                "rss/",
                new { controller = "Home", action = "Rss" }
            );

            //分类,id号查看
            routes.MapRoute(
                "Category",
                "cate/{id}/{pageNo}",
                new { controller = "Home", action = "Category", pageNo = UrlParameter.Optional },
                new { id = "[0-9]+", pageNo = "[0-9]*" }
            );

            //文章,id查看
            routes.MapRoute(
                "Article",
                "archive/{id}",
                new { controller = "Home", action = "Article" },
                new { id = "[0-9]+" }
            );

            //文章查看,别名查看
            routes.MapRoute(
                "ArticleByKey",
                "article/{key}",
                new { controller = "Home", action = "ArticleByKey" },
                new { key = @"^[a-zA-Z0-9\-]+$" }

            );

            //相册查看,id查看
            routes.MapRoute(
                "Album",
                "album/{id}",
                new { controller = "Home", action = "Album" },
                new { id = "[0-9]+" }
            );

            //标签查看
            routes.MapRoute(
                "Tag",
                "tag/{key}/{pageNo}",
                new { controller = "Home", action = "Tag", pageNo = UrlParameter.Optional },
                new { pageNo = "[0-9]*" }
            );

            //文章归档
            routes.MapRoute(
                "Archives",
                "Archives/{year}/{month}/{pageNo}",
                new { controller = "Home", action = "Archives", pageNo = UrlParameter.Optional },
                new { year = "[0-9]+", month = "[0-9]+", pageNo = "[0-9]*" }
            );

            //文章按天归档
            routes.MapRoute(
                "ArchivesDate",
                "Archives/{year}-{month}-{day}/{pageNo}",
                new { controller = "Home", action = "ArchivesDate", pageNo = UrlParameter.Optional },
                new { year = "[0-9]+", month = "[0-9]+", day = "[0-9]+", pageNo = "[0-9]*" }
            );

            //留言查看
            routes.MapRoute(
                "NoteBook",
                "NoteBook/{pageNo}",
                new { controller = "Home", action = "NoteBook", pageNo = UrlParameter.Optional },
                new { pageNo = "[0-9]*" }
            );

            //分类,别名查看
            routes.MapRoute(
                "CategoryByKey",
                "{key}/{pageNo}",
                new { controller = "Home", action = "CategoryByKey", pageNo = UrlParameter.Optional },
                new { key = @"^[a-zA-Z0-9\-]+$", pageNo = "[0-9]*" }
            );

            //默认路由
            routes.MapRoute(
                "Default", // 路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // 参数默认值
            );

            /*
            示例(最多为5个参数)：
            public static Route MapRoute(
                this RouteCollection routes,
                string name, //路由在路由列表里的唯一名字(两次MapRoute时name不能重复)
                string url, //路由匹配的url格式
                Object defaults,//路由url {占位符} 的默认值
                Object constraints,
            //url的 {占位符} 的约束 ,示例
            //(1)通过正则的形式：表示controller，action必须为英文字符,最少长度为4
            //new { controller = @"[a-z]{4,}",    action = @"[a-z]{4,}" }
            //(2)如果你觉得正则匹配不符合你的要求那么可以去实现System.Web.Routing.IRouteConstraint接口
                string[] namespaces//设置路由搜索的控制器命名空间
            //在你的MVC3应用程序里建立了不是以Controllers结尾的控制器类命名空间时,就可以通过设置这个属性来让路由系统在url匹配时应该去找那些命名空间
            //eg:new string[] { "MvcApplication1.Custom" }
            //设置后此参数后路由系统就会去找
            //MvcApplication1.Controllers, MvcApplication1.Custom下带Controller结尾的继承于Controller的类
            )

            //ps:路由url是不区分大小写的
             */
        }

        //Application_Start 和 Application_End 方法是表示 HttpApplication 事件的特殊方法。
        //在应用程序域的生命周期期间，ASP.NET 仅调用这些方法一次，
        //而不是对每个 HttpApplication 实例都调用一次。
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            //调用RegisterRoutes方法，通过routes.MapRoute( … )对RouteCollection对象RouteTable.Routes添加更多的扩展方法
            RegisterRoutes(RouteTable.Routes);
        }
    }
}