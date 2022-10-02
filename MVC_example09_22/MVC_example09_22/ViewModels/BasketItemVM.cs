using MVC_example09_22.Models;

namespace MVC_example09_22.ViewModels
{
    public class BasketItemVM
    {
        public Product Product { get; set; } = null!;
        public decimal Price { get; set; }
        public int Count { get; set; }
    }
}
