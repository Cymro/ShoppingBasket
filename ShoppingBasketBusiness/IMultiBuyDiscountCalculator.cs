using System.Collections.Generic;

namespace ShoppingBasketBusiness
{
    public interface IMultiBuyDiscountCalculator
    {
        decimal GetDiscount(IList<IProductItem> items);
    }
}