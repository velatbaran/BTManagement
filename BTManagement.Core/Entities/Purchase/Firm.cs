using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTManagement.Core.Entities.Purchase
{
    public class Firm : CommonEntity
    {
        [DisplayName("Adı"), StringLength(150), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        [DisplayName("Yetkili"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Authorized { get; set; }

        public FirmKind? FirmKinds { get; set; }
        [DisplayName("Firma Türü")]
        public int? FirmKindId { get; set; }

        [DisplayName("Sicil No"), StringLength(25)]
        public string? RecordNo { get; set; }

        [DisplayName("Vergi Dairesi"), StringLength(100)]
        public string? TaxOffice { get; set; }

        [DisplayName("Vergi No"), StringLength(25)]
        public string? TaxNo { get; set; }

        [DisplayName("TC No"), StringLength(25)]
        public string? TcNo { get; set; }

        [DisplayName("Telefon"), StringLength(25), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Phone { get; set; }

        [DisplayName("Adres"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Address { get; set; }

        [StringLength(50),Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Eposta { get; set; }

        [DisplayName("Web Adres"), StringLength(50)]
        public string? WebAddress { get; set; }

        [DisplayName("Banka Adı"), StringLength(100)]
        public string? BankName { get; set; }

        [DisplayName("İban No"), StringLength(50)]
        public string? IbanNo { get; set; }

        public IList<Purchases> Purchases { get; set; }

        public Firm()
        {
            Purchases = new List<Purchases>();
        }
    }
}
