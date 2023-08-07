using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P137Pronia.Models;
using P137Pronia.ViewModels.AuthVMs;

namespace P137Pronia.Controllers
{
    public class AuthController : Controller
    {
    readonly UserManager<AppUser> _userManager;

        public AuthController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm) 
        {
            if(!ModelState.IsValid) return BadRequest();
            AppUser user = new AppUser()
            {
                FullName = vm.Name + " " + vm.Surname,
                Email=vm.EmailAddress,
                UserName=vm.Username
                
            };
            var result= await _userManager.CreateAsync(user,vm.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return RedirectToAction(nameof(Login));
            }
            return Json(vm);
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}
