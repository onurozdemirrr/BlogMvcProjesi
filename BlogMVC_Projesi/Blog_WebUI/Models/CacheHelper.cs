using Blog_BussinesLayer;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Blog_WebUI.Models
{
    public class CacheHelper
    {
        public static List<Category> GetCategoriesFromCache()
        {
            // WebHelper içinde bulunan WebCache isimli class'ı kullanacağız. Bunun içinde Nuget içinden Microsoft.AspNet.WebHelpers paketi projemize dahil etmemiz gerekir.

            var result = WebCache.Get("category");
            if (result == null)
            {
                CategoryManager categoryManager = new CategoryManager();
                result = categoryManager.List();
                //WebCache.Set(key, value, cache'de kalacağı süre, her kullanımda Cache'de kalma süresi verilen değer kadar ötelenecek.)
                WebCache.Set("category", result, 20, true);
            }
            return result;
        }

        public static void RemoveCategorşesFromCache()
        {
            WebCache.Remove("category");
        }

        public static void Remove(string key)
        {
            WebCache.Remove(key);
        }
        
    }
}