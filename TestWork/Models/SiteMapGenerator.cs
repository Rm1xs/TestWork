using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace TestWork.Models
{
    public class SiteMapGenerator
    {
        public IEnumerable<string> ParseSite(string url)
        {
            Uri myUri = new Uri(url);
            List<Task> tasks = new List<Task>();
            try
            {

                string data = null;
                List<string> newurl = new List<string>();
                using (WebClient wc = new WebClient())
                {
                    wc.Headers["User-Agent"] = "Mozilla/5.0";
                    wc.Encoding = System.Text.Encoding.UTF8;
                    data = wc.DownloadString(url);
                }
                var parser = new HtmlParser();
                var document = parser.ParseDocument(data);
                var links = document.QuerySelectorAll("a");
                foreach (var link in links)
                {
                    Task task = Task.Run(() =>
                    {
                        if (link.GetAttribute("href") != null)
                        {
                            var urldata = IsUrlValid(link.GetAttribute("href"), url);
                            if (urldata != null)
                            {
                                newurl.Add(urldata);
                            }
                        }
                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());

                var uniqurl = newurl.Distinct();
                return uniqurl;
            }

            catch (WebException ex)
            {
                return null;
            }
        }
        public string SiteMapCreator(IEnumerable<string> url)
        {
            try
            {
                using (var sw = new StringWriter())
                {
                    using (var xw = XmlWriter.Create(sw))
                    {
                        //xw.WriteStartDocument();
                        //xw.WriteStartElement("urlset");
                        xw.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
                        xw.WriteStartElement("url");
                        foreach (var item in url)
                        {
                            xw.WriteElementString("loc", string.Format(item));
                            xw.WriteEndElement();
                            xw.WriteStartElement("url");
                        }
                    }
                    return sw.ToString();
                }
            }

            catch (XmlException ex)
            {
                return null;
            }
        }

        public List<string> SiteMap(string document)
        {
            List<string> linkList = new List<string>();
            try
            {
                XmlDocument urldoc = new XmlDocument();
                urldoc.LoadXml(document);

                //with <url>
                XmlNodeList xmlSitemapList = urldoc.GetElementsByTagName("url");
                if (xmlSitemapList.Count != 0)
                {
                    foreach (XmlNode node in xmlSitemapList)
                    {
                        if (node["loc"] != null)
                        {
                            linkList.Add(node["loc"].InnerText);

                        }
                    }
                }
                //with <sitemap>
                else
                {
                    xmlSitemapList = urldoc.GetElementsByTagName("sitemap");
                    foreach (XmlNode node in xmlSitemapList)
                    {
                        if (node["loc"] != null)
                        {
                            linkList.Add(node["loc"].InnerText);
                        }
                    }
                }
                return linkList;
            }
            //catch if not sitemap or not xml structure
            catch (XmlException ex)
            {
                return null;
            }
            catch (WebException ex)
            {
                return null;
            }
        }
        private string IsUrlValid(string url, string domein)
        {
            Uri myUri = new Uri(domein);
            string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (reg.IsMatch(url) == true)
            {
                if (url.Contains("http://") || url.Contains("https://"))
                {
                    if (url.Contains(myUri.Host))
                    {
                        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                        {
                            return url;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (url.Contains("//"))
                    {
                        if (url.Contains(myUri.Host))
                        {
                            if (Uri.IsWellFormedUriString(myUri.Scheme + "://" + url, UriKind.Absolute))
                            {
                                return myUri.Scheme + "://" + url;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        if (Uri.IsWellFormedUriString(myUri.Scheme + "://" + url, UriKind.Absolute))
                        {
                            if (url.Contains(myUri.Host))
                            {
                                return myUri.Scheme + "://" + url;
                            }
                            else
                            {
                                return null;
                            }
                        }

                        else
                        {
                            return null;
                        }
                    }
                }
            }
            else
            {
                if (!url.Contains("http://") || !url.Contains("https://"))
                {
                    url = myUri.Scheme + "://" + myUri.Host + url;
                    if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    {
                        return url;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (url.Contains(myUri.Host))
                    {
                        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                        {
                            return url;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}