using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace ShoppingBasketBusiness
{
    public interface IProductRepository
    {
        public List<IProductItem> GetProductItems(IList<int> productIds);
    }
}