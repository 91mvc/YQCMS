using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common
{
    /// <summary>
    /// 分页类
    /// </summary>
    public class Pager
    {
        private int _pageSize;
        /// <summary>
        /// 每页数据量
        /// </summary>
        public int PageSize
        {
            get { return _pageSize == 0 ? 10 : _pageSize; }
            set { _pageSize = value; }
        }
        private int _pageCount;
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get { return CounterPageCount(); }
            set { _pageCount = value; }
        }
        private int _pageNo;
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNo
        {
            get { return _pageNo - 1 == -1 ? 0 : _pageNo - 1; }
            set { _pageNo = value; }
        }
        /// <summary>
        /// 总数据量
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// 实体
        /// </summary>
        public IQueryable Entity { get; set; }
        /// <summary>
        /// 计算总页数
        /// </summary>
        /// <returns></returns>
        private int CounterPageCount()
        {
            if (Amount % PageSize == 0)
            {
                return Amount / PageSize > 0 ? Amount / PageSize : 0;
            }
            else
            {
                return Amount / PageSize > 0 ? Amount / PageSize + 1 : 0;
            }
        }
    }

    public class AjaxPager
    {
        private int pagesize = 4;
        private int currentpage = 1;
        private int maxpage = 1;
        private int prevPage;
        private int nextPage;

        public AjaxPager(int p_size, int p_currentpage, int p_maxpage)
        {
            this.pagesize = p_size;
            this.currentpage = p_currentpage;
            this.maxpage = p_maxpage;
            setPrevPage();
            setNextPage();
        }

        public void setPrevPage()
        {
            if (currentpage > 1)
                prevPage = currentpage - 1;
            else
                prevPage = currentpage;
        }

        public void setNextPage()
        {
            if (currentpage < maxpage)
                nextPage = currentpage + 1;
            else
                nextPage = currentpage;
        }

        public string getPageInfoHtml()
        {
            if (maxpage < 2)
                return "";
            string btnPrevPage = "<a href='javascript:;' p=" + prevPage + ">Preview</a>&nbsp;";
            string btnNextPage = "&nbsp;<a href='javascript:;' p=" + nextPage + ">Next</a>";
            if (currentpage == 1)
            {
                btnPrevPage = "Preview &nbsp;";
            }
            if (currentpage == maxpage)
            {
                btnNextPage = "&nbsp;Next";
            }
            string pageNumberList = "";
            if (maxpage > 2 * pagesize + 3)
            {
                pageNumberList = pageNumberList += getNumberPage(1);
                int start = 2;
                int end = maxpage - 1;
                int added = 0;

                if ((currentpage - pagesize) <= 2)
                {
                    added = 2 - (currentpage - pagesize);
                    end = currentpage + pagesize + added;
                }
                else if ((currentpage + pagesize) >= (maxpage - 1))
                {
                    added = (currentpage + pagesize) - (maxpage - 1);
                    start = currentpage - pagesize - added;
                }
                else
                {
                    start = currentpage - pagesize;
                    end = currentpage + pagesize;
                }

                if (start > 2)
                { pageNumberList = pageNumberList + "... "; }

                for (int i = start; i <= end; i++)
                {
                    pageNumberList += getNumberPage(i);
                }

                if (end < (maxpage - 1))
                { pageNumberList = pageNumberList + "... "; }

                pageNumberList += getNumberPage(maxpage);
            }
            else
            {
                for (int i = 1; i <= maxpage; i++)
                {
                    pageNumberList += getNumberPage(i);
                }
            }

            string html = btnPrevPage + pageNumberList + btnNextPage;
            return html;
        }

        public string getNumberPage(int p_pageno)
        {
            string t = "";
            string p = p_pageno.ToString();
            if (p_pageno == currentpage)
            { t = "<b style='color:red'>" + p + "</b>&nbsp;"; }
            else
            {
                t = "<a  href='javascript:;' p=" + p_pageno + ">" + p + "</a>&nbsp;";
            }
            return t;
        }
    }

}