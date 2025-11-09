using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTManagement.Core.Entities.Purchase
{
    public class PurchaseForm : CommonEntity
    {
        [DisplayName("Satınalma Şekli"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        public IList<Purchases> Purchases { get; set; }

        public PurchaseForm()
        {
            Purchases = new List<Purchases>();
        }
    }
}
