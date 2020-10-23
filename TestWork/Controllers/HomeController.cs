using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
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
            var sitemap = map.SiteMap(website);
            var result = map.LoadingTimeForUrl(sitemap);
            result = result.OrderByDescending(e => Convert.ToInt32(e.Speed)).ToList();
            var chart = map.ConvertToChartData(result);
            ViewBag.DataPoints = chart;
            sw.Stop();
            Db db = new Db();
            db.AddToDb(result, ValidateUrl.CheckURL(website), sw.Elapsed.TotalSeconds.ToString());
            return View(result);
        }
        public ActionResult History()
        {
            ApplicationContext context = new ApplicationContext();
            var data = context.Histories.ToList();
            return View(data);
        }
    }
}