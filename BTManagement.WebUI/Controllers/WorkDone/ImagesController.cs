using BTManagement.Core.Entities.WorkDone;
using BTManagement.Service.IRepository;
using BTManagement.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTManagement.WebUI.Controllers.WorkDone
{
    [Authorize]
    [Authorize(Policy = "AdminPolicy")]
    public class ImagesController : Controller
    {
        private readonly IRepository<Images> _repoImages;
        private readonly IRepository<WorksDone> _repoWorksDone;

        public ImagesController(IRepository<Images> repoImages, IRepository<WorksDone> repoWorksDone)
        {
            _repoImages = repoImages;
            _repoWorksDone = repoWorksDone;
        }

        [Route("resimler/{id?}")]
        public async Task<IActionResult> Index(int id)
        {
            var images = _repoImages.GetQueryable().Include(p => p.WorksDone).Where(p => p.WorkDoneId == id).ToList();
            var _workName = _repoWorksDone.GetById(id);
            ViewData["WorksDoneName"] = _workName.Work;
            return View(images);
        }

        public async Task<IActionResult> ViewImage(int id)
        {
            var image = await _repoImages.GetByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            return File(image.Image, image.FileType);
        }

        public async Task<IActionResult> Download(int? id)
        {
            var document = await _repoImages.GetQueryable()
                .Include(p => p.WorksDone)
                .Where(p => p.Id == id)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();

            if (document == null)
                return NotFound();

            var workDone = await _repoWorksDone.GetAsync(x => x.Id == document.WorkDoneId);

            // 🔹 Uzantıyı MIME tipinden al
            var extension = NormalizeExtension(document.FileType);

            // 🔹 Dosya adını güvenli hale getir (Türkçe karakterler korunur, boşluklar alt çizgiye çevrilir)
            var safeFileName = MakeFileNameSafe(workDone.Work) + extension;

            // 🔹 MIME tipi doğrudan document.FileType
            var mimeType = document.FileType; // "image/jpeg"

            return File(document.Image, mimeType, safeFileName);
        }

        private string NormalizeExtension(string fileType)
        {
            if (string.IsNullOrWhiteSpace(fileType))
                return ".bin";

            fileType = fileType.Trim().ToLower();

            // MIME tipiyse
            switch (fileType)
            {
                case "image/jpeg": return ".jpeg";
                case "image/jpg": return ".jpg";
                case "image/png": return ".png";
                case "image/gif": return ".gif";
                case "image/bmp": return ".bmp";
                case "image/tiff": return ".tiff";
                case "image/webp": return ".webp";
                default: return ".bin";
            }
        }

        private string MakeFileNameSafe(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Dosya";

            // Geçersiz dosya karakterlerini kaldır
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            // Boşlukları alt çizgiye çevir
            name = name.Replace(" ", "_");

            return name;
        }


    }
}
