using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingBasketBusiness
{
    public interface IPriceCalculator
    {
        public decimal GetTotalPrice(IList<int> productIds);
    }
}
