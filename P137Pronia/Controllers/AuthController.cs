using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P137Pronia.Models;
using P137Pronia.ViewModels.AuthVMs;

namespace P137Pronia.Controllers
{
    public class AuthController : Controller
    {
    readonly UserManager<AppUser> _userManager;
        readonly SignInManager<AppUser> _signInManager;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
            return View();
            }
                return RedirectToAction(nameof(Login));
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if(!ModelState.IsValid) return BadRequest();
            var user = await _userManager.FindByNameAsync(vm.EmailOrUsername);
            if(user == null)
            {
                user = await _userManager.FindByEmailAsync(vm.EmailOrUsername);
                if(user == null)
                {
                    ModelState.AddModelError("", "Email or Username wrong");
                    return View();
                }
            }
            var result = await _signInManager.PasswordSignInAsync(user,vm.Password,vm.RemmemberMe,true);
            if (user.LockoutEnd !=null )
            {
                ModelState.AddModelError("", "wait" + user.LockoutEnd);
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or Username wrong");
                return View();
            }
            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

    }
}
