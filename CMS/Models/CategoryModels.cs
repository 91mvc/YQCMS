using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace YQCMS.Models
{
    /// <summary>
    /// 分类实体类
    /// </summary>
    public class CategoryModel
    {
        //分类id
        public string CateId { get; set; }
        //分类名称
        [Required]
        [Display(Name = "分类名称")]
        public string CateName { get; set; }
        //页面类型
        [Required]
        [Display(Name = "类型")]
        public string Type { get; set; }
        //首页调用时候显示记录条数，IsList为1时有效
        [Display(Name = "首页显示记录条数")]
        [RegularExpression(@"^\d+", ErrorMessage = "必须为数字")]
        public string ListNum { get; set; }
        //排序id
        public string OrderId { get; set; }
        //该分类是否可评论
        [Display(Name = "是否可评论")]
        public string ReplyPermit { get; set; }
        //父分类id
        [Display(Name = "父节点")]
        public string ParentId { get; set; }
        //是否在站点导航里显示
        [Display(Name = "是否显示在导航")]
        public string IsNav { get; set; }
        //子分类数
        public string SubCount { get; set; }
        //是否显示在首页
        [Display(Name = "是否显示在首页")]
        public string IsIndex { get; set; }
        //状态
        [Display(Name = "状态")]
        public string Status { get; set; }
        //路径
        public string Path { get; set; }
        //别名
        [Display(Name = "别名")]
        [RegularExpression(@"^[a-zA-Z]{1}[a-zA-Z0-9_\-]{2,19}$", ErrorMessage = "格式要求：大小写字母开头，允许3-19字节，允许字母数字中，下划线")]
        public string ReName { get; set; }
        //自定义视图
        [Display(Name = "自定义视图")]
        [RegularExpression(@"^[a-zA-Z]{1}[a-zA-Z0-9_\-]{2,19}$", ErrorMessage = "格式要求：大小写字母开头，允许3-19字节，允许字母数字中，下划线")]
        public string CustomView { get; set; }

        //页面类型有，普通单页，文章列表，留言板，论坛，投稿。
    }
}