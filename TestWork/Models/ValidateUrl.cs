﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestWork.Models
{
    public static class ValidateUrl
    {
        public static string CheckURL(string url)
        {
            if (url.Contains("sitemap.xml") && url.Contains("/sitemap.xml"))
            {
                return url;
            }
            else if (url.Length != 0 && url[url.Length - 1] == '/')
            {
                return url + "sitemap.xml";
            }
            else
            {
                return url + "/sitemap.xml";
            }
        }
    }
}