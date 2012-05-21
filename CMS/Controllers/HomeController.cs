using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;
using YQCMS.Models;
using Common;
using Ninject;
using System.Collections;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using YQCMS.General;

namespace YQCMS.Controllers
{
    public class HomeController : Controller
    {
        //表示该对象需要被注入依赖关系
        [Inject]
        public IServices cmsService { get; set; }
        //站点配置
        protected static GeneralConfigInfo configinfo = GeneralConfigs.GetConfig();

        /// <summary>
        /// 首页
        /// </summary>
        [OutputCache(CacheProfile = "CacheIndex")]
        public ActionResult Index(int? pageNo)
        {
            ViewBag.CI = configinfo;
            ViewBag.Title = configinfo.Webtitle;
            List<CategoryModel> lst = cmsService.GetIndexCategoryList();
            
            foreach(CategoryModel category in lst)
            {
                if (category.Type == "1" && Utils.StrToInt(category.ListNum) > 0)
                {
                    ViewData["DL_" + category.CateId] = cmsService.GetArticles(0, Convert.ToInt32(category.CateId), Utils.StrToInt(category.ListNum));
                }
                else if (category.Type == "2")
                {
                    ViewData["Data_" + category.CateId] = new cms_varticle();
                    var re = cmsService.GetArticles(0, Utils.StrToInt(category.CateId), 0);
                    if (re.Count() > 0)
                    {
                        ViewData["Data_" + category.CateId] = re.ToList()[0];
                    }
                }
                else if (category.Type == "7")
                {
                    var re = cmsService.GetArticles(0, Utils.StrToInt(category.CateId), 0);
                    if (re.Count() > 0)
                    {
                        foreach (cms_varticle varticle in re)
                        {
                            ViewData["D_" + varticle.id.ToString()] = varticle;
                        }
                    }
                }
            }
            //文章按天归档
            string strArticleDates = "[";
            foreach (string a in cmsService.GetArticleDates())
            {
                strArticleDates += "\""+a+"\",";
            }
            strArticleDates = "var strArticleDates=" + strArticleDates.Trim(',') + "];";
            ViewBag.ArticleDates = strArticleDates;
            //文章归档
            ViewBag.ArticleArchives = cmsService.GetArticleArchives();
            //论坛最新10条
            ViewBag.NewBbsTopics = cmsService.GetArticles(5, 0, 10);
            //访问最多的文章10条
            ViewBag.MostViewArticles = cmsService.GetArticles(1, 0, 10,"viewcount");
            //推荐最多的文章10条
            ViewBag.MostCommendArticles = cmsService.GetArticles(1, 0, 10, "favor");
            //评论最多的文章10条
            ViewBag.MostReplyArticles = cmsService.GetArticles(1, 0, 10, "subcount");
            //有最新评论文章10条
            ViewBag.NewReplyArticles = cmsService.GetArticles(1, 0, 10, "lastreplydate");
            ViewBag.NewReplyArticles2 = cmsService.GetReplyArticles(1, 0, 10);
            //ViewData["Categories"] = cmsService.GetCategories();
            Pager pager = new Pager();
            pager.PageNo = pageNo ?? 1;
            pager.PageSize = configinfo.IndexPagerCount;
            pager = cmsService.GetArticlePaging(pager,1,0);
            ViewBag.PageNo = pageNo ?? 1;//页码
            ViewBag.PageCount = pager.PageCount;//总页数
            string jsonSummaryImg = "";
            pager.Entity = GetNewArticleQuery(pager.Entity as IQueryable<cms_varticle>, out jsonSummaryImg);
            ViewBag.SummaryImgs = jsonSummaryImg;
            ViewBag.TestCache = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            return View(pager.Entity);
        }

        /// <summary>
        /// 记录结果处理
        /// </summary>
        private IQueryable<cms_varticle> GetNewArticleQuery(IQueryable<cms_varticle> d, out string jsonSummaryImg)
        {
            List<cms_varticle> list = new List<cms_varticle>();
            string str = "[";
            string img = "";
            Random ran = new Random(); 
            foreach (cms_varticle a in d)
            {

                list.Add(
                    new cms_varticle { 
                        id=a.id,
                        title=a.title,
                        cateid=a.cateid,
                        summary=Utils.ClearImgs(a.summary).Trim()==""?Utils.CutString(Utils.RemoveHtml(a.content), 0,180)+"...":Utils.ClearImgs(a.summary),
                        createdate=a.createdate,
                        viewcount=a.viewcount,
                        subcount=a.subcount,
                        catepath = cmsService.GetCategoryPathUrl(a.catepath),
                        rename=a.rename,
                        url=a.url
                    });
                //提取summary中第一张图
                img = Utils.GetImg(a.summary);
                //提取内容中的第一张图
                //if (img == "")
                //    img = Utils.GetImg(a.content);
                if (img == "")
                {
                    int RandKey = ran.Next(1001, 1000 + configinfo.DecorateImgCount);
                    img = "/Content" + configinfo.WebPath + "image/decorate/" + RandKey.ToString() + ".jpg";
                } 
                if (img != "")
                    str += "{\"id\":\"" + a.id.ToString() + "\",\"img\":\"" + img + "\"},";
            }
            jsonSummaryImg = "var summaryimgs=" + str.Trim(',') + "];";
            return list.AsQueryable<cms_varticle>();
        }

        /// <summary>
        /// 分类页,别名访问
        /// </summary>
        [OutputCache(CacheProfile = "CacheIndex")]
        public ActionResult CategoryByKey(string key, int? pageNo)
        {
            CategoryModel category = cmsService.GetCategoryByReName(key);
            if (category != null)
            {
                return Category(Utils.StrToInt(category.CateId), pageNo);
            }
            else
            {
                ViewBag.CI = configinfo;
                return View("Error");
            }
        }


        /// <summary>
        /// 分类页，ID访问
        /// </summary>
        [OutputCache(CacheProfile = "CacheIndex")]
        public ActionResult Category(int id, int? pageNo)
        {
            try
            {
                ViewBag.CI = configinfo;
                ViewBag.TestCache = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                CategoryModel category = cmsService.GetCategoryByID(id);
                ViewBag.Title = category.CateName;
                ViewBag.WebPath = cmsService.GetCategoryPathUrl(category.Path);
                Pager pager = new Pager();
                pager.PageNo = pageNo ?? 1;

                switch(category.Type)
                {
                    //单页
                    case "2":
                        cms_varticle varticle = new cms_varticle();
                        var re = cmsService.GetArticles(0,id,0);
                        if (re.Count() > 0)
                        {
                            //点击率
                            varticle.viewcount++;
                            cmsService.UpdateArticle(varticle);
                            varticle = re.ToList()[0];
                            ViewBag.VoteTip = IsVote(varticle.id) ? "你已经投过票！" : "请您对文章做出评价！";
                            ViewBag.PreNextLink = GetNextPreArticleLink(varticle.id);
                            ViewBag.ArticleId = varticle.id;
                            return View(WebUtils.GetViewName(category.CustomView, "Article"), varticle);
                        }
                        else
                            return View("Error");
                    //投稿
                    case "3":
                        ViewData["PublishWeb"] = new SelectList(cmsService.GetPublishWebList(), "Id", "Name");
                        ViewData["CateId"] = new SelectList(cmsService.getFCategoryList("", cmsService.GetCategoryIds(1)+"," + cmsService.GetCategoryIds(2)," -- "), "CateId", "CateName");
                        return View(WebUtils.GetViewName(category.CustomView, "Publish"));
                    //相册
                    case "4":
                        return View(WebUtils.GetViewName(category.CustomView, "Albums"), cmsService.GetAlbums(id));
                    //BBS
                    case "5":
                        PostModel post = new PostModel();
                        post.CateId = id;
                        post.UserName = User.Identity.Name;
                        BbsBoardModel boardinfo = cmsService.GetBbsBoard(id);
                        ViewBag.BoardInfo = boardinfo;
                        if (boardinfo.PostCount > 0)
                        {
                            pager.PageSize = 10;
                            pager = cmsService.GetArticlePaging(pager, 0, id);
                            ViewBag.PageNo = pageNo ?? 1;//页码
                            ViewBag.PageCount = pager.PageCount;//总页数
                            ViewBag.CateId = id;//分类Id
                            ViewData["PostList"] = pager.Entity;
                        }
                        ViewData["BoardList"] = cmsService.GetBbsBoards(id);
                        return View(WebUtils.GetViewName(category.CustomView, "Forum"), post);
                    //留言板
                    case "6":
                        NoteModel note = new NoteModel();
                        note.CateId = id;
                        note.UserName = User.Identity.Name;
                        pager.PageSize = configinfo.NotePagerCount;
                        pager = cmsService.GetArticlePaging(pager, 0, id);
                        ViewBag.PageNo = pageNo ?? 1;//页码
                        ViewBag.PageCount = pager.PageCount;//总页数
                        ViewBag.CateId = id;//分类Id
                        ViewData["NoteList"] = pager.Entity;
                        return View(WebUtils.GetViewName(category.CustomView, "Note"), note);
                    //文章列表
                    default:
                        pager.PageSize = configinfo.CatePagerCount;
                        bool HasSub = true;//是否还有子分类
                        if (Utils.StrToInt(category.SubCount) > 0)
                            pager = cmsService.GetArticlePaging(pager, 0, cmsService.GetCategoryIds(id));
                        else
                        {
                            pager = cmsService.GetArticlePaging(pager, 0, id);
                            HasSub = false;
                        }
                        ViewBag.HasSub = HasSub;
                        ViewBag.PageNo = pageNo ?? 1;//页码
                        ViewBag.PageCount = pager.PageCount;//总页数
                        ViewBag.CateId = id;//分类Id
                        string jsonSummaryImg = "";
                        pager.Entity = GetNewArticleQuery(pager.Entity as IQueryable<cms_varticle>, out jsonSummaryImg);
                        ViewBag.SummaryImgs = jsonSummaryImg;
                        ViewData["CateLinks"] = cmsService.getFCategoryList("1", "");
                        return View(WebUtils.GetViewName(category.CustomView, "Category"),pager.Entity);
                }
            }
            catch
            { return View("Error"); }
        }

        /// <summary>
        /// 留言本NoteBook
        /// </summary>
        public ActionResult NoteBook(int? pageNo)
        {
            ViewBag.CI = configinfo;
            CategoryModel category = cmsService.GetCategoryByReName("NoteBook");
            ViewBag.Title = category.CateName;
            ViewBag.WebPath = cmsService.GetCategoryPathUrl(category.Path);
            Pager pager = new Pager();
            pager.PageNo = pageNo ?? 1;

            NoteModel note = new NoteModel();
            note.CateId = Utils.StrToInt(category.CateId);
            note.UserName = User.Identity.Name;
            pager.PageSize = configinfo.NotePagerCount;
            pager = cmsService.GetArticlePaging(pager, 0, note.CateId);
            ViewBag.PageNo = pageNo ?? 1;//页码
            ViewBag.PageCount = pager.PageCount;//总页数
            ViewBag.CateId = note.CateId;//分类Id
            ViewData["NoteList"] = pager.Entity;
            return View(WebUtils.GetViewName(category.CustomView, "Note"), note);
        }

        /// <summary>
        /// 标签文章列表
        /// </summary>
        public ActionResult Tag(string key, int? pageNo)
        {
            ViewBag.CI = configinfo;
            Pager pager = new Pager();
            pager.PageNo = pageNo ?? 1;
            pager.PageSize = configinfo.CatePagerCount;
            pager = cmsService.GetTagArticlePaging(pager, 1, key.Replace("@", "."));
            ViewBag.PageNo = pageNo ?? 1;//页码
            ViewBag.PageCount = pager.PageCount;//总页数
            ViewBag.Path = "[标签] "+key.Replace("@", ".");
            string jsonSummaryImg = "";
            pager.Entity = GetNewArticleQuery(pager.Entity as IQueryable<cms_varticle>, out jsonSummaryImg);
            ViewBag.SummaryImgs = jsonSummaryImg;
            return View(pager.Entity);
        }

        /// <summary>
        /// 文章归档
        /// </summary>
        public ActionResult Archives(int year, int month, int? pageNo)
        {
            ViewBag.CI = configinfo;
            Pager pager = new Pager();
            pager.PageNo = pageNo ?? 1;
            pager.PageSize = configinfo.CatePagerCount;
            pager = cmsService.GetArchivesArticlePaging(pager,1,year,month);
            ViewBag.PageNo = pageNo ?? 1;//页码
            ViewBag.PageCount = pager.PageCount;//总页数
            ViewBag.Path = "[归档] " + year.ToString()+"年"+month.ToString()+"月";
            string jsonSummaryImg = "";
            pager.Entity = GetNewArticleQuery(pager.Entity as IQueryable<cms_varticle>, out jsonSummaryImg);
            ViewBag.SummaryImgs = jsonSummaryImg;
            return View("Tag", pager.Entity);
        }

        /// <summary>
        /// 文章按天归档
        /// </summary>
        public ActionResult ArchivesDate(int year, int month, int day, int? pageNo)
        {
            ViewBag.CI = configinfo;
            Pager pager = new Pager();
            pager.PageNo = pageNo ?? 1;
            pager.PageSize = configinfo.CatePagerCount;
            pager = cmsService.GetArchivesArticlePaging(pager, 1, year, month,day);
            ViewBag.PageNo = pageNo ?? 1;//页码
            ViewBag.PageCount = pager.PageCount;//总页数
            ViewBag.Path = year.ToString() + "年" + month.ToString() + "月"+day.ToString()+"日";
            string jsonSummaryImg = "";
            pager.Entity = GetNewArticleQuery(pager.Entity as IQueryable<cms_varticle>, out jsonSummaryImg);
            ViewBag.SummaryImgs = jsonSummaryImg;
            return View("Tag", pager.Entity);
        }
        

        /// <summary>
        /// 文章页
        /// </summary>
        public ActionResult Article(int id)
        {
            try
            {
                ViewBag.CI = configinfo;
                cms_varticle varticle = new cms_varticle();
                varticle = cmsService.GetArticleByID(id);
                //点击率
                varticle.viewcount++;
                cmsService.UpdateArticle(varticle);
                ViewBag.Title = string.IsNullOrWhiteSpace(varticle.seotitle) ? varticle.title : varticle.seotitle;
                string metaDescription=string.IsNullOrWhiteSpace(varticle.seodescription) ? "" : "<meta content=\""+varticle.seodescription+"\" name=\"Description\" />\r\n";
                string metaKeywords=string.IsNullOrWhiteSpace(varticle.seokeywords) ? "" : "<meta content=\""+varticle.seokeywords+"\" name=\"keywords\" />\r\n";
                string metaInfo = string.IsNullOrWhiteSpace(varticle.seometas) ? "" : varticle.seometas+"\r\n";
                ViewBag.Seo = metaDescription + metaKeywords + metaInfo;
                ViewBag.ArticleId = varticle.id;
                ViewBag.WebPath = cmsService.GetCategoryPathUrl(varticle.catepath);
                ViewBag.VoteTip = IsVote(id) ? "你已经投过票！" : "请您对文章做出评价！";
                ViewBag.PreNextLink = GetNextPreArticleLink(varticle.id);
                return View("Article",varticle);
            }
            catch
            { return View("Error"); }
        }

        /// <summary>
        /// 文章页
        /// </summary>
        public ActionResult ArticleByKey(string key)
        {
            try
            {
                cms_varticle varticle = new cms_varticle();
                varticle = cmsService.GetArticleByReName(key);
                if (varticle != null)
                {
                    return Article(varticle.id);
                }
                else
                { 
                    ViewBag.CI = configinfo;
                    return View("Error"); 
                }
            }
            catch
            { return View("Error"); }
        }
        

        /// <summary>
        ///  导航（没有用到）
        /// </summary>
        public ActionResult GetNav()
        {
            var navs = cmsService.GetCategories();
            return Content("navstr", "text/html;charset=UTF-8");
        }

        /// <summary>
        ///  回复
        /// </summary>
        public ActionResult Reply(int id)
        {
            ViewBag.CI = configinfo;
            ReplyModel reply = new ReplyModel();
            cms_varticle article = cmsService.GetArticleByID(id);
            CategoryModel category = cmsService.GetCategoryByID(article.cateid);
            reply.ArticleId = id;
            reply.CateId = article.cateid;
            if (category.ReplyPermit == "1" && article.replypermit==1)
                reply.ReplyPermit = 1;
            else
                reply.ReplyPermit = 2;
            reply.UserName = User.Identity.Name;
            return PartialView("_Comment", reply);
        }

        /// <summary>
        ///  提交评论
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AjaxReply(ReplyModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cms_varticle obj = new cms_varticle();

                    CategoryModel category = cmsService.GetCategoryByID(model.CateId);
                    obj.typeid = Utils.StrToInt(category.Type);
                    obj.cateid = model.CateId;
                    obj.catename = category.CateName;
                    obj.catepath = category.Path;
                    obj.articleid = model.ArticleId;
                    obj.title = string.IsNullOrWhiteSpace(model.Email) ? "" : model.Email;
                    obj.summary = string.IsNullOrWhiteSpace(model.Url) ? "" : model.Url;
                    obj.content = Utils.FileterStr(Server.UrlDecode(model.Reply).Trim());
                    obj.ip = Utils.GetIP();
                    obj.layer = 1;
                    obj.orderid = 1;
                    obj.parentid = model.ArticleId;
                    obj.replypermit = 1;
                    obj.seodescription = "";
                    obj.seokeywords = "";
                    obj.seometas = "";
                    obj.seotitle = "";
                    obj.status = 0;

                    if (Request.IsAuthenticated)
                    {
                        obj.userid = 1;
                        obj.username = User.Identity.Name;
                    }
                    else
                    {
                        obj.userid = 0;
                        obj.username = model.UserName;
                    }
                    obj.viewcount = 0;
                    cmsService.AddArticle(obj);
                }
                return Content("0", "text/html;charset=UTF-8");
            }
            catch
            {
                return Content("1", "text/html;charset=UTF-8");
            }
        }

        /// <summary>
        ///  评论列表
        /// </summary>
        public ActionResult CommentList(int id, int? pageNo)
        {
            Pager pager = new Pager();
            pager.PageNo = pageNo ?? 1;
            pager.PageSize = configinfo.CommentPagerCount;
            pager = cmsService.GetReplyPaging(pager, id);
            ViewBag.PageNo = pageNo ?? 1;//页码
            ViewBag.PageCount = pager.PageCount;//总页数
            ViewBag.ArticleId = id;//文章Id
            AjaxPager ajaxpager = new AjaxPager(3, pageNo ?? 1, pager.PageCount);
            ViewBag.AjaxPager = ajaxpager.getPageInfoHtml();
            return PartialView("_CommentList", pager.Entity);
        }

        /// <summary>
        ///  提交留言
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Note(NoteModel model)
        {
            if (ModelState.IsValid)
            {
                cms_varticle obj = new cms_varticle();

                CategoryModel category = cmsService.GetCategoryByID(model.CateId);
                obj.articleid = 0;
                obj.typeid = Utils.StrToInt(category.Type);
                obj.cateid = model.CateId;
                obj.catename = category.CateName;
                obj.catepath = category.Path;
                //obj.title = Utils.CutString(Utils.RemoveHtml(model.Message), 0, 10);
                obj.title = string.IsNullOrWhiteSpace(model.Email) ? "" : model.Email;
                obj.summary = string.IsNullOrWhiteSpace(model.Url) ? "" : model.Url;
                obj.content = Utils.FileterStr(Server.UrlDecode(model.Message));
                obj.ip = Utils.GetIP();
                obj.layer = 0;
                obj.orderid = 1;
                obj.parentid = 0;
                obj.replypermit = 0;
                obj.seodescription = "";
                obj.seokeywords = "";
                obj.seometas = "";
                obj.seotitle = "";
                obj.status = 0;
                if (Request.IsAuthenticated)
                {
                    obj.userid = 1;
                    obj.username = User.Identity.Name;
                }
                else
                {
                    obj.userid = 0;
                    obj.username = model.UserName;
                }
                obj.viewcount = 0;
                cmsService.AddArticle(obj);
                return Redirect(WebUtils.GetCateUrl(category));

            }
            return View(model);
        }

        /// <summary>
        ///  提交评分
        /// </summary>
        [HttpPost]
        public ActionResult AjaxVote(int articleid,int vote)
        {
            try
            {
                if (IsVote(articleid))
                    return VoteJsonRe(1, "你已经投过票！", 0);
                //记录评分cookie
                if (Request.Cookies["cms.vote"] == null)
                {
                    HttpCookie voteCookie = new HttpCookie("cms.vote");
                    voteCookie["articleid"] = articleid.ToString()+",";
                    voteCookie.Expires = DateTime.Now.AddDays(7d);
                    Response.Cookies.Add(voteCookie);
                }
                else
                {
                    string cookieString = Request.Cookies["cms.vote"]["articleid"];
                    string[] cookies = cookieString.Split(new char[] { ',' });
                    if (!cookies.Contains(articleid.ToString()))
                    {
                        HttpCookie voteCookie = new HttpCookie("cms.vote");
                        voteCookie["articleid"] = cookieString + articleid.ToString() + ",";
                        voteCookie.Expires = DateTime.Now.AddDays(7d);
                        Response.Cookies.Add(voteCookie);
                    }
                }
                int re = 0;
                cms_varticle varticle = new cms_varticle();
                varticle = cmsService.GetArticleByID(articleid);
                if (vote == 1)
                {
                    varticle.favor++;
                    re = varticle.favor;
                }
                else
                {
                    varticle.against++;
                    re = varticle.against;
                }
                cmsService.UpdateArticle(varticle);
                return VoteJsonRe(0, "谢谢您的参与！", re);
            }
            catch
            {
                return VoteJsonRe(1, "意外错误！", 0);  
            }
        }

        /// <summary>
        /// 投票返回Json提示
        /// </summary>
        private JsonResult VoteJsonRe(int error, string message, int value)
        {
            Hashtable hash = new Hashtable();
            hash["error"] = error;
            hash["message"] = message;
            hash["value"] = value.ToString();
            return Json(hash, "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 是否已投过票
        /// </summary>
        private bool IsVote(int articleid)
        {
            if (Request.Cookies["cms.vote"] == null)
                return false;

            string cookieString = Request.Cookies["cms.vote"]["articleid"];
            if (string.IsNullOrWhiteSpace(cookieString))
                return false;
            else
            {
                string[] cookies = cookieString.Split(new char[] { ',' });
                if (cookies.Contains(articleid.ToString()))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 表单投稿
        /// </summary
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Publish(ArticleModel model)
        {
            if (ModelState.IsValid)
            {
                cms_varticle obj = new cms_varticle();

                CategoryModel category = cmsService.GetCategoryByID(model.CateId);
                obj.articleid = 0;
                obj.typeid = Utils.StrToInt(category.Type);
                obj.cateid = model.CateId;
                obj.catename = category.CateName;
                obj.catepath = category.Path;
                obj.title = Utils.FileterStr(model.Title);
                obj.summary = string.IsNullOrWhiteSpace(model.Summary) ? "" : Utils.FileterStr(Server.UrlDecode(Utils.DownloadImages(model.Summary, "/Content" + configinfo.WebPath + "Upload/", configinfo.Weburl)));
                obj.content = Utils.FileterStr(Server.UrlDecode(Utils.DownloadImages(model.Content, "/Content" + configinfo.WebPath + "Upload/", configinfo.Weburl)));
                obj.ip = Utils.GetIP();
                obj.layer = 0;
                obj.orderid = 1;
                obj.parentid = 0;
                obj.replypermit = Convert.ToByte(category.ReplyPermit); ;
                obj.seodescription = "";
                obj.seokeywords = "";
                obj.seometas = "";
                obj.seotitle = "";
                obj.status = 2;
                obj.iscommend = 2;
                obj.istop = 2;
                if (Request.IsAuthenticated)
                {
                    obj.userid = 1;
                    obj.username = User.Identity.Name;
                }
                else
                {
                    obj.userid = 0;
                    obj.username = "";
                }
                obj.viewcount = 0;
                int re=cmsService.AddArticle(obj);
                return Redirect(WebUtils.GetArticleUrl(re, ""));
            }
            return View(model);
        }

        /// <summary>
        /// web-url投稿
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PublishWeb(int cateid, int publishwebid, string url)
        {
            try
            {
                PublishWebModel publishWeb = cmsService.GetPublishWebByID(publishwebid);

                if (publishWeb.Content.Trim() == "")
                    return PublishWebJsonRe(1, "提交失败，转载网站配置Content不能为空。");

                string webtitle = "";
                string author = "";
                string authorWeb = "";
                string title = "";
                string content = "";
                string copyright="";
                HtmlNode authorNode = null;
                HtmlNode titleNode = null;
                HtmlNode contentNode = null;
                var web = new HtmlWeb();
                HtmlDocument doc = new HtmlDocument();
                doc = web.Load(url.Trim());

                var webtitleNode = doc.DocumentNode.SelectSingleNode("//title");

                if (webtitleNode != null)
                    webtitle = webtitleNode.InnerText;

                //内容
                if (publishWeb.Content.Trim().Substring(0, 1) == "#")
                { contentNode = doc.GetElementbyId(Utils.CutString(publishWeb.Content.Trim(), 1)); }
                else
                { contentNode = doc.DocumentNode.SelectSingleNode("//*[@class=\"" + Utils.CutString(publishWeb.Content.Trim(), 1) + "\"]"); }

                //标题
                if (publishWeb.Title.Trim() != "")
                {
                    if (publishWeb.Title.Trim().Substring(0, 1) == "#")
                    { titleNode = doc.GetElementbyId(Utils.CutString(publishWeb.Title.Trim(), 1)); }
                    else
                    { titleNode = doc.DocumentNode.SelectSingleNode("//*[@class=\"" + Utils.CutString(publishWeb.Title.Trim(), 1) + "\"]"); }
                }

                //作者
                if (publishWeb.Author.Trim() != "")
                {
                    if (publishWeb.Author.Trim().Substring(0, 1) == "#")
                    { authorNode = doc.GetElementbyId(Utils.CutString(publishWeb.Author.Trim(), 1)); }
                    else
                    { authorNode = doc.DocumentNode.SelectSingleNode("//*[@class=\"" + Utils.CutString(publishWeb.Author.Trim(), 1) + "\"]"); }
                }

                if (titleNode != null)
                    title = titleNode.InnerText;
                else
                    title = Regex.Split(webtitle, " - ")[0];

                if (authorNode != null)
                    author = authorNode.InnerText;
                else
                    author = Regex.Split(webtitle, " - ")[1];

                //cnblogs.com
                if (publishWeb.Id == "1" && authorNode != null)
                    authorWeb = authorNode.Attributes["href"].Value.ToString();

                content = contentNode.InnerHtml;

                if (title.Trim() == "" || content.Trim() == "")
                    return PublishWebJsonRe(1, "提交失败，未能正确采集到数据。");

                if (author.Trim() != "")
                    copyright = author.Trim();
                if (authorWeb.Trim() != "" && copyright != "")
                    copyright = "<a href=\"" + authorWeb + "\" target=\"_blank\">" + copyright + "</a>";

                copyright = (copyright != "" ? "作者：" + copyright+" &nbsp;" : "") + "<a href=\"" + url + "\" target=\"_blank\">原文链接</a>";
                content += "<div class=\"article-copyright\">"+copyright+"</div>";
                cms_varticle obj = new cms_varticle();

                CategoryModel category = cmsService.GetCategoryByID(cateid);
                obj.articleid = 0;
                obj.typeid = Utils.StrToInt(category.Type);
                obj.cateid = cateid;
                obj.catename = category.CateName;
                obj.catepath = category.Path;
                obj.title = Utils.FileterStr(title);
                obj.summary = "";
                obj.content = Utils.FileterStr(Server.UrlDecode(Utils.DownloadImages(content, "/Content" + configinfo.WebPath + "Upload/", configinfo.Weburl)));
                obj.ip = Utils.GetIP();
                obj.layer = 0;
                obj.orderid = 1;
                obj.parentid = 0;
                obj.replypermit = Convert.ToByte(category.ReplyPermit);
                obj.seodescription = "";
                obj.seokeywords = "";
                obj.seometas = "";
                obj.seotitle = "";
                obj.status = 1;
                obj.iscommend = 2;
                obj.istop = 2;
                if (Request.IsAuthenticated)
                {
                    obj.userid = 1;
                    obj.username = User.Identity.Name;
                }
                else
                {
                    obj.userid = 0;
                    obj.username = "";
                }
                obj.viewcount = 0;

                int re = cmsService.AddArticle(obj);
                return PublishWebJsonRe(0, "提交成功，你可以<a href=\"" + configinfo.WebPath + "\">转到首页</a> <a href=\"" + WebUtils.GetArticleUrl(re,"") + "\">查看文章</a>  或者继续提交。");
            }
            catch
            {
                return PublishWebJsonRe(1, "提交失败，暂时不支持该页面Url提交。");
            }
        }

        /// <summary>
        /// url投稿json异步返回值
        /// </summary>
        private JsonResult PublishWebJsonRe(int error, string message)
        {
            Hashtable hash = new Hashtable();
            hash["error"] = error;
            hash["message"] = message;
            return Json(hash, "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 上一篇，下一篇文章链接
        /// </summary>
        private string GetNextPreArticleLink(int id)
        {
            string linkstr = "";
            cms_varticle prearticle = cmsService.GetPreviewArticle(id);
            cms_varticle nextarticle = cmsService.GetNextArticle(id);

            if (prearticle!=null&&!string.IsNullOrWhiteSpace(prearticle.title))
                linkstr += "<p>上一篇：<a href=\"" + WebUtils.GetArticleUrl(prearticle.url) + "\">" + prearticle.title + "</a></p>";
            if (nextarticle != null && !string.IsNullOrWhiteSpace(nextarticle.title))
                linkstr += "<p>下一篇：<a href=\"" + WebUtils.GetArticleUrl(nextarticle.url) + "\">" + nextarticle.title + "</a></p>";
            return linkstr;
        }

        /// <summary>
        /// 上一个，下一个相册链接
        /// </summary>
        private string GetNextPreAlbumLink(int id)
        {
            string linkstr = "";
            cms_varticle prearticle = cmsService.GetPreviewArticle(id,4);
            cms_varticle nextarticle = cmsService.GetNextArticle(id,4);

            if (prearticle != null && !string.IsNullOrWhiteSpace(prearticle.title))
                linkstr += "<p>上一个相册：<a href=\"" + configinfo.WebPath + "Album/" + prearticle.id.ToString() + "\">" + prearticle.title + "</a></p>";
            if (nextarticle != null && !string.IsNullOrWhiteSpace(nextarticle.title))
                linkstr += "<p>上一个相册：<a href=\"" + configinfo.WebPath + "Album/" + nextarticle.id.ToString() + "\">" + nextarticle.title + "</a></p>";
            return linkstr;
        }

        /// <summary>
        /// 相册展示页
        /// </summary>
        public ActionResult Album(int id)
        {
            try
            {
                ViewBag.CI = configinfo;
                cms_varticle varticle = cmsService.GetArticleByID(id);
                AlbumModel album = cmsService.GetAlbum(varticle);
                //点击率
                varticle.viewcount++;
                cmsService.UpdateArticle(varticle);
                ViewBag.Title = varticle.title;
                ViewBag.ArticleId = varticle.id;
                album.AlbumPath = cmsService.GetCategoryPathUrl(varticle.catepath);
                ViewBag.VoteTip = IsVote(id) ? "你已经投过票！" : "请您做出评价！";
                ViewBag.PreNextLink = GetNextPreAlbumLink(varticle.id);
                return View(album);
            }
            catch
            { return View("Error"); }
        }

        /// <summary>
        /// 论坛发帖
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Post(PostModel model)
        {
            if (ModelState.IsValid)
            {
                cms_varticle obj = new cms_varticle();

                CategoryModel category = cmsService.GetCategoryByID(model.CateId);
                obj.articleid = 0;
                obj.typeid = Utils.StrToInt(category.Type);
                obj.cateid = model.CateId;
                obj.catename = category.CateName;
                obj.catepath = category.Path;
                obj.title = string.IsNullOrWhiteSpace(model.Title) ? "" : model.Title;
                obj.summary = "";
                obj.content = Utils.FileterStr(Server.UrlDecode(Utils.DownloadImages(model.Message, "/Content" + configinfo.WebPath + "Upload/",configinfo.Weburl)));
                obj.ip = Utils.GetIP();
                obj.layer = 0;
                obj.orderid = 1;
                obj.parentid = 0;
                obj.replypermit = 0;
                obj.seodescription = "";
                obj.seokeywords = "";
                obj.seometas = "";
                obj.seotitle = "";
                obj.status = 0;
                if (Request.IsAuthenticated)
                {
                    obj.userid = 1;
                    obj.username = User.Identity.Name;
                }
                else
                {
                    obj.userid = 0;
                    obj.username = model.UserName;
                }
                obj.viewcount = 0;

                cmsService.AddArticle(obj);

                return Redirect(WebUtils.GetCateUrl(category));

            }

            return View("Error");
        }

        /// <summary>
        /// 论坛帖子显示页
        /// </summary>
        public ActionResult Thread(int id, int? pageNo)
        {
            try
            {
                ViewBag.CI = configinfo;
                cms_varticle varticle = new cms_varticle();
                varticle = cmsService.GetArticleByID(id);
                //点击率
                varticle.viewcount++;
                cmsService.UpdateArticle(varticle);
                ViewBag.Article = varticle;
                ViewBag.Title = varticle.title;
                ViewBag.WebPath = cmsService.GetCategoryPathUrl(varticle.catepath);

                PostReplyModel postreply = new PostReplyModel();
                postreply.PostId = id;
                postreply.CateId = varticle.cateid;
                postreply.UserName = User.Identity.Name;

                Pager pager = new Pager();
                pager.PageNo = pageNo ?? 1;
                pager.PageSize = 10;
                pager = cmsService.GetPostPaging(pager, id);
                ViewBag.PageNo = pageNo ?? 1;//页码
                ViewBag.PageCount = pager.PageCount;//总页数
                ViewData["ThreadList"] = pager.Entity;

                return View(postreply);
            }
            catch
            { return View("Error"); }
        }

        /// <summary>
        /// 论坛回复
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PostReply(PostReplyModel model)
        {
            if (ModelState.IsValid)
            {
                cms_varticle obj = new cms_varticle();

                CategoryModel category = cmsService.GetCategoryByID(model.CateId);
                obj.typeid = Utils.StrToInt(category.Type);
                obj.cateid = model.CateId;
                obj.catename = category.CateName;
                obj.catepath = category.Path;
                obj.articleid = model.PostId;
                obj.title = "";
                obj.summary = "";
                obj.content = Utils.FileterStr(Server.UrlDecode(Utils.DownloadImages(model.Message, "/Content" + configinfo.WebPath + "Upload/", configinfo.Weburl)));
                obj.ip = Utils.GetIP();
                obj.layer = 1;
                obj.parentid = model.PostId;


                if (Request.IsAuthenticated)
                {
                    obj.userid = 1;
                    obj.username = User.Identity.Name;
                }
                else
                {
                    obj.userid = 0;
                    obj.username = model.UserName;
                }
                obj.viewcount = 0;
                cmsService.AddArticle(obj);

                return Redirect("" + configinfo.WebPath + "Thread/" + model.PostId.ToString());

            }

            return View("Error");
        }

        /// <summary>
        /// 站点rss
        /// </summary>
        public RssResult Rss()
        {
            return new RssResult(configinfo.Webtitle, configinfo.WebDescription, configinfo.Weburl + configinfo.WebPath + "rss", cmsService.GetRss(cmsService.GetArticles(1, 0, 20).ToList()));
        }

    }
}
