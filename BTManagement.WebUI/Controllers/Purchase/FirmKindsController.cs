using BTManagement.Core.Entities.Purchase;
using BTManagement.Service.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace BTManagement.WebUI.Controllers.Purchase
{
    [Authorize]
    [Authorize(Policy = "AdminPolicy")]
    public class FirmKindsController : Controller
    {
        private readonly IRepository<FirmKind> _repoFirmKind;
        private readonly IToastNotification _toastNotification;

        public FirmKindsController(IRepository<FirmKind> repoFirmKind, IToastNotification toastNotification)
        {
            _repoFirmKind = repoFirmKind;
            _toastNotification = toastNotification;
        }

        [Route("firmaturleri")]
        public async Task<IActionResult> Index()
        {
            return View(await _repoFirmKind.GetQueryable().OrderByDescending(x => x.CreatedDate).ToListAsync());
        }

        [Route("firmaturuekle")]
        public IActionResult Create()
        {
            return View();
        }

        [Route("firmaturuekle")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FirmKind firmKind)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _firmKind = _repoFirmKind.GetQueryable().Where(x => x.Name == firmKind.Name).FirstOrDefault();
                    if (_firmKind != null)
                    {
                        _toastNotification.AddWarningToastMessage("Aynı firma türü sistemde kayıtlı. Lütfen başka bir firma türü bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        return View(firmKind);
                    }

                    _repoFirmKind.Add(firmKind);
                    await _repoFirmKind.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _toastNotification.AddErrorToastMessage(ex.Message, new ToastrOptions { Title = "Hata" });
                }

            }
            return View(firmKind);
        }

        [Route("firmaturuguncelle/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var firmKind = await _repoFirmKind.GetByIdAsync(id.Value);
            if (firmKind == null)
            {
                return NotFound();
            }
            return View(firmKind);
        }

        [Route("firmaturuguncelle/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FirmKind firmKind)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _firmKind = await _repoFirmKind.GetAsync(x => x.Id == id);
                    if (await _repoFirmKind.AnyAsync(x => x.Name == firmKind.Name && x.Id != id))
                    {
                        _toastNotification.AddWarningToastMessage("Aynı firma türü sistemde kayıtlı. Lütfen başka bir firma türü bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        return View(firmKind);
                    }

                    _firmKind.Name = firmKind.Name;

                    _repoFirmKind.Update(_firmKind);
                    await _repoFirmKind.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!FirmKindExists(firmKind.Id))
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
            return View(firmKind);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var firmKind = await _repoFirmKind.GetByIdAsync(id);
            if (firmKind != null)
            {
                _repoFirmKind.Delete(firmKind);
            }

            await _repoFirmKind.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }

        private bool FirmKindExists(int id)
        {
            return _repoFirmKind.GetQueryable().Any(e => e.Id == id);
        }
    }
}
