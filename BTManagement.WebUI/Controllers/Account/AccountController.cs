using BTManagement.Core.Entities.User;
using BTManagement.Service.IRepository;
using BTManagement.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Claims;

namespace BTManagement.WebUI.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly IRepository<Users> _repoUser;
        private readonly IToastNotification _toastNotification;

        public AccountController(IRepository<Users> repoUser, IToastNotification toastNotification)
        {
            _repoUser = repoUser;
            _toastNotification = toastNotification;
        }

        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var account = await _repoUser.GetAsync(x => x.Username == model.Username & x.Password == model.Password & x.IsActive);
                    if (account == null)
                    {

                        _toastNotification.AddErrorToastMessage("Kullanıcı adı veya şifre hatalı", new ToastrOptions { Title = "Hatalı" });
                        return View(model);
                    }
                    else
                    {
                        var claims = new List<Claim>()
                        {
                            new(ClaimTypes.Name,account.NameSurname),
                            new(ClaimTypes.Role,account.IsAdmin ? "Admin" : "Standart"),
                            new("Username",account.Username.ToString()),
                            new("UserId",account.Id.ToString()),
                            new("UserGuid",account.UserGuid.ToString())
                        };

                        var userIdentity = new ClaimsIdentity(claims, "Login");
                        ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);
                        await HttpContext.SignInAsync(userPrincipal);
                        _toastNotification.AddSuccessToastMessage("Giriş işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                        return Redirect(string.IsNullOrEmpty(model.ReturnUrl) ? "/anasayfa" : model.ReturnUrl);
                    }
                }
                catch (Exception)
                {
                    // loglama
                    _toastNotification.AddErrorToastMessage("Lütfen kullanıcı bilgilerini kontrol ediniz", new ToastrOptions { Title = "Hatalı" });
                    return View(model);
                }
            }
            return View(model);
        }

        [Route("cikis")]
        public async Task<IActionResult> LogOutAsync()
        {
            await HttpContext.SignOutAsync();
            _toastNotification.AddSuccessToastMessage("Çıkış işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction("login");
        }

        [Route("erisimengellendi"), Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("profilim")]
        [Authorize]
        public async Task<IActionResult> MyProfileAsync()
        {
            Users user = await _repoUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value.ToString());
            if (user == null)
            {
                return NotFound();
            }
            var model = new MyProfileViewModel()
            {
                Id = user.Id,
                Username = user.Username,
                NameSurname = user.NameSurname,
            };
            return View(model);
        }

        [Route("profilim")]
        [HttpPost, Authorize]
        public async Task<IActionResult> MyProfileAsync(MyProfileViewModel model, int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _repoUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
                    if (await _repoUser.AnyAsync(x => x.Username == model.Username && x.Id != id))
                    {
                        _toastNotification.AddWarningToastMessage("Aynı kullanıcı adı sistemde kayıtlı. Lütfen başka bir kullanıcı adı giriniz!", new ToastrOptions { Title = "Uyarı" });
                        return View(model);
                    }

                    user.Username = model.Username;
                    user.NameSurname = model.NameSurname;
                    _repoUser.Update(user);
                    var sonuc = await _repoUser.SaveChangesAsync();
                    if (sonuc > 0)
                    {
                        _toastNotification.AddSuccessToastMessage("Bilgileriniz başarıyla güncellenmiştir", new ToastrOptions { Title = "Başarılı" });
                        return View(model);
                    }
                }
                catch (Exception)
                {
                    _toastNotification.AddErrorToastMessage("Bilgileriniz güncellenirken hata", new ToastrOptions { Title = "Hata" });
                }
            }
            return View(model);
        }

        [Route("sifremiunuttum")]
        [HttpGet]
        public IActionResult ForgetPasswordAsync()
        {
            return View();
        }

        [Route("sifremiunuttum")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    Users user = await _repoUser.GetAsync(x => x.Username == model.Username);
                    if (user is null)
                    {
                        _toastNotification.AddErrorToastMessage("Sistemde kayıtlı böyle bir kullanıcı adı yok!", new ToastrOptions { Title = "Hata" });
                        return View(model);
                    }

                    user.Password = model.Password;
                    _repoUser.Update(user);
                    var sonuc = await _repoUser.SaveChangesAsync();
                    if (sonuc > 0)
                    {
                        _toastNotification.AddSuccessToastMessage("Şifreniz başarıyla değişti.", new ToastrOptions { Title = "Başarılı" });
                        return View(model);
                    }

                }
                catch (Exception)
                {
                    _toastNotification.AddErrorToastMessage("Şifreniz değiştirilirken hata oluştu!", new ToastrOptions { Title = "Hata" });
                }
            }
            return View(model);
        }

        [Route("sifremidegistir")]
        [HttpGet, Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Route("sifremidegistir")]
        [HttpPost, Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Users user = await _repoUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
                    user.Password = model.Password;
                    _repoUser.Update(user);
                    await _repoUser.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Şifre değiştirme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return View(model);
                }
                catch (Exception)
                {
                    // loglama
                    _toastNotification.AddErrorToastMessage("Lütfen şifre bilgilerini kontrol ediniz", new ToastrOptions { Title = "Hatalı" });
                    return View(model);
                }
            }
            return View(model);
        }
    }
}
