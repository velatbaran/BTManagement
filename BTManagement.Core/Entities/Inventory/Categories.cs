using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTManagement.Core.Entities.Inventory
{
    public class Categories :CommonEntity
    {
        [DisplayName("Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        public IList<Products> Products { get; set; }

        public Categories()
        {
             Products = new List<Products>();
        }
    }
}
