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

namespace BTManagement.WebUI.Controllers.Product
{

    public class ProductsController : Controller
    {
        private readonly IRepository<Products> _repoProducts;
        private readonly IRepository<Categories> _repoCategories;
        private readonly IRepository<Departments> _repoDepartments;
        private readonly IToastNotification _toastNotification;

        public ProductsController(DatabaseContext context, IRepository<Products> repoProducts, IRepository<Categories> repoCategories, IRepository<Departments> repoDepartments, IToastNotification toastNotification)
        {
            _repoProducts = repoProducts;
            _repoCategories = repoCategories;
            _repoDepartments = repoDepartments;
            _toastNotification = toastNotification;
        }

        [Route("urunler")]
        public async Task<IActionResult> Index()
        {
            return View(await _repoProducts.GetQueryable().Include(p => p.Category).Include(p => p.Department).OrderByDescending(x => x.CreatedDate).ToListAsync());
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
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
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
                    product.NetworkName = products.Brand;
                    product.HardwareName = products.HardwareName;
                    product.Feature = products.Feature;
                    product.Username = products.Username;
                    product.State = products.State;
                    product.Description = products.Description;
                    product.CategoryId = products.CategoryId;
                    product.DepartmentId = products.DepartmentId;

                    _repoProducts.Update(product);
                    await _repoProducts.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
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
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _repoProducts.GetQueryable().Any(e => e.Id == id);
        }
    }
}
