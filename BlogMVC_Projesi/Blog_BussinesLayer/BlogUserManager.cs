using Blog_Common.Helpers;
using Blog_DataAccessLayer.EtityFrameworkSQL;
using Entities;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog_BussinesLayer
{
    public class BlogUserManager : BaseManager<BlogUser>
    {
        //Repository<BlogUser> repository = new Repository<BlogUser>();

        public BussinesLayerResult<BlogUser> DeleteUser(int id)
        {
            BussinesLayerResult<BlogUser> blResult = new BussinesLayerResult<BlogUser>();
            BlogUser user = Find(x => x.Id == id);
            if (user != null)
            {
                // User'a ait Note'lar silinmeli
                // User'a ait Like'ler silinmeli
                //User'a Comment'ler silinmeli
                NoteManager noteManager = new NoteManager();
                CommentManager commentManager = new CommentManager();
                LikedManager likedManager = new LikedManager();
                foreach (var note in user.Notes.ToList())
                {
                    foreach (var comment in note.Comments.ToList())
                    {
                        commentManager.Delete(comment);
                    }
                    foreach (var like in note.Likes.ToList())
                    {
                        likedManager.Delete(like);
                    }
                    noteManager.Delete(note);
                }

                //****************************
                if (Delete(user) ==0)
                {
                    blResult.Errors.Add("Kullanıcı silinemedi.");
                    return blResult;
                }
            }
            else
            {
                blResult.Errors.Add("Kullanıcı bulunamadı");
            }
            return blResult;
        }

        public BussinesLayerResult<BlogUser> GetUserById(int id)
        {
            BussinesLayerResult<BlogUser> blResult = new BussinesLayerResult<BlogUser>();
            BlogUser user = Find(x=> x.Id == id);
            if (user == null)
            {
                blResult.Errors.Add("Kullanıcı bulunamadı");
            }
            else
            {
                blResult.Result = user;
            }

            return blResult;
        }

        public BussinesLayerResult<BlogUser> LoginUser(LoginViewModel model)
        {
            BussinesLayerResult<BlogUser> blResult = new BussinesLayerResult<BlogUser>();
            blResult.Result = Find(x=> x.Username ==model.UserName && x.Password ==model.Password);

            if (blResult.Result != null)
            {
                // Kullanıcı sistemde kayıtlı ise geriye bu kullanıcı bilgileri gönderilecek.

                if (!blResult.Result.IsActive)
                {
                    // Kullanıcı kayıtlı ise aktif olup olmadığı kontrol edilir. Aktif değilse Error listesine aşağıdaki mesaj eklenir.
                    blResult.Errors.Add("Hesabınız aktif değil. Lütfen e-postanızı kontrol ediniz.");
                }
                
            }
            else
            {
                blResult.Errors.Add("Kullanıcı adı ya da şifreniz hatalı veya kayıtlı kullanıcı değilsiniz.");
            }

            return blResult;
        }
        

        public BussinesLayerResult<BlogUser> RegisterUser(RegisterViewModel model)
        {
            BlogUser user = Find(x=> x.Username==model.UserName || x.Email== model.Email);
            BussinesLayerResult<BlogUser> layerResult = new BussinesLayerResult<BlogUser>();

            if (user != null)
            {
                // kullanıcı adı ve email: bunlardan hangisi sistemde var
                if (user.Username == model.UserName)
                {
                    layerResult.Errors.Add("Kullanıcı adı sistemde kayıtlı");
                }
                if (user.Email == model.Email)
                {
                    layerResult.Errors.Add("Girdiğiniz E-posta sitemde kayıtlı");
                }

            }
            else
            {
                // Veritabanına bu kullanıcı kaydedilecek..
                int result = base.Insert(new BlogUser { 
                Name = model.Name,
                Surname = model.Surname,
                UserProfileImage = "user-profile.jpg",
                Username = model.UserName,
                Email = model.Email,
                Password = model.Password,
                IsActive= false,
                IsAdmin = false,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ModifiedUserName=model.UserName,
                ActivateGuid = Guid.NewGuid()
                
                });
                if (result>0)
                {
                   layerResult.Result= Find(x=> x.Username==model.UserName && x.Email==model.Email);

                    // Kullanıcıya aktivasyon maili göndermek için gerekli kodları buraya yazacağım..

                    // Web.Config dosyasına sitemizin root'unu eklemiştik. Bu bilgiyi alıyoruz.
                    string siteUrl = ConfigHelper.Get<string>("SiteRootUrl");
                    // Gönderilen Maildeki active linkini oluşturmak için activateUrl değişkenini tanımladık. root/controller/action/guidParametresi ile link oluşturuldu.
                    string activateUrl = $"{siteUrl}/Home/UserActivate/{layerResult.Result.ActivateGuid}";
                    // Mail'de göndereceğimiz Mesajın içeriğini aşağıda oluşturdum. html tagleri kullandım. MailHelper Classında isHtml=true olduğu için html taglerini kullanabiliyorum.
                    string messageBody = $"Merhaba, Hesabınızı aktifleştirmek için <a href='{activateUrl}' target='_blank'> tıklayınız</a>";
                    // Mail için konu.
                    string subject = "NA-203 Blog Hesap Aktifleştirme";
                    // MailHelper'ın SendMail Metoduna yukarıdaki parametreleri veriyorum. Gönderilecek kişinin mailini yukarıda sorguladığım layerResult.Result içinden alıyorum.
                    MailHelper.SendMail(messageBody, layerResult.Result.Email, subject);

                }
                

            }
            return layerResult;
        }

        public BussinesLayerResult<BlogUser> UpdateProfile(BlogUser userData)
        {
            BussinesLayerResult<BlogUser> blResult = new BussinesLayerResult<BlogUser>();
            BlogUser userDb = Find(x => x.Id != userData.Id && (x.Email == userData.Email || x.Username == userData.Username));
            if (userDb !=null && userDb.Id != userData.Id)
            {
                if (userDb.Username == userData.Username)
                {
                    blResult.Errors.Add("Girdiğiniz kullanıcı adı başka bir üyemiz tarafından kullanılmaktadır. Lütfen farklı bir kullanıcı adı giriniz.");
                }
                if (userDb.Email == userData.Email)
                {
                    blResult.Errors.Add("Girdiğiniz E-posta adresi sistemde kayıtlıdır. Lütfen farklı bir E-posta giriniz.");
                }
                return blResult;
            }
            // Eğer herhangi bir hata yoksa o zaman if bloğuna girmeyecek ve buradan devam edecek. Bu satırdan sonra Update işlemlerini yapmam gerekecek.
            blResult.Result = Find(x=> x.Id == userData.Id);
            blResult.Result.Name = userData.Name;
            blResult.Result.Surname = userData.Surname;
            blResult.Result.Email = userData.Email;
            blResult.Result.Username = userData.Username;
            blResult.Result.Password = userData.Password;

            // Fotoğraf geldiyse bunun kontrolünü yapıyorum
            if (string.IsNullOrEmpty(userData.UserProfileImage) == false)
            {
                blResult.Result.UserProfileImage = userData.UserProfileImage;
            }

            if (base.Update(blResult.Result) == 0)
            {
                blResult.Errors.Add("Profil güncellenemedi.");
            }
            return blResult;
        }

        public BussinesLayerResult<BlogUser> UserActivate(Guid id)
        {
            BussinesLayerResult<BlogUser> blResult = new BussinesLayerResult<BlogUser>();
            blResult.Result = Find(x=> x.ActivateGuid==id);

            if (blResult.Result!=null)
            {
                if (blResult.Result.IsActive)
                {
                    blResult.Errors.Add("Kullanıcı zaten aktif edilmiştir.");
                }
                else
                {
                    blResult.Result.IsActive = true;
                    Update(blResult.Result);
                }
            }
            else
            {
                blResult.Errors.Add("Aktifleştirilecek kullanıcı bulunamadı.");
            }
            return blResult;
        }

        //Method Hiding
        // Miras olarak gelen bir metodu ezmek istiyorsam ve geridönüş tipini değiştirmek istiyorsam
        // (BaseManager içindeki Insert isimli metodu ezmek istiyoruz.. BaseManager içindeki Insert metodunun geri dönüş tipi int türünde. Fakat ben farklı bir türün geriye dönmesini istiyorsam (örneğin string ya da BussinesLayerResult<BlogUser>), bu durumda aşağıdaki gibi new keyword'ünü kullanarak Ezmek istediğim metodun geri dönüş tipini değiştirebilirim ve artık Insert metodu kullanılmak istendiğinde buradaki metot kullanılacak.
        //  Yukarıda RegisterUser metodunda BaseManager'daki Insert metodunu kullanmak istediğimizden orada Insert metodunun önüne base'i ekledik: base.Insert (BaseManager'daki insert metodu.))

        public new BussinesLayerResult<BlogUser> Insert(BlogUser data)
        {
            BlogUser user = Find(x=> x.Username== data.Username || x.Email == data.Email);
            BussinesLayerResult<BlogUser> layerResult = new BussinesLayerResult<BlogUser>();
            layerResult.Result = data;
            if (user != null)
            {
                // bu durumda bir hata olmalı.. Yani email ve kullanıcı adı başka bir kullanıcı tarafından kullanılıyor.. Kaydetme işlemi yapılmamalı ve geriye de kaydı giren kişiyue uyarı mesajları gönderilmeli..

                if (user.Email == data.Email)
                {
                    layerResult.Errors.Add("E-posta adresi kayıtlı.");
                }
                if (user.Username == data.Username)
                {
                    layerResult.Errors.Add("Kullanıcı adı kayıtlı.");
                }
            }
            else
            {
                // username ve email ile eşleşen kayıt yok ise veriyi ekleme işlemini yapmamız gerekiyor.
                layerResult.Result.UserProfileImage = "user-profile.jgp";
                layerResult.Result.ActivateGuid = Guid.NewGuid();

                if (base.Insert(layerResult.Result) == 0)
                {
                    layerResult.Errors.Add("Yeni üye kaydedilirken bir hata oluştu.");
                }
            }
            return layerResult;
        }

        public new BussinesLayerResult<BlogUser> Update(BlogUser data)
        {
            BussinesLayerResult<BlogUser> layerResult = new BussinesLayerResult<BlogUser>();

            BlogUser dbUser = Find(x=>x.Id != data.Id && (x.Email == data.Email || x.Username == data.Username));
            layerResult.Result = data;
            if (dbUser !=null && dbUser.Id != data.Id)
            {
                if (dbUser.Username == data.Username)
                {
                    layerResult.Errors.Add("Girdiğiniz kullanıcı adı başka bir üyemiz tarafından kullanılıyor. Lütfen farklı bir kullanıcı adı girin.");
                }

                if (dbUser.Email == data.Email)
                {
                    layerResult.Errors.Add("Girdiğiniz E-Posta başka bir üyemiz tarafından kullanılıyor. Lütfen farklı bir E-posta girin.");
                }
                return layerResult;
            }

            // Eğer hata yoksa update işlemi ile ilgili işlemleri yapmalıyız.
            layerResult.Result = Find(x=>x.Id == data.Id);
            layerResult.Result.Email = data.Email;
            layerResult.Result.Name = data.Name; 
            layerResult.Result.Surname = data.Surname; 
            layerResult.Result.Password = data.Password; 
            layerResult.Result.Username = data.Username; 
            layerResult.Result.IsActive = data.IsActive;
            layerResult.Result.IsAdmin = data.IsAdmin;

            if(base.Update(layerResult.Result) ==0)
            {
                layerResult.Errors.Add("Profil güncellenirken bir hata oluştu.");
            }

            return layerResult;
        }
    }
}
