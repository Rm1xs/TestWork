using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace TestWork.Models
{
    public class GetSiteMap
    {
        List<string> linkList = new List<string>();
        List<Map> result = new List<Map>();
        public List<string> SiteMap(string url)
        {
            WebClient wc = new WebClient();
            wc.Headers["User-Agent"] = "Mozilla/5.0";
            wc.Encoding = System.Text.Encoding.UTF8;
            try
            {
                string sitemapString = wc.DownloadString(url);
                XmlDocument urldoc = new XmlDocument();
                urldoc.LoadXml(sitemapString);

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
            catch(WebException ex)
            {
                return null;
            }
        }
        public List<Map> LoadingTimeForUrl(List<string> url)
        {
            try
            {
                int countId = 1;
                List<Task> tasks = new List<Task>();
                foreach (var check in url)
                {
                    Task task = Task.Run(() =>
                    {
                        var sw = Stopwatch.StartNew();
                        var request = (HttpWebRequest)WebRequest.Create(check);
                        request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
                        WebResponse responce = request.GetResponse();
                        sw.Stop();
                        result.Add(new Map { IdNumb = countId, Url = check, Speed = sw.ElapsedMilliseconds.ToString() });
                        countId++;
                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
            }
            return result;
        }
        public string ConvertToChartData(List<Map> result)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            foreach (var convert in result)
            {
                dataPoints.Add(new DataPoint(Convert.ToDouble(convert.IdNumb), Convert.ToDouble(convert.Speed)));
            }
            var data = JsonConvert.SerializeObject(dataPoints);
            return data;
        }
    }
}