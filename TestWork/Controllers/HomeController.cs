using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TestWork.EF;
using TestWork.Models;

namespace TestWork.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string website)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            GetSiteMap map = new GetSiteMap();
            string checkurl = ValidateUrl.CheckURL(website);


            var sitemap = map.SiteMap(checkurl);
            if (sitemap != null)
            {
                var result = map.LoadingTimeForUrl(sitemap);
                result = result.OrderByDescending(e => Convert.ToInt32(e.Speed)).ToList();
                var chart = map.ConvertToChartData(result);
                ViewBag.DataPoints = chart;
                sw.Stop();
                Db db = new Db();
                db.AddToDb(result, ValidateUrl.CheckURL(website), sw.Elapsed.TotalSeconds.ToString());
                return View(result);
            }
            else
            {
                ViewData["Message"] = "Sitemap not found! Searching for this url - " + checkurl;
                return View();
            }
        }
        public ActionResult History()
        {
            ApplicationContext context = new ApplicationContext();
            var data = context.Histories.ToList();
            return View(data);
        }
        public ActionResult Details(int id)
        {

            GetSiteMap map = new GetSiteMap();
            ApplicationContext context = new ApplicationContext();
            var data = context.Histories.Include(c => c.ChildURLs).Where(c => c.Id == id).FirstOrDefault();
            var chart = map.ConvertToChartData(data.ChildURLs.ToList());
            ViewBag.DataPoints = chart;
            return View(data);
        }

        public JsonResult GetData(int id)
        {
            List<TableMap> maps = new List<TableMap>();
            try
            {
                ApplicationContext context = new ApplicationContext();
                var y = context.Histories.Include(c => c.ChildURLs).Where(c => c.Id == id).FirstOrDefault();
                foreach (var a in y.ChildURLs)
                {
                    maps.Add(new TableMap { URL = a.Url, Speed = a.Speed });
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
            }
            return Json(new { data = maps }, JsonRequestBehavior.AllowGet);
        }
        public class TableMap
        {
            public string URL { get; set; }
            public string Speed { get; set; }
        }

    }
}