using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Common
{
    /// <summary>
    /// 常用工具类
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// 读取本地磁盘文件内容
        /// </summary>
        /// <param name="url">文件路径</param>
        /// <returns></returns>
        public static string GetFileSource(string path)
        {
            try
            {
                using (StreamReader myFile = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(path), Encoding.Default))
                { return myFile.ReadToEnd(); }
            }
            catch { return ""; }
        }


        /// <summary>
        /// 生成Json格式
        /// 首先应添加对System.ServiceModel.Web,System.Runtime.Serialization的引用，
        /// 然后添加System.Runtime.Serialization.Json命名空间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetJson<T>(T obj)
        {
            DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, obj);
                string szJson = Encoding.UTF8.GetString(stream.ToArray()); return szJson;
            }
        }
        /// <summary>
        /// 获取Json的Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="szJson"></param>
        /// <returns></returns>
        public static T ParseFromJson<T>(string szJson)
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(szJson)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// 返回文件是否存在
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否存在</returns>
        public static bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        /// <summary>
        /// 过滤字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FileterStr(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                str = Regex.Replace(str, @"(<script>)", "&lt;script&gt;", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                str = Regex.Replace(str, @"(</script>)", "&lt;/script&gt;", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                str = Regex.Replace(str, @"(&nbsp;){4,}", "&nbsp;&nbsp;&nbsp;&nbsp;", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                str = Regex.Replace(str, @"(<br>){1,}|(<br/>){1,}", "<br>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                str = Regex.Replace(str, @"(<br>){1,}", "<br>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            else
            { str = ""; }
            return str;
        }

        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }

        /// <summary>
        /// 得到网站的真实路径
        /// </summary>
        /// <returns></returns>
        public static string GetTrueWebPath()
        {
            string webPath = HttpContext.Current.Request.Path;
            if (webPath.LastIndexOf("/") != webPath.IndexOf("/"))
                webPath = webPath.Substring(webPath.IndexOf("/"), webPath.LastIndexOf("/") + 1);
            else
                webPath = "/";

            return webPath;
        }

        /// <summary>
        /// 获取站点根目录URL
        /// </summary>
        /// <returns></returns>
        public static string GetRootUrl(string forumPath)
        {
            int port = HttpContext.Current.Request.Url.Port;
            return string.Format("{0}://{1}{2}{3}",
                                 HttpContext.Current.Request.Url.Scheme,
                                 HttpContext.Current.Request.Url.Host.ToString(),
                                 (port == 80 || port == 0) ? "" : ":" + port,
                                 forumPath);
        }

        /// <summary>
        /// 验证文件名
        /// </summary>
        private static readonly char[] InvalidFileNameChars = new[]
                                                                  {
                                                                      '"',
                                                                      '<',
                                                                      '>',
                                                                      '|',
                                                                      '\0',
                                                                      '\u0001',
                                                                      '\u0002',
                                                                      '\u0003',
                                                                      '\u0004',
                                                                      '\u0005',
                                                                      '\u0006',
                                                                      '\a',
                                                                      '\b',
                                                                      '\t',
                                                                      '\n',
                                                                      '\v',
                                                                      '\f',
                                                                      '\r',
                                                                      '\u000e',
                                                                      '\u000f',
                                                                      '\u0010',
                                                                      '\u0011',
                                                                      '\u0012',
                                                                      '\u0013',
                                                                      '\u0014',
                                                                      '\u0015',
                                                                      '\u0016',
                                                                      '\u0017',
                                                                      '\u0018',
                                                                      '\u0019',
                                                                      '\u001a',
                                                                      '\u001b',
                                                                      '\u001c',
                                                                      '\u001d',
                                                                      '\u001e',
                                                                      '\u001f',
                                                                      ':',
                                                                      '*',
                                                                      '?',
                                                                      '\\',
                                                                      '/'
                                                                  };

        public static string CleanInvalidFileName(string fileName)
        {
            fileName = fileName + "";
            fileName = InvalidFileNameChars.Aggregate(fileName, (current, c) => current.Replace(c + "", ""));

            if (fileName.Length > 1)
                if (fileName[0] == '.')
                    fileName = "dot" + fileName.TrimStart('.');

            return fileName;
        }

        /// <summary>
        /// 将远程图片保存到服务器
        /// </summary>
        /// <param name="remotePath"></param>
        /// <param name="filePath"></param>
        public static string DownLoadImg(string url, string folderPath)
        {
            try
            {
                string fileName = url.Substring(url.LastIndexOf("/") + 1);
                if (fileName.IndexOf("?") > -1)
                {
                    fileName = fileName.Substring(0, fileName.IndexOf("?"));
                }
                fileName = CleanInvalidFileName(fileName);
                var fileFolder = HttpContext.Current.Server.MapPath(folderPath);
                DownloadImage(url, fileFolder + fileName);
                return folderPath + fileName;
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// 将远程图片保存到服务器
        /// </summary>
        /// <param name="remotePath"></param>
        /// <param name="filePath"></param>
        public static void DownloadImage(string remotePath, string filePath)
        {
            WebClient w = new WebClient();
            try
            {
                w.DownloadFile(remotePath, filePath);
            }
            finally
            {
                w.Dispose();
            }
        }

        /// <summary>
        /// 把内容中的图片下载到本地
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public static string DownloadImages(string htmlContent, string folderPath,string webUrl)
        {
            string newContent = htmlContent;
            //实例化HtmlAgilityPack.HtmlDocument对象
            HtmlDocument doc = new HtmlDocument();
            //载入HTML
            doc.LoadHtml(htmlContent);
            var imgs = doc.DocumentNode.SelectNodes("//img");
            if (imgs != null && imgs.Count > 0)
            {
                foreach (HtmlNode child in imgs)
                {
                    if (child.Attributes["src"] == null)
                        continue;

                    string imgurl = child.Attributes["src"].Value;

                    if (imgurl.IndexOf(webUrl) > -1 || imgurl.IndexOf("http://") == -1)
                        continue;

                    string newimgurl = DownLoadImg(imgurl, folderPath);
                    if (newimgurl != "")
                    {
                        newContent=newContent.Replace(imgurl, webUrl + newimgurl);
                    }
                }
            }
            return newContent;
        }


        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(result) || !IsIP(result))
                return "127.0.0.1";

            return result;
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }


        private static Regex RegexBr = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);
        /// <summary>
        /// 清除给定字符串中的回车及换行符
        /// </summary>
        /// <param name="str">要清除的字符串</param>
        /// <returns>清除后返回的字符串</returns>
        public static string ClearBR(string str)
        {
            Match m = null;

            for (m = RegexBr.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "");
            }
            return str;
        }


        public static Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
        /// <summary>
        /// 获得指定内容中的第一张图片
        /// </summary>
        /// <returns></returns>
        public static string GetImg(string str)
        {
            Match m = regImg.Match(str);
            if (m.Success)
                return m.Groups["imgUrl"].ToString();

            return "";
        }

        /// <summary>
        /// 获得指定内容中的所有图片
        /// </summary>
        /// <returns></returns>
        public static string GetImgs(string str)
        {
            string strTemp = "";
            MatchCollection matches = regImg.Matches(str);
            foreach (Match match in matches)
            {
                strTemp += "'" + match.Groups["imgUrl"].Value + "',";
            }

            return strTemp.Trim(',');
        }

        /// <summary>
        /// 删除指定内容中的所有图片
        /// </summary>
        /// <returns></returns>
        public static string ClearImgs(string str)
        {
            Match m = null;

            for (m = regImg.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "");
            }
            return str;
        }

        /// <summary>
        /// 根据Url获得源文件内容
        /// </summary>
        /// <param name="url">合法的Url地址</param>
        /// <returns></returns>
        public static string GetSourceTextByUrl(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Timeout = 20000;//20秒超时
                WebResponse response = request.GetResponse();

                Stream resStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(resStream);
                return sr.ReadToEnd();
            }
            catch { return ""; }
        }

        /// <summary>
        /// 移除Html标记
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            return Regex.Replace(content, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(string content)
        {
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }

        /// <summary>
        /// 从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string HTML)
        {
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(@"</?(?!br|/?p|img)[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return regEx.Replace(HTML, "");
        }

        /// <summary>
        /// 根据字符串获取枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">字符串枚举值</param>
        /// <param name="defValue">缺省值</param>
        /// <returns></returns>
        public static T GetEnum<T>(string value, T defValue)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch (ArgumentException)
            {
                return defValue;
            }
        }

        /// <summary>
        /// 删除字符串尾部的回车/换行/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RTrim(string str)
        {
            for (int i = str.Length; i >= 0; i--)
            {
                if (str[i].Equals(" ") || str[i].Equals("\r") || str[i].Equals("\n"))
                {
                    str.Remove(i, 1);
                }
            }
            return str;
        }

        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                        startIndex = startIndex - length;
                }

                if (startIndex > str.Length)
                    return "";
            }
            else
            {
                if (length < 0)
                    return "";
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                        return "";
                }
            }

            if (str.Length - startIndex < length)
                length = str.Length - startIndex;

            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ObjectToInt(object expression)
        {
            return ObjectToInt(expression, 0);
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ObjectToInt(object expression, int defValue)
        {
            if (expression != null)
                return StrToInt(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// 将对象转换为Int32类型,转换失败返回0
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string str)
        {
            return StrToInt(str, 0);
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string str, int defValue)
        {
            if (string.IsNullOrEmpty(str) || str.Trim().Length >= 11 || !Regex.IsMatch(str.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
                return defValue;

            int rv;
            if (Int32.TryParse(str, out rv))
                return rv;

            return Convert.ToInt32(StrToFloat(str, defValue));
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(string strValue, float defValue)
        {
            if ((strValue == null) || (strValue.Length > 10))
                return defValue;

            float intValue = defValue;
            if (strValue != null)
            {
                bool IsFloat = Regex.IsMatch(strValue, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                    float.TryParse(strValue, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// 获得指定数量的符号字符串
        /// </summary>
        /// <param name="count"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetSpace(int count, string str)
        {
            string tmp = "";
            for (int i = 1; i < count; i++)
            {
                tmp += str;
            }
            return tmp;
        }

    }
}