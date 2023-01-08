using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Table("BlogUsers")]
    public class BlogUser : BaseEntity
    {
        [DisplayName("Adı"), StringLength(25)]
        public string Name { get; set; }
        [DisplayName("Soyadı"), StringLength(25)]
        public string Surname { get; set; }
        [StringLength(50)]

        [DisplayName("Profil Fotoğrafı"), ScaffoldColumn(false)]
        public string UserProfileImage { get; set; }
        [Required, DisplayName("E-Posta"), StringLength(50)]
        public string Email { get; set; }
        [Required, DisplayName("Kullanıcı Adı"), StringLength(25)]
        public  string Username { get; set; }
        [Required]
        [StringLength(100), DisplayName("Şifre"),]
        public string Password { get; set; }
        [DisplayName("Aktif mi")]
        public bool IsActive { get; set; }
        [DisplayName("Admin mi?")]
        public bool IsAdmin { get; set; }
        [ScaffoldColumn(false)]
        public Guid ActivateGuid { get; set; }


        // ilişkiler
        public virtual List<Note> Notes { get; set; } // Bir kullanıcının Birden fazla notu olabilir
        public virtual List<Comment> Comments{ get; set; } // Bir kullanıcının Birden fazla yorumu olabilir
        public virtual List<Liked> Likes { get; set; } // Bir kullanıcının Birden fazla beğenisi olabilir
    }
}
