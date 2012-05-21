using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.ServiceModel.Syndication;
using System.Xml;


namespace YQCMS.Models
{
    /// <summary>
    /// RSS数据操作实现
    /// </summary>
    public partial class ServiceImpl : IServices
    {
        /// <summary>
        /// 获取rss
        /// </summary>
        public List<SyndicationItem> GetRss(List<cms_varticle> list)
        {
            List<SyndicationItem> ret = new List<SyndicationItem>();

            foreach (var r in list)
            {
                var synObj = new SyndicationItem();
                synObj.Title = TextSyndicationContent.CreatePlaintextContent(r.title);
                synObj.Summary = TextSyndicationContent.CreatePlaintextContent(r.summary);
                synObj.Content = TextSyndicationContent.CreateHtmlContent(r.content);
                synObj.Links.Add(new SyndicationLink(new Uri(configinfo.Weburl+configinfo.WebPath+r.url)));
                synObj.PublishDate = new DateTimeOffset(r.createdate);
                synObj.LastUpdatedTime = new DateTimeOffset(r.createdate);
                ret.Add(synObj);
            }
            return ret;
        }
    }
}