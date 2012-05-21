using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using System.Configuration;

namespace YQCMS.Models
{
    /// <summary>
    /// 投稿相关数据操作实现
    /// </summary>
    public partial class ServiceImpl : IServices
    {
        /// <summary>
        /// 获取指定Id 站点信息
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        public PublishWebModel GetPublishWebByID(int id)
        {
            PublishWebModel web = new PublishWebModel();
            try
            {
                List<PublishWebModel> lst = GetPublishWebList();
                web = lst.Find((PublishWebModel p) => { return p.Id == id.ToString(); });
                return web;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 读取json,反序列化
        /// </summary>
        /// <returns></returns>
        public List<PublishWebModel> GetPublishWebList()
        {
            List<PublishWebModel> lst = new List<PublishWebModel>();
            try
            {
                string jsonnav = GetPublishWebStr().Replace("\n", "");
                if (jsonnav != "")
                {
                    lst = Utils.ParseFromJson<List<PublishWebModel>>(jsonnav);
                }
            }
            catch (Exception) { }
            return lst;
        }

        /// <summary>
        /// 读取json文件,返回字符串
        /// </summary>
        /// <returns></returns>
        public string GetPublishWebStr()
        {
            string cacheKey = "Json-PublishWeb";
            object cache = DataCache.GetCache(cacheKey);
            if (cache == null)
            {
                try
                {
                    cache = Utils.GetFileSource("/Content" +configinfo.WebPath+"PublishWeb.js").Trim();
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