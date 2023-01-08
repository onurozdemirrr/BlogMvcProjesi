using Blog_DataAccessLayer.EtityFrameworkSQL;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog_BussinesLayer
{
    public class CategoryManager : BaseManager<Category>
    {
        //private Repository<Category> repository = new Repository<Category>();

        //public List<Category> GetCategories()
        //{
        //    return repository.List();
        //}

        //public Category GetCategoryByID(int id)
        //{
        //    return repository.Find(x=> x.Id==id);
        //}

        // Delete metodunu normalde BaseManager sınıfındaki metodu kullanıyor. biz BaseManager sınıfını abstract olarak işaretledik ve içindeki metotları da virtual olarak işaretledik.. Bu sınıf(CategoryManager) BaseManager Sınıfını miras aldığı için ve yukarıda saydığımız özelliklerden dolayı, orada tanımalnan metotları burada ezebiliriz. 
        public override int Delete(Category category)
        {
            // Bir kategori silinebilmesi için ilişkili olan kayıtların da silinmesi gerekiyor. (Note, Comment, Liked)
            // 
            NoteManager noteManager = new NoteManager();
            CommentManager commentManager = new CommentManager();
            LikedManager likedManager = new LikedManager();
            foreach (var note in category.Notes.ToList())
            {
                // Bu note'a ait comment'leri de silmem gerekli

                foreach (var comment in note.Comments.ToList())
                {
                    commentManager.Delete(comment);
                }

                // Bu Note'a ait Like'ları da silmem gerekli
                foreach (var like in note.Likes.ToList())
                {
                    likedManager.Delete(like);
                }
                noteManager.Delete(note);
            }
            return base.Delete(category);    // Bu satır, bu metodun içindeki kodların yanında BaseManager içindeki Delete metodunun da çalışacağı anlamına gelir.
        }
    }
}
