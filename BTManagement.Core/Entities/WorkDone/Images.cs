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
    public class Images : CommonEntity
    {
        public WorksDone? WorksDone { get; set; }
        [DisplayName("Yapılan İş")]
        public int? WorkDoneId { get; set; }

        [DisplayName("Resim")]
        public byte[] Image { get; set; }

        [DisplayName("Dosya Tipi"), StringLength(10), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string? FileType { get; set; }
    }
}
