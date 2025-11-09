using BTManagement.Core.Entities.User;
using BTManagement.Service.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace BTManagement.WebUI.Controllers.User
{
    [Authorize]
    [Authorize(Policy = "AdminPolicy")]
    public class UsersController : Controller
    {
        private readonly IRepository<Users> _repoUser;
        private readonly IToastNotification _toastNotification;

        public UsersController(IRepository<Users> repoUser, IToastNotification toastNotification)
        {
            _repoUser = repoUser;
            _toastNotification = toastNotification;
        }

        [Route("kullanicilar")]
        public IActionResult Index()
        {
            return View(_repoUser.GetAll(x => x.IsActive == true).OrderByDescending(x => x.CreatedDate));
        }

        [Route("kullaniciekle")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("kullaniciekle")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(Users userr)
        {
            if (ModelState.IsValid)
            {
                var _user = _repoUser.GetQueryable().Where(x => x.Username == userr.Username).FirstOrDefault();
                if (_user != null)
                {
                    _toastNotification.AddWarningToastMessage("Aynı kullanıcı adı sistemde kayıtlı. Lütfen başka bir kullanıcı adı giriniz!", new ToastrOptions { Title = "Uyarı" });
                    return View(userr);
                }

                _repoUser.Add(userr);
                await _repoUser.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                return RedirectToAction(nameof(Index));
            }
            _toastNotification.AddWarningToastMessage("Lütfen gerekli alanları doldurun", new ToastrOptions { Title = "Uyarı" });
            return View(userr);
        }

        [Route("kullaniciguncelle/{id?}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _repoUser.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [Route("kullaniciguncelle/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(Users userr, int id)
        {
            if (ModelState.IsValid)
            {
                var _user = await _repoUser.GetByIdAsync(id);
                if (await _repoUser.AnyAsync(x => x.Username == userr.Username && x.Id != id))
                {
                    _toastNotification.AddWarningToastMessage("Aynı kullanıcı adı sistemde kayıtlı. Lütfen başka bir kullanıcı adı giriniz!", new ToastrOptions { Title = "Uyarı" });
                    return View(userr);
                }

                _user.NameSurname = userr.NameSurname;
                _user.Username = userr.Username;
                _user.Password = userr.Password;
                _user.IsAdmin = userr.IsAdmin;
                _user.IsActive = userr.IsActive;

                _repoUser.Update(_user);
                await _repoUser.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                return RedirectToAction(nameof(Index));
            }
            _toastNotification.AddWarningToastMessage("Lütfen gerekli alanları doldurun", new ToastrOptions { Title = "Uyarı" });
            return View(userr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _repoUser.GetByIdAsync(id);
            if (user != null)
            {
                _repoUser.Delete(user);
            }

            await _repoUser.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }
    }
}
