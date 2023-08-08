﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P137Pronia.Extensions;
using P137Pronia.Services.Interfaces;
using P137Pronia.ViewModels.ProductVMs;

namespace P137Pronia.Areas.Manage.Controllers
{
	[Area("Manage")]
    [Authorize(Roles = "Admin")]


    public class ProductController : Controller
	{
		readonly IProductService _service;
		readonly ICategoryService _catService;

        public ProductController(IProductService service, ICategoryService catService)
        {
            _service = service;
            _catService = catService;
        }

        public async Task<IActionResult> Index()
		{
			return View(await _service.GetTable.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).ToListAsync());
		}
		public IActionResult Create()
		{
			ViewBag.Categories = new SelectList(_catService.GetTable, "Id", "Name");
			//ViewBag.Categories = _catService.GetTable;
            return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(CreateProductVM vm)
		{
			if (vm.MainImageFile != null)
			{
				if (!vm.MainImageFile.IsTypeValid("image"))
				{
					ModelState.AddModelError("MainImageFile", "Wrong file type");
				}
				if (!vm.MainImageFile.IsSizeValid(2))
				{
					ModelState.AddModelError("MainImageFile", "File max size is 2mb");
				}
			}
            if (vm.HoverImageFile != null)
            {
				if (!vm.HoverImageFile.IsTypeValid("image"))
				{
					ModelState.AddModelError("HoverImageFile", "Wrong file type");
				}
				if (!vm.HoverImageFile.IsSizeValid(2))
				{
					ModelState.AddModelError("HoverImageFile", "File max size is 2mb");
				}
            }
            if (vm.ImageFiles != null)
            {
				foreach (var img in vm.ImageFiles)
				{
                    if (!img.IsTypeValid("image"))
                    {
                        ModelState.AddModelError("ImageFiles", "Wrong file type " + img.FileName);
                    }
                    if (!img.IsSizeValid(2))
                    {
                        ModelState.AddModelError("ImageFiles", "File max size is 2mb" + img.FileName);
                    }
                }
            }
			if (!ModelState.IsValid)
			{
                ViewBag.Categories = new SelectList(_catService.GetTable, "Id", "Name");
                return View();
			}
            await _service.Create(vm);
			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Delete(int? id)
		{
			await _service.Delete(id);
			return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            await _service.SoftDelete(id);
            TempData["IsDeleted"] = true;
            return RedirectToAction(nameof(Index));
        }
		public async Task<IActionResult> Update(int? id)
		{
			if (id == null || id <= 0) return BadRequest();
			var entity = await _service.GetTable.Include(p=>p.ProductImages).Include(p=>p.ProductCategories).SingleOrDefaultAsync(p=>p.Id == id);
			if (entity == null) return BadRequest();
            ViewBag.Categories = new SelectList(_catService.GetTable, "Id", "Name");
            UpdateProductGETVM vm = new UpdateProductGETVM
			{
				Name = entity.Name,
				Description = entity.Description,
				Discount = entity.Discount,
				Price = entity.Price,
				StockCount = entity.StockCount,
				Rating = entity.Rating,
				MainImage = entity.MainImage,
				HoverImage = entity.HoverImage,
				ProductImages = entity.ProductImages,
				ProductCategoryIds = entity.ProductCategories.Select(p=>p.CategoryId).ToList()
			};
			return View(vm);
        }
		[HttpPost]
		public async Task<IActionResult> Update(int? id, UpdateProductGETVM vm)
		{
            if (id == null || id <= 0) return BadRequest();
            var entity = await _service.GetById(id);
            if (entity == null) return BadRequest();
			UpdateProductVM updateVm = new UpdateProductVM
			{
				Name = vm.Name,
				Description = vm.Description,
				Discount= vm.Discount,
				Price= vm.Price,
				Rating= vm.Rating,
				StockCount = vm.StockCount,
				HoverImage = vm.HoverImageFile,
				MainImage = vm.MainImageFile,
				ProductImageFiles = vm.ProductImageFiles,
				CategoryIds = vm.ProductCategoryIds
			};
			await _service.Update(id, updateVm);
			return RedirectToAction(nameof(Index));
        }
		public async Task<IActionResult> DeleteImage(int id)
		{
			if (id == null || id <= 0) return BadRequest();
			await _service.DeleteImage(id);
            return Ok();
		}

    }
}
