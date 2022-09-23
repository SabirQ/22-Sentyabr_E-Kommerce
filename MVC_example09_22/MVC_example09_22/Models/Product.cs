using MVC_example09_22.Models.Base;

namespace MVC_example09_22.Models
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public double Rate { get; set; }
        public decimal Price { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public Product()
        {
            ProductImages = new HashSet<ProductImage>();
        }
    }
}
