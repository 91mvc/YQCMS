using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using System.ServiceModel.Syndication;

namespace YQCMS.Models
{
    /// <summary>
    /// 数据访问接口
    /// </summary>
    public interface IServices
    {
        /// <summary>
        /// 读取json文件,返回字符串
        /// </summary>
        string GetCategoryStr();

        /// <summary>
        /// 获取指定Id分类信息
        /// </summary>
        CategoryModel GetCategoryByID(int cid);

        /// <summary>
        /// 获取最大分类id
        /// </summary>
        int GetMaxCategoryID();

        /// <summary>
        /// 获取指定Rename分类信息
        /// </summary>
        CategoryModel GetCategoryByReName(string rename);

        /// <summary>
        /// 获得格式化后的所有分类列表
        /// </summary>
        /// <returns></returns>
        List<CategoryModel> getFCategoryList(string space = "");

        /// <summary>
        /// 获得格式化后的分类列表
        /// </summary>
        /// <returns></returns>
        List<CategoryModel> getFCategoryList(string tids, string cids, string space="");

        /// <summary>
        /// 获取分类路径url
        /// </summary>
        string GetCategoryPathUrl(string path);

        /// <summary>
        /// 获取分类路径url2（不显示根目录，1级目录显示文字，往后显示为链接）
        /// </summary>
        string GetCategoryPathUrl2(string path);

        /// <summary>
        /// 返回需要显示在首页的分类数据
        /// </summary>
        List<CategoryModel> GetIndexCategoryList();

        /// <summary>
        /// 返回所有分类数据
        /// </summary>
        IQueryable<CategoryModel> GetCategories();

        /// <summary>
        /// 获取指定Id所有子分类信息
        /// </summary>
        IQueryable<CategoryModel> GetSubCategoryList(int cid);

        /// <summary>
        /// 获取指定Id连同其所有子分类信息的id字符串集合
        /// </summary>
        string GetCategoryIds(int cid);

        /// <summary>
        /// 获取指定数量文章记录
        /// </summary>
        IQueryable<cms_varticle> GetArticles(int tid, int cid, int count);

        /// <summary>
        /// 获取指定数量文章记录
        /// </summary>
        IQueryable<cms_varticle> GetArticles(int tid, int cid, int count, string field);

        /// <summary>
        /// 获取指定Id的文章记录
        /// </summary>
        cms_varticle GetArticleByID(int id);

        /// <summary>
        /// 获取指定别名的文章记录
        /// </summary>
        cms_varticle GetArticleByReName(string rename);

        /// <summary>
        /// 获取指定Id的上一条记录（分类默认为文章）
        /// </summary>
        cms_varticle GetPreviewArticle(int id);

        /// <summary>
        /// 获取指定Id在指定分类id下的上一条记录
        /// </summary>
        cms_varticle GetPreviewArticle(int id,int tid);

        /// <summary>
        /// 获取指定Id的下一条记录（分类默认为文章）
        /// </summary>
        cms_varticle GetNextArticle(int id);

        /// <summary>
        /// 获取指定Id在指定分类id下的上一条记录
        /// </summary>
        cms_varticle GetNextArticle(int id, int tid);

        /// <summary>
        /// 文章分页
        /// </summary>
        Pager GetArticlePaging(Pager pager, int tid, int cid);

        /// <summary>
        /// 标签下文章分页
        /// </summary>
        Pager GetTagArticlePaging(Pager pager, int tid, string tag);

        /// <summary>
        /// 获取多个分类下文章记录
        /// </summary>
        IQueryable<cms_varticle> GetArticles(int tid, string cids, int count);

        /// <summary>
        /// 多个分类下文章分页
        /// </summary>
        Pager GetArticlePaging(Pager pager, int tid, string cids);

        /// <summary>
        /// 回复分页
        /// </summary>
        Pager GetReplyPaging(Pager pager, int parentid);

        /// <summary>
        /// 回复分页
        /// </summary>
        Pager GetReplyPaging(Pager pager, int tid,int cid,int layer);

        /// <summary>
        /// 新增文章
        /// </summary>
        int AddArticle(cms_varticle varticle);

        /// <summary>
        /// 更新文章(VArticle整体更新)
        /// </summary>
        void UpdateVArticle(cms_varticle varticle);

        /// <summary>
        /// 更新文章(Article表更新)
        /// </summary>
        void UpdateArticle(cms_varticle varticle);

        /// <summary>
        /// 删除文章
        /// </summary>
        void DelArticle(cms_varticle varticle);

        /// <summary>
        /// 转载网站列表
        /// </summary>
        List<PublishWebModel> GetPublishWebList();

        /// <summary>
        /// 获取指定Id 网站信息
        /// </summary>
        PublishWebModel GetPublishWebByID(int id);

        /// <summary>
        /// 返回指定分类下的相册列表
        /// </summary>
        IQueryable<AlbumModel> GetAlbums(int cid);

        /// <summary>
        /// 获取指定Id的相册信息
        /// </summary>
        AlbumModel GetAlbum(int id);

        /// <summary>
        /// 文章对象转换为相册对象
        /// </summary>
        AlbumModel GetAlbum(cms_varticle varticle);

        /// <summary>
        /// 获取论坛板块列表
        /// </summary>
        IQueryable<BbsBoardModel> GetBbsBoards(int cid);

        /// <summary>
        /// 获取论坛板块信息 CategoryModel
        /// </summary>
        BbsBoardModel GetBbsBoard(int cid);

        /// <summary>
        /// 论坛帖子信息分页(主题及其回复)
        /// </summary>
        Pager GetPostPaging(Pager pager, int id);

        /// <summary>
        /// 今日分类Id下文章数
        /// </summary>
        int GetTodayArticleCountByCategory(int cid);

        /// <summary>
        /// 今日类型Id下文章数
        /// </summary>
        int GetTodayArticleCountByType(int tid);

        /// <summary>
        /// 分类Id下文章数
        /// </summary>
        int GetArticleCountByCategory(int cid);
        /// <summary>
        /// 类型Id下文章数
        /// </summary>
        int GetArticleCountByType(int tid);

        /// <summary>
        /// 分类Id下文章及回复数
        /// </summary>
        int GetDataCountByCategory(int cid);

        /// <summary>
        /// 类型Id下文章及回复数
        /// </summary>
        int GetDataCountByType(int tid);

        /// <summary>
        /// 分类下最近一条文章记录
        /// </summary>
        cms_varticle GetLatestArticle(int cid);

        /// <summary>
        /// 最近一条回复
        /// </summary>
        cms_article GetLatestReply(int id);

        /// <summary>
        /// 读取json文件,返回论坛设置字符串
        /// </summary>
        string GetBBSExtendedStr();

        /// <summary>
        /// 读取json文件,返回转载网站设置字符串
        /// </summary>
        /// <returns></returns>
        string GetPublishWebStr();

        //文章按日期归档
        IQueryable<ArticleArchives> GetArticleArchives();

        /// <summary>
        /// 获得归档文章
        /// </summary>
        Pager GetArchivesArticlePaging(Pager pager, int tid, int year,int month);

        /// <summary>
        /// 获得归档文章(按日期)
        /// </summary>
        Pager GetArchivesArticlePaging(Pager pager, int tid, int year, int month, int day);

        /// <summary>
        /// 获取最新评论文章记录
        /// </summary>
        IQueryable<cms_varticle> GetReplyArticles(int tid, int cid, int count);

        /// <summary>
        /// 文章按日期(天)归档
        /// </summary>
        IQueryable<string> GetArticleDates();

        /// <summary>
        /// 获取rss
        /// </summary>
        List<SyndicationItem> GetRss(List<cms_varticle> list);

    }
}