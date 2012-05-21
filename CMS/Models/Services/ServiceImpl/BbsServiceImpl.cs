using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using System.Data.SqlClient;
using System.Configuration;

namespace YQCMS.Models
{
    /// <summary>
    /// BBS相关数据操作实现
    /// </summary>
    public partial class ServiceImpl : IServices
    {
        /// <summary>
        /// 获取论坛板块列表 CategoryModel
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public IQueryable<BbsBoardModel> GetBbsBoards(int cid)
        {
            IQueryable<CategoryModel> list = GetSubCategoryList(cid);
            List<BbsBoardModel> boardlist = new List<BbsBoardModel>();
            foreach (CategoryModel c in list)
            {
                boardlist.Add(GetBbsBoard(c));
            }
            return boardlist.AsQueryable();
        }


        /// <summary>
        /// 获取论坛板块信息 CategoryModel
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public BbsBoardModel GetBbsBoard(int cid)
        {
            return GetBbsBoard(GetCategoryByID(cid));
        }

        /// <summary>
        /// 获取论坛板块信息 CategoryModel
        /// </summary>
        public BbsBoardModel GetBbsBoard(CategoryModel c)
        {
            cms_varticle lastpost = new cms_varticle();
            BbsBoardModel board = new BbsBoardModel();
            if (c != null)
            {
                board = GetBbsBoardStat(Utils.StrToInt(c.CateId));
                board.CateId = c.CateId;
                board.Path = c.Path;
                board.CateName = c.CateName;
                board.ReName = c.ReName;
                board.SubCount = c.SubCount;
                board.ReplyPermit = c.ReplyPermit;
                board.ParentId = c.ParentId;
            }
            return board;
        }

        /// <summary>
        /// 获取论坛板块统计信息
        /// </summary>
        public BbsBoardModel GetBbsBoardStat(int cid)
        {
            BbsBoardModel newboard = new BbsBoardModel();

            newboard.Admins = "";
            newboard.Description = "";
            newboard.TodayPostCount = GetTodayArticleCountByCategory(cid);
            newboard.PostCount = GetArticleCountByCategory(cid);
            newboard.ItemCount = GetDataCountByCategory(cid);
            newboard.LastPost = GetLatestArticle(cid);
            BbsExtendedModel bbsextendedModel=GetBbsExtendedByID(cid);
            if (bbsextendedModel != null)
            {
                newboard.Admins = bbsextendedModel.Admins;
                newboard.Description = bbsextendedModel.Description;
            }

            return newboard;
        }

        /// <summary>
        /// 论坛帖子信息分页(主题及其回复)
        /// </summary>
        public Pager GetPostPaging(Pager pager, int id)
        {
            IQueryable<cms_article> query = entity.cms_article.Where(m => m.articleid == id).Where(m => m.layer < 2);
            pager.Amount = query.Count();
            query = query.OrderBy(m => m.id).Skip(pager.PageSize * pager.PageNo).Take(pager.PageSize);
            pager.Entity = query;
            return pager;
        }

        /// <summary>
        /// 获取指定Id论坛版块扩展信息
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public BbsExtendedModel GetBbsExtendedByID(int cid)
        {
            return GeBBSExtendedList().Find((BbsExtendedModel p) => { return p.CateId == cid.ToString(); });
        }

        /// <summary>
        /// 读取json,反序列化
        /// </summary>
        /// <returns></returns>
        public List<BbsExtendedModel> GeBBSExtendedList()
        {
            List<BbsExtendedModel> lst = new List<BbsExtendedModel>();
            try
            {
                string jsonnav = GetBBSExtendedStr().Replace("\n", "");
                if (jsonnav != "")
                {
                    lst = Utils.ParseFromJson<List<BbsExtendedModel>>(jsonnav);
                }
            }
            catch (Exception) { }
            return lst;
        }

        /// <summary>
        /// 读取json文件,返回字符串
        /// </summary>
        /// <returns></returns>
        public string GetBBSExtendedStr()
        {
            string cacheKey = "Json-BBS";
            object cache = DataCache.GetCache(cacheKey);
            if (cache == null)
            {
                try
                {
                    cache = Utils.GetFileSource("/Content"+configinfo.WebPath+"BBS.js").Trim();
                    if (cache != null)
                    {
                        DataCache.SetCache(cacheKey, cache, DateTime.Now.AddMinutes(180.0), TimeSpan.Zero);
                    }
                }
                catch (Exception) { }
            }
            return cache.ToString();
        }


    }
}