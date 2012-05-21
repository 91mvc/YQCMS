using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YQCMS.Models
{
    /// <summary>
    /// 论坛板块
    /// </summary>
    public class BbsBoardModel : CategoryModel
    {
        //板块管理员
        public string Admins { get; set; }
        //描述
        public string Description { get; set; }
        //总帖子数量
        public int PostCount { get; set; }
        //今日帖子数
        public int TodayPostCount { get; set; }
        //总记录数
        public int ItemCount { get; set; }
        //最新帖子信息
        public cms_varticle LastPost { get; set; }
    }

    /// <summary>
    /// 论坛板块额外信息
    /// </summary>
    public class BbsExtendedModel
    {
        public string CateId { get; set; }
        public string Description { get; set; }
        public string Admins { get; set; }
    }
}