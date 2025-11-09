using BTManagement.Core.Entities.Guide;
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
    public class FirmsController : Controller
    {
        private readonly IRepository<Firm> _repoFirms;
        private readonly IRepository<FirmKind> _repoFirmKind;
        private readonly IToastNotification _toastNotification;

        public FirmsController(IRepository<Firm> repoFirms, IRepository<FirmKind> repoFirmKind, IToastNotification toastNotification)
        {
            _repoFirms = repoFirms;
            _repoFirmKind = repoFirmKind;
            _toastNotification = toastNotification;
        }

        [Route("firmalar")]
        public async Task<IActionResult> Index()
        {
            return View(await _repoFirms.GetQueryable().Include(p => p.FirmKinds).OrderByDescending(x => x.CreatedDate).ToListAsync());
        }

        [Route("firmaekle")]
        public IActionResult Create()
        {
            ViewData["FirmKindId"] = new SelectList(_repoFirmKind.GetAll(), "Id", "Name");
            return View();
        }

        [Route("firmaekle")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Firm firm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repoFirms.Add(firm);
                    await _repoFirms.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _toastNotification.AddErrorToastMessage(ex.Message, new ToastrOptions { Title = "Hata" });
                }

            }
            ViewData["FirmKindId"] = new SelectList(_repoFirmKind.GetAll(), "Id", "Name", firm.FirmKindId);
            return View(firm);
        }

        [Route("firmaguncelle/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var firm = await _repoFirms.GetByIdAsync(id.Value);
            if (firm == null)
            {
                return NotFound();
            }
            ViewData["FirmKindId"] = new SelectList(_repoFirmKind.GetAll(), "Id", "Name", firm.FirmKindId);
            return View(firm);
        }

        [Route("firmaguncelle/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Firm firm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _firm = await _repoFirms.GetAsync(x => x.Id == id);

                    _firm.Name = firm.Name;
                    _firm.Authorized = firm.Authorized;
                    _firm.FirmKindId = firm.FirmKindId;
                    _firm.RecordNo = firm.RecordNo;
                    _firm.TaxOffice = firm.TaxOffice;
                    _firm.TaxNo = firm.TaxNo;
                    _firm.TcNo = firm.TcNo;
                    _firm.Phone = firm.Phone;
                    _firm.Eposta = firm.Eposta;
                    _firm.Address = firm.Address;
                    _firm.WebAddress = firm.WebAddress;
                    _firm.BankName = firm.BankName;
                    _firm.IbanNo = firm.IbanNo;


                    _repoFirms.Update(_firm);
                    await _repoFirms.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!FirmExists(firm.Id))
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
            ViewData["FirmKindId"] = new SelectList(_repoFirmKind.GetAll(), "Id", "Name", firm.FirmKindId);
            return View(firm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var firm = await _repoFirms.GetByIdAsync(id);
            if (firm != null)
            {
                _repoFirms.Delete(firm);
            }

            await _repoFirms.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }

        private bool FirmExists(int id)
        {
            return _repoFirms.GetQueryable().Any(e => e.Id == id);
        }
    }
}
