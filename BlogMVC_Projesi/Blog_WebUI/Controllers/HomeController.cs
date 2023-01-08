using Blog_BussinesLayer;
using Entities.ViewModels;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog_WebUI.Models;
using Blog_WebUI.Filter;

namespace Blog_WebUI.Controllers
{
    [HandleException]
    public class HomeController : Controller
    {
        // GET: Home
        //Test test = new Test();
        private NoteManager noteManager = new NoteManager();
        private CategoryManager categoryManager = new CategoryManager();
        private BlogUserManager blogUserManager = new BlogUserManager();
        public ActionResult Index()
        {            
            return View(noteManager.ListQueryable().Where(x=> x.IsDraft==false).OrderByDescending(x=> x.ModifiedDate).ToList());
        }

        public ActionResult MostLiked()
        {
            // En beğenilenler
            

            return View("Index", noteManager.ListQueryable().OrderByDescending(x=> x.LikeCount).ToList());
        }

        public ActionResult SelectCategory(int id)
        {
           
            Category category = categoryManager.Find(x=> x.Id == id);

            return View("Index", category.Notes.OrderByDescending(x=> x.ModifiedDate).ToList());
        }
        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            // Giriş kontrolü
            // Anasayfaya yönlendirme
            // Kullanıcı bilgilerini Session'a aktarma..
            if (ModelState.IsValid)
            {                
                BussinesLayerResult<BlogUser> blResult = blogUserManager.LoginUser(model);
                // Eğer Hata varsa blResult içinde Erros liste eklenmiş olacak bunun kontrolünü yapıyorum.
                if (blResult.Errors.Count >0)
                {   //Hata mesajlarını ModelState'e ekliyorum.. Hatalar ekranda görünücek.
                    blResult.Errors.ForEach(x=> ModelState.AddModelError("",x));
                    return View(model);
                }
                // Session'da kullanıcının bilgilerini saklıyorum..
                //Session["login"] = blResult.Result; 
                CurrentSession.Set<BlogUser>("login", blResult.Result);
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Logout()
        {
            //Session.Clear();
            CurrentSession.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                BussinesLayerResult<BlogUser> blResult = blogUserManager.RegisterUser(model);
                if (blResult.Errors.Count >0)
                {
                    // Kod buraya girdiyse bu durumda email ya da kullanıcı adı kullanılıyor demektir. bu hata mesajlarını da ekrana yazdırmam gerekiyor ve kullanıcıyı uyarmam gerekiyor.
                    blResult.Errors.ForEach(x=> ModelState.AddModelError("", x));
                    //AddModelError içindeki hata mesajlarını ekranda görebiliyorum ama BussinessLayerREsult içindeki ERRors Listten gelen hatamesajlarını göremiyorum.Yukarıdaki kod ile Error Listteki mesajları AddModelError içine eklemiş oluyorum.
                    return View(model);
                }

                return RedirectToAction("RegisterSuccess");
            }
            return View(model);
        }

        public ActionResult RegisterSuccess()
        {
            return View();
        }

        public ActionResult UserActivate(Guid id)
        {
            // Maile gelen Aktivasyon linkine tıklandığında çalışacak olan Action burasıdır.
            BussinesLayerResult<BlogUser> blResult = blogUserManager.UserActivate(id);
            if (blResult.Errors.Count>0)
            {
                TempData["errors"] = blResult.Errors;
                return RedirectToAction("ActivateUserCancel");
            }

            return RedirectToAction("ActivateUserOk");
        }
        public ActionResult ActivateUserOk()
        {
            return View();
        }

        public ActionResult ActivateUserCancel()
        {
            List<string> errors = null;

            if (TempData["errors"] != null)
            {
                errors = TempData["errors"] as List<string>;
            }

            return View(errors);
        }

        [Auth]
        public ActionResult ShowProfile()
        {
            //BlogUser currentUser = Session["login"] as BlogUser;
            BlogUser currentUser = CurrentSession.User;
            BussinesLayerResult<BlogUser> blResult = blogUserManager.GetUserById(currentUser.Id);
            if (blResult.Errors.Count>0)
            {
                return View("ProfileLoadError", blResult.Errors);
            }

            return View(blResult.Result);
        }

        [Auth]
        [HttpGet]
        public ActionResult EditProfile()
        {
            BlogUser currentUser = CurrentSession.User;
            BussinesLayerResult<BlogUser> blResult = blogUserManager.GetUserById(currentUser.Id);
            if (blResult.Errors.Count>0)
            {
                return View("ProfileLoadError", blResult.Errors);
            }
            return View(blResult.Result);
        }

        [Auth]
        [HttpPost]
        public ActionResult EditProfile(BlogUser user, HttpPostedFileBase ProfileImage)
        {
            // HttpPostedFileBase ile gönderilen dosyayı alabilmem için bu türde bir parametre tanımlamam/eklemem gerekiyor. Değişkenin ismi (ProfileImage), View tarafında input içerisinde name'e verdiğim değer ile aynı olmalı.
            // Göderilen dosyanın türünü kontrol etmem gerekiyor... jpg, jpeg, png türünde olup olmadığını kontrol etmeliyim. . Ve son olarak da Veritabanına hangi isim ile kaydedeceksem o ismi oluşturmalıyım ve Daha sonra server tarafında İmages klasörnün altına bu fotoğrafı bu isimle kaydetmeliyim.
            // Dosya türünün kontrolünü ContentType ile yapıyorum.

            ModelState.Remove("ModifiedUserName");
            if (ModelState.IsValid)
            {
                if (ProfileImage !=null && (
                    ProfileImage.ContentType =="image/jpg" ||
                    ProfileImage.ContentType == "image/jpeg" ||
                    ProfileImage.ContentType == "image/png"))
                {
                    string fileName = $"user_{user.Id}.{ProfileImage.ContentType.Split('/')[1]}";
                    // user_10.jpeg(.jpg - .png) gibi bir isim oluşuyor.
                    // Aşağıdaki kod ile birlikte fotoğrafı, server'daki images klasörünün altına oluşturduğum dosya ismi ile kopyalıyorum.
                    ProfileImage.SaveAs(Server.MapPath($"~/Images/{fileName}"));
                    // Son olarak da dosya adının veritabanında tutulamsı gerekiyor.
                    user.UserProfileImage = fileName;
                }
                // Artık View'den gelen değişiklikleri Veritabanına kaydetmek için geerekli kodları yazacağım.
                BussinesLayerResult<BlogUser> blResult = blogUserManager.UpdateProfile(user);
                if (blResult.Errors.Count>0)
                {
                    // hata oluştu demektir.
                    blResult.Errors.ForEach(x => ModelState.AddModelError("", x));
                    return View(blResult.Result);
                }

                // hata yok ise
                //Session["login"] = blResult.Result;
                CurrentSession.Set<BlogUser>("login", blResult.Result);
                return RedirectToAction("ShowProfile");
            }

            return View(user);
        }

        [Auth]
        public ActionResult DeleteProfile()
        {
            BlogUser currentUser = CurrentSession.User;
            BussinesLayerResult<BlogUser> blResult = blogUserManager.DeleteUser(currentUser.Id);
            if (blResult.Errors.Count>0)
            {
                blResult.Errors.ForEach(x => ModelState.AddModelError("", x));
                return View("ProfileLoadError", blResult.Errors);
            }
            Session.Clear();

            return RedirectToAction("Index");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HasError()
        {
            return View();
        }
    }
}

