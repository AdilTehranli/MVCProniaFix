using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P137Pronia.Models;
using P137Pronia.ViewModels.UserVMs;

namespace P137Pronia.Areas.Manage.Controllers;

[Area("Manage")]
[Authorize(Roles ="Admin")]
public class UserController : Controller
{
    readonly UserManager<AppUser> _userManager;
    readonly RoleManager<IdentityRole> _roleManager;

    public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult>  Index()
    {
      var users=  await _userManager.Users.ToListAsync();
        List<UserRoleVM> vm = new List<UserRoleVM>();
        foreach (var item in users)
        {
            var roles = await _userManager.GetRolesAsync(item);
          
            vm.Add(new UserRoleVM
            {
                FullName = item.UserName,
                Name = item.UserName,
                Role= roles.FirstOrDefault()
            });
        }
        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name") ;
        return View(vm);
    }
    [HttpPost]
    public async Task<IActionResult> ChangeRole(string username,string role)
    {
        var currentUser=await _userManager.FindByNameAsync(User.Identity?.Name);
        var user = await _userManager.FindByNameAsync(username);
        if (currentUser == null || user == null) return BadRequest();
        var userRoles= await _userManager.GetRolesAsync(user);
        if (userRoles.FirstOrDefault() == "Admin")
        {
            return BadRequest();
        }
        else
        {
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRoleAsync(user,role);
        }
        return Ok();
    }
}
