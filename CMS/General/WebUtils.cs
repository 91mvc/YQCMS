using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YQCMS.Models;
using System.Web.Mvc;

namespace YQCMS.General
{
    public class WebUtils
    {
        protected static GeneralConfigInfo configinfo = GeneralConfigs.GetConfig();

        //分类链接url
        public static string GetCateUrl(CategoryModel category)
        {
            return configinfo.WebPath + (!string.IsNullOrWhiteSpace(category.ReName) ? category.ReName + "/" : "cate/" + category.CateId);
        }

        //文章链接url
        public static string GetArticleUrl(string url)
        {
            return configinfo.WebPath + url;
        }

        //文章链接url
        public static string GetArticleUrl(int id, string reName)
        {
            return configinfo.WebPath + (!string.IsNullOrWhiteSpace(reName) ? "article/" + reName : "archive/" + id.ToString());
        }

        //文章链接url
        public static string GetArticleUrl(cms_varticle varticle)
        {
            return configinfo.WebPath + (!string.IsNullOrWhiteSpace(varticle.rename) ? "article/" + varticle.rename : "archive/" + varticle.id.ToString());
        }

        //获得对应视图名称
        public static string GetViewName(string customView,string defaultView)
        {
            return string.IsNullOrWhiteSpace(customView) ? defaultView : customView;
        }

        //获得页面类型下拉列表
        //public static List<SelectListItem> GetTypeList()
        //{
        //    List<SelectListItem> items = new List<SelectListItem>();
        //    items.Add(new SelectListItem
        //    {
        //        Text = "文章",
        //        Value = "1"
        //    });
        //    items.Add(new SelectListItem
        //    {
        //        Text = "单页",
        //        Value = "2",
        //        Selected = true
        //    });
        //    items.Add(new SelectListItem
        //    {
        //        Text = "留言",
        //        Value = "6"
        //    });
        //    items.Add(new SelectListItem
        //    {
        //        Text = "投稿",
        //        Value = "3"
        //    });
        //    return items;
        //}

        //获得页面类型下拉列表
        public static List<PageType> GetTypeList()
        {
            List<PageType> items = new List<PageType>();
            items.Add(new PageType
            {
                TypeName = "文章",
                TypeId = 1
            });
            items.Add(new PageType
            {
                TypeName = "单页",
                TypeId = 2
            });
            items.Add(new PageType
            {
                TypeName = "留言",
                TypeId = 6
            });
            items.Add(new PageType
            {
                TypeName = "投稿",
                TypeId = 3
            });
            return items;
        }

    }
    //页面类型
    public class PageType
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
    }
}