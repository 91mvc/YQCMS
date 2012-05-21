using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YQCMS.Models
{
    /// <summary>
    /// 相册实体类
    /// </summary>
    public class AlbumModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ImgCount { get; set; }
        public string Description { get; set; }
        public string AlbumPath { get; set; }
        public string AlbumCategory { get; set; }
        public int CommentCount { get; set; }
        public int ViewCount { get; set; }
        public List<AlbumImageModel> ImageList { get; set; }
        //封面
        public AlbumImageModel Cover { get; set; }
        public string ReName { get; set; }
        public DateTime Createdate { get; set; }
        public int Favor { get; set; }
        public int Against { get; set; }
    }

    public class AlbumImageModel
    {
        public string Src { get; set; }
        public string Title { get; set; }
        public string Alt { get; set; }
        //是否封面
        public string IsCover { get; set; }
    }
}