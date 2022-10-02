using Microsoft.EntityFrameworkCore;
using MVC_example09_22.DAL;
using MVC_example09_22.Models;
using MVC_example09_22.ViewModels;
using Newtonsoft.Json;

namespace MVC_example09_22.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;
        private const string BASKET_SESSION_KEY = "basket";

        public LayoutService(AppDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }
        public async Task<List<BasketItemVM>> GetBasket()
        {
            List<BasketItemVM> items = new List<BasketItemVM>();
            string basketStr = _http.HttpContext.Session.GetString(BASKET_SESSION_KEY);
            if (!string.IsNullOrEmpty(basketStr))
            {
                List<Product> products = _context.Products.Include(p => p.ProductImages).ToList();
               
                BasketVM basket = JsonConvert.DeserializeObject<BasketVM>(basketStr);

                for (int i = 0; i < basket.BasketSessionItems?.Count; i++)
                {
                    if (!products.Exists(p => p.Id == basket.BasketSessionItems[i].Id))
                    {
                        basket.BasketSessionItems.Remove(basket.BasketSessionItems[i]);
                    }
                }
                foreach (BasketSessionItemVM item in basket.BasketSessionItems)
                {
                    Product product = products.FirstOrDefault(p => p.Id == item.Id);
                    BasketItemVM basketItem = new BasketItemVM
                    {
                        Product = product,
                        Price = product.Price,
                        Count = item.Count,
                    };
                    items.Add(basketItem);
                }
            }
            return items;
        }
    }
}
