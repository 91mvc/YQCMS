using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YQCMS.Models;
using System.Web.Routing;
using Common;
using System.Configuration;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text;
using Ninject;
using YQCMS.General;

namespace YQCMS.Controllers
{
    [Authorize(Roles = "Admin")]
    //[Authorize]
    public class AdminController : Controller
    {
        [Inject]
        public IServices cmsService { get; set; }

        protected static GeneralConfigInfo configinfo = GeneralConfigs.GetConfig();

        public ActionResult Index()
        {
            //网站配置信息
            ViewBag.CI = configinfo;
            return View("AdminIndex");
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        public ActionResult AdminArticle(int? pageNo,int? tid,int? layer)
        {
            ViewBag.CI = configinfo;
            Pager pager = new Pager();
            int typeid = tid ?? 1;
            int layerid = layer ?? 0;
            pager.PageNo = pageNo ?? 1;
            pager.PageSize = 20;
            pager = cmsService.GetReplyPaging(pager, typeid, 0, layerid);
            ViewBag.PageNo = pageNo ?? 1;//页码
            ViewBag.PageCount = pager.PageCount;//总页数
            ViewBag.TypeId = typeid;
            ViewBag.LayerId = layerid;
            return View(pager.Entity);
        }
        
        /// <summary>
        /// 新增文章
        /// </summary>
        public ActionResult AdminAdd(string tid)
        {
            ViewBag.CI = configinfo;
            ViewData["CateId"] = new SelectList(cmsService.getFCategoryList(tid, ""," -- "), "CateId", "CateName");
            return View();
        }
  
        /// <summary>
        /// 新增文章（提交）
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminAdd(ArticleModel model)
        {
            //添加[ValidateInput(false)]特性，否则提交内容有html代码会报错
            if (ModelState.IsValid)
            {
                cms_varticle obj = new cms_varticle();
                CategoryModel category = cmsService.GetCategoryByID(model.CateId);
                obj.articleid = 0;
                obj.typeid = Utils.StrToInt(category.Type);
                obj.cateid = model.CateId;
                obj.catename = category.CateName;
                obj.catepath = category.Path; ;
                obj.title = Utils.FileterStr(model.Title);
                obj.summary = Utils.FileterStr(Server.UrlDecode(model.Summary));
                obj.content = Utils.FileterStr(Server.UrlDecode(Utils.DownloadImages(model.Content, "/Content"+configinfo.WebPath+"Upload/", configinfo.Weburl)));
                obj.ip = Utils.GetIP();
                obj.tags = (string.IsNullOrWhiteSpace(model.Tags) ? "" : model.Tags).Trim();
                obj.layer = 0;
                obj.orderid = 1;
                obj.parentid = 0;
                obj.replypermit = model.ReplyPermit;
                obj.seodescription = Utils.RemoveHtml((string.IsNullOrWhiteSpace(model.SeoDescription) ? "" : model.SeoDescription)).Trim();
                obj.seokeywords = Utils.RemoveHtml((string.IsNullOrWhiteSpace(model.Seokeywords) ? "" : model.Seokeywords)).Trim();
                obj.seometas = string.IsNullOrWhiteSpace(model.SeoMetas) ? "" : model.SeoMetas;
                obj.seotitle = Utils.RemoveHtml((string.IsNullOrWhiteSpace(model.SeoTitle) ? "" : model.SeoTitle)).Trim();
                obj.rename = Utils.RemoveHtml((string.IsNullOrWhiteSpace(model.ReName) ? "" : model.ReName)).Trim();
                obj.status = model.Status;
                obj.userid = 1;
                obj.username = User.Identity.Name;
                obj.iscommend = model.IsCommend;
                obj.istop = model.IsTop;

                int re=cmsService.AddArticle(obj);

                return Redirect(configinfo.WebPath+"Admin/AdminArticle/?tid="+obj.typeid);

            }
            return View(model);
        }

        /// <summary>
        /// 文章修改
        /// </summary>
        public ActionResult AdminEdit(int id)
        {
            ViewBag.CI = configinfo;
            //根据ID获取文章信息
            cms_varticle obj = new cms_varticle();
            obj = cmsService.GetArticleByID(id);

            //把修改操作需要的字段赋给ArticleModel对象
            ArticleModel model = new ArticleModel();
            model.ID = obj.id;
            model.CateId = obj.cateid;
            model.Title = Utils.FileterStr(obj.title);
            model.Summary = string.IsNullOrWhiteSpace(obj.summary) ? "" : obj.summary;
            model.Content = string.IsNullOrWhiteSpace(obj.content) ? "" : obj.content;
            model.Tags = string.IsNullOrWhiteSpace(obj.tags) ? "" : obj.tags;
            model.SeoDescription = string.IsNullOrWhiteSpace(obj.seodescription) ? "" : obj.seodescription;
            model.Seokeywords = string.IsNullOrWhiteSpace(obj.seokeywords) ? "" : obj.seokeywords;
            model.SeoMetas = string.IsNullOrWhiteSpace(obj.seometas) ? "" : obj.seometas;
            model.SeoTitle = string.IsNullOrWhiteSpace(obj.seotitle) ? "" : obj.seotitle;
            model.ReName = string.IsNullOrWhiteSpace(obj.rename) ? "" : obj.rename;
            model.Status = obj.status;
            model.ReplyPermit = obj.replypermit;
            model.IsCommend = obj.iscommend;
            model.IsTop = obj.istop;

            ViewData["CateId"] = new SelectList(cmsService.getFCategoryList(obj.typeid.ToString(),""," -- "), "CateId", "CateName", model.CateId);
            return View(model);
        }

        /// <summary>
        /// 提交文章修改
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminEdit(ArticleModel model)
        {
            if (ModelState.IsValid)
            {
                cms_varticle obj = new cms_varticle();
                CategoryModel category = cmsService.GetCategoryByID(model.CateId);
                obj.id = model.ID;
                obj.typeid = Utils.StrToInt(category.Type);
                obj.cateid = model.CateId;
                obj.catename = category.CateName;
                obj.catepath = category.Path;
                obj.articleid = 0;
                obj.title = Utils.FileterStr(model.Title);
                obj.summary = Utils.FileterStr(Server.UrlDecode(model.Summary));
                obj.content = Utils.FileterStr(Server.UrlDecode(Utils.DownloadImages(model.Content, "/Content" + configinfo.WebPath + "Upload/", configinfo.Weburl)));
                obj.ip = Utils.GetIP();
                obj.tags = (string.IsNullOrWhiteSpace(model.Tags) ? "" : model.Tags).Trim();
                obj.layer = 0;
                obj.orderid = 1;
                obj.parentid = 0;
                obj.replypermit = model.ReplyPermit;
                obj.seodescription = Utils.RemoveHtml((string.IsNullOrWhiteSpace(model.SeoDescription) ? "" : model.SeoDescription)).Trim();
                obj.seokeywords = Utils.RemoveHtml((string.IsNullOrWhiteSpace(model.Seokeywords) ? "" : model.Seokeywords)).Trim();
                obj.seometas = string.IsNullOrWhiteSpace(model.SeoMetas) ? "" : model.SeoMetas;
                obj.seotitle = Utils.RemoveHtml((string.IsNullOrWhiteSpace(model.SeoTitle) ? "" : model.SeoTitle)).Trim();
                obj.rename = Utils.RemoveHtml((string.IsNullOrWhiteSpace(model.ReName) ? "" : model.ReName)).Trim();
                obj.status = model.Status;
                obj.userid = 1;
                obj.username = User.Identity.Name;
                obj.iscommend = model.IsCommend;
                obj.istop = model.IsTop;

                cmsService.UpdateVArticle(obj);

                return Redirect(configinfo.WebPath+"Admin/AdminArticle/?tid=" + obj.typeid);

            }
            return View(model);
        }

        /// <summary>
        /// 文章删除
        /// </summary>
        [HttpPost]
        public ActionResult AdminDel(int id, int parentid)
        {
            try
            {
                cms_varticle obj = new cms_varticle();
                obj.id = id;
                obj.parentid = parentid;
                cmsService.DelArticle(obj);
            }
            catch (Exception)
            {
                return Content( "删除失败！", "text/html;charset=UTF-8");
            }
            return Content("删除成功！", "text/html;charset=UTF-8");

        }

        /// <summary>
        /// 分类配置管理
        /// </summary>
        public ActionResult AdminCategory()
        {
            ViewBag.CI = configinfo;
            ViewBag.Content = cmsService.GetCategoryStr();
            return View();
        }

        /// <summary>
        /// 提交分类配置（手动修改）
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminCategory(string content)
        {
            try
            {
                List<CategoryModel> lst = Utils.ParseFromJson<List<CategoryModel>>(content);
                List<CategoryModel> newlst = RefreshCateList(lst);
                SaveCateInfo(newlst);
            }
            catch (Exception)
            {
                return Content("修改失败！格式不符合要求。<a href=\"" + configinfo.WebPath + "admin/AdminCategory\">继续修改</a>", "text/html;charset=UTF-8");
            }
            return Content("修改成功！<a href=\"" + configinfo.WebPath + "admin/AdminCategory\">继续修改</a>", "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 刷新catelist，更新subcuont,path字段
        /// </summary>
        public List<CategoryModel> RefreshCateList(List<CategoryModel> lst)
        {
            List<CategoryModel> newlst = new List<CategoryModel>();
            foreach (CategoryModel c in lst)
            {
                CategoryModel category = new CategoryModel();
                category = c;
                category.Path = GetCategoryPath(lst, category);
                category.SubCount = GetSubCount(lst, category.CateId).ToString();
                newlst.Add(category);
            }
            return newlst;
        }

        /// <summary>
        /// 保存分类信息到json
        /// </summary>
        public void SaveCateInfo(List<CategoryModel> lst)
        {
            string jsonstr = Utils.GetJson<List<CategoryModel>>(lst);
            string file = System.Web.HttpContext.Current.Server.MapPath("/Content" + configinfo.WebPath + "Category.js");

            using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                writer.Write("var category = " + jsonstr);
            }
            DataCache.SetCache("Json-Category", jsonstr, DateTime.Now.AddMinutes(180.0), TimeSpan.Zero);
        }

        /// <summary>
        /// 计算当前分类的子分类数
        /// </summary>
        private int GetSubCount(List<CategoryModel> lst, string parent)
        {
            int total = 0;
            foreach (CategoryModel c in lst)
            {
                if (c.ParentId == parent)
                    total++;
            }
            return total;
        }

        /// <summary>
        /// 计算当前分类的path
        /// </summary>
        private string GetCategoryPath(List<CategoryModel> lst, CategoryModel category)
        {
            CategoryModel c = category;
            string path = GetCurrentCategoryUrl(c);
            while (c.ParentId != "0")
            {
                foreach (CategoryModel cc in lst)
                {
                    if (cc.CateId == c.ParentId)
                    {
                        path = GetCurrentCategoryUrl(cc) + path;
                        c = cc;
                        continue;
                    }
                }
            }
            return path.Trim(',');
        }

        private string GetCurrentCategoryUrl(CategoryModel category)
        {
            return "," + category.CateId;
        }

        public ActionResult RestoreCategory()
        {
            try
            {
                string file = System.Web.HttpContext.Current.Server.MapPath("/Content" + configinfo.WebPath + "Category.js");
                System.IO.File.Copy(System.Web.HttpContext.Current.Server.MapPath("/Content"+configinfo.WebPath+"Categorybak.js"), file, true);
            }
            catch (Exception)
            {
            }
            return RedirectToAction("AdminCategory");
        }

        /// <summary>
        /// 论坛json配置修改
        /// </summary>
        public ActionResult AdminBBSConfig()
        {
            ViewBag.CI = configinfo;
            ViewBag.Content = cmsService.GetBBSExtendedStr();
            return View();
        }

        /// <summary>
        /// 提交论坛json配置修改
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminBBSConfig(string content)
        {
            try
            {
                List<BbsExtendedModel> lst = Utils.ParseFromJson<List<BbsExtendedModel>>(content);
                string jsonstr = Utils.GetJson<List<BbsExtendedModel>>(lst);
                string file = System.Web.HttpContext.Current.Server.MapPath("/Content"+configinfo.WebPath+"BBS.js");
                using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
                {
                    writer.Write(jsonstr);
                }
                DataCache.SetCache("Json-BBS", jsonstr, DateTime.Now.AddMinutes(180.0), TimeSpan.Zero);
            }
            catch (Exception)
            {
                return Content("修改失败！格式不符合要求。<a href=\"" + configinfo.WebPath + "admin/AdminBBSConfig\">继续修改</a>", "text/html;charset=UTF-8");
            }
            return Content("修改成功！<a href=\"" + configinfo.WebPath + "admin/AdminBBSConfig\">继续修改</a>", "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 转载网站json配置修改
        /// </summary>
        public ActionResult AdminPublishWebConfig()
        {
            ViewBag.CI = configinfo;
            ViewBag.Content = cmsService.GetPublishWebStr();
            return View();
        }

        /// <summary>
        /// 提交转载网站json配置修改
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminPublishWebConfig(string content)
        {
            try
            {
                List<PublishWebModel> lst = Utils.ParseFromJson<List<PublishWebModel>>(content);
                string jsonstr = Utils.GetJson<List<PublishWebModel>>(lst);
                string file = System.Web.HttpContext.Current.Server.MapPath("/Content"+configinfo.WebPath+"PublishWeb.js");
                using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
                {
                    writer.Write(jsonstr);
                }
                DataCache.SetCache("Json-PublishWeb", jsonstr, DateTime.Now.AddMinutes(180.0), TimeSpan.Zero);
            }
            catch (Exception)
            {
                return Content("修改失败！格式不符合要求。<a href=\"" + configinfo.WebPath + "admin/AdminPublishWebConfig\">继续修改</a>", "text/html;charset=UTF-8");
            }
            return Content("修改成功！<a href=\"" + configinfo.WebPath + "admin/AdminPublishWebConfig\">继续修改</a>", "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 站点基础设置
        /// </summary>
        public ActionResult AdminBaseConfig()
        {
            ViewBag.CI = configinfo;
            return View(configinfo);
        }

        /// <summary>
        /// 提交站点基础设置
        /// </summary>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdminBaseConfig(GeneralConfigInfo model)
        {
            try
            {
                GeneralConfigInfo config = configinfo;
                config.Weburl = model.Weburl;
                config.Webtitle = model.Webtitle;
                config.WebPath = string.IsNullOrEmpty(model.WebPath) ? "/" : model.WebPath;
                config.Icp = model.Icp;
                config.IndexPagerCount = model.IndexPagerCount;
                config.CatePagerCount = model.CatePagerCount;
                config.CommentPagerCount = model.CommentPagerCount;
                config.NotePagerCount = model.NotePagerCount;
                config.WebDescription = model.WebDescription;
                GeneralConfigs.Serialiaze(config, Server.MapPath(ConfigurationManager.AppSettings["WebConfig"].ToString()));
            }
            catch (Exception)
            {
                return Content("修改失败！<a href=\"" + configinfo.WebPath + "admin/AdminBaseConfig\">继续修改</a>", "text/html;charset=UTF-8");
            }
            return Content("修改成功！<a href=\"" + configinfo.WebPath + "admin/AdminBaseConfig\">继续修改</a>", "text/html;charset=UTF-8");
        }


        //【上传图片】
        //KindEditor 返回JSON格式说明： 
        //格式：{"error":0,"message":".....","url":"/img/1111.gif"} 
        //error：0成功，1失败，成功需要指定url值为图片/文件保存后的URL地址，如果error值不为0，则设置message值为错误提示信息 
        /// <summary>
        /// KE文件上传
        /// </summary>
        [HttpPost]
        public ActionResult UploadFile(string dir)
        {
            //定义一个返回提示Hashtable对象
            Hashtable hash = new Hashtable(); 
            //文件保存路径
            string savePath = "/Content" + configinfo.WebPath + "Attached/";
            //文件Url
            string saveUrl = configinfo.Weburl + "/Content" + configinfo.WebPath + "Attached/";
            //文件大小限制
            int maxSize = 1000000; 
            //定义允许上传的文件扩展名
            Hashtable extTable = new Hashtable();
            extTable.Add("image", "gif,jpg,jpeg,png,bmp");
            extTable.Add("flash", "swf,flv");
            extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
            extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2");

            //针对不同的上传文件类型做差异化设置
            switch (dir)
            {
                case "image":
                    savePath = "/Content" + configinfo.WebPath + "Upload/";
                    saveUrl = configinfo.Weburl + "/Content" + configinfo.WebPath + "Upload/";
                    break;
                case "flash":
                    break;
                case "media":
                    break;
                case "file":
                    break;
            }

            //获得上传文件
            HttpPostedFileBase file = Request.Files["imgFile"];
            //文件保存磁盘路径
            string dirPath = Server.MapPath(savePath);
            //文件名及后缀
            string fileName = file.FileName;
            string fileExt = Path.GetExtension(fileName).ToLower();
            
            //文件是否为空在前端也判断过一次
            if (file == null)
                return UploadJsonRe(1, "请选择文件", "");

            if (!Directory.Exists(dirPath))
                return UploadJsonRe(1, "上传目录不存在", "");

            if (file.InputStream == null || file.InputStream.Length > maxSize)
                return UploadJsonRe(1, "上传文件大小超过限制", ""); 

            if (string.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable[dir]).Split(','), fileExt.Substring(1).ToLower()) == -1)
                return UploadJsonRe(1, "上传文件扩展名是不允许的扩展名", "");

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt; 
            string filePath = dirPath + newFileName; 
            file.SaveAs(filePath); 
            string fileUrl = saveUrl + newFileName; 

            return UploadJsonRe(0, "", fileUrl);    
        }

        /// <summary>
        /// 上传返回提示
        /// </summary>
        private JsonResult UploadJsonRe(int error, string message, string url)
        {
            Hashtable hash = new Hashtable();
            hash["error"] = error;
            hash["message"] = message;
            hash["url"] = url;
            return Json(hash, "text/html;charset=UTF-8"); 
        }

        /// <summary>
        /// 分类排序
        /// </summary>
        public ActionResult AdminCategorySort()
        {
            ViewBag.CI = configinfo;
            var lst = cmsService.getFCategoryList(" -- ");
            return View(lst);
        }

        /// <summary>
        /// 保存分类排序
        /// </summary>
        [HttpPost]
        public ActionResult AdminCategorySort(string ids)
        {
            List<CategoryModel> orderlst = new List<CategoryModel>();
            List<CategoryModel> newlst = new List<CategoryModel>();
            string[] arrId = ids.Trim(',').Split(',');
            for (int i = 0; i < arrId.Length; i++)
            {
                int orderid = i + 1;
                CategoryModel category = new CategoryModel();
                category = cmsService.GetCategoryByID(Utils.StrToInt(arrId[i]));
                category.OrderId = orderid.ToString();
                orderlst.Add(category);
            }
            GetNewCategoryList(orderlst, ref newlst, "0");
            SaveCateInfo(newlst);
    
            string re = "";
            var lst = cmsService.getFCategoryList(" -- ");
            foreach (CategoryModel c in lst)
            {
                string rootClass = c.ParentId == "0" ? " class=\"cl_root\"" : "";
                re += "<li id=\"" + c.CateId + "\"" + rootClass + ">" + c.CateName + "</li>";
            }
            return Content(re, "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 重置分类OrderStr
        /// </summary>
        private void GetNewCategoryList(List<CategoryModel> orderlst, ref List<CategoryModel> newlst, string parentId)
        {
            List<CategoryModel> tmplist = new List<CategoryModel>();
            foreach (CategoryModel c in orderlst)
            {
                if (c.ParentId == parentId)
                {
                    CategoryModel category = c;
                    category.OrderId = (newlst.Count() + 1).ToString();
                    newlst.Add(category);

                    if (Utils.StrToInt(c.SubCount) > 0)
                    {
                        GetNewCategoryList(orderlst, ref newlst, c.CateId);
                    }
                    continue;
                }
            }
        }

        /// <summary>
        /// 新增分类
        /// </summary>
        public ActionResult AdminCategoryAdd(int id)
        {
            ViewBag.CI = configinfo;
            ViewBag.CateId = id;
            ViewBag.CateName = id>0?cmsService.GetCategoryByID(id).CateName:"";
            ViewData["Type"] = new SelectList(WebUtils.GetTypeList(), "TypeId", "TypeName", 1);
            CategoryModel category = new CategoryModel();
            category.ParentId = id.ToString();
            if (id > 0)
                category.Type = cmsService.GetCategoryByID(id).Type;
            return View(category);
        }

        /// <summary>
        /// 新增分类
        /// </summary>
        [HttpPost]
        public ActionResult AdminCategoryAdd(CategoryModel model)
        {
            List<CategoryModel> newlst = new List<CategoryModel>();
            List<CategoryModel> catelist = cmsService.getFCategoryList();
            CategoryModel category = new CategoryModel();
            category.CateId = (cmsService.GetMaxCategoryID() + 1).ToString();
            category.CateName = model.CateName;
            category.IsIndex = model.IsIndex;
            category.IsNav = model.IsNav;
            category.ListNum = string.IsNullOrWhiteSpace(model.ListNum) ? "0" : model.ListNum;
            category.ParentId = model.ParentId;
            category.ReName = string.IsNullOrWhiteSpace(model.ReName) ? "" : model.ReName;
            category.CustomView = string.IsNullOrWhiteSpace(model.CustomView) ? "" : model.CustomView;
            category.ReplyPermit = model.ReplyPermit;
            category.Status = model.Status;
            category.Type = model.Type;
            category.OrderId = (catelist.Count() + 1).ToString();
            category.SubCount = "0";
            category.Path = category.CateId;
            if (category.ParentId == "0")
            {
                catelist.Add(category);
                newlst = catelist;
            }
            else
            {
                catelist.Add(category);
                //刷新subcount,path值
                catelist = RefreshCateList(catelist);
                //重置顺序orderid
                GetNewCategoryList(catelist, ref newlst, "0");
            }
            //保存为json
            SaveCateInfo(newlst);
            return Content("新增成功！<a href=\"" + configinfo.WebPath + "admin/AdminCategorySort\">查看</a>", "text/html;charset=UTF-8");
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        public ActionResult AdminCategoryEdit(int id)
        {
            ViewBag.CI = configinfo;
            CategoryModel category = cmsService.GetCategoryByID(id);
            ViewData["Type"] = new SelectList(WebUtils.GetTypeList(), "TypeId", "TypeName", category.Type);
            List<CategoryModel> list = cmsService.getFCategoryList("", "", " -- ").Where(m => m.CateId != category.CateId).ToList();
            list.Add(new CategoryModel { CateId="0",CateName="Root"});
            ViewData["ParentId"] = new SelectList(list, "CateId", "CateName", category.ParentId);
            return View(category);
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        [HttpPost]
        public ActionResult AdminCategoryEdit(CategoryModel model)
        {
            List<CategoryModel> catelist = cmsService.getFCategoryList();
            List<CategoryModel> newlst = new List<CategoryModel>();
            List<CategoryModel> newlst2 = new List<CategoryModel>();
            CategoryModel category = new CategoryModel();
            category.CateId = model.CateId;
            category.CateName = model.CateName;
            category.IsIndex = model.IsIndex;
            category.IsNav = model.IsNav;
            category.ListNum = string.IsNullOrWhiteSpace(model.ListNum) ? "0" : model.ListNum;
            category.ParentId = model.ParentId;
            category.ReName = string.IsNullOrWhiteSpace(model.ReName) ? "" : model.ReName;
            category.CustomView = string.IsNullOrWhiteSpace(model.CustomView) ? "" : model.CustomView;
            category.ReplyPermit = model.ReplyPermit;
            category.Status = model.Status;
            category.Type = model.Type;

            category.OrderId = (catelist.Count() + 1).ToString();
            category.SubCount = "0";
            category.Path = "0";
            
            foreach (CategoryModel c in catelist)
            {
                if (category.CateId == c.CateId)
                {
                    if (category.ParentId == c.ParentId)
                    {
                        category.OrderId = c.OrderId;
                        category.SubCount = c.SubCount;
                        category.Path = c.Path;
                    }
                    newlst.Add(category);
                }
                else
                    newlst.Add(c);
            }

            if (category.Path == "0")
            {
                //刷新subcount,path值
                newlst = RefreshCateList(newlst);
                //重置顺序orderid
                GetNewCategoryList(newlst, ref newlst2, "0");
            }
            else
            { newlst2 = newlst; }

            //保存为json
            SaveCateInfo(newlst2);
            return Content("修改成功！<a href=\"" + configinfo.WebPath + "admin/AdminCategorySort\">查看</a>", "text/html;charset=UTF-8");
        }


        /// <summary>
        /// 删除分类
        /// </summary>
        [HttpPost]
        public ActionResult AdminCategoryDel(int id)
        {
            List<CategoryModel> catelist = cmsService.getFCategoryList();
            List<CategoryModel> newlst = new List<CategoryModel>();
            List<CategoryModel> newlst2 = new List<CategoryModel>();
            foreach (CategoryModel c in catelist)
            {
                if (id.ToString() != c.CateId)
                    newlst.Add(c);
            }

            GetNewCategoryList(newlst, ref newlst2, "0");
            newlst2 = RefreshCateList(newlst2);

            //保存为json
            SaveCateInfo(newlst2);
            return Content("删除成功！<a href=\"" + configinfo.WebPath + "admin/AdminCategorySort\">查看</a>", "text/html;charset=UTF-8");
        }
    }
}
