using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YQCMS.Models
{
    /// <summary>
    /// 转载网站信息实体类
    /// </summary>
    public class PublishWebModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}