using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_example09_22.DAL;
using MVC_example09_22.Models;
using MVC_example09_22.Utilities;
using System.Collections.Generic;

namespace MVC_example09_22.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<ActionResult> Index()
        {
            List<Product> products=new List<Product>();
            products = await _context.Products.ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0) return NotFound();
            Product product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Product product)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(int? id, Product product)
        {
            Product existed = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (!ModelState.IsValid)
            {
                return View(existed);
            }
            if (existed == null) return NotFound();

            existed.Name = product.Name;
            existed.Rate = product.Rate;
            existed.Price = product.Price;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();
            Product existed = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) return NotFound();
            foreach (var item in existed.ProductImages)
            {
                FileValidator.FileDelete(_env.WebRootPath, "images/products", item.Name);
                _context.ProductImages.Remove(item);
            }
            _context.Products.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //public async Task<ProductImage> CreateProImg(IFormFile file, Product product, bool result)
        //{
        //    ProductImage image = new ProductImage
        //    {
        //        Name = await file.FileCreate(await CheckExistence(file), _env.WebRootPath, "assets/img"),
        //        Primary = result,
        //        Alt = product.Name,
        //        Product = product
        //    };
        //    return image;
        //}
        //public async Task<string> CheckExistence(IFormFile file)
        //{
        //    string filename = file.FileName;
        //    List<ProductImage> productImages = await _context.ProductImages.ToListAsync();
        //    if (productImages != null)
        //    {
        //        if (productImages.Any(p => p.Name == file.FileName))
        //        {
        //            filename = string.Concat(Guid.NewGuid(), file.FileName);
        //        }
        //    }
        //    return filename;
        //}
    }
}
