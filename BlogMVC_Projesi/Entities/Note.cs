using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Table("Notes")]
    public class Note : BaseEntity
    {
        [Required, StringLength(50)]
        public string Title { get; set; }
        [Required, StringLength(2000)]
        public string Text { get; set; }
        public bool IsDraft { get; set; }
        public int LikeCount { get; set; }
        public int CategoryId { get; set; }

        //ilişkiler aşağıda tanımlanıyor
        public virtual Category Category { get; set; }

        public virtual BlogUser Owner { get; set; }

        public virtual List<Comment> Comments { get; set; }
        public virtual List<Liked> Likes { get; set; }

        public Note()
        {
            // Fake data oluştururken hata verecek bunun önüne geçmek için bu satırları ekliyorum.
            Comments = new List<Comment>();
            Likes = new List<Liked>();
        }

    }
}
