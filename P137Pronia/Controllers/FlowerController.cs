

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P137Pronia.DataAccess;
using P137Pronia.Models;
using P137Pronia.Services.Interfaces;
using P137Pronia.ViewModels.ProductVMs;

public class FlowerController : Controller
{
    readonly IProductService _service;
    readonly ProniaDbContext _context;

    public FlowerController(IProductService service, ProniaDbContext context)
    {
        _service = service;
        _context = context;
    }

    public async Task<IActionResult>  Index()
    {
        IQueryable<Product> products = _service.GetTable;
        GetShopVM shopVM = new GetShopVM
        {
            Products= products.Include(p => p.ProductImages),
            ProductCount=await products.CountAsync(),
            Categories=_context.Categories.Include(p => p.ProductCategories),
            CategoryCount=await _context.ProductCategories.CountAsync(),
        };
        return View(shopVM);
    }
    public async Task<IActionResult> Detail(int? id)
    {
        if (id == null || id <= 0) return BadRequest();

        //var entity = await _service.GetById(id);
        var entity = await _service.GetTable.Include(p => p.ProductImages).SingleOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        if (entity == null) return NotFound();
        return View(entity);
    }
    [HttpPost]
    public async Task<IActionResult> Filter(FilterVM vM)
    {
        if(vM.MinPrice>vM.MaxPrice)return BadRequest();
        if (String.IsNullOrEmpty(vM.Search))
        {
            vM.Search = "";
        }
        var model = _context.Products.Include(p => p.ProductCategories).ThenInclude(p => p.Category);
        var result = model.Where(p => p.Name.Contains(vM.Search));
        if(vM.CategoryId > 0)
        {
            result=result.Where(p=>p.ProductCategories.Any(pc=>pc.CategoryId == vM.CategoryId));
        }
        if(vM.MaxPrice != 0)
        {
            result = result.Where(p => p.Price <= vM.MaxPrice && p.Price >= vM.MinPrice);

        }
        return PartialView("_ProductFilterPartial",result);
    }
}
