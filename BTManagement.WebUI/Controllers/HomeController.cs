using BTManagement.Core.Entities.Inventory;
using BTManagement.Service.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BTManagement.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Products> _repoProducts;
        private readonly IRepository<Categories> _repoCategories;
        private readonly IRepository<Departments> _repoDepartments;

        public HomeController(IRepository<Products> repoProducts, IRepository<Categories> repoCategories, IRepository<Departments> repoDepartments)
        {
            _repoProducts = repoProducts;
            _repoCategories = repoCategories;
            _repoDepartments = repoDepartments;
        }

        [Route("anasayfa")]
        public IActionResult Index()
        {
            var productAIO = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "AIO").OrderByDescending(x => x.CreatedDate).Count();
            var productLaptop = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "LAPTOP").OrderByDescending(x => x.CreatedDate).Count();
            var productMasaüstü = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "MASAÜSTÜ").OrderByDescending(x => x.CreatedDate).Count();
            var productNetworkYazici = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "NETWORK YAZICI").OrderByDescending(x => x.CreatedDate).Count();
            var productTablet = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "TABLET").OrderByDescending(x => x.CreatedDate).Count();
            ViewData["AIO"] = productAIO;
            ViewData["LAPTOP"] = productLaptop;
            ViewData["MASAÜSTÜ"] = productMasaüstü;
            ViewData["NETWORK YAZICI"] = productNetworkYazici;
            ViewData["TABLET"] = productTablet;

            return View();
        }

    }
}
