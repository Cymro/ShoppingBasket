using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Moq;
using NUnit.Framework;
using ShoppingBasketBusiness;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingBasketTest
{
    class PriceCalculatorTests
    {
        private IPriceCalculator _priceCalculator; 

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

            _priceCalculator = new PriceCalculator(mockLogger.Object, mockProductRepository.Object, null);
        }

        [Test]
        public void OneProduct()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 100, Price = 8m},
            };

            var price = _priceCalculator.GetTotalPrice(new List<int>(){ 1 });
            Assert.AreEqual(8, price);
        }

        [Test]
        public void TwoProducts()
        {
            var productList = new List<IProductItem>
            {
                new ProductItem() { ProductId = 100, Price = 8m},
            };

            var price = _priceCalculator.GetTotalPrice(new List<int>() { 1, 2 });
            Assert.AreEqual(16, price);
        }

    }
}
