using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace ShoppingBasketBusiness
{
    public interface IMultiBuyOfferRepository
    {
        public IList<int> EligableProducts { get; set; }
        public Dictionary<int, decimal> DiscountLookup { get; set; }
    }
}