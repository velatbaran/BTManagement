using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTManagement.Core.Entities.User
{
    public class Users : CommonEntity
    {
        [DisplayName("Ad Soyad"), StringLength(50), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string NameSurname { get; set; }

        [DisplayName("Kullanıcı Adı"), StringLength(50), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Username { get; set; }

        [DisplayName("Şifre"), StringLength(50), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Password { get; set; }

        [DisplayName("Admin Mi?")]
        public bool IsAdmin { get; set; } = true;

        [DisplayName("Aktif Mi?")]
        public bool IsActive { get; set; } = true;

        [DisplayName("Kullanıcı Guid"), ScaffoldColumn(false)]
        public Guid? UserGuid { get; set; } = Guid.NewGuid();
    }
}
