using BTManagement.Core.Entities.Inventory;
using BTManagement.Core.Entities.WorkDone;
using BTManagement.Service.IRepository;
using BTManagement.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.IO;

namespace BTManagement.WebUI.Controllers.WorkDone
{
    [Authorize]
    [Authorize(Policy = "AdminPolicy")]
    public class WorkDoneController : Controller
    {
        private readonly IRepository<Departments> _repoDepartments;
        private readonly IRepository<Images> _repoImages;
        private readonly IRepository<WorksDone> _repoWorksDone;
        private readonly IToastNotification _toastNotification;

        public WorkDoneController(IRepository<Departments> repoDepartments, IRepository<Images> repoImages, IRepository<WorksDone> repoWorksDone, IToastNotification toastNotification)
        {
            _repoDepartments = repoDepartments;
            _repoImages = repoImages;
            _repoWorksDone = repoWorksDone;
            _toastNotification = toastNotification;
        }

        [Route("yapilanisler")]
        public async Task<IActionResult> Index()
        {
            return View(await _repoWorksDone.GetQueryable().Include(p => p.Department).OrderByDescending(x => x.CreatedDate).ToListAsync());
        }

        [Route("yapilanisekle")]
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_repoDepartments.GetAll(), "Id", "Name");
            return View();
        }

        [Route("yapilanisekle")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorksDoneAndImagesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    WorksDone workDone = new WorksDone()
                    {
                        DepartmentId = viewModel.DepartmentId,
                        Work = viewModel.Work,
                        State = viewModel.State,
                    };

                    _repoWorksDone.Add(workDone);
                    await _repoWorksDone.SaveChangesAsync();

                    foreach (var file in viewModel.Files)
                    {
                        if (file.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await file.CopyToAsync(memoryStream);
                                var document = new Images
                                {
                                    WorkDoneId = workDone.Id,
                                    FileType = file.ContentType,
                                    Image = memoryStream.ToArray()
                                };
                                _repoImages.Add(document);
                            }
                        }
                    }

                    await _repoImages.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _toastNotification.AddErrorToastMessage(ex.Message, new ToastrOptions { Title = "Hata" });
                }

            }
            ViewData["DepartmentId"] = new SelectList(_repoDepartments.GetAll(), "Id", "Name", viewModel.DepartmentId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _repoWorksDone.GetByIdAsync(id);
            if (user != null)
            {
                _repoWorksDone.Delete(user);
            }

            await _repoWorksDone.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }
    }
}
