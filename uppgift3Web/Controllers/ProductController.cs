using System;
using uppgift3Web.Data;
using uppgift3Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;


namespace uppgift3Web.Controllers
{ 
  public class ProductController : Controller
  {
    private readonly IApplicationDbContext _db;

    public ProductController(IApplicationDbContext db)
    {
      _db = db;
    }

    // Index action method for the Product controller
    public IActionResult Index()
    {
      List<Product> objList = _db.Products.ToList();
      return View(objList);
    }

[Authorize(Roles = "Admin")]
    // This is the Get action method - retrieve the selected data by its ID from the database
   public IActionResult Create()
{
    ViewBag.Categories = _db.Categories.ToList(); // Populate categories for dropdown
    return View();
}

[Authorize(Roles = "Admin")]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Product obj)
{
    if (ModelState.IsValid)
    {
        _db.Products.Add(obj);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    else
    {
        Console.WriteLine("ModelState is not valid. Errors:");
        foreach (var modelState in ModelState.Values)
        {
            foreach (var error in modelState.Errors)
            {
                Console.WriteLine($"- {error.ErrorMessage}");
            }
        }

        ViewBag.Categories = _db.Categories?.ToList() ?? new List<Category>();
        return View(obj);
    }
}


[Authorize(Roles = "Admin")]
    // This is the Get action method - retrieve the selected data by its ID from the database
    public IActionResult Edit(int? id)
    {
      if (id == null || id == 0)
      {
        return NotFound();
      }

      Product productFromDb = _db.Products.Find(id);

      if (productFromDb == null)
      {
        return NotFound();
      }

      return View(productFromDb);
    }

[Authorize(Roles = "Admin")]
    // This is the POST action method
[HttpPost]
public IActionResult Edit(Product obj)
{
  if (ModelState.IsValid)
  {
    _db.Products.Update(obj);
    _db.SaveChanges();
    return RedirectToAction("Index");
  }
  return View(obj);
}

[Authorize(Roles = "Admin")]
// This is the GET action method for Delete
public IActionResult Delete(int? id)
{
    if (id == null || id == 0)
    {
        return NotFound();
    }

    Product productFromDb = _db.Products.Find(id);

    if (productFromDb == null)
    {
        return NotFound();
    }

    return View(productFromDb);
}

[Authorize(Roles = "Admin")]
[HttpPost, ActionName("Delete")] public IActionResult DeletePOST(int? id)
{
  Product obj = _db.Products.Find(id);

  if (obj == null)
  {
    return NotFound();
  }

  _db.Products.Remove(obj);
  _db.SaveChanges();
  return RedirectToAction("Index");
}
  }
}
