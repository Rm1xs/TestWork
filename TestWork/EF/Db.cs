using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TestWork.EF;

namespace TestWork.Models
{
    public class Db
    {
        public void AddToDb(IEnumerable<Map> maps, string url, string time)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                var chekinfo = context.Histories.Where(x => x.URL == url).Include(x => x.ChildURLs).FirstOrDefault();
                if (chekinfo == null)
                {
                    History history = new History { ChildURLs = maps.ToList(), DateCheck = DateTime.Now, URL = url, TimeForCheck = time, MaxValue = maps.ElementAt(0).Speed, MinValue = maps.LastOrDefault().Speed };
                    context.Histories.Add(history);
                    context.SaveChanges();
                }
                else
                {
                    chekinfo.URL = url;
                    chekinfo.ChildURLs = maps.ToList();
                    chekinfo.DateCheck = DateTime.Now;
                    chekinfo.TimeForCheck = time;
                    chekinfo.MaxValue = maps.ElementAt(0).Speed;
                    chekinfo.MinValue = maps.LastOrDefault().Speed;
                    context.SaveChanges();
                }
            }
        }
    }
}