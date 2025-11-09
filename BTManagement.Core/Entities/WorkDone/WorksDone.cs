using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTManagement.Core.Entities.Inventory;

namespace BTManagement.Core.Entities.WorkDone
{
    public class WorksDone : CommonEntity
    {
        public Departments? Department { get; set; }
        [DisplayName("Şube")]
        public int? DepartmentId { get; set; }

        [DisplayName("Yapılan İş"), StringLength(150), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Work { get; set; }

        [DisplayName("Durum"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string State { get; set; }

        public IList<Images> Images { get; set; }

        public WorksDone()
        {
            Images= new List<Images>();
        }
    }
}
