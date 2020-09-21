using System;

namespace ShoppingBasketBusiness
{
    public interface IProductItem
    {
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int ProductId { get; set; }
    }
}