using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using System.Data.SqlClient;

namespace YQCMS.Models
{
    /// <summary>
    /// 相册相关数据操作实现
    /// </summary>
    public partial class ServiceImpl : IServices
    {
        /// <summary>
        /// 获取指定Id的相册信息
        /// </summary>
        public AlbumModel GetAlbum(int id)
        {
            return GetAlbum(GetArticleByID(id));
        }

        /// <summary>
        /// 文章对象转换为相册对象
        /// </summary>
        /// <param name="varticle"></param>
        /// <returns></returns>
        public AlbumModel GetAlbum(cms_varticle varticle)
        {
            AlbumModel album = new AlbumModel();
            AlbumImageModel cover = new AlbumImageModel();
            if (varticle != null)
            {
                album.Id = varticle.id;
                album.ImageList = GetAlbumImageList(varticle.content, out cover);
                album.Cover = cover;
                album.Title = varticle.title;
                album.ReName = varticle.rename;
                album.Description = Utils.RemoveHtml(varticle.summary);
                album.ImgCount = album.ImageList.Count();
                album.CommentCount = varticle.subcount;
                album.ViewCount = varticle.viewcount;
                album.AlbumPath = GetCategoryPathUrl2(varticle.catepath);
                album.AlbumCategory = varticle.catename;
                album.Createdate = varticle.createdate;
                album.Favor = varticle.favor;
                album.Against = varticle.against;
            }
            return album;
        }

        /// <summary>
        /// 获取指定分类下所有相册信息
        /// </summary>
        public IQueryable<AlbumModel> GetAlbums(int cid)
        {
            IQueryable<cms_varticle> varticles = GetArticles(4, GetCategoryIds(cid), 0).OrderBy(m => m.catepath);
            List<AlbumModel> lst = new List<AlbumModel>();
            foreach (cms_varticle varticle in varticles)
            {
                lst.Add(GetAlbum(varticle));
            }
            return lst.AsQueryable();
        }

        /// <summary>
        /// 读取json,反序列化为相册图片列表
        /// </summary>
        /// <returns></returns>
        public List<AlbumImageModel> GetAlbumImageList(string content, out AlbumImageModel cover)
        {
            List<AlbumImageModel> lst = new List<AlbumImageModel>();
            cover = new AlbumImageModel();
            try
            {
                string jsonnav = Utils.RemoveHtml(content).Replace("\n", "").Replace("&nbsp;", "");
                if (jsonnav != "")
                {
                    lst = Utils.ParseFromJson<List<AlbumImageModel>>(jsonnav);
                    cover = lst.OrderByDescending(m => m.IsCover).FirstOrDefault();
                }
            }
            catch (Exception) { }
            return lst;
        }
    }
}