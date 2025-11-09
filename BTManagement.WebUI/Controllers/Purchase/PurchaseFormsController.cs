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
    public class PurchaseFormsController : Controller
    {
        private readonly IRepository<PurchaseForm> _repoPurchaseForm;
        private readonly IToastNotification _toastNotification;

        public PurchaseFormsController(IRepository<PurchaseForm> repoPurchaseForm, IToastNotification toastNotification)
        {
            _repoPurchaseForm = repoPurchaseForm;
            _toastNotification = toastNotification;
        }

        [Route("satinalimsekilleri")]
        public async Task<IActionResult> Index()
        {
            return View(await _repoPurchaseForm.GetQueryable().OrderByDescending(x => x.CreatedDate).ToListAsync());
        }

        [Route("satinalimsekliekle")]
        public IActionResult Create()
        {
            return View();
        }

        [Route("satinalimsekliekle")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseForm purchaseForm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _purchaseForm = _repoPurchaseForm.GetQueryable().Where(x => x.Name == purchaseForm.Name).FirstOrDefault();
                    if (_purchaseForm != null)
                    {
                        _toastNotification.AddWarningToastMessage("Aynı satın alma şekli sistemde kayıtlı. Lütfen başka bir satın alma şekli bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        return View(purchaseForm);
                    }

                    _repoPurchaseForm.Add(purchaseForm);
                    await _repoPurchaseForm.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _toastNotification.AddErrorToastMessage(ex.Message, new ToastrOptions { Title = "Hata" });
                }

            }
            return View(purchaseForm);
        }

        [Route("satinalimsekliguncelle/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseForm = await _repoPurchaseForm.GetByIdAsync(id.Value);
            if (purchaseForm == null)
            {
                return NotFound();
            }
            return View(purchaseForm);
        }

        [Route("satinalimsekliguncelle/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseForm purchaseForm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _purchaseForm = await _repoPurchaseForm.GetAsync(x => x.Id == id);
                    if (await _repoPurchaseForm.AnyAsync(x => x.Name == purchaseForm.Name && x.Id != id))
                    {
                        _toastNotification.AddWarningToastMessage("Aynı satın alma şekli sistemde kayıtlı. Lütfen başka bir satın alma şekli bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        return View(purchaseForm);
                    }

                    _purchaseForm.Name = purchaseForm.Name;

                    _repoPurchaseForm.Update(_purchaseForm);
                    await _repoPurchaseForm.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!PurchaseFormExists(purchaseForm.Id))
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
            return View(purchaseForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var purchaseForm = await _repoPurchaseForm.GetByIdAsync(id);
            if (purchaseForm != null)
            {
                _repoPurchaseForm.Delete(purchaseForm);
            }

            await _repoPurchaseForm.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseFormExists(int id)
        {
            return _repoPurchaseForm.GetQueryable().Any(e => e.Id == id);
        }
    }
}
