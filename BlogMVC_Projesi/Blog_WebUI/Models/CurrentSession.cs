using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog_WebUI.Models
{
    public class CurrentSession
    {
        public static BlogUser User
        {
            // Bu metotu static yapıyorum. Newlemeden ulaşabilmek için.
            // Session'da bulunan kullanıcının alınması için kullanılacak.

            get
            {
                //if (HttpContext.Current.Session["login"] != null)
                //{
                //    return HttpContext.Current.Session["login"] as BlogUser;
                //} 
                return Get<BlogUser>("login");
            }
        }

        
        // Aşağıdaki metotta, Generic bir yapı kullandım. Sadece BlogUser türünde değil, farklı türden verileri de Session'a koyabilmek için kullanacağım metottur.
        // CurrenSession.Set<string>("Name", "Onur")
        // CurrenSession.Set<BlogUser>("login", [BlogUser türündeki nesneyi vereceğiz])
        public static void Set<T>(string key, T obj)
        {
            HttpContext.Current.Session[key] = obj;
        }

        // Session'daki veriyi almak için kullanacağım Generic Get metodu.
        public static T Get<T>(string key)
        {
            if (HttpContext.Current.Session[key] != null)
            {
                return (T)HttpContext.Current.Session[key];
            }

            return default(T);
        }

        // Session'da bulunan bir veriyi, parametresini vererek, sessiondan kaldırmak veya silmek için kullanacağım metot.
        public static void Remove(string key)
        {
            if (HttpContext.Current.Session[key] != null)
            {
                HttpContext.Current.Session.Remove(key);
            }
        }

        // Sessiondaki bütün veriyi temizliyor.
        public static void Clear()
        {
            HttpContext.Current.Session.Clear();
        }
    }
}

// Session'da verinin saklanma süresi 20 dakikadır. Eğer sayfada herhangi bir işlem olmazsa ve 20 dakika geçerse bu bilgi session'dan silinir.
// Sessionda tutulacak verinin tutulma süresini değiştirebiliriz.
// web.config dosyası içinde 
// Veri, timeout' verdiğimiz sayısal değer kadar dakika 
// <system.web>
// <sessionState mode="Inproc" timeout="60">
//

// <system.web>