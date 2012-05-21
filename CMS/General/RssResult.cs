using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel.Syndication;
using System.Xml;

namespace YQCMS.General
{
    /// <summary>
    /// 利用SyndicationFeed实现rss发布
    /// </summary>
    public class RssResult : ActionResult
    {
        private string _title;
        private string _desc;
        private Uri _altLink;

        private List<SyndicationItem> _items;

        public RssResult(string title, string desc, string link, List<SyndicationItem> items)
        {
            _title = title;
            _desc = desc;
            _altLink = new Uri(link);
            _items = items;

        }

        public override void ExecuteResult(ControllerContext context)
        {
            SyndicationFeed rss = new SyndicationFeed(
                _title,
                _desc,
                _altLink,
                _items
            );

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Entitize;

            using (XmlWriter _writer = XmlWriter.Create(context.HttpContext.Response.OutputStream, settings))
            {
                rss.SaveAsAtom10(_writer);
            }
        }
    }


}