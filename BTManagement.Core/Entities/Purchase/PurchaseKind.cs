using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTManagement.Core.Entities.Purchase
{
    public class PurchaseKind : CommonEntity
    {
        [DisplayName("Satınalma Türü"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        public IList<Purchases> Purchases { get; set; }

        public PurchaseKind()
        {
            Purchases = new List<Purchases>();
        }
    }
}
