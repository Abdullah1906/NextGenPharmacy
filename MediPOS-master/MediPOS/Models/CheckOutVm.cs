namespace MediPOS.Models
{
    public class CheckOutVm
    {
        public OrderInfoVm OrderInfo { get; set; } = new OrderInfoVm();
        public List<CartItemVm> CartItems { get; set; } = new List<CartItemVm>();

    }

}
