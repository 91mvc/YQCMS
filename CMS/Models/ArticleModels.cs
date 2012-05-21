using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace YQCMS.Models
{
    /// <summary>
    /// 文章实体类
    /// </summary>
    public class ArticleModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "请输入 {0}")]
        [StringLength(100, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "标题")]
        public string Title { get; set; }

        [StringLength(300, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "摘要")]
        public string Summary { get; set; }

        [Required]
        [Display(Name = "内容")]
        public string Content { get; set; }

        [Required]
        [Display(Name = "分类")]
        public int CateId { get; set; }

        [StringLength(50, ErrorMessage = "标签至多不超过50个字符")]
        [Display(Name = "标签(多个标签请用,号隔开)")]
        public string Tags { get; set; }

        // 是否允许评论 1：是；0：否
        [Display(Name = "是否允许评论")]
        public byte ReplyPermit { get; set; }

        // 状态 1：立即发布；2：保存草稿
        [Display(Name = "状态")]
        public byte Status { get; set; }

        [Display(Name = "是否推荐")]
        public byte IsCommend { get; set; }

        [Display(Name = "是否置顶")]
        public byte IsTop { get; set; }

        [StringLength(500, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "页面Title")]
        public string SeoTitle { get; set; }

        [StringLength(1000, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "页面描述")]
        public string SeoDescription { get; set; }

        [StringLength(500, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "页面关键字")]
        public string Seokeywords { get; set; }

        [StringLength(1000, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "页面Metas")]
        public string SeoMetas { get; set; }

        [StringLength(60, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "页面重命名")]
        [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "格式错误")]
        public string ReName { get; set; }
    }

    public class PostModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "请输入 {0}")]
        [StringLength(20, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请输入 {0}")]
        [StringLength(100, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "标题")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "内容")]
        public string Message { get; set; }

        [Required]
        [Display(Name = "分类")]
        public int CateId { get; set; }
    }

    public class ReplyModel
    {
        public int ReplyId { get; set; }

        public int ArticleId { get; set; }

        public int CateId { get; set; }

        [Display(Name = "是否允许评论")]
        public byte ReplyPermit { get; set; }

        [Required(ErrorMessage = "请输入 {0}")]
        [StringLength(20, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [StringLength(30, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "Url")]
        public string Url { get; set; }

        [Required]
        [Display(Name = "内容")]
        public string Reply { get; set; }
    }

    public class PostReplyModel
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public int CateId { get; set; }

        [Required(ErrorMessage = "请输入 {0}")]
        [StringLength(20, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "回复内容")]
        public string Message { get; set; }
    }


    public class NoteModel
    {
        public int NoteId { get; set; }

        public int CateId { get; set; }

        [Required(ErrorMessage = "请输入 {0}")]
        [StringLength(20, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [StringLength(30, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "{0} 至多不超过 {1} 个字符")]
        [Display(Name = "Url")]
        public string Url { get; set; }

        [Required]
        [Display(Name = "内容")]
        public string Message { get; set; }
    }

    //文章按月归档
    public class ArticleArchives
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
    }

}