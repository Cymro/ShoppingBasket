using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingBasketBusiness
{
    
    public class PriceCalculator: IPriceCalculator
    {
        private ILogger _logger;
        private IProductRepository _productRepository;
        private IMultiBuyDiscountCalculator _multiBuyDiscountCalculator;

        public PriceCalculator(ILogger logger, IProductRepository productRepository, IMultiBuyDiscountCalculator multiBuyDiscountCalculator)
        {
            _logger = logger;
            _productRepository = productRepository;
            _multiBuyDiscountCalculator = multiBuyDiscountCalculator;
        }
        public decimal GetTotalPrice(IList<int> productIds)
        {
            var products = _productRepository.GetProductItems(productIds);
            var priceBeforeDiscount = products.Sum(p => p.Price);
            return priceBeforeDiscount -= _multiBuyDiscountCalculator?.GetDiscount(products) ?? 0;
        }
    }
}
