using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace TestWork.Models
{
    public class SiteMapGenerator
    {
        public IEnumerable<string> ParseSite(string url)
        {
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
                    if (link.GetAttribute("href") != null)
                    {
                        if (IsUrlValid(link.GetAttribute("href"), url) == true)
                        {
                            newurl.Add(link.GetAttribute("href"));
                        }
                    }
                }
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
        private bool IsUrlValid(string url, string domain)
        {
            Uri myUri = new Uri(domain);
            string host = myUri.Host;
            if (url.Contains(host))
            {
                string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
                Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return reg.IsMatch(url);
            }
            else
            {
                return false;
            }
        }
    }
}