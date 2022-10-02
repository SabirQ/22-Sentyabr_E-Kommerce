using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_example09_22.DAL;
using MVC_example09_22.Models;
using MVC_example09_22.ViewModels;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MVC_example09_22.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private const string BASKET_SESSION_KEY = "basket";

        public HomeController(ILogger<HomeController> logger,AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Product>products=await _context.Products.Include(x=>x.ProductImages).ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> AddBasket(int id)
        {
            if (id == 0) return NotFound();
            Product product = await _context.Products.FirstOrDefaultAsync(x=>x.Id == id);
            if (product == null) return NotFound();
            string existedBasket = HttpContext.Session.GetString(BASKET_SESSION_KEY);
            BasketVM basket;
            if (string.IsNullOrEmpty(existedBasket))
            {
                basket=new BasketVM()
                {
                  BasketSessionItems=new List<BasketSessionItemVM>() 
                  { 
                    new BasketSessionItemVM 
                    { 
                        Id = id,
                        Count=1
                    }
                  },
                  TotalPrice=product.Price
                };
                HttpContext.Session.SetString(BASKET_SESSION_KEY, JsonConvert.SerializeObject(basket));
            }
            else
            {
                basket=JsonConvert.DeserializeObject<BasketVM>(existedBasket);
                BasketSessionItemVM basketSessionItem = basket.BasketSessionItems.FirstOrDefault(x => x.Id == id);
                if (basketSessionItem==null)
                {
                    basketSessionItem = new BasketSessionItemVM
                    {
                        Id=id,
                        Count=1,
                    };
                    basket.BasketSessionItems.Add(basketSessionItem);
                }
                else
                {
                    basketSessionItem.Count++;
                }
                basket.TotalPrice+=product.Price;
                HttpContext.Session.SetString(BASKET_SESSION_KEY, JsonConvert.SerializeObject(basket));
            }
                return RedirectToAction("Index");
        }
        public async Task<IActionResult> RemoveBasket(int id)
        {
            if (id == 0) return NotFound();
            string existedBasket = HttpContext.Session.GetString(BASKET_SESSION_KEY);
            if (!string.IsNullOrEmpty(existedBasket))
            {
                BasketVM basket = JsonConvert.DeserializeObject<BasketVM>(existedBasket);
                Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (product == null) return NotFound();
                BasketSessionItemVM basketSessionItem = basket.BasketSessionItems.FirstOrDefault(x => x.Id == id);
                if (basketSessionItem != null)
                {
                  basket.BasketSessionItems.Remove(basketSessionItem);
                  basket.TotalPrice -= (decimal)(product.Price*basketSessionItem.Count);
                  HttpContext.Session.SetString(BASKET_SESSION_KEY, JsonConvert.SerializeObject(basket));
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}