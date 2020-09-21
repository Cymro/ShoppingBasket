using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Moq;
using ShoppingBasketBusiness;
using Microsoft.Extensions.Logging;

namespace ShoppingBasketTest
{
    public class PriceWithDiscountCalculatorIntegrationTests
    {
        private PriceCalculator _priceCalculator;

        [SetUp]
        public void Setup()
        {
            Mock<Microsoft.Extensions.Logging.ILogger> mockLogger = 
                new Mock<Microsoft.Extensions.Logging.ILogger>();

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(m => m.GetProductItems(It.IsAny<IList<int>>()))
                .Returns((IList<int> productIds) =>
                {
                    var products = new List<IProductItem>();
                    foreach (var id in productIds)
                    {
                        products.Add(new ProductItem() { ProductId = id, Price = 8 });
                    }
                    return products;
                });

            var mockMultiBuyOfferRepository = new Mock<IMultiBuyOfferRepository>();
            mockMultiBuyOfferRepository
                .SetupGet<IList<int>>(m => m.EligableProducts)
                .Returns(new List<int> { 1, 2, 3, 4, 5, 6 });

            mockMultiBuyOfferRepository
                .SetupGet<Dictionary<int, decimal>>(m => m.DiscountLookup)
                .Returns(new Dictionary<int, decimal>
                {
                    { 2, 0.05m },
                    { 3, 0.1m },
                    { 4, 0.2m },
                    { 5, 0.25m }
                });

            var discountCalculator = new MultiBuyDiscountCalculator(mockLogger.Object, mockMultiBuyOfferRepository.Object);
            _priceCalculator = new PriceCalculator(mockLogger.Object, mockProductRepository.Object, discountCalculator);
        }

        [Test]
        public void OneItem_NoDiscount()
        {
            var totalPrice = _priceCalculator.GetTotalPrice(new List<int>() { 1 });
            Assert.AreEqual(8m, totalPrice);
        }

        [Test]
        public void TwoEligibleItems_5PercentDiscount()
        {
            var totalPrice = _priceCalculator.GetTotalPrice(new List<int>() { 1, 2 });
            Assert.AreEqual(15.2m, totalPrice);
        }

        [Test]
        public void ThreeEligibleItems_10PercentDiscount()
        {
            var totalPrice = _priceCalculator.GetTotalPrice(new List<int>() { 1, 2, 3 });
            Assert.AreEqual(21.6m, totalPrice);
        }

        [Test]
        public void MultipleEligibleItemCombos_2x20PercentDiscount()
        {
            var totalPrice = _priceCalculator.GetTotalPrice(new List<int>() { 1, 1, 2, 2, 3, 3, 4, 5});
            Assert.AreEqual(51.2m, totalPrice);
        }

    }
}
