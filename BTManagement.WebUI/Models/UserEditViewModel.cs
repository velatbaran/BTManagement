using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BTManagement.WebUI.Models
{
    public class UserEditViewModel
    {
        public int Id { get; set; }
        [DisplayName("Ad Soyad"), StringLength(50), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string NameSurname { get; set; }

        [DisplayName("Kullanıcı Adı"), StringLength(50), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Username { get; set; }

        [DisplayName("Şifre")]
        public string? Password { get; set; }

        [DisplayName("Admin Mi?")]
        public bool IsAdmin { get; set; } = true;

        [DisplayName("Aktif Mi?")]
        public bool IsActive { get; set; } = true;
    }
}
