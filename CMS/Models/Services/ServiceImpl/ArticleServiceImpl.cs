using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using System.Data.SqlClient;

namespace YQCMS.Models
{
    /// <summary>
    /// 文章信息相关数据操作实现
    /// </summary>
    public partial class ServiceImpl : IServices
    {
        /// <summary>
        /// CmsEntitie是我们从数据库导入的数据实体模型对象
        /// </summary>
        CmsEntities entity = new CmsEntities();

        /// <summary>
        /// 获取文章记录
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IQueryable<cms_varticle> GetArticles(int tid, int cid, int count)
        {
            return GetArticles(tid, cid, count,"");
        }

        /// <summary>
        /// 获取文章记录
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IQueryable<cms_varticle> GetArticles(int tid, int cid, int count,string field)
        {
            IQueryable<cms_varticle> query = entity.cms_varticle;
            if (tid > 0)
                query = query.Where(m => m.typeid == tid);
            if (cid > 0)
                query = query.Where(m => m.cateid == cid);
            if (field != "")
            {
                switch (field)
                {
                    case "viewcount":
                        query = query.OrderByDescending(m =>m.viewcount);
                        break;
                    case "subcount":
                        query = query.OrderByDescending(m => m.subcount);
                        break;
                    case "lastreplydate":
                        query = query.Where(m => m.subcount>0).OrderByDescending(m => m.lastreplydate);
                        break;
                    case "favor":
                        query = query.OrderByDescending(m => m.favor);
                        break;
                }
                
            }
            if (count > 0)
                query = query.Take(count);

            return query;
        }


        private static object GetPropertyValue(object obj, string property)
        {
            System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }

        /// <summary>
        /// 文章分页
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public Pager GetArticlePaging(Pager pager,int tid, int cid)
        {
            IQueryable<cms_varticle> query = GetArticles(tid,cid, 0);

            pager.Amount = query.Count();
            query = query.OrderByDescending(m => m.id).Skip(pager.PageSize * pager.PageNo).Take(pager.PageSize);
            pager.Entity = query;
            return pager;
        }

        /// <summary>
        /// 获取多个分类下文章记录
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IQueryable<cms_varticle> GetArticles(int tid, string cids, int count)
        {
            IQueryable<cms_varticle> query = entity.cms_varticle;
            if (tid > 0)
                query = query.Where(m => m.typeid == tid);
            if (cids!="")
            {
                List<int> listids = new List<string>(cids.Split(',')).ConvertAll(i => int.Parse(i));
                query = query.Where(m => listids.Contains(m.cateid));
            }
            if (count > 0)
                query = query.Take(count);
            return query;
        }

        /// <summary>
        /// 多个分类下文章分页
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public Pager GetArticlePaging(Pager pager, int tid, string cids)
        {
            IQueryable<cms_varticle> query = GetArticles(tid, cids, 0);

            pager.Amount = query.Count();
            query = query.OrderByDescending(m => m.id).Skip(pager.PageSize * pager.PageNo).Take(pager.PageSize);
            pager.Entity = query;
            return pager;
        }

        /// <summary>
        /// 标签下文章分页
        /// </summary>
        public Pager GetTagArticlePaging(Pager pager, int tid, string tag)
        {
            IQueryable<cms_varticle> query = GetArticles(tid, 0, 0).Where(m =>
                                      ("," + m.tags + ",").Contains("," + tag + ","));

            pager.Amount = query.Count();
            query = query.OrderByDescending(m => m.id).Skip(pager.PageSize * pager.PageNo).Take(pager.PageSize);
            pager.Entity = query;
            return pager;
        }

        /// <summary>
        /// 回复分页
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public Pager GetReplyPaging(Pager pager, int parentid)
        {
            IQueryable<cms_article> query = entity.cms_article;

            if (parentid > 0)
                query = query.Where(m => m.parentid == parentid);

            pager.Amount = query.Count();
            query = query.OrderBy(m => m.id).Skip(pager.PageSize * pager.PageNo).Take(pager.PageSize);
            pager.Entity = query;
            return pager;
        }

        /// <summary>
        /// 回复分页
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public Pager GetReplyPaging(Pager pager, int tid,int cid,int layer)
        {
            IQueryable<cms_article> query = entity.cms_article;
            query = query.Where(m => m.layer == layer);
            if (cid > 0)
                query = query.Where(m => m.cateid == cid);

            if (tid > 0)
                query = query.Where(m => m.typeid == tid);

            pager.Amount = query.Count();
            query = query.OrderByDescending(m => m.id).Skip(pager.PageSize * pager.PageNo).Take(pager.PageSize);
            pager.Entity = query;
            return pager;
        }

        /// <summary>
        /// 获取指定id文章记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public cms_varticle GetArticleByID(int id)
        {
            return entity.cms_varticle.First(m => m.id == id);
        }

        /// <summary>
        /// 获取指定别名的文章记录
        /// </summary>
        public cms_varticle GetArticleByReName(string rename)
        {
            return entity.cms_varticle.FirstOrDefault(m => m.rename == rename);
        }

        /// <summary>
        /// 获取指定Id的上一篇文章记录
        /// </summary>
        public cms_varticle GetPreviewArticle(int id)
        {
            return GetPreviewArticle(id,1);
        }
        public cms_varticle GetPreviewArticle(int id,int tid)
        {
            return entity.cms_varticle.Where(m => m.typeid == tid).Where(m => m.id < id).OrderByDescending(m => m.id).FirstOrDefault();
        }

        /// <summary>
        /// 获取指定Id的下一篇文章记录
        /// </summary>
        public cms_varticle GetNextArticle(int id)
        {
            return GetNextArticle(id,1);
        }
        public cms_varticle GetNextArticle(int id,int tid)
        {
            return entity.cms_varticle.Where(m => m.typeid == tid).Where(m => m.id > id).OrderBy(m => m.id).FirstOrDefault();
        }

        /// <summary>
        /// 添加文章
        /// </summary>
        /// <param name="varticle"></param>
        public int AddArticle(cms_varticle varticle)
        {
            using (CmsEntities entity = new CmsEntities())
            {
                cms_varticle obj = varticle;
                entity.cms_varticle.AddObject(obj);
                entity.SaveChanges();
                return obj.id;
            }
        }

        /// <summary>
        /// 更新文章(VArticle整体更新)
        /// </summary>
        /// <param name="varticle"></param>
        public void UpdateVArticle(cms_varticle varticle)
        {

            using (CmsEntities entity = new CmsEntities())
            {
                var obj = entity.cms_varticle.FirstOrDefault(m => m.id == varticle.id);
                if (obj != null)
                {
                    obj.title = varticle.title;
                    obj.cateid = varticle.cateid;
                    obj.catename = varticle.catename;
                    obj.catepath = varticle.catepath;
                    obj.summary = varticle.summary;
                    obj.content = varticle.content;
                    obj.tags = varticle.tags;
                    obj.seodescription = varticle.seodescription;
                    obj.seokeywords = varticle.seokeywords;
                    obj.seometas = varticle.seometas;
                    obj.seotitle = varticle.seotitle;
                    obj.rename = varticle.rename;
                    obj.status = varticle.status;
                    obj.replypermit = varticle.replypermit;
                    obj.iscommend = varticle.iscommend;
                    obj.istop = varticle.istop;
                    entity.SaveChanges();
                }
            }
        }
        /// <summary>
        /// 更新文章(Article表更新)
        /// </summary>
        public void UpdateArticle(cms_varticle varitcle)
        {

            using (CmsEntities entity = new CmsEntities())
            {
                var query = entity.cms_article.FirstOrDefault(m => m.id == varitcle.id);
                if (query != null)
                {
                    query.typeid = varitcle.typeid;
                    query.cateid = varitcle.cateid;
                    query.catepath = varitcle.catepath;
                    query.articleid = varitcle.articleid;
                    query.parentid = varitcle.parentid;
                    query.layer = varitcle.layer;
                    query.subcount = varitcle.subcount;
                    query.catename = varitcle.catename;
                    query.userid = varitcle.userid;
                    query.username = varitcle.username;
                    query.title = varitcle.title;
                    query.summary = varitcle.summary;
                    query.content = varitcle.content;
                    query.viewcount = varitcle.viewcount;
                    query.orderid = varitcle.orderid;
                    query.replypermit = varitcle.replypermit;
                    query.status = varitcle.status;
                    query.ip = varitcle.ip;
                    query.favor = varitcle.favor;
                    query.against = varitcle.against;
                    query.createdate = varitcle.createdate;
                    entity.SaveChanges();
                }
            }

        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="varticle"></param>
        public void DelArticle(cms_varticle varticle)
        {
            using (CmsEntities entity = new CmsEntities())
            {
                var query = entity.cms_varticle.FirstOrDefault(m => m.id == varticle.id);
                if (query != null)
                {
                    entity.cms_varticle.DeleteObject(query);
                    entity.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 今日分类Id下文章数 
        /// </summary>
        public int GetTodayArticleCountByCategory(int cid)
        {
            return entity.cms_article.Where(m => m.cateid == cid && m.layer == 0 && System.Data.Objects.EntityFunctions.DiffDays(m.createdate ,DateTime.Now)==0).Count();
        }

        /// <summary>
        /// 今日类型Id下文章数
        /// </summary>
        public int GetTodayArticleCountByType(int tid)
        {
            return entity.cms_article.Where(m => m.typeid == tid && m.layer == 0 && System.Data.Objects.EntityFunctions.DiffDays(m.createdate, DateTime.Now) == 0).Count();
        }

        /// <summary>
        /// 分类Id下文章数
        /// </summary>
        public int GetArticleCountByCategory(int cid)
        {
            return entity.cms_article.Where(m => m.cateid == cid && m.layer==0).Count();
        }

        /// <summary>
        /// 类型Id下文章数
        /// </summary>
        public int GetArticleCountByType(int tid)
        {
            return entity.cms_article.Where(m => m.typeid == tid && m.layer == 0).Count();
        }

        /// <summary>
        /// 分类Id下文章及回复数
        /// </summary>
        public int GetDataCountByCategory(int cid)
        {
            return entity.cms_article.Where(m => m.cateid == cid).Count();
        }

        /// <summary>
        /// 类型Id下文章及回复数
        /// </summary>
        public int GetDataCountByType(int tid)
        {
            return entity.cms_article.Where(m => m.typeid == tid).Count();
        }

        /// <summary>
        /// 分类下最近一条文章记录
        /// </summary>
        public cms_varticle GetLatestArticle(int cid)
        {
            return entity.cms_varticle.Where(m => m.cateid == cid).OrderByDescending(m => m.id).FirstOrDefault();
        }

        /// <summary>
        /// 最近一条回复
        /// </summary>
        public cms_article GetLatestReply(int id)
        {
            return entity.cms_article.Where(m => m.parentid == id).OrderByDescending(m => m.id).FirstOrDefault();
        }

        /// <summary>
        /// 文章按日期(月)归档
        /// </summary>
        public IQueryable<ArticleArchives> GetArticleArchives()
        {

            var grouped = (from p in entity.cms_varticle
                           where p.typeid==1
                           group p by new { month = p.createdate.Month, year = p.createdate.Year } into d
                           select new ArticleArchives { Year = d.Key.year, Month = d.Key.month, Count = d.Count() }).OrderByDescending(g => g.Year).ThenByDescending(g => g.Month); 

            return grouped;
        }

        /// <summary>
        /// 获取指定日期归档文章
        /// </summary>
        public Pager GetArchivesArticlePaging(Pager pager, int tid, int year, int month)
        {
            IQueryable<cms_varticle> query = GetArticles(tid, 0, 0).Where(m => m.createdate.Year == year && m.createdate.Month == month);

            pager.Amount = query.Count();
            query = query.OrderByDescending(m => m.id).Skip(pager.PageSize * pager.PageNo).Take(pager.PageSize);
            pager.Entity = query;
            return pager;
        }

        /// <summary>
        /// 获取指定日期归档文章(按日期)
        /// </summary>
        public Pager GetArchivesArticlePaging(Pager pager, int tid, int year, int month,int day)
        {
            IQueryable<cms_varticle> query = GetArticles(tid, 0, 0).Where(m => m.createdate.Year == year && m.createdate.Month == month && m.createdate.Day == day);

            pager.Amount = query.Count();
            query = query.OrderByDescending(m => m.id).Skip(pager.PageSize * pager.PageNo).Take(pager.PageSize);
            pager.Entity = query;
            return pager;
        }

        /// <summary>
        /// 获取最新评论文章记录（基于reply）
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IQueryable<cms_varticle> GetReplyArticles(int tid, int cid, int count)
        {
            var cms_reply = entity.cms_article.Where(m => m.layer == 1);
            if (tid > 0)
                cms_reply = cms_reply.Where(m => m.typeid == tid);
            if (cid > 0)
                cms_reply = cms_reply.Where(m => m.cateid == cid);

            var query = (from r in cms_reply
                         join a in entity.cms_varticle
                         on r.parentid equals a.id
                         select new
                         {
                             id = r.id,
                             parentid = r.parentid,
                             username = r.username,
                             content = r.content,
                             rename = a.rename,
                             title = a.title,
                             url = a.url
                         }).OrderByDescending(m => m.id).Select(m => m);

            if (count > 0)
                query = query.Take(count);

            List<cms_varticle> varticles = query.ToList().Select(m => new cms_varticle
            {
                id = m.id,
                parentid = m.parentid,
                username = m.username,
                content = m.content,
                rename = m.rename,
                title = m.title,
                url = m.url
            }).ToList();

            return varticles.AsQueryable();
        }

        /// <summary>
        /// 文章按日期(天)归档
        /// </summary>
        public IQueryable<string> GetArticleDates()
        {
            var grouped = (from p in entity.cms_varticle
                           where p.typeid == 1
                           group p by new { day = p.createdate.Day, month = p.createdate.Month, year = p.createdate.Year } into d
                           select new
                           {
                               day = d.Key.day,
                               month = d.Key.month,
                               year = d.Key.year
                           });

            List<string> dates = grouped.ToList().Select(m => m.year.ToString() + "-" + m.month.ToString() + "-" + m.day.ToString()).ToList();

            return dates.AsQueryable();
        }
    }
}