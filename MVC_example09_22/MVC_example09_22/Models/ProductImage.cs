using MVC_example09_22.Models.Base;

namespace MVC_example09_22.Models
{
    public class ProductImage:BaseEntity
    {
        public string Name { get; set; }
        public string Alt { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public bool Primary { get; set; }
    }
}
