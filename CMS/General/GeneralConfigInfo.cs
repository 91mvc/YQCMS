using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace YQCMS.General
{
    /// <summary>
    /// 基本设置描述类, 加[Serializable]标记为可序列化 
    /// </summary>
    [Serializable]
    public class GeneralConfigInfo : IConfigInfo
    {
        #region 字段
        private string m_webtitle = ""; //网站名称
        private string m_weburl = ""; //网站url地址
        private string m_webDescription = ""; //网站url地址
        private string m_icp = ""; //网站备案信息
        private string m_webpath = "/"; //网站路径
        private int m_indexPagerCount = 10;
        private int m_catePagerCount = 10;
        private int m_commentPagerCount = 10;
        private int m_notePagerCount = 10;
        private int m_decorateImgCount = 20;//插图总数
        #endregion

        /// <summary>
        /// 自定义用户显示项目
        /// </summary>

        /// <summary>
        /// 网站名称
        /// </summary>
        [Required]
        [Display(Name = "网站名称")]
        public string Webtitle
        {
            get { return m_webtitle; }
            set { m_webtitle = value; }
        }
        /// <summary>
        /// 网站描述
        /// </summary>
        [Display(Name = "网站简介")]
        public string WebDescription
        {
            get { return m_webDescription; }
            set { m_webDescription = value; }
        }

        /// <summary>
        /// 网站url地址
        /// </summary>
        [Required]
        [Display(Name = "网站URL")]
        public string Weburl
        {
            get { return m_weburl; }
            set { m_weburl = value; }
        }

        /// <summary>
        /// 网站备案信息
        /// </summary>
         [Display(Name = "备案号")]
        public string Icp
        {
            get { return m_icp; }
            set { m_icp = value; }
        }

         /// <summary>
         /// 站点路径
         /// </summary>
         [Display(Name = "站点路径")]
         public string WebPath
         {
             get { return m_webpath; }
             set { m_webpath = value; }
         }

         /// <summary>
         /// 首页分页记录数
         /// </summary>
         [Display(Name = "首页分页记录数")]
         [RegularExpression(@"^\d+", ErrorMessage = "必须为数字")]
         public int IndexPagerCount
         {
             get { return m_indexPagerCount; }
             set { m_indexPagerCount = value; }
         }

         /// <summary>
         /// 文章列表分页记录数
         /// </summary>
         [Display(Name = "文章列表分页记录数")]
         [RegularExpression(@"^\d+", ErrorMessage = "必须为数字")]
         public int CatePagerCount
         {
             get { return m_catePagerCount; }
             set { m_catePagerCount = value; }
         }
         /// <summary>
         /// 评论分页记录数
         /// </summary>
         [Display(Name = "评论分页记录数")]
         [RegularExpression(@"^\d+", ErrorMessage = "必须为数字")]
         public int CommentPagerCount
         {
             get { return m_commentPagerCount; }
             set { m_commentPagerCount = value; }
         }

         /// <summary>
         /// 留言分页记录数
         /// </summary>
         [Display(Name = "留言分页记录数")]
         [RegularExpression(@"^\d+", ErrorMessage = "必须为数字")]
         public int NotePagerCount
         {
             get { return m_notePagerCount; }
             set { m_notePagerCount = value; }
         }

        /// <summary>
        /// 插图总数
        /// </summary>
         public int DecorateImgCount
         {
             get { return m_decorateImgCount; }
             set { m_decorateImgCount = value; }
         }

        
    }
}
