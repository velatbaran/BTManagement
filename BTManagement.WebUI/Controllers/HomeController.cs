using BTManagement.Core.Entities.Inventory;
using BTManagement.Core.Entities.Purchase;
using BTManagement.Service.IRepository;
using BTManagement.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BTManagement.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRepository<Products> _repoProducts;
        private readonly IRepository<Purchases> _repoPurcahes;

        public HomeController(IRepository<Products> repoProducts, IRepository<Purchases> repoPurcahes)
        {
            _repoProducts = repoProducts;
            _repoPurcahes = repoPurcahes;
        }

        [Route("anasayfa")]
        public async Task<IActionResult> Index()
        {
            // Envanter
            var productAIO = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "AIO").OrderByDescending(x => x.CreatedDate).Count();
            var productFaalAIO = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "AIO" && x.State == "Faal").OrderByDescending(x => x.CreatedDate).Count();
            var productFaalAIOAmbarda = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "AIO" && x.State == "Faal" && x.Username.ToLower() == "ambarda").OrderByDescending(x => x.CreatedDate).Count();
            var productFaalAIOKullanimda = productFaalAIO - productFaalAIOAmbarda;
            var productArizaliAIO = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "AIO" && x.State == "Arýzalý").OrderByDescending(x => x.CreatedDate).Count();
            var productKayittanDusurulmusAIO = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "AIO" && x.State == "Kayýttan Düþürülmüþ").OrderByDescending(x => x.CreatedDate).Count();
            var productLaptop = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "LAPTOP").OrderByDescending(x => x.CreatedDate).Count();
            var productFaalLaptop = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "LAPTOP" && x.State == "Faal").OrderByDescending(x => x.CreatedDate).Count();
            var productFaalLaptopAmbarda = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "LAPTOP" && x.State == "Faal" && x.Username.ToLower() == "ambarda").OrderByDescending(x => x.CreatedDate).Count();
            var productFaalLaptopKullanimda = productFaalLaptop - productFaalLaptopAmbarda;
            var productArizaliLaptop = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "LAPTOP" && x.State == "Arýzalý").OrderByDescending(x => x.CreatedDate).Count();
            var productKayittanDusurulmusLaptop = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "LAPTOP" && x.State == "Kayýttan Düþürülmüþ").OrderByDescending(x => x.CreatedDate).Count();
            var productMasaustu = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "MASAÜSTÜ").OrderByDescending(x => x.CreatedDate).Count();
            var productFaalMasaustu = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x => x.Category.Name == "MASAÜSTÜ" && x.State == "Faal").OrderByDescending(x => x.CreatedDate).Count();
            var productFaalMasaustuAmbarda = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x => x.Category.Name == "MASAÜSTÜ" && x.State == "Faal" && x.Username.ToLower() == "ambarda").OrderByDescending(x => x.CreatedDate).Count();
            var productFaalMasaustuKullanimda = productFaalMasaustu - productFaalMasaustuAmbarda;
            var productArizaliMasaustu = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x => x.Category.Name == "MASAÜSTÜ" && x.State == "Arýzalý").OrderByDescending(x => x.CreatedDate).Count();
            var productKayittanDusurulmusMasaustu = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x => x.Category.Name == "MASAÜSTÜ" && x.State == "Kayýttan Düþürülmüþ").OrderByDescending(x => x.CreatedDate).Count();
            var productNetworkYazici = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "NETWORK YAZICI").OrderByDescending(x => x.CreatedDate).Count();
            var productNetworkYaziciFaal = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "NETWORK YAZICI" && x.State == "Faal").OrderByDescending(x => x.CreatedDate).Count();
            var productNetworkYaziciArizali = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "NETWORK YAZICI" && x.State == "Arýzalý").OrderByDescending(x => x.CreatedDate).Count();
            var productNetworkYaziciKayittanDusmus = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "NETWORK YAZICI" && x.State == "Kayýttan Düþürülmüþ").OrderByDescending(x => x.CreatedDate).Count();
            var productTablet = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x=> x.Category.Name == "TABLET").OrderByDescending(x => x.CreatedDate).Count();
            var productFaalTablet = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x => x.Category.Name == "TABLET" && x.State == "Faal").OrderByDescending(x => x.CreatedDate).Count();
            var productFaalTabletAmbarda = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x => x.Category.Name == "TABLET" && x.State == "Faal" && x.Username.ToLower() == "ambarda").OrderByDescending(x => x.CreatedDate).Count();
            var productFaalTabletKullanimda = productFaalTablet - productFaalTabletAmbarda;
            var productArizaliTablet = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x => x.Category.Name == "TABLET" && x.State == "Arýzalý").OrderByDescending(x => x.CreatedDate).Count();
            var productKayittanDusurulmusTablet = _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).Where(x => x.Category.Name == "TABLET" && x.State == "Kayýttan Düþürülmüþ").OrderByDescending(x => x.CreatedDate).Count();
            ViewData["TOTALAIO"] = productAIO;
            ViewData["FAALAIO"] = productFaalAIO;
            ViewData["FAALAIOAMBARDA"] = productFaalAIOAmbarda;
            ViewData["FAALAIOKULLANIMDA"] = productFaalAIOKullanimda;
            ViewData["ARIZALIAIO"] = productArizaliAIO;
            ViewData["KAYITTANDÜSMÜSAIO"] = productKayittanDusurulmusAIO;
            ViewData["TOTALLAPTOP"] = productLaptop;
            ViewData["FAALLAPTOP"] = productFaalLaptop;
            ViewData["FAALLAPTOPAMBARDA"] = productFaalLaptopAmbarda;
            ViewData["FAALLAPTOPKULLANIMDA"] = productFaalLaptopKullanimda;
            ViewData["ARIZALILAPTOP"] = productArizaliLaptop;
            ViewData["KAYITTANDÜSMÜSLAPTOP"] = productKayittanDusurulmusLaptop;
            ViewData["TOTALMASAÜSTÜ"] = productMasaustu;
            ViewData["FAALMASAÜSTÜ"] = productFaalMasaustu;
            ViewData["FAALMASAÜSTÜAMBARDA"] = productFaalMasaustuAmbarda;
            ViewData["FAALMASAÜSTÜKULLANIMDA"] = productFaalMasaustuKullanimda;
            ViewData["ARIZALIMASAÜSTÜ"] = productArizaliMasaustu;
            ViewData["KAYITTANDÜSMÜSMASAÜSTÜ"] = productKayittanDusurulmusMasaustu;
            ViewData["NETWORKYAZICI"] = productNetworkYazici;
            ViewData["NETWORKYAZICIFAAl"] = productNetworkYaziciFaal;
            ViewData["NETWORKYAZICIARIZALI"] = productNetworkYaziciArizali;
            ViewData["NETWORKYAZICIAYITTANDÜSMÜS"] = productNetworkYaziciKayittanDusmus;
            ViewData["TOTALTABLET"] = productTablet;
            ViewData["FAALTABLET"] = productFaalTablet;
            ViewData["FAALTABLETAMBARDA"] = productFaalTabletAmbarda;
            ViewData["FAALTABLETKULLANIMDA"] = productFaalTabletKullanimda;
            ViewData["ARIZALITABLET"] = productArizaliTablet;
            ViewData["KAYITTANDÜSMÜSTABLET"] = productKayittanDusurulmusTablet;


            // Satýn Alýmlar
            var yearlyTotals = await _repoPurcahes.GetQueryable()
                .Where(x => x.FileNo != null && x.FileNo.Length >= 4)
                .GroupBy(x => x.FileNo.Substring(0, 4))
                .Select(g => new YearlyPurchaseTotalViewModel
                {
                    Year = g.Key,
                    Total = g.Sum(x => x.Price)
                })
                .OrderBy(x => x.Year)
                .ToListAsync();

            return View(yearlyTotals);
        }

    }
}
