using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTManagement.Core.Entities.Purchase
{
    public class Purchases : CommonEntity
    {
        [DisplayName("Dosya No"), StringLength(15), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string FileNo { get; set; }

        [DisplayName("İşin Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string WorkingName { get; set; }

        public Firm? Firms { get; set; }
        [DisplayName("Firma Adı")]
        public int? FirmId { get; set; }

        public PurchaseForm? PurchaseForms { get; set; }
        [DisplayName("Satınalma Şekli")]
        public int? PurchaseFormId { get; set; }

        public PurchaseKind? PurchaseKinds { get; set; }
        [DisplayName("Satınalma Türü")]
        public int? PurchaseKindId { get; set; }

        public PurchaseType? PurchaseTypes { get; set; }
        [DisplayName("Satınalma Tipi")]
        public int? PurchaseTypeId { get; set; }

        [DisplayName("Açıklama")]
        public string Description { get; set; }

        [DisplayName("Dosya Yolu"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string FilePath { get; set; }

        [DisplayName("Satınalma Tarihi"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public DateTime PurchaseDate { get; set; }

        [DisplayName("Fatura Tutarı"), Column(TypeName = "decimal(18,4)"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public decimal Price { get; set; }
    }
}
