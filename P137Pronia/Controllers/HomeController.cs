using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P137Pronia.DataAccess;
using P137Pronia.Models;
using P137Pronia.Services.Interfaces;
using P137Pronia.ViewModels.BasketVMs;
using P137Pronia.ViewModels.HomeVMs;

namespace P137Pronia.Controllers;

public class HomeController : Controller
{
    private readonly ISliderService _sliderService;
    private readonly IProductService _productService;

    public HomeController(IProductService productService,
       ISliderService sliderService)
    {
        _productService = productService;
        _sliderService = sliderService;
    }

    public async Task<IActionResult> Index()
    {
        HomeVM vm = new HomeVM
        {
            Sliders = await _sliderService.GetAll(),
            Products = await _productService.GetTable.Take(4).ToListAsync()
        };
        ViewBag.ProductCount = await _productService.GetTable.CountAsync();
        return View(vm);
    }
    public async Task<IActionResult> LoadMore(int skip, int take)
    {
        return PartialView("_ProductPartial",await _productService.GetTable.Skip(skip).Take(take).ToListAsync());
    }
    //public string GetSession(string key)
    //{
    //   return HttpContext.Session.GetString(key??"")??"";
    //}
    //public void SetSession(string key,string value)
    //{
    //    HttpContext.Session.SetString(key, value);
    //}
    //public string GetCookie(string key)
    //{
    //    string a= HttpContext.Request.Cookies[key];
    //    return a;
    //}
    //public void SetCookie(string key, string value)
    //{
    //    HttpContext.Response.Cookies.Append(key, value);
    //}

    public async Task<IActionResult> AddBasket(int id)
    {
        if (id == 0 || id < 1) return BadRequest();
        if (!await _productService.GetTable.AnyAsync(p => p.Id == id)) return BadRequest();

        var basket = HttpContext.Request.Cookies["basket"];
        List<BasketItemVM> items = basket == null ? new List<BasketItemVM>() :
            JsonConvert.DeserializeObject<List<BasketItemVM>>(basket);

        var item = items.SingleOrDefault(item => item.Id == id);
        if (item == null)
        {
            item = new BasketItemVM()
            {
                Id = (int)id,
                Count = 1
            };
        }
        else
        {
            item.Count++;
        }

       
        HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(items));

        return Ok();
    }

    public IActionResult GetBasket()
    {
        return Json(JsonConvert.DeserializeObject<List<BasketItemVM>>(HttpContext.Request.Cookies["basket"]));
    }
}