namespace MVC_example09_22.ViewModels
{
    public class BasketVM
    {
        public List<BasketSessionItemVM> BasketSessionItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
