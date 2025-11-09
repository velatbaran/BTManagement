using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTManagement.Core.Entities.Inventory;

namespace BTManagement.Core.Entities.Guide
{
    public class MyGuide : CommonEntity
    {
        [DisplayName("Ad Soyad"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string NameSurname { get; set; }

        [DisplayName("Unvan"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Title { get; set; }

        public Departments? Department { get; set; }
        [DisplayName("Şube")]
        public int? DepartmentId { get; set; }

        [DisplayName("Dahili No"), StringLength(10), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string InternalNo { get; set; }

        [DisplayName("Cep No"), StringLength(20), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string CepNo { get; set; }

        [DisplayName("Silindi Mi?")]
        public bool? IsDeleted { get; set; } = false;
    }
}
