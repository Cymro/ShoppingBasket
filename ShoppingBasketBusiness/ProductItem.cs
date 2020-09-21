using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingBasketBusiness
{
    public class ProductItem : IProductItem
    {
        public int ProductId { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }
    }
}
