using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTManagement.Core.Entities.Inventory
{
    public class Products : CommonEntity
    {
        [DisplayName("Marka"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Brand { get; set; }

        [StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Model { get; set; }

        [DisplayName("Seri No"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string SerialNo { get; set; }

        [DisplayName("Ağ Adresi"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string NetworkAddress { get; set; }

        [DisplayName("Ağ Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string NetworkName { get; set; }

        [DisplayName("Donanım Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string HardwareName { get; set; }

        [DisplayName("Özellik"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Feature { get; set; }

        [DisplayName("Kullanıcı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Username { get; set; }

        [DisplayName("Durum")]
        public string? State { get; set; } = "Faal";

        [DisplayName("Açıklama")]
        public string? Description { get; set; }

        public Categories? Category { get; set; }
        [DisplayName("Kategori")]
        public int? CategoryId { get; set; }

        public Departments? Department { get; set; }
        [DisplayName("Şube")]
        public int? DepartmentId { get; set; }
    }
}
