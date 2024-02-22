using NUnit.Framework;
using NUnit.Framework.Legacy;
using Moq;
using uppgift3Web.Controllers;
using uppgift3Web.Data;
using uppgift3Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace uppgift3Web.Tests
{
  public class ProductControllerTests
  {
    private ProductController _controller;
    private List<Product> _products;
    private Mock<DbSet<Product>> _mockSet;
    private Mock<IApplicationDbContext> _mockContext;

    [SetUp]
    public void Setup()
    {
      // Create a mock DbContext
      _mockContext = new Mock<IApplicationDbContext>();
      // Create the controller and inject the DbContext
      _controller = new ProductController(_mockContext.Object);
      // Create a list of products
      _products = new List<Product>()
      {
        new Product { Id = 1, Name = "Banan", Description = "En gul frukt", Price = 10, CategoryId = 1 },
            new Product { Id = 2, Name = "Äpple", Description = "En röd frukt", Price = 5, CategoryId = 1 },
      };
      // Create a mock set
      var mockSet = new Mock<DbSet<Product>>();
      // Setup the mock set to behave like a List<Product>
      mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(_products.AsQueryable().Provider);
      mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(_products.AsQueryable().Expression);
      mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(_products.AsQueryable().ElementType);
      mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(_products.GetEnumerator());

      // Setup the mock DbContext to return the mock set
      _mockContext.Setup(db => db.Products).Returns(mockSet.Object);
    }

    [Test]
    public void Index_ReturnsAViewResult_WithAListOfProducts()
    {
      var result = _controller.Index();

      var viewResult = result as ViewResult;
      Assert.That(viewResult, Is.Not.Null);

      var model = viewResult.ViewData.Model as List<Product>;
      Assert.That(model, Is.Not.Null);

      Assert.That(model.Count, Is.EqualTo(_products.Count));
    }

    [Test]
    public async Task Create_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
    {
      var newProduct = new Product { Id = 3, Name = "Päron", Description = "En grön frukt", Price = 15, CategoryId = 1 };

      var result = await _controller.Create(newProduct);

      var redirectToActionResult = result as RedirectToActionResult;
      Assert.That(redirectToActionResult, Is.Not.Null);
      Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Index"));
    }

    [Test]
    public void Edit_Get_ReturnsViewResult_WithProduct()
    {
      // Arrange
      var mockProduct = new Product { Id = 1 };
      _mockContext.Setup(x => x.Products.Find(It.IsAny<int>())).Returns(mockProduct);
      var controller = new ProductController(_mockContext.Object);

      // Act
      var result = controller.Edit(1);

      // Assert
      Assert.That(result, Is.InstanceOf<ViewResult>());
      var viewResult = result as ViewResult;

      ClassicAssert.IsNotNull(viewResult, "ViewResult should not be null");

      // Check if Model property is not null
      ClassicAssert.IsNotNull(viewResult.Model, "Model property of ViewResult should not be null");

      // Check the type of the model
      Assert.That(viewResult.Model, Is.InstanceOf<Product>());

      var model = viewResult.Model as Product;
      Assert.That(model.Id, Is.EqualTo(1));
    }

    [Test]
    public void Edit_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
    {
      // Arrange
      var updatedProduct = new Product { Id = 1, Name = "Updated Banan", Description = "En gul frukt", Price = 10, CategoryId = 1 };

      // Act
      var result = _controller.Edit(updatedProduct);

      // Assert
      var redirectToActionResult = result as RedirectToActionResult;
      Assert.That(redirectToActionResult, Is.Not.Null);
      Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Index"));
    }

    [Test]
    public void Delete_Get_ReturnsViewResult_WithProduct()
    {
      // Arrange
      var mockProduct = new Product { Id = 1 };
      _mockContext.Setup(x => x.Products.Find(It.IsAny<int>())).Returns(mockProduct);
      var controller = new ProductController(_mockContext.Object);

      // Act
      var result = controller.Delete(1);

      // Assert
      Assert.That(result, Is.InstanceOf<ViewResult>());
      var viewResult = result as ViewResult;

      ClassicAssert.IsNotNull(viewResult, "ViewResult should not be null");

      // Check if Model property is not null
      ClassicAssert.IsNotNull(viewResult.Model, "Model property of ViewResult should not be null");

      // Check the type of the model
      Assert.That(viewResult.Model, Is.InstanceOf<Product>());

      var model = viewResult.Model as Product;
      Assert.That(model.Id, Is.EqualTo(1));
    }

    [Test]
    public void Delete_Post_ReturnsRedirectToActionResult()
    {
      // Arrange
      var mockProduct = new Product { Id = 1 };
      _mockContext.Setup(x => x.Products.Find(It.IsAny<int>())).Returns(mockProduct);
      var controller = new ProductController(_mockContext.Object);

      // Act
      var result = controller.DeletePOST(1);

      // Assert
      Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
      var redirectToActionResult = result as RedirectToActionResult;

      ClassicAssert.IsNotNull(redirectToActionResult, "RedirectToActionResult should not be null");

      // Check the action name
      Assert.That(redirectToActionResult.ActionName, Is.EqualTo("Index"));
    }

    [Test]
public async Task Create_Post_ReturnsViewResult_WhenModelStateIsInvalid()
{
    var newProduct = new Product { Id = 3, Name = "Päron", Description = "En grön frukt", Price = 15, CategoryId = 1 };
    _controller.ModelState.AddModelError("Error", "Model state is invalid");

    var result = await _controller.Create(newProduct);

    Assert.That(result, Is.InstanceOf<ViewResult>());
}

[Test]
public void Edit_Post_ReturnsViewResult_WhenModelStateIsInvalid()
{
    var updatedProduct = new Product { Id = 1, Name = "Updated Banan", Description = "En gul frukt", Price = 10, CategoryId = 1 };
    _controller.ModelState.AddModelError("Error", "Model state is invalid");

    var result = _controller.Edit(updatedProduct);

    Assert.That(result, Is.InstanceOf<ViewResult>());
}

[Test]
public void Delete_Get_ReturnsNotFoundResult_WhenProductDoesNotExist()
{
    _mockContext.Setup(x => x.Products.Find(It.IsAny<int>())).Returns((Product)null);
    var controller = new ProductController(_mockContext.Object);

    var result = controller.Delete(1);

    Assert.That(result, Is.InstanceOf<NotFoundResult>());
}
  }
}
