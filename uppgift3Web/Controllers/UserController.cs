using System;
using uppgift3Web.Models;
using Microsoft.AspNetCore.Mvc;
using uppgift3Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using uppgift3Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace uppgift3Web.Controllers
{

  public class UserController:Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;


    public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

[Authorize]
    public async Task<IActionResult> Index()
    {
      var model = new UsersIndexViewModel();

      var users =await _context.Users.ToListAsync();

      foreach (var user in users)
      {
        var roles = await _userManager.GetRolesAsync(user);

        var userWithRoles=new UserWithRoles
        {
          User = user,
               Roles = roles.ToList()
        };
        model.Users.Add(userWithRoles);
      }

      return View(model);

    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangeRole([FromForm] string userId,[FromForm] string newRole)
    {
      var user = await _userManager.FindByIdAsync(userId);

      if (user == null)
      {
        return NotFound();

      }

      var roles = await _userManager.GetRolesAsync(user);
      await _userManager.RemoveFromRolesAsync(user, roles.ToArray());

      var result = await _userManager.AddToRoleAsync(user, newRole);

      if (result.Succeeded)
      {
        return RedirectToAction("Index");
      }

      return View("Error");
    }

  }
}

