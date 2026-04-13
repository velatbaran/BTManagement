using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BTManagement.Core.Entities.Inventory;
using BTManagement.Data.DataContext;
using BTManagement.Service.IRepository;
using NToastNotify;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Text;
using BTManagement.WebUI.Models;
using System.Diagnostics.Metrics;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;

namespace BTManagement.WebUI.Controllers.Product
{

    public class ProductsController : Controller
    {
        private readonly IRepository<Products> _repoProducts;
        private readonly IRepository<Categories> _repoCategories;
        private readonly IRepository<Departments> _repoDepartments;
        private readonly IToastNotification _toastNotification;
        private readonly DatabaseContext _context;

        public ProductsController(IRepository<Products> repoProducts, IRepository<Categories> repoCategories, IRepository<Departments> repoDepartments, IToastNotification toastNotification, DatabaseContext context)
        {
            _repoProducts = repoProducts;
            _repoCategories = repoCategories;
            _repoDepartments = repoDepartments;
            _toastNotification = toastNotification;
            _context = context;
        }

        [Authorize]
        [Route("urunler")]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new[] { "*" })]
        public async Task<IActionResult> Index()
        {
            // Sadece ihtiyacınız olan alanları select edin
            //var products = await _repoProducts.GetQueryable()
            //    .Include(p => p.Category)
            //    .Include(p => p.Department)
            //    .AsNoTracking() 
            //    .OrderByDescending(x => x.CreatedDate)
            //    .ToListAsync();

            //return View(products);
            return View();
        }

        [HttpPost]
        [Route("urunler/LoadData")]
        public async Task<IActionResult> LoadData()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                var query = _repoProducts.GetQueryable()
                    .Include(p => p.Category)
                    .Include(p => p.Department)
                    .AsNoTracking();

                int pageSize = length != null ? Convert.ToInt32(length) : 10;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                if (pageSize <= 0)
                {
                    pageSize = await query.CountAsync(); // tüm kayıt sayısı
                    skip = 0; // baştan al
                }

                var recordsTotal = await query.CountAsync();

                // 🔥 TÜRKÇE KARAKTER DÖNÜŞÜM FONKSİYONU (SQL tarafında)
                string NormalizeString(string input)
                {
                    if (string.IsNullOrEmpty(input)) return input;

                    return input
                        .ToLower()
                        .Replace("ı", "i")
                        .Replace("ğ", "g")
                        .Replace("ü", "u")
                        .Replace("ş", "s")
                        .Replace("ö", "o")
                        .Replace("ç", "c")
                        .Replace("İ", "i")
                        .Replace("Ğ", "g")
                        .Replace("Ü", "u")
                        .Replace("Ş", "s")
                        .Replace("Ö", "o")
                        .Replace("Ç", "c");
                }

                // Kolon filtrelerini al ve normalize et
                var brandFilter = NormalizeString(Request.Form["columns[1][search][value]"].FirstOrDefault());
                var modelFilter = NormalizeString(Request.Form["columns[2][search][value]"].FirstOrDefault());
                var serialNoFilter = NormalizeString(Request.Form["columns[3][search][value]"].FirstOrDefault());
                var usernameFilter = NormalizeString(Request.Form["columns[4][search][value]"].FirstOrDefault());
                var categoryFilter = NormalizeString(Request.Form["columns[5][search][value]"].FirstOrDefault());
                var departmentFilter = NormalizeString(Request.Form["columns[6][search][value]"].FirstOrDefault());
                var stateFilter = NormalizeString(Request.Form["columns[7][search][value]"].FirstOrDefault());
                var descriptionFilter = NormalizeString(Request.Form["columns[8][search][value]"].FirstOrDefault());
                var networkAddressFilter = NormalizeString(Request.Form["columns[9][search][value]"].FirstOrDefault());
                var networkNameFilter = NormalizeString(Request.Form["columns[10][search][value]"].FirstOrDefault());
                var hardwareNameFilter = NormalizeString(Request.Form["columns[11][search][value]"].FirstOrDefault());
                var featureFilter = NormalizeString(Request.Form["columns[12][search][value]"].FirstOrDefault());
                var createdFilter = NormalizeString(Request.Form["columns[13][search][value]"].FirstOrDefault());
                var createdDateFilter = NormalizeString(Request.Form["columns[14][search][value]"].FirstOrDefault());

                // Kolon filtrelerini uygula (normalize edilmiş değerlerle)
                if (!string.IsNullOrEmpty(brandFilter))
                    query = query.Where(p => p.Brand.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(brandFilter));

                if (!string.IsNullOrEmpty(modelFilter))
                    query = query.Where(p => p.Model.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(modelFilter));

                if (!string.IsNullOrEmpty(serialNoFilter))
                    query = query.Where(p => p.SerialNo.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(serialNoFilter));

                if (!string.IsNullOrEmpty(usernameFilter))
                    query = query.Where(p => p.Username.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(usernameFilter));

                if (!string.IsNullOrEmpty(categoryFilter))
                    query = query.Where(p => p.Category.Name.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(categoryFilter));

                if (!string.IsNullOrEmpty(departmentFilter))
                    query = query.Where(p => p.Department.Name.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(departmentFilter));

                if (!string.IsNullOrEmpty(stateFilter))
                    query = query.Where(p => p.State.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(stateFilter));

                if (!string.IsNullOrEmpty(descriptionFilter))
                    query = query.Where(p => p.Description.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(descriptionFilter));

                if (!string.IsNullOrEmpty(networkAddressFilter))
                    query = query.Where(p => p.NetworkAddress.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(networkAddressFilter));

                if (!string.IsNullOrEmpty(networkNameFilter))
                    query = query.Where(p => p.NetworkName.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(networkNameFilter));

                if (!string.IsNullOrEmpty(hardwareNameFilter))
                    query = query.Where(p => p.HardwareName.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(hardwareNameFilter));

                if (!string.IsNullOrEmpty(featureFilter))
                    query = query.Where(p => p.Feature.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(featureFilter));

                if (!string.IsNullOrEmpty(createdFilter))
                    query = query.Where(p => p.Created.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(createdFilter));

                if (!string.IsNullOrEmpty(createdDateFilter))
                    query = query.Where(p => p.CreatedDate.ToString().ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(createdDateFilter));

                // Genel arama (DataTable'ın üst arama kutusu - kaldırdıysanız bu kısım çalışmaz)
                if (!string.IsNullOrEmpty(searchValue))
                {
                    var normalizedSearch = NormalizeString(searchValue);
                    query = query.Where(p =>
                        p.Brand.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(normalizedSearch) ||
                        p.Model.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(normalizedSearch) ||
                        p.SerialNo.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(normalizedSearch) ||
                        p.Username.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(normalizedSearch) ||
                        p.Category.Name.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(normalizedSearch) ||
                        p.Department.Name.ToLower().Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s").Replace("ö", "o").Replace("ç", "c").Contains(normalizedSearch)
                    );
                }

                var recordsFiltered = await query.CountAsync();

                // 🔥 SIRALAMA - SABİT: CreatedDate'e göre azalan (en yeniden en eskiye)
                // Kullanıcının sıralama taleplerini tamamen görmezden gel
                query = query.OrderByDescending(p => p.CreatedDate);

                // Veriyi çek
                var products = await query
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(p => new
                    {
                        p.Id,
                        p.Brand,
                        p.Model,
                        p.SerialNo,
                        p.Username,
                        CategoryName = p.Category != null ? p.Category.Name : "",
                        DepartmentName = p.Department != null ? p.Department.Name : "",
                        p.State,
                        p.Description,
                        p.NetworkAddress,
                        p.NetworkName,
                        p.HardwareName,
                        p.Feature,
                        p.Created,
                        p.CreatedDate
                    })
                    .ToListAsync();

                // Formatlama
                var data = products.Select(p => new
                {
                    p.Id,
                    p.Brand,
                    p.Model,
                    p.SerialNo,
                    p.Username,
                    p.CategoryName,
                    p.DepartmentName,
                    p.State,
                    p.Description,
                    p.NetworkAddress,
                    p.NetworkName,
                    p.HardwareName,
                    p.Feature,
                    p.Created,
                    CreatedDate = p.CreatedDate.ToString()
                }).ToList();

                return Json(new
                {
                    draw = draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = Request.Form["draw"].FirstOrDefault(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>(),
                    error = ex.Message
                });
            }
        }

        [Authorize]
        [Authorize(Policy = "AdminPolicy")]
        [Route("urunekle")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_repoCategories.GetAll(), "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_repoDepartments.GetAll(), "Id", "Name");
            return View();
        }


        [Authorize]
        [Authorize(Policy = "AdminPolicy")]
        [Route("urunekle")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Products products)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = _repoProducts.GetQueryable().Where(x => x.SerialNo == products.SerialNo).FirstOrDefault();
                    if (product != null)
                    {
                        _toastNotification.AddWarningToastMessage("Aynı seri no lu ürün sistemde kayıtlı. Lütfen başka bir seri no bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        ViewData["CategoryId"] = new SelectList(_repoCategories.GetAll(), "Id", "Name", products.CategoryId);
                        ViewData["DepartmentId"] = new SelectList(_repoDepartments.GetAll(), "Id", "Name", products.DepartmentId);
                        return View(products);
                    }

                    _repoProducts.Add(products);
                    await _repoProducts.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı bir şekilde gerçekleşti", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _toastNotification.AddErrorToastMessage(ex.Message, new ToastrOptions { Title = "Hata" });
                }

            }
            ViewData["CategoryId"] = new SelectList(_repoCategories.GetAll(), "Id", "Name", products.CategoryId);
            ViewData["DepartmentId"] = new SelectList(_repoDepartments.GetAll(), "Id", "Name", products.DepartmentId);
            return View(products);
        }

        [Authorize]
        [Authorize(Policy = "AdminPolicy")]
        [Route("urunguncelle/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _repoProducts.GetByIdAsync(id.Value);
            if (products == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_repoCategories.GetAll(), "Id", "Name", products.CategoryId);
            ViewData["DepartmentId"] = new SelectList(_repoDepartments.GetAll(), "Id", "Name", products.DepartmentId);
            return View(products);
        }

        [Authorize]
        [Authorize(Policy = "AdminPolicy")]
        [Route("urunguncelle/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Products products)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _repoProducts.GetAsync(x => x.Id == id);
                    if (await _repoProducts.AnyAsync(x => x.SerialNo == products.SerialNo && x.Id != id))
                    {
                        _toastNotification.AddWarningToastMessage("Aynı seri no lu ürün sistemde kayıtlı. Lütfen başka bir seri no bilgisi giriniz!", new ToastrOptions { Title = "Uyarı" });
                        ViewData["CategoryId"] = new SelectList(_repoCategories.GetAll(), "Id", "Name", products.CategoryId);
                        ViewData["DepartmentId"] = new SelectList(_repoDepartments.GetAll(), "Id", "Name", products.DepartmentId);
                        return View(products);
                    }

                    product.Brand = products.Brand;
                    product.Model = products.Model;
                    product.SerialNo = products.SerialNo;
                    product.NetworkAddress = products.NetworkAddress;
                    product.NetworkName = products.NetworkName;
                    product.HardwareName = products.HardwareName;
                    product.Feature = products.Feature;
                    product.Username = products.Username;
                    product.State = products.State;
                    product.Description = products.Description;
                    product.CategoryId = products.CategoryId;
                    product.DepartmentId = products.DepartmentId;

                    _repoProducts.Update(product);
                    await _repoProducts.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı bir şekilde gerçekleşti.", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ProductsExists(products.Id))
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
            ViewData["CategoryId"] = new SelectList(_repoCategories.GetAll(), "Id", "Name", products.CategoryId);
            ViewData["DepartmentId"] = new SelectList(_repoDepartments.GetAll(), "Id", "Name", products.DepartmentId);
            return View(products);
        }

        [Authorize]
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _repoProducts.GetByIdAsync(id);
            if (product != null)
            {
                _repoProducts.Delete(product);
            }

            await _repoProducts.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı bir şekilde gerçekleşti", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _repoProducts.GetQueryable().Any(e => e.Id == id);
        }

    }
}
