using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Models
{
    /// <summary>
    /// 分类实体类
    /// </summary>
    public class Category
    {
        //分类id
        public string CateId { get; set; }
        //分类名称
        public string CateName { get; set; }
        //是页面类型
        public string Type { get; set; }
        //首页调用时候显示记录条数，IsList为1时有效
        public string ListNum { get; set; }
        //排序id
        public string OrderId { get; set; }
        //该分类是否可评论
        public string ReplyPermit { get; set; }
        //父分类id
        public string ParentId { get; set; }
        //是否在站点导航里显示
        public string IsNav { get; set; }
        //子分类数
        public string SubCount { get; set; }
        //是否显示在首页
        public string IsIndex { get; set; }
        //状态
        public string Status { get; set; }
        //路径
        public string Path { get; set; }
        //别名
        public string ReName { get; set; }

        //页面类型有，普通单页，文章列表，留言板，论坛，投稿。
    }
}