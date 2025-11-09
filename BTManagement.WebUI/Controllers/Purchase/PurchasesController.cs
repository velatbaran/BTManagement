using BTManagement.Core.Entities.Inventory;
using BTManagement.Core.Entities.Purchase;
using BTManagement.Service.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace BTManagement.WebUI.Controllers.Purchase
{
    [Authorize]
    [Authorize(Policy = "AdminPolicy")]
    public class PurchasesController : Controller
    {
        private readonly IRepository<Purchases> _repoPurchases;
        private readonly IRepository<Firm> _repoFirms;
        private readonly IRepository<PurchaseForm> _repoPurchaseForm;
        private readonly IRepository<PurchaseKind> _repoPurchaseKind;
        private readonly IRepository<PurchaseType> _repoPurchaseType;
        private readonly IToastNotification _toastNotification;

        public PurchasesController(IRepository<Purchases> repoPurchases, IRepository<Firm> repoFirms, IRepository<PurchaseForm> repoPurchaseForm, IRepository<PurchaseKind> repoPurchaseKind, IRepository<PurchaseType> repoPurchaseType, IToastNotification toastNotification)
        {
            _repoPurchases = repoPurchases;
            _repoFirms = repoFirms;
            _repoPurchaseForm = repoPurchaseForm;
            _repoPurchaseKind = repoPurchaseKind;
            _repoPurchaseType = repoPurchaseType;
            _toastNotification = toastNotification;
        }

        [Route("satinalimlarim")]
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalAll = _repoPurchases.GetAll().Sum(x => x.Price); // tüm kayıtların toplamı
            return View(await _repoPurchases.GetQueryable().Include(p => p.Firms).Include(p => p.PurchaseKinds).Include(p => p.PurchaseTypes).Include(p => p.PurchaseForms).OrderByDescending(x => x.CreatedDate).ToListAsync());
        }

        [Route("satinalimiekle")]
        public IActionResult Create()
        {
            ViewData["FirmId"] = new SelectList(_repoFirms.GetAll(), "Id", "Name");
            ViewData["PurchaseFormId"] = new SelectList(_repoPurchaseForm.GetAll(), "Id", "Name");
            ViewData["PurchaseKindId"] = new SelectList(_repoPurchaseKind.GetAll(), "Id", "Name");
            ViewData["PurchaseTypeId"] = new SelectList(_repoPurchaseType.GetAll(), "Id", "Name");

            return View();
        }

        [Route("satinalimiekle")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Purchases purchase)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = _repoPurchases.GetQueryable().Where(x => x.FileNo == purchase.FileNo).FirstOrDefault();
                    if (product != null)
                    {
                        _toastNotification.AddWarningToastMessage("Aynı dosya no lu satın alımı sistemde kayıtlı. Lütfen başka bir dosya no bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        ViewData["FirmId"] = new SelectList(_repoFirms.GetAll(), "Id", "Name",purchase.FirmId);
                        ViewData["PurchaseFormId"] = new SelectList(_repoPurchaseForm.GetAll(), "Id", "Name",purchase.PurchaseFormId);
                        ViewData["PurchaseKindId"] = new SelectList(_repoPurchaseKind.GetAll(), "Id", "Name",purchase.PurchaseKindId);
                        ViewData["PurchaseTypeId"] = new SelectList(_repoPurchaseType.GetAll(), "Id", "Name",purchase.PurchaseTypeId);
                        return View(purchase);
                    }

                    _repoPurchases.Add(purchase);
                    await _repoPurchases.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _toastNotification.AddErrorToastMessage(ex.Message, new ToastrOptions { Title = "Hata" });
                }

            }
            ViewData["FirmId"] = new SelectList(_repoFirms.GetAll(), "Id", "Name", purchase.FirmId);
            ViewData["PurchaseFormId"] = new SelectList(_repoPurchaseForm.GetAll(), "Id", "Name", purchase.PurchaseFormId);
            ViewData["PurchaseKindId"] = new SelectList(_repoPurchaseKind.GetAll(), "Id", "Name", purchase.PurchaseKindId);
            ViewData["PurchaseTypeId"] = new SelectList(_repoPurchaseType.GetAll(), "Id", "Name", purchase.PurchaseTypeId);
            return View(purchase);
        }

        [Route("satinalimiguncelle/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _repoPurchases.GetByIdAsync(id.Value);
            if (purchase == null)
            {
                return NotFound();
            }
            ViewData["FirmId"] = new SelectList(_repoFirms.GetAll(), "Id", "Name", purchase.FirmId);
            ViewData["PurchaseFormId"] = new SelectList(_repoPurchaseForm.GetAll(), "Id", "Name", purchase.PurchaseFormId);
            ViewData["PurchaseKindId"] = new SelectList(_repoPurchaseKind.GetAll(), "Id", "Name", purchase.PurchaseKindId);
            ViewData["PurchaseTypeId"] = new SelectList(_repoPurchaseType.GetAll(), "Id", "Name", purchase.PurchaseTypeId);
            return View(purchase);
        }

        [Route("satinalimiguncelle/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Purchases purchase)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _purchase = await _repoPurchases.GetAsync(x => x.Id == id);
                    if (await _repoPurchases.AnyAsync(x => x.FileNo == purchase.FileNo && x.Id != id))
                    {
                        _toastNotification.AddWarningToastMessage("Aynı dosya no lu satın alımı sistemde kayıtlı. Lütfen başka bir dosya no bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        ViewData["FirmId"] = new SelectList(_repoFirms.GetAll(), "Id", "Name", purchase.FirmId);
                        ViewData["PurchaseFormId"] = new SelectList(_repoPurchaseForm.GetAll(), "Id", "Name", purchase.PurchaseFormId);
                        ViewData["PurchaseKindId"] = new SelectList(_repoPurchaseKind.GetAll(), "Id", "Name", purchase.PurchaseKindId);
                        ViewData["PurchaseTypeId"] = new SelectList(_repoPurchaseType.GetAll(), "Id", "Name", purchase.PurchaseTypeId);
                        return View(purchase);
                    }

                    _purchase.FileNo = purchase.FileNo;
                    _purchase.WorkingName = purchase.WorkingName;
                    _purchase.FirmId = purchase.FirmId;
                    _purchase.PurchaseKindId = purchase.PurchaseKindId;
                    _purchase.PurchaseFormId = purchase.PurchaseFormId;
                    _purchase.PurchaseTypeId = purchase.PurchaseTypeId;
                    _purchase.Description = purchase.Description;
                    _purchase.FilePath = purchase.FilePath;
                    _purchase.PurchaseDate = purchase.PurchaseDate;
                    _purchase.Price = purchase.Price;

                    _repoPurchases.Update(_purchase);
                    await _repoPurchases.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!PurchasesExists(purchase.Id))
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
            ViewData["FirmId"] = new SelectList(_repoFirms.GetAll(), "Id", "Name", purchase.FirmId);
            ViewData["PurchaseFormId"] = new SelectList(_repoPurchaseForm.GetAll(), "Id", "Name", purchase.PurchaseFormId);
            ViewData["PurchaseKindId"] = new SelectList(_repoPurchaseKind.GetAll(), "Id", "Name", purchase.PurchaseKindId);
            ViewData["PurchaseTypeId"] = new SelectList(_repoPurchaseType.GetAll(), "Id", "Name", purchase.PurchaseTypeId);
            return View(purchase);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var purchase = await _repoPurchases.GetByIdAsync(id);
            if (purchase != null)
            {
                _repoPurchases.Delete(purchase);
            }

            await _repoPurchases.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }

        private bool PurchasesExists(int id)
        {
            return _repoPurchases.GetQueryable().Any(e => e.Id == id);
        }

    }
}
