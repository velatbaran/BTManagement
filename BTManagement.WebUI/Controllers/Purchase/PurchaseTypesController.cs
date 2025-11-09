using BTManagement.Core.Entities.Inventory;
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
    public class PurchaseTypesController : Controller
    {
        private readonly IRepository<PurchaseType> _repoPurchaseType;
        private readonly IToastNotification _toastNotification;

        public PurchaseTypesController(IRepository<PurchaseType> repoPurchaseType, IToastNotification toastNotification)
        {
            _repoPurchaseType = repoPurchaseType;
            _toastNotification = toastNotification;
        }

        [Route("satinalimtipleri")]
        public async Task<IActionResult> Index()
        {
            return View(await _repoPurchaseType.GetQueryable().OrderByDescending(x => x.CreatedDate).ToListAsync());
        }

        [Route("satinalimtipiekle")]
        public IActionResult Create()
        {
            return View();
        }

        [Route("satinalimtipiekle")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseType purchaseType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _purchaseType = _repoPurchaseType.GetQueryable().Where(x => x.Name == purchaseType.Name).FirstOrDefault();
                    if (_purchaseType != null)
                    {
                        _toastNotification.AddWarningToastMessage("Aynı satın alma tipi sistemde kayıtlı. Lütfen başka bir satın alma tipi bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        return View(purchaseType);
                    }

                    _repoPurchaseType.Add(purchaseType);
                    await _repoPurchaseType.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _toastNotification.AddErrorToastMessage(ex.Message, new ToastrOptions { Title = "Hata" });
                }

            }
            return View(purchaseType);
        }

        [Route("satinalimtipiguncelle/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseType = await _repoPurchaseType.GetByIdAsync(id.Value);
            if (purchaseType == null)
            {
                return NotFound();
            }
            return View(purchaseType);
        }

        [Route("satinalimtipiguncelle/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseType purchaseType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _purchaseType = await _repoPurchaseType.GetAsync(x => x.Id == id);
                    if (await _repoPurchaseType.AnyAsync(x => x.Name == purchaseType.Name && x.Id != id))
                    {
                        _toastNotification.AddWarningToastMessage("Aynı satın alma tipi sistemde kayıtlı. Lütfen başka bir satın alma tipi bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        return View(purchaseType);
                    }

                    _purchaseType.Name = purchaseType.Name;

                    _repoPurchaseType.Update(_purchaseType);
                    await _repoPurchaseType.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!PurchaseTypeExists(purchaseType.Id))
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
            return View(purchaseType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var purchaseType = await _repoPurchaseType.GetByIdAsync(id);
            if (purchaseType != null)
            {
                _repoPurchaseType.Delete(purchaseType);
            }

            await _repoPurchaseType.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseTypeExists(int id)
        {
            return _repoPurchaseType.GetQueryable().Any(e => e.Id == id);
        }
    }
}
