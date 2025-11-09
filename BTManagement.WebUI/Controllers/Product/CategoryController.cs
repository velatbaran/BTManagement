using BTManagement.Core.Entities.Inventory;
using BTManagement.Service.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace BTManagement.WebUI.Controllers.Product
{
    [Authorize]
    [Authorize(Policy = "AdminPolicy")]
    public class CategoryController : Controller
    {
        private readonly IRepository<Categories> _repoCategories;
        private readonly IToastNotification _toastNotification;

        public CategoryController(IRepository<Categories> repoCategories, IToastNotification toastNotification)
        {
            _repoCategories = repoCategories;
            _toastNotification = toastNotification;
        }

        [Route("kategoriler")]
        public async Task<IActionResult> Index()
        {
            return View(await _repoCategories.GetQueryable().OrderByDescending(x => x.CreatedDate).ToListAsync());
        }

        [Route("kategoriekle")]
        public IActionResult Create()
        {
            return View();
        }

        [Route("kategoriekle")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categories category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _category = _repoCategories.GetQueryable().Where(x => x.Name == category.Name).FirstOrDefault();
                    if (_category != null)
                    {
                        _toastNotification.AddWarningToastMessage("Aynı kategori adı sistemde kayıtlı. Lütfen başka bir kategori adı bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        return View(category);
                    }

                    _repoCategories.Add(category);
                    await _repoCategories.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _toastNotification.AddErrorToastMessage(ex.Message, new ToastrOptions { Title = "Hata" });
                }

            }
            return View(category);
        }

        [Route("kategoriguncelle/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _repoCategories.GetByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [Route("kategoriguncelle/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categories category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _category = await _repoCategories.GetAsync(x => x.Id == id);
                    if (await _repoCategories.AnyAsync(x => x.Name == category.Name && x.Id != id))
                    {
                        _toastNotification.AddWarningToastMessage("Aynı kategori adı sistemde kayıtlı. Lütfen başka bir kategori adı bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        return View(category);
                    }

                    _category.Name = category.Name;

                    _repoCategories.Update(_category);
                    await _repoCategories.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage(ex.Message, new ToastrOptions { Title = "Hata" });
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _repoCategories.GetByIdAsync(id);
            if (category != null)
            {
                _repoCategories.Delete(category);
            }

            await _repoCategories.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _repoCategories.GetQueryable().Any(e => e.Id == id);
        }
    }
}
