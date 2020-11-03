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
            catch (WebException ex)
            {
                return null;
            }
        }
        public List<Map> LoadingTimeForUrl(List<string> url)
        {

            int countId = 1;
            List<Task> tasks = new List<Task>();
            foreach (var check in url)
            {
                Task task = Task.Run(() =>
                {
                    try
                    {
                        if (Uri.IsWellFormedUriString(check, UriKind.Absolute))
                        {
                            var sw = Stopwatch.StartNew();
                            Uri myUri = new Uri(check);
                            var request = (HttpWebRequest)WebRequest.Create(check);
                            request.Method = "GET";
                            request.AllowAutoRedirect = false;
                            request.Credentials = CredentialCache.DefaultCredentials;
                            request.Accept = "*/*";
                            request.ContentType = "application/x-www-form-urlencoded";

                            //request.Proxy = new System.Net.WebProxy("193.203.11.229", 8085);
                            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.0.5) Gecko/2008120122 Firefox/3.0.5";
                            WebResponse responce = request.GetResponse();
                            sw.Stop();
                            string urlfoorm = null;
                            if (check.Contains("?") == true)
                            {
                                int pos = check.LastIndexOf('?');
                                string bbb = check.Substring(pos);
                                urlfoorm = check.Substring(0, pos);
                                result.Add(new Map { IdNumb = countId, Url = urlfoorm, Speed = sw.ElapsedMilliseconds.ToString() });
                                countId++;
                            }
                            else
                            {
                                result.Add(new Map { IdNumb = countId, Url = check, Speed = sw.ElapsedMilliseconds.ToString() });
                                countId++;
                            }
                        }
                    }
                    catch (UriFormatException e)
                    {
                        Console.WriteLine(e);
                    }
                    //catch (WebException e)
                    //{
                    //    Console.WriteLine(e);
                    //}
                    catch (InvalidCastException e)
                    {
                        Console.WriteLine(e);
                    }
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
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