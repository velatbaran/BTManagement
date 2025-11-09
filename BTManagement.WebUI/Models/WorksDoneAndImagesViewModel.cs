using BTManagement.Core.Entities.Inventory;
using BTManagement.Core.Entities.WorkDone;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BTManagement.WebUI.Models
{
    public class WorksDoneAndImagesViewModel
    {
        public int? DepartmentId { get; set; }

        [DisplayName("Yapılan İş"), StringLength(150), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Work { get; set; }

        [DisplayName("Durum"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string State { get; set; }

        [DisplayName("Dosya"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public List<IFormFile> Files { get; set; }
    }
}
