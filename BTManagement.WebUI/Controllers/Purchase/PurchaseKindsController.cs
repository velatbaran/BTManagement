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
    public class PurchaseKindsController : Controller
    {
        private readonly IRepository<PurchaseKind> _repoPurchaseKind;
        private readonly IToastNotification _toastNotification;

        public PurchaseKindsController(IRepository<PurchaseKind> repoPurchaseKind, IToastNotification toastNotification)
        {
            _repoPurchaseKind = repoPurchaseKind;
            _toastNotification = toastNotification;
        }

        [Route("satinalimturleri")]
        public async Task<IActionResult> Index()
        {
            return View(await _repoPurchaseKind.GetQueryable().OrderByDescending(x => x.CreatedDate).ToListAsync());
        }

        [Route("satinalimturuekle")]
        public IActionResult Create()
        {
            return View();
        }

        [Route("satinalimturuekle")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseKind purchaseKind)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _purchaseKind = _repoPurchaseKind.GetQueryable().Where(x => x.Name == purchaseKind.Name).FirstOrDefault();
                    if (_purchaseKind != null)
                    {
                        _toastNotification.AddWarningToastMessage("Aynı satın alma türü sistemde kayıtlı. Lütfen başka bir satın alma türü bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        return View(purchaseKind);
                    }

                    _repoPurchaseKind.Add(purchaseKind);
                    await _repoPurchaseKind.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _toastNotification.AddErrorToastMessage(ex.Message, new ToastrOptions { Title = "Hata" });
                }

            }
            return View(purchaseKind);
        }

        [Route("satinalimturuguncelle/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseKind = await _repoPurchaseKind.GetByIdAsync(id.Value);
            if (purchaseKind == null)
            {
                return NotFound();
            }
            return View(purchaseKind);
        }

        [Route("satinalimturuguncelle/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseKind purchaseKind)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _purchaseKind = await _repoPurchaseKind.GetAsync(x => x.Id == id);
                    if (await _repoPurchaseKind.AnyAsync(x => x.Name == purchaseKind.Name && x.Id != id))
                    {
                        _toastNotification.AddWarningToastMessage("Aynı satın alma türü sistemde kayıtlı. Lütfen başka bir satın alma türü bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        return View(purchaseKind);
                    }

                    _purchaseKind.Name = purchaseKind.Name;

                    _repoPurchaseKind.Update(_purchaseKind);
                    await _repoPurchaseKind.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!PurchaseKindExists(purchaseKind.Id))
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
            return View(purchaseKind);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var purchaseKind = await _repoPurchaseKind.GetByIdAsync(id);
            if (purchaseKind != null)
            {
                _repoPurchaseKind.Delete(purchaseKind);
            }

            await _repoPurchaseKind.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseKindExists(int id)
        {
            return _repoPurchaseKind.GetQueryable().Any(e => e.Id == id);
        }
    }
}
