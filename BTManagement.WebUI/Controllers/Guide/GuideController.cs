using BTManagement.Core.Entities.Guide;
using BTManagement.Core.Entities.Inventory;
using BTManagement.Service.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace BTManagement.WebUI.Controllers.Guide
{
    [Authorize]
    [Authorize(Policy = "AdminPolicy")]
    public class GuideController : Controller
    {
        private readonly IRepository<MyGuide> _repoMyGuide;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<Departments> _repoDepartments;

        public GuideController(IRepository<MyGuide> repoMyGuide, IToastNotification toastNotification, IRepository<Departments> repoDepartments)
        {
            _repoMyGuide = repoMyGuide;
            _toastNotification = toastNotification;
            _repoDepartments = repoDepartments;
        }

        [Route("rehberim")]
        public async Task<IActionResult> Index()
        {
            return View(await _repoMyGuide.GetQueryable().Include(p => p.Department).Where(x => x.IsDeleted == false).OrderByDescending(x => x.CreatedDate).ToListAsync());
        }

        [Route("rehberekle")]
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_repoDepartments.GetAll(), "Id", "Name");
            return View();
        }

        [Route("rehberekle")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MyGuide myguide)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repoMyGuide.Add(myguide);
                    await _repoMyGuide.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _toastNotification.AddErrorToastMessage(ex.Message, new ToastrOptions { Title = "Hata" });
                }

            }
            ViewData["DepartmentId"] = new SelectList(_repoDepartments.GetAll(), "Id", "Name", myguide.DepartmentId);
            return View(myguide);
        }

        [Route("rehberguncelle/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myguide = await _repoMyGuide.GetByIdAsync(id.Value);
            if (myguide == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_repoDepartments.GetAll(), "Id", "Name", myguide.DepartmentId);
            return View(myguide);
        }

        [Route("rehberguncelle/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MyGuide myGuide)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var _myGuide = await _repoMyGuide.GetAsync(x => x.Id == id);

                    _myGuide.NameSurname = myGuide.NameSurname;
                    _myGuide.Title = myGuide.Title;
                    _myGuide.DepartmentId = myGuide.DepartmentId;
                    _myGuide.InternalNo = myGuide.InternalNo;
                    _myGuide.CepNo = myGuide.CepNo;

                    _repoMyGuide.Update(_myGuide);
                    await _repoMyGuide.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!MyGuideExists(myGuide.Id))
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
            ViewData["DepartmentId"] = new SelectList(_repoDepartments.GetAll(), "Id", "Name", myGuide.DepartmentId);
            return View(myGuide);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var myguide = await _repoMyGuide.GetByIdAsync(id);
            if (myguide != null)
            {
                myguide.IsDeleted = true;
            }

            await _repoMyGuide.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }

        private bool MyGuideExists(int id)
        {
            return _repoMyGuide.GetQueryable().Any(e => e.Id == id);
        }
    }
}
