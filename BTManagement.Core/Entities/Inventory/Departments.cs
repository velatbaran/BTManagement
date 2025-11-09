using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTManagement.Core.Entities.Guide;
using BTManagement.Core.Entities.WorkDone;

namespace BTManagement.Core.Entities.Inventory
{
    public class Departments : CommonEntity
    {
        [DisplayName("Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        public IList<Products> Products { get; set; }
        public IList<MyGuide> MyGuide { get; set; }
        public IList<WorksDone> WorksDone { get; set; }

        public Departments()
        {
            Products = new List<Products>();
            MyGuide = new List<MyGuide>();
            WorksDone = new List<WorksDone>();
        }
    }
}
